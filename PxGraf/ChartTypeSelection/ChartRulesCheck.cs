using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Defines the limits for data dimensions for a spesific chart type.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits">Numeric limits used with this limit object</param>
    public abstract class ChartRulesCheck(IChartTypeLimits limits)
    {
        /// <summary>
        /// The type of the chart this selection limit applies to.
        /// </summary>
        public abstract VisualizationType Type { get; }

        /// <summary>
        /// Chart type spesific limits
        /// </summary>
        IChartTypeLimits Limits { get; } = limits;

        /// <summary>
        /// Checks if the given query fits in the limits of this limit object
        /// </summary>
        /// <returns></returns>
        public List<ChartRejectionInfo> CheckValidity(VisualizationTypeSelectionObject input)
        {
            var reasons = new List<ChartRejectionInfo>();

            // Data validity
            reasons.AddRange(CheckData(input));

            // Abstract, needs chart specific override
            reasons.AddRange(CheckChartSpecificRules(input));

            // Number of multiselects
            reasons.AddRange(CheckMultiselectLimits(input, Limits.NumberOfMultiselectsRange));

            // Time
            reasons.AddRange(CheckTimeLimits(input.Variables.FirstOrDefault(v => v.Type == DimensionType.Time), Limits.TimeRange, Limits.IrregularTimeRange));

            // Content
            reasons.AddRange(CheckContentLimits(input.Variables.FirstOrDefault(v => v.Type == DimensionType.Content), Limits.ContentRange));

            // Content units
            reasons.AddRange(CheckContentUnitLimits(input.Variables.FirstOrDefault(v => v.Type == DimensionType.Content), Limits.ContentUnitRange));

            var multiselects = input.Variables
                .Where(variable => variable.Size > 1)
                .OrderByDescending(x => x.Size)
                .ToList();

            // First multiselect
            reasons.AddRange(CheckVariableLimits((multiselects.Count != 0 ? multiselects[0] : null),
                Limits.FirstMultiselectRange, RejectionReason.FirstMultiselectOverMax, RejectionReason.FirstMultiselectBelowMin));

            // Second multiselect
            reasons.AddRange(CheckVariableLimits((multiselects.Count > 1 ? multiselects[1] : null),
                Limits.SecondMultiselectRange, RejectionReason.SecondMultiselectOverMax, RejectionReason.SecondMultiselectBelowMin));

            // Additional multiselects
            if (multiselects.Count > 2)
            {
                foreach (VisualizationTypeSelectionObject.VariableInfo exdim in multiselects.Skip(2))
                {
                    reasons.AddRange(CheckAdditionalDimensionsLimits(exdim, Limits.AdditionalMultiselectDimensionsRange));
                }
            }

            // Product of multiselects (virtual)
            reasons.AddRange(CheckMultiselectProductLimits(multiselects, Limits.ProductOfMultiselectsRange));

            reasons.Sort(); // Sorts the reasons based on their priority.
            return reasons;
        }

        /// <summary>
        /// Checks that data contais atleas one actual value.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckData(VisualizationTypeSelectionObject input)
        {
            if (!input.HasActualData) yield return BuildRejectionInfo(RejectionReason.DataRequired, 0, 1);
        }

        /// <summary>
        /// Contains chart type specific checks
        /// </summary>
        protected abstract IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input);

        /// <summary>
        /// Checks if the number of multiselect dimensions is within allowed limits.
        /// </summary>
        private IEnumerable<ChartRejectionInfo> CheckMultiselectLimits(VisualizationTypeSelectionObject input, DimensionRange numberOfMultiselectsRange)
        {
            int minNumOfMultiselects = input.Variables.Count(var => var.Size > 1 || var.FilterCanChangeToMultiValue); // Variables that can change to multiselect later based on the filter must be taken into account here.
            int currentNumOfMultiselects = input.Variables.Count(var => var.Size > 1); // Variables cannot change from multiselect to single select.
            if (minNumOfMultiselects > numberOfMultiselectsRange.Max) yield return BuildRejectionInfo(RejectionReason.TooManyMultiselections, minNumOfMultiselects, numberOfMultiselectsRange.Max);
            if (currentNumOfMultiselects < numberOfMultiselectsRange.Min) yield return BuildRejectionInfo(RejectionReason.NotEnoughMultiselections, currentNumOfMultiselects, numberOfMultiselectsRange.Min);
        }

        /// <summary>
        /// Yields rejection enums if the provided variable does not fit in the time variable limits.
        /// </summary>
        private IEnumerable<ChartRejectionInfo> CheckTimeLimits(VisualizationTypeSelectionObject.VariableInfo timeVar, DimensionRange timeRange, DimensionRange irregularRange)
        {
            if (timeVar is null)
            {
                if (timeRange.DimensionRequired) yield return BuildRejectionInfo(RejectionReason.TimeRequired);
                yield break;
            }

            if (timeRange.DimensionNotAllowed)
            {
                yield return BuildRejectionInfo(RejectionReason.TimeNotAllowed, timeVar);
                yield break;
            }

            if (timeVar.IsIrregular ?? throw new InvalidOperationException("Ragularity was not defined for the time variable"))
            {
                if (irregularRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.IrregularTimeNotAllowed, timeVar);
                else if (timeVar.Size < irregularRange.Min) yield return BuildRejectionInfo(RejectionReason.IrregularTimeBelowMin, timeVar.Size, irregularRange.Min, timeVar);
                else if (timeVar.Size > irregularRange.Max) yield return BuildRejectionInfo(RejectionReason.IrregularTimeOverMax, timeVar.Size, irregularRange.Max, timeVar);
            }
            if (timeVar.Size < timeRange.Min) yield return BuildRejectionInfo(RejectionReason.TimeBelowMin, timeVar.Size, timeRange.Min, timeVar);
            else if (timeVar.Size > timeRange.Max) yield return BuildRejectionInfo(RejectionReason.TimeOverMax, timeVar.Size, timeRange.Max, timeVar);
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the content dimension limits.
        /// </summary>
        /// <param name="contentVar"></param>
        /// <param name="contentRange"></param>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckContentLimits(VisualizationTypeSelectionObject.VariableInfo contentVar, DimensionRange contentRange)
        {
            if (contentVar == null && contentRange.DimensionRequired)
            {
                yield return BuildRejectionInfo(RejectionReason.ContentRequired);
            }

            if (contentVar != null)
            {
                if (contentRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.ContentNotAllowed, contentVar);
                else
                {
                    int conttentSize = contentVar.Size;
                    if (conttentSize < contentRange.Min) yield return BuildRejectionInfo(RejectionReason.ContentBelowMin, conttentSize, contentRange.Min, contentVar);
                    if (conttentSize > contentRange.Max) yield return BuildRejectionInfo(RejectionReason.ContentOverMax, conttentSize, contentRange.Max, contentVar);
                }
            }
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the unique unit limits.
        /// </summary>
        /// <param name="contentVar"></param>
        /// <param name="contentUnitRange"></param>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckContentUnitLimits(VisualizationTypeSelectionObject.VariableInfo contentVar, DimensionRange contentUnitRange)
        {
            // ContentUnitCheck is currently used as alternative to regular ContentCheck.
            // Therefore handle DimensionRequired and ContentNotAllowed here as well.
            if (contentVar is null)
            {
                if (contentUnitRange.DimensionRequired) yield return BuildRejectionInfo(RejectionReason.ContentRequired);
                yield break;
            }

            if (contentUnitRange.DimensionNotAllowed)
            {
                yield return BuildRejectionInfo(RejectionReason.ContentNotAllowed, contentVar);
                yield break;
            }

            if (contentUnitRange.Min == 1 && contentUnitRange.Max == 1)
            {   // Currently all checks should have min and max = 1.
                if (contentVar.NumberOfUnits.Value != 1) yield return BuildRejectionInfo(RejectionReason.UnambiguousContentUnitRequired, contentVar.Size, 1, contentVar);
                yield break;
            }

            if (contentVar.NumberOfUnits.Value < contentUnitRange.Min) yield return BuildRejectionInfo(RejectionReason.ContentUnitsBelowMin, contentVar.NumberOfUnits.Value, contentUnitRange.Min, contentVar);
            else if (contentVar.NumberOfUnits.Value > contentUnitRange.Max) yield return BuildRejectionInfo(RejectionReason.ContentUnitsOverMax, contentVar.NumberOfUnits.Value, contentUnitRange.Max, contentVar);
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the specified variable limits.
        /// Checking if the variable is required/allowed is handled by the number of multiselects check.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckVariableLimits(VisualizationTypeSelectionObject.VariableInfo variable, DimensionRange range, RejectionReason over, RejectionReason under)
        {
            if (variable != null && range.DimensionRequired)
            {
                int varSize = variable.Size;
                if (varSize < range.Min) yield return BuildRejectionInfo(under, varSize, range.Min, variable);
                if (varSize > range.Max) yield return BuildRejectionInfo(over, varSize, range.Max, variable);
            }
        }


        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the extra classifier dimension limits.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckAdditionalDimensionsLimits(VisualizationTypeSelectionObject.VariableInfo variable, DimensionRange additionalDimensionsRange)
        {
            if (variable == null && additionalDimensionsRange.DimensionRequired)
            {
                yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsRequired);
            }

            if (variable != null)
            {
                if (additionalDimensionsRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsNotAllowed, variable);
                else
                {
                    if (variable.Size < additionalDimensionsRange.Min) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsBelowMin, variable.Size, additionalDimensionsRange.Min, variable);
                    if (variable.Size > additionalDimensionsRange.Max) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsOverMax, variable.Size, additionalDimensionsRange.Max, variable);
                }
            }
        }

        /// <summary>
        /// Checks if the product of the multiselect variable sizes is in an acceptable range.
        /// Some chart types need to modify this check, threfore it must be virtual for overloading.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ChartRejectionInfo> CheckMultiselectProductLimits(IEnumerable<VisualizationTypeSelectionObject.VariableInfo> multiSelectVars, DimensionRange productOfMultiselectsRange)
        {
            int product = 1;
            foreach (var variable in multiSelectVars) product *= variable.Size;

            if (product < productOfMultiselectsRange.Min) yield return BuildRejectionInfo(RejectionReason.MultiselectProductBelowMin, product, productOfMultiselectsRange.Min);
            if (product > productOfMultiselectsRange.Max) yield return BuildRejectionInfo(RejectionReason.MultiselectProductOverMax, product, productOfMultiselectsRange.Max);
        }

        /// <summary>
        /// Function to simplify rejection info generation.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="value"></param>
        /// <param name="threshold"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        protected ChartRejectionInfo BuildRejectionInfo(RejectionReason reason, int value, int threshold, VisualizationTypeSelectionObject.VariableInfo dimension = null)
            => new(reason, GetPriority(reason), Type, value, threshold, dimension);

        protected ChartRejectionInfo BuildRejectionInfo(RejectionReason reason, VisualizationTypeSelectionObject.VariableInfo dimension = null, string invalidValueName = null)
            => new(reason, GetPriority(reason), Type, null, null, dimension, invalidValueName);

        /// <summary>
        /// Each chart type has its own priority order for reasons of rejection,
        /// override this method to set that order.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected abstract int GetPriority(RejectionReason reason);

        protected VisualizationTypeSelectionObject.VariableInfo GetLargestMultiselect(VisualizationTypeSelectionObject input)
        {
            var multiselects = GetSortedMultiselects(input.Variables);
            return multiselects != null && multiselects.Count != 0 ? multiselects[0] : null;

        }

        protected VisualizationTypeSelectionObject.VariableInfo GetSmallerMultiselect(VisualizationTypeSelectionObject input)
        {
            var multiselects = GetSortedMultiselects(input.Variables);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimennsion if the query contains one, if not, returns a variable which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        protected VisualizationTypeSelectionObject.VariableInfo GetTimeOrLargestOrdinal(IEnumerable<VisualizationTypeSelectionObject.VariableInfo> variables)
        {
            var multiselects = GetSortedMultiselects(variables); //OBS: multiselects are sorted here so First() can be used!
            if (multiselects.Find(v => v.Type == DimensionType.Time) is VisualizationTypeSelectionObject.VariableInfo tv) return tv;
            if (multiselects.Find(v => v.Type == DimensionType.Ordinal) is VisualizationTypeSelectionObject.VariableInfo ov) return ov;

            return null;
        }

        private static List<VisualizationTypeSelectionObject.VariableInfo> GetSortedMultiselects(IEnumerable<VisualizationTypeSelectionObject.VariableInfo> variables)
        {
            return [.. variables
                .Where(variable => variable.Size > 1)
                .OrderByDescending(x => x.Size)];
        }

        /// <summary>
        /// Helper for getting priority from list
        /// </summary>
        /// <param name="reasonPriorityList">List where RejectionReasons are in order</param>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected int GetPriorityIndex(IList<RejectionReason> reasonPriorityList, RejectionReason reason)
        {
            var index = reasonPriorityList.IndexOf(reason);
            if (index >= 0)
                return index + 1;
            else
                return reasonPriorityList.Count + 1;
        }
    }
}
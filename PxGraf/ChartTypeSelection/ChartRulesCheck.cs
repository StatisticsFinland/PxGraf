using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Defines the limits for data dimensions for a specific chart type.
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
        /// Chart type specific limits
        /// </summary>
        IChartTypeLimits Limits { get; } = limits;

        /// <summary>
        /// Checks if the given query fits in the limits of this limit object
        /// </summary>
        /// <returns></returns>
        public List<ChartRejectionInfo> CheckValidity(VisualizationTypeSelectionObject input)
        {
            List<ChartRejectionInfo> reasons = [];

            // Data validity
            reasons.AddRange(CheckData(input));

            // Abstract, needs chart specific override
            reasons.AddRange(CheckChartSpecificRules(input));

            // Number of multiselects
            reasons.AddRange(CheckMultiselectLimits(input, Limits.NumberOfMultiselectsRange));

            // Time
            VisualizationTypeSelectionObject.DimensionInfo timeDimension = input.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
            bool checkIrregularity = GetTimeOrLargestOrdinal(input.Dimensions) == timeDimension; // Checking for time dimension irregularity is only relevant if the time dimension is the column dimension.
            reasons.AddRange(CheckTimeLimits(timeDimension, Limits.TimeRange, Limits.IrregularTimeRange, checkIrregularity));

            // Content
            reasons.AddRange(CheckContentLimits(input.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content), Limits.ContentRange));

            // Content units
            reasons.AddRange(CheckContentUnitLimits(input.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Content), Limits.ContentUnitRange));

            List<VisualizationTypeSelectionObject.DimensionInfo> multiselects =
            [
                .. input.Dimensions
                    .Where(dimension => dimension.Size > 1)
                    .OrderByDescending(x => x.Size)
            ];

            // First multiselect
            reasons.AddRange(CheckDimensionLimits((multiselects.Count != 0 ? multiselects[0] : null),
                Limits.FirstMultiselectRange, RejectionReason.FirstMultiselectOverMax, RejectionReason.FirstMultiselectBelowMin));

            // Second multiselect
            reasons.AddRange(CheckDimensionLimits((multiselects.Count > 1 ? multiselects[1] : null),
                Limits.SecondMultiselectRange, RejectionReason.SecondMultiselectOverMax, RejectionReason.SecondMultiselectBelowMin));

            // Additional multiselects
            if (multiselects.Count > 2)
            {
                foreach (VisualizationTypeSelectionObject.DimensionInfo exdim in multiselects.Skip(2))
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
        /// Checks that data contains at least one actual value.
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
            int minNumOfMultiselects = input.Dimensions.Count(dim => dim.Size > 1 || dim.FilterCanChangeToMultiValue); // Dimensions that can change to multiselect later based on the filter must be taken into account here.
            int currentNumOfMultiselects = input.Dimensions.Count(dim => dim.Size > 1); // Dimensions cannot change from multiselect to single select.
            if (minNumOfMultiselects > numberOfMultiselectsRange.Max) yield return BuildRejectionInfo(RejectionReason.TooManyMultiselections, minNumOfMultiselects, numberOfMultiselectsRange.Max);
            if (currentNumOfMultiselects < numberOfMultiselectsRange.Min) yield return BuildRejectionInfo(RejectionReason.NotEnoughMultiselections, currentNumOfMultiselects, numberOfMultiselectsRange.Min);
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the time dimension limits.
        /// </summary>
        private IEnumerable<ChartRejectionInfo> CheckTimeLimits(VisualizationTypeSelectionObject.DimensionInfo timeDimension, DimensionRange timeRange, DimensionRange irregularRange, bool checkIrregularity)
        {
            if (timeDimension is null)
            {
                if (timeRange.DimensionRequired) yield return BuildRejectionInfo(RejectionReason.TimeRequired);
                yield break;
            }

            if (timeRange.DimensionNotAllowed)
            {
                yield return BuildRejectionInfo(RejectionReason.TimeNotAllowed, timeDimension);
                yield break;
            }

            if (checkIrregularity && (timeDimension.IsIrregular ?? throw new InvalidOperationException("Regularity was not defined for the time dimension")))
            {
                if (irregularRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.IrregularTimeNotAllowed, timeDimension);
                else if (timeDimension.Size < irregularRange.Min) yield return BuildRejectionInfo(RejectionReason.IrregularTimeBelowMin, timeDimension.Size, irregularRange.Min, timeDimension);
                else if (timeDimension.Size > irregularRange.Max) yield return BuildRejectionInfo(RejectionReason.IrregularTimeOverMax, timeDimension.Size, irregularRange.Max, timeDimension);
            }

            if (timeDimension.Size < timeRange.Min) yield return BuildRejectionInfo(RejectionReason.TimeBelowMin, timeDimension.Size, timeRange.Min, timeDimension);
            else if (timeDimension.Size > timeRange.Max) yield return BuildRejectionInfo(RejectionReason.TimeOverMax, timeDimension.Size, timeRange.Max, timeDimension);
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the content dimension limits.
        /// </summary>
        /// <param name="contentDimension"></param>
        /// <param name="contentRange"></param>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckContentLimits(VisualizationTypeSelectionObject.DimensionInfo contentDimension, DimensionRange contentRange)
        {
            if (contentDimension == null && contentRange.DimensionRequired)
            {
                yield return BuildRejectionInfo(RejectionReason.ContentRequired);
            }

            if (contentDimension != null)
            {
                if (contentRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.ContentNotAllowed, contentDimension);
                else
                {
                    int contentSize = contentDimension.Size;
                    if (contentSize < contentRange.Min) yield return BuildRejectionInfo(RejectionReason.ContentBelowMin, contentSize, contentRange.Min, contentDimension);
                    if (contentSize > contentRange.Max) yield return BuildRejectionInfo(RejectionReason.ContentOverMax, contentSize, contentRange.Max, contentDimension);
                }
            }
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the unique unit limits.
        /// </summary>
        /// <param name="contentDimension"></param>
        /// <param name="contentUnitRange"></param>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckContentUnitLimits(VisualizationTypeSelectionObject.DimensionInfo contentDimension, DimensionRange contentUnitRange)
        {
            // ContentUnitCheck is currently used as alternative to regular ContentCheck.
            // Therefore handle DimensionRequired and ContentNotAllowed here as well.
            if (contentDimension is null)
            {
                if (contentUnitRange.DimensionRequired) yield return BuildRejectionInfo(RejectionReason.ContentRequired);
                yield break;
            }

            if (contentUnitRange.DimensionNotAllowed)
            {
                yield return BuildRejectionInfo(RejectionReason.ContentNotAllowed, contentDimension);
                yield break;
            }

            if (contentUnitRange.Min == 1 && contentUnitRange.Max == 1)
            {   // Currently all checks should have min and max = 1.
                if (contentDimension.NumberOfUnits.Value != 1) yield return BuildRejectionInfo(RejectionReason.UnambiguousContentUnitRequired, contentDimension.Size, 1, contentDimension);
                yield break;
            }

            if (contentDimension.NumberOfUnits.Value < contentUnitRange.Min) yield return BuildRejectionInfo(RejectionReason.ContentUnitsBelowMin, contentDimension.NumberOfUnits.Value, contentUnitRange.Min, contentDimension);
            else if (contentDimension.NumberOfUnits.Value > contentUnitRange.Max) yield return BuildRejectionInfo(RejectionReason.ContentUnitsOverMax, contentDimension.NumberOfUnits.Value, contentUnitRange.Max, contentDimension);
        }

        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the specified dimension limits.
        /// Checking if the dimension is required/allowed is handled by the number of multiselects check.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckDimensionLimits(VisualizationTypeSelectionObject.DimensionInfo dimension, DimensionRange range, RejectionReason over, RejectionReason under)
        {
            if (dimension != null && range.DimensionRequired)
            {
                int dimensionSize = dimension.Size;
                if (dimensionSize < range.Min) yield return BuildRejectionInfo(under, dimensionSize, range.Min, dimension);
                if (dimensionSize > range.Max) yield return BuildRejectionInfo(over, dimensionSize, range.Max, dimension);
            }
        }


        /// <summary>
        /// Yields rejection enums if the provided dimension does not fit in the extra classifier dimension limits.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ChartRejectionInfo> CheckAdditionalDimensionsLimits(VisualizationTypeSelectionObject.DimensionInfo dimension, DimensionRange additionalDimensionsRange)
        {
            if (dimension == null && additionalDimensionsRange.DimensionRequired)
            {
                yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsRequired);
            }

            if (dimension != null)
            {
                if (additionalDimensionsRange.DimensionNotAllowed) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsNotAllowed, dimension);
                else
                {
                    if (dimension.Size < additionalDimensionsRange.Min) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsBelowMin, dimension.Size, additionalDimensionsRange.Min, dimension);
                    if (dimension.Size > additionalDimensionsRange.Max) yield return BuildRejectionInfo(RejectionReason.AdditionalDimensionsOverMax, dimension.Size, additionalDimensionsRange.Max, dimension);
                }
            }
        }

        /// <summary>
        /// Checks if the product of the multiselect dimension sizes is in an acceptable range.
        /// Some chart types need to modify this check, threfore it must be virtual for overloading.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ChartRejectionInfo> CheckMultiselectProductLimits(IEnumerable<VisualizationTypeSelectionObject.DimensionInfo> multiSelectDimensions, DimensionRange productOfMultiselectsRange)
        {
            int product = 1;
            foreach (VisualizationTypeSelectionObject.DimensionInfo dimension in multiSelectDimensions) product *= dimension.Size;

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
        protected ChartRejectionInfo BuildRejectionInfo(RejectionReason reason, int value, int threshold, VisualizationTypeSelectionObject.DimensionInfo dimension = null)
            => new(reason, GetPriority(reason), Type, value, threshold, dimension);

        protected ChartRejectionInfo BuildRejectionInfo(RejectionReason reason, VisualizationTypeSelectionObject.DimensionInfo dimension = null, string invalidValueName = null)
            => new(reason, GetPriority(reason), Type, null, null, dimension, invalidValueName);

        /// <summary>
        /// Each chart type has its own priority order for reasons of rejection,
        /// override this method to set that order.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected abstract int GetPriority(RejectionReason reason);

        protected static VisualizationTypeSelectionObject.DimensionInfo GetLargestMultiselect(VisualizationTypeSelectionObject input)
        {
            List<VisualizationTypeSelectionObject.DimensionInfo> multiselects = GetSortedMultiselects(input.Dimensions);
            return multiselects != null && multiselects.Count != 0 ? multiselects[0] : null;

        }

        protected static VisualizationTypeSelectionObject.DimensionInfo GetSmallerMultiselect(VisualizationTypeSelectionObject input)
        {
            List<VisualizationTypeSelectionObject.DimensionInfo> multiselects = GetSortedMultiselects(input.Dimensions);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimension if the query contains one, if not, returns a dimension which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        protected static VisualizationTypeSelectionObject.DimensionInfo GetTimeOrLargestOrdinal(IEnumerable<VisualizationTypeSelectionObject.DimensionInfo> dimensions)
        {
            List<VisualizationTypeSelectionObject.DimensionInfo> multiselects = GetSortedMultiselects(dimensions); //OBS: multiselects are sorted here so First() can be used!
            if (multiselects.Find(v => v.Type == DimensionType.Time) is VisualizationTypeSelectionObject.DimensionInfo tv) return tv;
            if (multiselects.Find(v => v.Type == DimensionType.Ordinal) is VisualizationTypeSelectionObject.DimensionInfo ov) return ov;

            return null;
        }

        private static List<VisualizationTypeSelectionObject.DimensionInfo> GetSortedMultiselects(IEnumerable<VisualizationTypeSelectionObject.DimensionInfo> dimensions)
        {
            return [.. dimensions
                .Where(dimension => dimension.Size > 1)
                .OrderByDescending(x => x.Size)];
        }

        /// <summary>
        /// Helper for getting priority from list
        /// </summary>
        /// <param name="reasonPriorityList">List where RejectionReasons are in order</param>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected static int GetPriorityIndex(IList<RejectionReason> reasonPriorityList, RejectionReason reason)
        {
            int index = reasonPriorityList.IndexOf(reason);
            if (index >= 0)
                return index + 1;
            else
                return reasonPriorityList.Count + 1;
        }
    }
}
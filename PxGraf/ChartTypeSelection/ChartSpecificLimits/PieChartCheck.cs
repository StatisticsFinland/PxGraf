using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the pie chart spesific rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class PieChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {

        // Elimination conditions and priorities:
        // 1. Number of multiselect dimension
        // 2. Number of selections from the content dimension
        // 3. Number of selections from the time dimension
        // 4. Data can not contain negative values.
        // 5. Combination values are not allowed (Fixed)
        // 6. Number of selections from the first multiselect dimension

        /// <summary>
        /// Pie Chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.PieChart;

        /// <summary>
        /// Checks the chart specific rules for PieCharts
        /// </summary>
        /// <param name="input"></param>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            var largestMultiselect = GetLargestMultiselect(input);
            if (largestMultiselect != null && !string.IsNullOrEmpty(largestMultiselect.CombinationValueCode))
            {
                yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, largestMultiselect, largestMultiselect.CombinationValueCode);
            }

            if (input.HasNegativeData) yield return BuildRejectionInfo(RejectionReason.NegativeDataNotAllowed);
        }

        /// <summary>
        /// Sets the priority for differenet rejection reasons.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected override int GetPriority(RejectionReason reason)
        {
            var reasons = new RejectionReason[]
            {
                RejectionReason.NotEnoughMultiselections,
                RejectionReason.TooManyMultiselections,
                RejectionReason.ContentRequired,
                RejectionReason.ContentNotAllowed,
                RejectionReason.UnambiguousContentUnitRequired,
                RejectionReason.ContentBelowMin,
                RejectionReason.ContentOverMax,
                RejectionReason.ContentUnitsBelowMin,
                RejectionReason.ContentUnitsOverMax,
                RejectionReason.TimeRequired,
                RejectionReason.TimeNotAllowed,
                RejectionReason.TimeBelowMin,
                RejectionReason.TimeOverMax,
                RejectionReason.NegativeDataNotAllowed,
                RejectionReason.CombinationValuesNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

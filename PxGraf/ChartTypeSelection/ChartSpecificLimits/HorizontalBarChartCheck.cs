using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the basic horizontal bar chart spesific rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class HorizontalBarChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {
        // Elimination conditions and priorities:
        // 1. Minimum number of multiselect dimension
        // 2. Maximum number of multiselect dimension
        // 3. Number of selections in the content diemsnion
        // 4. Number of selections in the time dimension
        // 5. Progressive dimensions are not allowed (Fixed rule, but could be made adjustable)
        // 6. Number of values in the greater multisect dimension

        /// <summary>
        /// Horizontal bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.HorizontalBarChart;

        /// <summary>
        /// Checks query compatibility with horizontal bar charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            var largestMultiselect = GetLargestMultiselect(input);
            if (largestMultiselect != null && largestMultiselect.Type == DimensionType.Ordinal)
            {
                yield return BuildRejectionInfo(RejectionReason.ProgressiveNotAllowed, largestMultiselect);
            }
        }

        /// <summary>
        /// Sets the chart type spesific priorities for differenet rejection reasons.
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
                RejectionReason.ProgressiveNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the basic vertical bar chart specific rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class VerticalBarChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {

        // Elimination conditions and priorities:
        // 1. Minimum number of multiselect dimensions
        // 2. Maximum number of multiselect dimensions
        // 3. Number of values selected from the content dimension 
        // 4. Multiselect dimension must be either time or progressive (Fixed rule)
        // 5. Maximum number of selections from the multiselect dimension

        /// <summary>
        /// Vertical bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.VerticalBarChart;


        /// <summary>
        /// Checks if provided query can be presented as a vertical bar chart
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            if (GetTimeOrLargestOrdinal(input.Dimensions) == null)
            {
                yield return BuildRejectionInfo(RejectionReason.TimeOrProgressiveRequired);
            }
        }

        /// <summary>
        /// Sets the priority for different rejection reasons.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected override int GetPriority(RejectionReason reason)
        {
            RejectionReason[] reasons =
            [
                RejectionReason.NotEnoughMultiselections,
                RejectionReason.TooManyMultiselections,
                RejectionReason.ContentRequired,
                RejectionReason.ContentNotAllowed,
                RejectionReason.UnambiguousContentUnitRequired,
                RejectionReason.ContentBelowMin,
                RejectionReason.ContentOverMax,
                RejectionReason.ContentUnitsBelowMin,
                RejectionReason.ContentUnitsOverMax,
                RejectionReason.TimeOrProgressiveRequired,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
            ];

            return GetPriorityIndex(reasons, reason);
        }
    }
}

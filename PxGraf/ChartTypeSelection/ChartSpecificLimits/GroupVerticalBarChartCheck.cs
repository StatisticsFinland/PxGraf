using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the group vertical bar chart spesific rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class GroupVerticalBarChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {
        // Elimination conditions and priorities:
        // 1. Minumum number of multiselect dimensions
        // 2. Maximum number of multiselect dimensions
        // 3. Number of values in the content dimension
        // 4. One of the multiselect dimensions must be either time or progressive
        // 5. Multiselect dimensions cannot have combination values
        // 6. Number of values in the greater multiselect dimension
        // 7. Number of values in the smaller multiselect dimension
        // 8. Value of the product of the sizes of the multiselect dimensions.

        /// <summary>
        /// Group vertical bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.GroupVerticalBarChart;

        /// <summary>
        /// Checks query compatibility with group vertical bar charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            if (GetTimeOrLargestOrdinal(input.Variables) is null)
            {
                yield return BuildRejectionInfo(RejectionReason.TimeOrProgressiveRequired);
            }

            var largestMultiselect = GetLargestMultiselect(input);
            if (largestMultiselect != null && !string.IsNullOrEmpty(largestMultiselect.CombinationValueCode))
            {
                yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, largestMultiselect, largestMultiselect.CombinationValueCode);
            }

            var smallerMultiselect = GetSmallerMultiselect(input);
            if (smallerMultiselect != null && !string.IsNullOrEmpty(smallerMultiselect.CombinationValueCode))
            {
                yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, smallerMultiselect, smallerMultiselect.CombinationValueCode);
            }
        }

        /// <summary>
        /// Provides the priorities for different rejection reasons.
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
                RejectionReason.TimeOrProgressiveRequired,
                RejectionReason.CombinationValuesNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.SecondMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
                RejectionReason.MultiselectProductOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

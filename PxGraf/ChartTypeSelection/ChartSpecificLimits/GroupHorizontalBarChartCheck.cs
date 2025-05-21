using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the group horizontal bar charts rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class GroupHorizontalBarChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {

        // Elimination conditions and priorities:
        // 1. Minimum number of multiselect dimensions
        // 2. Maximum number of multiselect dimensions
        // 3. Number of selections from the content dimension
        // 4. Progressive dimensions are not allowed (Fixed rule, but could be made adjustable)
        // 5. Number of selections from the time dimension
        // 6. Combination dimensions are not allowed (Fixed rule)
        // 7. Number of values in the greater multiselect dimension.
        // 8. Number of values in the smaller multiselect dimension.
        // 9. Value of the product of the sizes of the multiselect dimensions.

        /// <summary>
        /// Group horizontal bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.GroupHorizontalBarChart;

        /// <summary>
        /// Checks query compatibility with group horizontal bar charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            VisualizationTypeSelectionObject.DimensionInfo largestMultiselect = GetLargestMultiselect(input);
            if (largestMultiselect != null)
            {
                if (largestMultiselect.Type == DimensionType.Ordinal)
                {
                    yield return BuildRejectionInfo(RejectionReason.ProgressiveNotAllowed, largestMultiselect);
                }

                if (!string.IsNullOrEmpty(largestMultiselect.CombinationValueCode))
                {
                    yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, largestMultiselect, largestMultiselect.CombinationValueCode);
                }
            }

            VisualizationTypeSelectionObject.DimensionInfo smallerMultiselect = GetSmallerMultiselect(input);
            if (smallerMultiselect != null)
            {
                if (smallerMultiselect.Type == DimensionType.Ordinal)
                {
                    yield return BuildRejectionInfo(RejectionReason.ProgressiveNotAllowed, smallerMultiselect);
                }
                if (!string.IsNullOrEmpty(smallerMultiselect.CombinationValueCode))
                {
                    yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, smallerMultiselect, smallerMultiselect.CombinationValueCode);
                }
            }
        }

        /// <summary>
        /// Returns the grouped horizontal bar chart specific priorities for rejection reasons
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected override int GetPriority(RejectionReason reason)
        {
            RejectionReason[] reasons =
            [
                RejectionReason.NotEnoughMultiselections,
                RejectionReason.TooManyMultiselections,
                RejectionReason.ContentNotAllowed,
                RejectionReason.ContentRequired,
                RejectionReason.UnambiguousContentUnitRequired,
                RejectionReason.ContentBelowMin,
                RejectionReason.ContentOverMax,
                RejectionReason.ContentUnitsBelowMin,
                RejectionReason.ContentUnitsOverMax,
                RejectionReason.ProgressiveNotAllowed,
                RejectionReason.TimeNotAllowed,
                RejectionReason.TimeRequired,
                RejectionReason.TimeBelowMin,
                RejectionReason.TimeOverMax,
                RejectionReason.CombinationValuesNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.SecondMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
                RejectionReason.MultiselectProductOverMax,
            ];

            return GetPriorityIndex(reasons, reason);
        }
    }
}

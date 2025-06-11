using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the stacked horizontal bar chart specific rules.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="limits"></param>
    public class StackedHorizontalBarChartCheck(IChartTypeLimits limits) : ChartRulesCheck(limits)
    {
        // Elimination conditions and priorities:
        // 1. Minimum number of multiselect dimensions
        // 2. Maximum number of multiselect dimensions
        // 3. Number of selections from the content dimension
        // 4. The time dimension is not allowed
        // 5. Progressive dimensions are not allowed (Fixed rule, but could be made adjustable)
        // 6. Data can not contain negative values.
        // 7. Multiselect dimensions cannot have combination values (Fixed rule)
        // 8. Number of values in the greater multiselect dimension
        // 9. Number of values in the smaller multiselect dimension

        /// <summary>
        /// Stacked horizontal bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.StackedHorizontalBarChart;


        /// <summary>
        /// Checks query compatibility with stacked horizontal charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            // Check for progressive dimensions
            VisualizationTypeSelectionObject.DimensionInfo largestMultiselect = GetLargestMultiselect(input);
            VisualizationTypeSelectionObject.DimensionInfo smallerMultiselect = GetSmallerMultiselect(input);

            if (largestMultiselect != null && largestMultiselect.Type == DimensionType.Ordinal)
            {
                yield return BuildRejectionInfo(RejectionReason.ProgressiveNotAllowed, largestMultiselect);
            }
            if (smallerMultiselect != null && smallerMultiselect.Type == DimensionType.Ordinal)
            {
                yield return BuildRejectionInfo(RejectionReason.ProgressiveNotAllowed, smallerMultiselect);
            }

            // Check for combination values
            if (largestMultiselect != null && !string.IsNullOrEmpty(largestMultiselect.CombinationValueCode))
            {
                yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, largestMultiselect, largestMultiselect.CombinationValueCode);
            }
            if (smallerMultiselect != null && !string.IsNullOrEmpty(smallerMultiselect.CombinationValueCode))
            {
                yield return BuildRejectionInfo(RejectionReason.CombinationValuesNotAllowed, smallerMultiselect, smallerMultiselect.CombinationValueCode);
            }

            if (input.HasNegativeData) yield return BuildRejectionInfo(RejectionReason.NegativeDataNotAllowed);
        }

        /// <summary>
        /// Provides the priorities for different rejection reasons.
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
                RejectionReason.TimeRequired,
                RejectionReason.TimeNotAllowed,
                RejectionReason.TimeBelowMin,
                RejectionReason.TimeOverMax,
                RejectionReason.ProgressiveNotAllowed,
                RejectionReason.NegativeDataNotAllowed,
                RejectionReason.CombinationValuesNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.SecondMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
            ];

            return GetPriorityIndex(reasons, reason);
        }
    }
}

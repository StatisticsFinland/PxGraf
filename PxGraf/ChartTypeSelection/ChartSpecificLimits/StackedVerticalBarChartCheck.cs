﻿using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the stacked vertical bar chart spesific rules.
    /// </summary>
    public class StackedVerticalBarChartCheck : ChartRulesCheck
    {
        // Elimination conditions and priorities:
        // 1. Minumum number of multiselect dimensions
        // 2. Maximum number of multiselect dimensions
        // 3. Number of values in the content dimension
        // 4. One of the multiselect dimensions must be either time or progressive (Fixed rule)
        // 5. Data can not contain negative values.
        // 6. Multiselect dimensions cannot have combination values (Fixed rule)
        // 7. Number of values in the greater multiselect dimension
        // 8. Number of values in the smaller multiselect dimension

        /// <summary>
        /// Stacked vertical bar chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.StackedVerticalBarChart;

        /// <summary>
        /// Default constructor
        /// </summary>
        public StackedVerticalBarChartCheck(IChartTypeLimits limits) : base(limits) {}

        /// <summary>
        /// Checks if provided query can be presented as a stacked vertical bar chart
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

            if (input.HasNegativeData) yield return BuildRejectionInfo(RejectionReason.NegativeDataNotAllowed);
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
                RejectionReason.NegativeDataNotAllowed,
                RejectionReason.CombinationValuesNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.SecondMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the pyramid chart spesific rules.
    /// </summary>
    public class PyramidChartCheck : ChartRulesCheck
    {
        // Elimination conditions and priorities:
        // 1. Number of multiselect dimensions
        // 2. Data can not contain negative values.
        // 3. Minumum number of selections from the greater multiselect dimension
        // 4. Number of selections from the content dimension
        // 5. Number of selections from the time dimension
        // 6. Number of selections from the smaller multiselect dimension.
        // 7. Atlest one of the multiselect dimensions must be progressive. (Fixed)
        // 8. Maximum number of selections from the greater multiselect dimension.
        // 9. Combination values are not allowed (Fixed)

        /// <summary>
        /// Pyramid chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.PyramidChart;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PyramidChartCheck(IChartTypeLimits limits) : base(limits) {}

        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            var largestMultiselect = GetLargestMultiselect(input);
            var smallerMultiselect = GetSmallerMultiselect(input);

            if ((largestMultiselect == null || largestMultiselect.Type != VariableType.Ordinal) &&
            (smallerMultiselect == null || smallerMultiselect.Type != VariableType.Ordinal))
            {
                yield return BuildRejectionInfo(RejectionReason.ProgressiveRequired);
            }

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
                RejectionReason.NegativeDataNotAllowed,
                RejectionReason.FirstMultiselectBelowMin, // <- special
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
                RejectionReason.SecondMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
                RejectionReason.ProgressiveRequired,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.CombinationValuesNotAllowed,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the line chart spesific rules.
    /// </summary>
    public class LineChartCheck : ChartRulesCheck
    {
        // Elimination conditions and priorities:
        // 1. Must contain time or progressive multiselect dimension
        // 2. Number of selections from time or progressive dimension (separate limit for irregular time)
        // 3. Value of the product of the sizes of the multiselect dimensions.

        /// <summary>
        /// Line chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.LineChart;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineChartCheck(IChartTypeLimits limits) : base(limits) { }


        /// <summary>
        /// Checks query compatibility with line charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            if (GetTimeOrLargestOrdinal(input.Variables) is null) yield return BuildRejectionInfo(RejectionReason.TimeOrProgressiveRequired);
        }

        /// <summary>
        /// Used for determining if the number of lines in the resulting chart will be withing acceptable limits.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ChartRejectionInfo> CheckMultiselectProductLimits(IEnumerable<VisualizationTypeSelectionObject.VariableInfo> multiSelectVars, DimensionRange productOfMultiselectsRange)
        {
            int product = 1;
            var timeOrLO = GetTimeOrLargestOrdinal(multiSelectVars);
            foreach (var variable in multiSelectVars)
            {
                // The variable on the x axis will not affect the number of lines.
                if (variable != timeOrLO) product *= variable.Size;
            }

            if (product < productOfMultiselectsRange.Min) yield return BuildRejectionInfo(RejectionReason.MultiselectProductBelowMin, product, productOfMultiselectsRange.Min);
            if (product > productOfMultiselectsRange.Max) yield return BuildRejectionInfo(RejectionReason.MultiselectProductOverMax, product, productOfMultiselectsRange.Max);
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
                RejectionReason.UnambiguousContentUnitRequired,
                RejectionReason.TimeOrProgressiveRequired,
                RejectionReason.IrregularTimeNotAllowed,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
                RejectionReason.MultiselectProductOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

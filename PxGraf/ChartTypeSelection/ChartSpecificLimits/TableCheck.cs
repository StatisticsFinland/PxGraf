using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the pie chart spesific rules.
    /// </summary>
    public class TableCheck : ChartRulesCheck
    {

        // Elimination conditions and priorities:
        // 1. Number of multiselect dimension.
        // 2. Value of the product of the sizes of the multiselect dimensions.
        // 3. Number of selections from the first multiselect dimension.
        // 4. Number of selections from the second multiselect dimension.
        // 5. Number of selections from the other multiselect dimensions.

        /// <summary>
        /// Pie Chart
        /// </summary>
        public override VisualizationType Type => VisualizationType.Table;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="limits"></param>
        public TableCheck(IChartTypeLimits limits) : base(limits) { }

        /// <summary>
        /// Table does not have any unique limits.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input) => Enumerable.Empty<ChartRejectionInfo>();

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
                RejectionReason.MultiselectProductBelowMin,
                RejectionReason.MultiselectProductOverMax,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.FirstMultiselectOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}
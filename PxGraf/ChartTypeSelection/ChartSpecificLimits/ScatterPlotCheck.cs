using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Functionality to check the query compatibility with the scatter plot spesific rules.
    /// </summary>
    public class ScatterPlotCheck : ChartRulesCheck
    {

        // Elimination conditions and priorities:
        // 1. Number of multiselect dimensions.
        // 2. Number of selections from the content dimension.
        // 3. Number of selections from the greater multiselect dimension.

        /// <summary>
        /// Scatter plot
        /// </summary>
        public override VisualizationType Type => VisualizationType.ScatterPlot;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="limits"></param>
        public ScatterPlotCheck(IChartTypeLimits limits) : base(limits) {}

        /// <summary>
        /// Checks query compatibility with pyramid charts fixed rules.
        /// </summary>
        protected override IEnumerable<ChartRejectionInfo> CheckChartSpecificRules(VisualizationTypeSelectionObject input)
        {
            // No fixed rules for scatter plots at the moment.
            return Enumerable.Empty<ChartRejectionInfo>();
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
                RejectionReason.ContentNotAllowed,
                RejectionReason.ContentRequired,
                RejectionReason.ContentBelowMin,
                RejectionReason.ContentOverMax,
                RejectionReason.NotEnoughMultiselections,
                RejectionReason.TooManyMultiselections,
                RejectionReason.UnambiguousContentUnitRequired,
                RejectionReason.ContentUnitsBelowMin,
                RejectionReason.ContentUnitsOverMax,
                RejectionReason.FirstMultiselectBelowMin,
                RejectionReason.SecondMultiselectOverMax,
            };

            return GetPriorityIndex(reasons, reason);
        }
    }
}

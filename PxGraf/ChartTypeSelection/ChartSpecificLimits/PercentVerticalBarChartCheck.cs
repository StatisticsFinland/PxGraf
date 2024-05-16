using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Selection rules for percent vertical bar chart are identical to stacked vertical bar chart
    /// </summary>
    public class PercentVerticalBarChartCheck : StackedVerticalBarChartCheck
    {
        public override VisualizationType Type => VisualizationType.PercentVerticalBarChart;

        /// <summary>
        /// Default constructor, calls the constructor of StackedVerticalBarChartCheck
        /// </summary>
        /// <param name="limits"></param>
        public PercentVerticalBarChartCheck(IChartTypeLimits limits) : base(limits) {}
    }
}

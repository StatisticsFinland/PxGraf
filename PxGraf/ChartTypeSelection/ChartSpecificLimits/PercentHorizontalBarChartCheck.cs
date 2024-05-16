using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Selection rules for percent horizontal bar chart are identical to stacked horizontal bar chart
    /// </summary>
    public class PercentHorizontalBarChartCheck : StackedHorizontalBarChartCheck
    {
        public override VisualizationType Type => VisualizationType.PercentHorizontalBarChart;

        /// <summary>
        /// Default constructor, calls the constructor of StackedHorizontalBarChartCheck
        /// </summary>
        /// <param name="limits"></param>
        public PercentHorizontalBarChartCheck(IChartTypeLimits limits) : base(limits)
        {
        }
    }
}

using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Selection rules for percent horizontal bar chart are identical to stacked horizontal bar chart
    /// </summary>
    /// <remarks>
    /// Default constructor, calls the constructor of StackedHorizontalBarChartCheck
    /// </remarks>
    /// <param name="limits"></param>
    public class PercentHorizontalBarChartCheck(IChartTypeLimits limits) : StackedHorizontalBarChartCheck(limits)
    {
        public override VisualizationType Type => VisualizationType.PercentHorizontalBarChart;
    }
}

using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;

namespace PxGraf.ChartTypeSelection.ChartSpecificLimits
{
    /// <summary>
    /// Selection rules for percent vertical bar chart are identical to stacked vertical bar chart
    /// </summary>
    /// <remarks>
    /// Default constructor, calls the constructor of StackedVerticalBarChartCheck
    /// </remarks>
    /// <param name="limits"></param>
    public class PercentVerticalBarChartCheck(IChartTypeLimits limits) : StackedVerticalBarChartCheck(limits)
    {
        public override VisualizationType Type => VisualizationType.PercentVerticalBarChart;
    }
}

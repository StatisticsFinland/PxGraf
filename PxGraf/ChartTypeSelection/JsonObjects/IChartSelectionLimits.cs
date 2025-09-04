
namespace PxGraf.ChartTypeSelection.JsonObjects
{
    /// <summary>
    /// Readonly numeric limits for each possible chart type
    /// </summary>
    public interface IChartSelectionLimits
    {
        /// <summary>
        /// Numeric limits for basic vertical bar charts
        /// </summary>
        public IChartTypeLimits VerticalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for group vertical bar charts
        /// </summary>
        public IChartTypeLimits GroupVerticalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for stacked vertical bar charts
        /// </summary>
        public IChartTypeLimits StackedVerticalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for basic horizontal bar charts
        /// </summary>
        public IChartTypeLimits HorizontalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for group horizontal bar charts
        /// </summary>
        public IChartTypeLimits GroupHorizontalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for stacked horizontal bar charts
        /// </summary>
        public IChartTypeLimits StackedHorizontalBarChartLimits { get; }

        /// <summary>
        /// Numeric limits for pie charts
        /// </summary>
        public IChartTypeLimits PieChartLimits { get; }

        /// <summary>
        /// Numeric limits for scatter plots
        /// </summary>
        public IChartTypeLimits ScatterPlotLimits { get; }

        /// <summary>
        /// Numeric limits for line charts
        /// </summary>
        public IChartTypeLimits LineChartLimits { get; }

        /// <summary>
        /// Numeric limits for pyramid charts
        /// </summary>
        public IChartTypeLimits PyramidChartLimits { get; }

        /// <summary>
        /// Numeric limits for table
        /// </summary>
        public IChartTypeLimits TableLimits { get; }

        /// <summary>
        /// Numeric limits for key figure
        /// </summary>
        public IChartTypeLimits KeyFigureLimits { get; }
    }
}

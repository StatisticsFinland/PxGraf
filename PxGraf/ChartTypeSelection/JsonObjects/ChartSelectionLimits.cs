using Newtonsoft.Json;

namespace PxGraf.ChartTypeSelection.JsonObjects
{
    /// <summary>
    /// Collection of limits to determine which chart types can be drawn from the data.
    /// This class is meant to be possible to construct from a json and its state should not change afterwards.
    /// IMPORTANT: USE ONLY VIA THE IChartSelectionLimits INTERFACE WHEN NOT PARSING/UNPARSING JSON!
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell", "S1192:String literals should not be duplicated",
        Justification = "Makes it harder to read if some of the values are strings and some are constants")]
    public class ChartSelectionLimits : IChartSelectionLimits
    {
        /// <summary>
        /// Limits for basic vertical bar chart
        /// </summary>
        [JsonProperty(nameof(VerticalBarChart))]
        public ChartTypeLimits VerticalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "1",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "Ignore",
                IrregularTimeSelection = "0-10",
                FirstMultiselect = "2-999",
                SecondMultiselect = "Not allowed",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        /// <summary>
        /// Limits for basic vertical bar chart provided in the json or as default values.
        /// </summary>
        public IChartTypeLimits VerticalBarChartLimits => VerticalBarChart;

        /// <summary>
        /// Numerical limits for the group vertical bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        [JsonProperty(nameof(GroupVerticalBarChart))]
        public ChartTypeLimits GroupVerticalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "Ignore",
                IrregularTimeSelection = "0-10",
                FirstMultiselect = "1-20",
                SecondMultiselect = "2-4",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "1-40"
            };

        /// <summary>
        /// Limits for group vertical bar chart provided in the json or as default values.
        /// </summary>
        public IChartTypeLimits GroupVerticalBarChartLimits => GroupVerticalBarChart;

        /// <summary>
        /// Numerical limits for the stacked vertical bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        [JsonProperty(nameof(StackedVerticalBarChart))]
        public ChartTypeLimits StackedVerticalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "Ignore",
                IrregularTimeSelection = "0-10",
                FirstMultiselect = "2-40",
                SecondMultiselect = "2-10",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        /// <summary>
        /// Limits for stacked vertical bar chart provided in the json or as default values.
        /// </summary>
        public IChartTypeLimits StackedVerticalBarChartLimits => StackedVerticalBarChart;

        /// <summary>
        /// Numerical limits for the horizontal bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits HorizontalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "1",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "1",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "2-30",
                SecondMultiselect = "Not allowed",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        /// <summary>
        /// Limits for basic horizontal bar chart provided in the json or as default values.
        /// </summary>
        public IChartTypeLimits HorizontalBarChartLimits => HorizontalBarChart;

        /// <summary>
        /// Numerical limits for the group horizontal bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits GroupHorizontalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "0-2",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "2-20",
                SecondMultiselect = "2-4",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "1-40"
            };

        /// <summary>
        /// Numerical limits for the group horizontal bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public IChartTypeLimits GroupHorizontalBarChartLimits => GroupHorizontalBarChart;

        /// <summary>
        /// Numerical limits for the stacked horizontal bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits StackedHorizontalBarChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "0-1",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "2-30",
                SecondMultiselect = "2-10",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        /// <summary>
        /// Numerical limits for the stacked horizontal bar chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public IChartTypeLimits StackedHorizontalBarChartLimits => StackedHorizontalBarChart;

        /// <summary>
        /// Numerical limits for the pie chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits PieChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "1",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "1",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "2-10",
                SecondMultiselect = "Not allowed",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        public IChartTypeLimits PieChartLimits => PieChart;

        /// <summary>
        /// Numerical limits for the scatter plot
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits ScatterPlot { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2", // Multi content selection and one additional
                ContentSelection = "2",
                ContentUnitSelection = "Ignore",
                TimeSelection = "Ignore",
                FirstMultiselect = "2-999",
                SecondMultiselect = "2-999",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        public IChartTypeLimits ScatterPlotLimits => ScatterPlot;

        /// <summary>
        /// Numerical limits for the line chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits LineChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "1-999",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "Ignore",
                IrregularTimeSelection = "Not allowed",
                FirstMultiselect = "2-999",
                SecondMultiselect = "Ignore",
                AdditionalMultiselectDimensions = "0-5",
                ProductOfMultiselects = "0-10"
            };

        public IChartTypeLimits LineChartLimits => LineChart;

        /// <summary>
        /// Numerical limits for the pie chart
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits PyramidChart { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "2",
                ContentSelection = "Ignore",
                ContentUnitSelection = "1",
                TimeSelection = "1",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "3-999",
                SecondMultiselect = "2",
                AdditionalMultiselectDimensions = "0-1",
                ProductOfMultiselects = "Ignore"
            };

        public IChartTypeLimits PyramidChartLimits => PyramidChart;

        /// <summary>
        /// Numerical limits for tables
        /// Some additional conditions are implemented in the class
        /// </summary>
        public ChartTypeLimits Table { get; set; } =
            new ChartTypeLimits()
            {
                NumberOfMultiselects = "0-20",
                ContentSelection = "Ignore",
                ContentUnitSelection = "Ignore",
                TimeSelection = "Ignore",
                IrregularTimeSelection = "Ignore",
                FirstMultiselect = "0-10000",
                SecondMultiselect = "0-10000",
                AdditionalMultiselectDimensions = "0-10000",
                ProductOfMultiselects = "1-100000"
            };

        public IChartTypeLimits TableLimits => Table;
    }
}

using System.Text.Json.Serialization;

namespace PxGraf.Enums
{
    /// <summary>
    /// Possible visualization types PxGraf can produce
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VisualizationType
    {
        /// <summary>
        /// Basic one dimensional vertical bar chart
        /// </summary>
        VerticalBarChart,

        /// <summary>
        /// Two dimensional vertical bar chart, where one dimension represents bar groups and the other individual bars within the groups
        /// </summary>
        GroupVerticalBarChart,

        /// <summary>
        /// Two dimensional vertical bar chart, where one dimension represents the bars and the other the layers within the bars
        /// </summary>
        StackedVerticalBarChart,

        /// <summary>
        /// Two dimensional vertical bar chart, where one dimension represents the bars and the other the layers within the bars as percent of the combined values of the bar
        /// </summary>
        PercentVerticalBarChart,

        /// <summary>
        /// Basic one dimensional horizontal bar chart
        /// </summary>
        HorizontalBarChart,

        /// <summary>
        /// Two dimensional horizontal bar chart, where one dimension represents bar groups and the other individual bars within the groups
        /// </summary>
        GroupHorizontalBarChart,

        /// <summary>
        /// Two dimensional horizontal bar chart, where one dimension represents the bars and the other the layers within the bars
        /// </summary>
        StackedHorizontalBarChart,

        /// <summary>
        /// Two dimensional horizontal bar chart, where one dimension represents the bars and the other the layers within the bars as percent of the combined values of the bar
        /// </summary>
        PercentHorizontalBarChart,

        /// <summary>
        /// Two dimensional variant of a horizontal bar chart, where two one dimensional horizontal bar groups grow to the opposite directions
        /// </summary>
        PyramidChart,

        /// <summary>
        /// One dimensional pie chart that reflects the relative sizes of the values
        /// </summary>
        PieChart,

        /// <summary>
        /// One or multiline line chart that can visualize one or two dimensional data
        /// </summary>
        LineChart,

        /// <summary>
        /// Visualizes a set of two dimensional data points
        /// </summary>
        ScatterPlot,

        /// <summary>
        /// Tables can have any number of dimensions that are separated to row and column dimensions
        /// </summary>
        Table,

        /// <summary>
        /// Key figure visualization highlights a single data point with additional context information
        /// </summary>
        KeyFigure
    }
}
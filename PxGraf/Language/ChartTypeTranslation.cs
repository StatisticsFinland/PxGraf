using PxGraf.Enums;
using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<Translation>))]
    public class ChartTypeTranslation
    {
        public string VerticalBar { get; set; }
        public string GroupVerticalBar { get; set; }
        public string StackedVerticalBar { get; set; }
        public string PercentVerticalBar { get; set; }
        public string HorizontalBar { get; set; }
        public string GroupHorizontalBar { get; set; }
        public string StackedHorizontalBar { get; set; }
        public string PercentHorizontalBar { get; set; }
        public string Pyramid { get; set; }
        public string Pie { get; set; }
        public string Line { get; set; }
        public string ScatterPlot { get; set; }
        public string Table { get; set; }

        public string GetTranslation(VisualizationType chartType)
        {
            return chartType switch
            {
                VisualizationType.VerticalBarChart => VerticalBar,
                VisualizationType.GroupVerticalBarChart => GroupVerticalBar,
                VisualizationType.StackedVerticalBarChart => StackedVerticalBar,
                VisualizationType.PercentVerticalBarChart => PercentVerticalBar,
                VisualizationType.HorizontalBarChart => HorizontalBar,
                VisualizationType.GroupHorizontalBarChart => GroupHorizontalBar,
                VisualizationType.StackedHorizontalBarChart => StackedHorizontalBar,
                VisualizationType.PercentHorizontalBarChart => PercentHorizontalBar,
                VisualizationType.PyramidChart => Pyramid,
                VisualizationType.PieChart => Pie,
                VisualizationType.LineChart => Line,
                VisualizationType.ScatterPlot => ScatterPlot,
                VisualizationType.Table => Table,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}

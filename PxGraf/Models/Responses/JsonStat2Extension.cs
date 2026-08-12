using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses
{
    public class JsonStat2Extension
    {
        [JsonPropertyName("missingValueDescriptions")]
        public IReadOnlyDictionary<string, string> MissingValueDescriptions { get; set; }

        [JsonPropertyName("selectableConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SelectableConfig SelectableConfig { get; set; }

        [JsonPropertyName("visualizationConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonStatVisualizationConfig VisualizationConfig { get; set; }

        [JsonPropertyName("jsonstatChart")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonStatChartExtension JsonStatChart { get; set; }
    }

    public class SelectableConfig
    {
        [JsonPropertyName("selectableSelections")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<string>> SelectableSelections { get; set; }

        [JsonPropertyName("defaultSelectableSelections")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<string>> DefaultSelectableSelections { get; set; }

        [JsonPropertyName("multiSelectableDimensionCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MultiSelectableDimensionCode { get; set; }
    }

    public class JsonStatVisualizationConfig
    {
        [JsonPropertyName("chartType")]
        public string ChartType { get; set; }

        [JsonPropertyName("layout")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonStatLayout Layout { get; set; }

        [JsonPropertyName("cutValueAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? CutValueAxis { get; set; }

        [JsonPropertyName("sorting")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Sorting { get; set; }
    }

    public class JsonStatLayout
    {
        [JsonPropertyName("rows")]
        public IReadOnlyList<string> Rows { get; set; }

        [JsonPropertyName("columns")]
        public IReadOnlyList<string> Columns { get; set; }
    }

    public class JsonStatChartExtension
    {
        [JsonPropertyName("sources")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonStatSourceExtension Sources { get; set; }
    }

    public class JsonStatSourceExtension
    {
        [JsonPropertyName("dimension")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string> Dimension { get; set; }

        [JsonPropertyName("category")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, Dictionary<string, string>> Category { get; set; }
    }
}

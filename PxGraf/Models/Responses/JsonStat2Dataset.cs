using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses
{
    public class JsonStat2Dataset
    {
        [JsonPropertyName("version")]
        [JsonPropertyOrder(1)]
        public string Version { get; set; } = "2.0";

        [JsonPropertyName("class")]
        [JsonPropertyOrder(2)]
        public string Class { get; set; } = "dataset";

        [JsonPropertyName("id")]
        [JsonPropertyOrder(3)]
        public IReadOnlyList<string> Id { get; set; }

        [JsonPropertyName("size")]
        [JsonPropertyOrder(4)]
        public IReadOnlyList<int> Size { get; set; }

        [JsonPropertyName("label")]
        [JsonPropertyOrder(5)]
        public string Label { get; set; }

        [JsonPropertyName("source")]
        [JsonPropertyOrder(6)]
        public string Source { get; set; }

        [JsonPropertyName("updated")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(7)]
        public string Updated { get; set; }

        [JsonPropertyName("note")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(8)]
        public IReadOnlyList<string> Note { get; set; }

        [JsonPropertyName("dimension")]
        [JsonPropertyOrder(9)]
        public IReadOnlyDictionary<string, JsonStat2Dimension> Dimension { get; set; }

        [JsonPropertyName("value")]
        [JsonPropertyOrder(10)]
        public IReadOnlyList<decimal?> Value { get; set; }

        [JsonPropertyName("status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(11)]
        public IReadOnlyDictionary<string, string> Status { get; set; }

        [JsonPropertyName("role")]
        [JsonPropertyOrder(12)]
        public JsonStat2Role Role { get; set; }

        [JsonPropertyName("extension")]
        [JsonPropertyOrder(13)]
        public JsonStat2Extension Extension { get; set; }
    }

    public class JsonStat2Dimension
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("note")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> Note { get; set; }

        [JsonPropertyName("category")]
        public JsonStat2Category Category { get; set; }
    }

    public class JsonStat2Category
    {
        [JsonPropertyName("index")]
        public IReadOnlyList<string> Index { get; set; }

        [JsonPropertyName("label")]
        public IReadOnlyDictionary<string, string> Label { get; set; }

        [JsonPropertyName("note")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Note { get; set; }

        [JsonPropertyName("unit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, JsonStat2CategoryUnit> Unit { get; set; }
    }

    public class JsonStat2CategoryUnit
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }
    }

    public class JsonStat2Role
    {
        [JsonPropertyName("time")]
        public IReadOnlyList<string> Time { get; set; }

        [JsonPropertyName("metric")]
        public IReadOnlyList<string> Metric { get; set; }

        [JsonPropertyName("geo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> Geo { get; set; }
    }

    public class JsonStat2Extension
    {
        [JsonPropertyName("missingValueDescriptions")]
        public IReadOnlyDictionary<string, string> MissingValueDescriptions { get; set; }

        [JsonPropertyName("visualizationSettings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VisualizationResponse.PxVisualizerSettings VisualizationSettings { get; set; }
    }
}
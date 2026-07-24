using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses
{
    public class JsonStat2Extension
    {
        [JsonPropertyName("missingValueDescriptions")]
        public IReadOnlyDictionary<string, string> MissingValueDescriptions { get; set; }

        [JsonPropertyName("visualizationSettings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VisualizationResponse.PxVisualizerSettings VisualizationSettings { get; set; }
    }
}

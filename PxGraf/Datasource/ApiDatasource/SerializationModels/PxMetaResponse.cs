using Px.Utils.Models.Metadata;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Datasource.PxWebInterface.SerializationModels
{
    public class PxMetaResponse : IMatrixMap
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("variables")]
        public Variable[] Variables { get; set; }

        [JsonIgnore]
        public IReadOnlyList<IDimensionMap> DimensionMaps => Variables;

        public class Variable : IDimensionMap
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("values")]
            public string[] Values { get; set; }

            [JsonPropertyName("valueTexts")]
            public string[] ValueTexts { get; set; }

            [JsonPropertyName("elimination")]
            public bool Elimination { get; set; }

            [JsonPropertyName("time")]
            public bool Time { get; set; }

            [JsonIgnore]
            public IReadOnlyList<string> ValueCodes => Values;
        }
    }
}

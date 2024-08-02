using Newtonsoft.Json;
using Px.Utils.Models.Metadata;
using System.Collections.Generic;

namespace PxGraf.Datasource.PxWebInterface.SerializationModels
{
    public class PxMetaResponse : IMatrixMap
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("variables")]
        public Variable[] Variables { get; set; }

        [JsonIgnore]
        public IReadOnlyList<IDimensionMap> DimensionMaps => Variables;

        public class Variable : IDimensionMap
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("values")]
            public string[] Values { get; set; }

            [JsonProperty("valueTexts")]
            public string[] ValueTexts { get; set; }

            [JsonProperty("elimination")]
            public bool Elimination { get; set; }

            [JsonProperty("time")]
            public bool Time { get; set; }

            [JsonIgnore]
            public IReadOnlyList<string> ValueCodes => Values;
        }
    }
}

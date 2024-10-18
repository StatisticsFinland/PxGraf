using System.Text.Json.Serialization;

namespace PxGraf.Datasource.PxWebInterface.SerializationModels
{
    public class PxWebDataQueryPostParams
    {
        [JsonPropertyName("query")]
        public VariableQuery[] Query { get; set; }

        [JsonPropertyName("response")]
        public ResponseInfo Response { get; set; }

        public class VariableQuery
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("selection")]
            public Selection Selectrion { get; set; }

            public class Selection
            {
                [JsonPropertyName("filter")]
                public string Filter { get; set; }

                [JsonPropertyName("values")]
                public string[] Values { get; set; }
            }
        }

        public class ResponseInfo
        {
            [JsonPropertyName("format")]
            public string Format { get; set; }
        }
    }
}

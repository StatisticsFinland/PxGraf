using System.Text.Json.Serialization;

namespace PxGraf.Datasource.ApiDatasource.SerializationModels
{
    public class PxWebJson
    {
        [JsonPropertyName("columns")]
        public Column[] Columns { get; set; }

        [JsonPropertyName("data")]
        public DataEntry[] Data { get; set; }

        public class Column
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("unit")]
            public string Unit { get; set; }

            [JsonPropertyName("comment")]
            public string Comment { get; set; }
        }

        public class DataEntry
        {
            [JsonPropertyName("key")]
            public string[] Key { get; set; }

            [JsonPropertyName("values")]
            public double[] Value { get; set; }

            [JsonPropertyName("comment")]
            public string[] Comment { get; set; }
        }
    }
}

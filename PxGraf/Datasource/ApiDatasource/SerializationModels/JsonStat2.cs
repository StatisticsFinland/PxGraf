using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Datasource.ApiDatasource.SerializationModels
{
    public class JsonStat2
    {
        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("updated")]
        public string Updated { get; set; }

        [JsonPropertyName("id")]
        public string[] Id { get; set; }

        [JsonPropertyName("size")]
        public int[] Size { get; set; }

        [JsonPropertyName("dimension")]
        public Dictionary<string, DimensionObj> Dimensions { get; set; }

        [JsonPropertyName("value")]
        public decimal?[] Value { get; set; }

        [JsonPropertyName("status")]
        public Dictionary<string, string> Status { get; set; }

        [JsonPropertyName("role")]
        public RoleObj Role { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("extension")]
        public dynamic Extension { get; set; }

        public class DimensionObj
        {
            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("category")]
            public CategoryObj Category { get; set; }

            [JsonPropertyName("link")]
            public LinkObj Link { get; set; }

            public class CategoryObj
            {
                [JsonPropertyName("index")]
                public Dictionary<string, int> Index { get; set; }

                [JsonPropertyName("label")]
                public Dictionary<string, string> Label { get; set; }

                /// <summary>
                /// From value code to UnitObj
                /// </summary>
                [JsonPropertyName("unit")]
                public Dictionary<string, UnitObj> Unit { get; set; }

                public class UnitObj
                {
                    [JsonPropertyName("base")]
                    public string Base { get; set; }

                    [JsonPropertyName("decimals")]
                    public int Decimals { get; set; }
                }
            }

            public class LinkObj
            {
                [JsonPropertyName("describedby")]
                public List<DescribedByObj> DescribedBy { get; set; }

                public class DescribedByObj
                {
                    [JsonPropertyName("extension")]
                    public Dictionary<string, string> Extension { get; set; }
                }
            }
        }

        public class RoleObj
        {
            [JsonPropertyName("time")]
            public string[] Time { get; set; }

            [JsonPropertyName("metric")]
            public string[] Metric { get; set; }

            [JsonPropertyName("geo")]
            public string[] Geo { get; set; }
        }
    }
}

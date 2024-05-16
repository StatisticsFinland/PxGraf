using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface.SerializationModels
{
    public class JsonStat2
    {
        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }

        [JsonProperty("id")]
        public string[] Id { get; set; }

        [JsonProperty("size")]
        public int[] Size { get; set; }

        [JsonProperty("dimension")]
        public Dictionary<string, DimensionObj> Dimensions { get; set; }

        [JsonProperty("value")]
        public double?[] Value { get; set; }

        [JsonProperty("status")]
        public Dictionary<string, string> Status { get; set; }

        [JsonProperty("role")]
        public RoleObj Role { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("extension")]
        public dynamic Extension { get; set; }

        public class DimensionObj
        {
            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("category")]
            public CategoryObj Category { get; set; }

            [JsonProperty("link")]
            public LinkObj Link { get; set; }

            public class CategoryObj
            {
                [JsonProperty("index")]
                public Dictionary<string, int> Index { get; set; }
                
                [JsonProperty("label")]
                public Dictionary<string, string> Label { get; set; }

                /// <summary>
                /// From value code to UnitObj
                /// </summary>
                [JsonProperty("unit")]
                public Dictionary<string, UnitObj> Unit { get; set; }

                public class UnitObj
                {
                    [JsonProperty("base")]
                    public string Base { get; set; }

                    [JsonProperty("decimals")]
                    public int Decimals { get; set; }
                }
            }

            public class LinkObj
            {
                [JsonProperty("describedby")]
                public List<DescribedByObj> DescribedBy { get; set; }

                public class DescribedByObj
                {
                    [JsonProperty("extension")]
                    public Dictionary<string, string> Extension { get; set; }
                }
            }
        }

        public class RoleObj
        {
            [JsonProperty("time")]
            public string[] Time { get; set; }
            
            [JsonProperty("metric")]
            public string[] Metric { get; set; }

            [JsonProperty("geo")]
            public string[] Geo { get; set; }
        }
    }
}

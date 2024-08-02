using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Datasource.PxWebInterface.SerializationModels
{
    public class PxWebJson
    {
        [JsonProperty("columns")]
        public Column[] Columns { get; set; }

        [JsonProperty("data")]
        public DataEntry[] Data { get; set; }

        public class Column
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("unit")]
            public string Unit { get; set; }

            [JsonProperty("comment")]
            public string Comment { get; set; }
        }

        public class DataEntry
        {
            [JsonProperty("key")]
            public string[] Key { get; set; }

            [JsonProperty("values")]
            public double[] Value { get; set; }

            [JsonProperty("comment")]
            public string[] Comment { get; set; }
        }
    }
}

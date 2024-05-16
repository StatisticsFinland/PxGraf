using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface.SerializationModels
{
    public class PxWebDataQueryPostParams
    {
        [JsonProperty("query")]
        public VariableQuery[] Query { get; set; }

        [JsonProperty("response")]
        public ResponseInfo Response { get; set; }

        public class VariableQuery
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("selection")]
            public Selection Selectrion { get; set; }

            public class Selection
            {
                [JsonProperty("filter")]
                public string Filter { get; set; }

                [JsonProperty("values")]
                public string[] Values { get; set; }
            }
        }

        public class ResponseInfo
        {
            [JsonProperty("format")]
            public string Format { get; set; }
        }
    }
}

﻿using Newtonsoft.Json;

namespace PxGraf.PxWebInterface.SerializationModels
{
    public class PxMetaResponse
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("variables")]
        public Variable[] Variables { get; set; }

        public class Variable
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
        }
    }
}

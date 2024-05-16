using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxGraf.Models.SavedQueries;
using PxGraf.Models.SavedQueries.Versions;
using System;

namespace PxGraf.Utility
{
    public class SavedQuerySerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(SavedQuery).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken obj = JToken.ReadFrom(reader);
            string version = GetPxGrafVersion(obj);

            return version switch
            {
                "1.0" => obj.ToObject<SavedQueryV10>(serializer).ToSavedQuery(),
                "1.1" => obj.ToObject<SavedQueryV11>(serializer).ToSavedQuery(),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // CanWrite == false => Use default serializer
            throw new NotImplementedException();
        }

        private static string GetPxGrafVersion(JToken queryToken)
        {
            var version = queryToken.SelectToken("Version");
            return version is null ? "1.0" : version.ToString();
        }
    }
}

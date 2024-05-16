using Newtonsoft.Json;
using System;

namespace PxGraf.Utility
{
    public class ForceStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override bool CanRead => true;

        public override string ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Date && reader.Value is DateTime dt)
            {
                // Return as an ISO 8601 formatted string
                return dt.ToString("o");
            }

            return reader.Value?.ToString();
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}


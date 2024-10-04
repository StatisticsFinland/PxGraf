#nullable enable
using PxGraf.Models.SavedQueries;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PxGraf.Utility
{
    public class SavedQuerySerializer : JsonConverter<SavedQuery>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(SavedQuery).IsAssignableFrom(objectType);
        }

        /*
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken obj = JToken.ReadFrom(reader);
            string version = GetPxGrafVersion(obj);

            return version switch
            {
                "1.0" => obj.ToObject<SavedQueryV10>().ToSavedQuery(),
                "1.1" => obj.ToObject<SavedQueryV11>().ToSavedQuery(),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }
        */

        public override Type Type => throw new NotImplementedException();

        /*
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // CanWrite == false => Use default serializer
            throw new NotImplementedException();
        }
        */

        public override SavedQuery Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
            string? version = jdoc.RootElement.GetProperty(nameof(SavedQuery.Version)).GetString()
                is string versionString ? versionString : "1.0";

            return version switch
            {
                "1.0" => obj.ToObject<SavedQueryV10>().ToSavedQuery(),
                "1.1" => obj.ToObject<SavedQueryV11>().ToSavedQuery(),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }

        public override void Write(Utf8JsonWriter writer, SavedQuery value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
#nullable disable

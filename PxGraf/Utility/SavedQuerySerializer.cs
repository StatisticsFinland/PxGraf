#nullable enable
using PxGraf.Models.SavedQueries;
using PxGraf.Models.SavedQueries.Versions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PxGraf.Utility
{
    public class SavedQuerySerializer : JsonConverter<SavedQuery>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(SavedQuery).IsAssignableFrom(typeToConvert);
        }

        public override SavedQuery Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
            string version = "1.0"; // Default version

            if (jdoc.RootElement.TryGetProperty(nameof(SavedQuery.Version), out JsonElement versionElement))
            {
                version = versionElement.GetString() ?? "1.0";
            }

            return version switch
            {
                "1.0" => JsonSerializer.Deserialize<SavedQueryV10>(jdoc.RootElement.GetRawText(), options)?.ToSavedQuery() ?? throw new JsonException("Failed to deserialize SavedQueryV10."),
                "1.1" => JsonSerializer.Deserialize<SavedQueryV11>(jdoc.RootElement.GetRawText(), options)?.ToSavedQuery() ?? throw new JsonException("Failed to deserialize SavedQueryV11."),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }

        public override void Write(Utf8JsonWriter writer, SavedQuery value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.Version switch
            {
                "1.0" => typeof(SavedQueryV10),
                "1.1" => typeof(SavedQueryV11),
                _ => throw new NotSupportedException($"Unknown version in saved query ({value.Version})."),
            }, options);
        }
    }
}
#nullable disable

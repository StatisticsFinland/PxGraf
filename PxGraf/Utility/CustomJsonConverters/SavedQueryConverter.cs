#nullable enable
using PxGraf.Models.SavedQueries.Versions;
using PxGraf.Models.SavedQueries;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class SavedQueryConverter : JsonConverter<SavedQuery>
    {
        public override SavedQuery Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
            string version = "1.0"; // Default version

            if (jdoc.RootElement.TryGetProperty(nameof(version), options, out JsonElement versionElement))
            {
                version = versionElement.GetString() ?? "1.0";
            }

            return version switch
            {
                "1.0" => JsonSerializer.Deserialize<SavedQueryV10>(jdoc.RootElement.GetRawText(), options)?.ToSavedQuery()
                    ?? throw new JsonException("Failed to deserialize SavedQueryV10."),
                "1.1" => JsonSerializer.Deserialize<SavedQueryV11>(jdoc.RootElement.GetRawText(), options)?.ToSavedQuery()
                    ?? throw new JsonException("Failed to deserialize SavedQueryV11."),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }

        public override void Write(Utf8JsonWriter writer, SavedQuery value, JsonSerializerOptions options)
        {
            SavedQueryV11 v11 = new(value);
            JsonSerializer.Serialize(writer, v11, options);
        }
    }
}
#nullable disable

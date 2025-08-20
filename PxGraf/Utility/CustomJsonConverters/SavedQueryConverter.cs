#nullable enable
using PxGraf.Models.SavedQueries;
using PxGraf.Models.SavedQueries.Versions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PxGraf.Models.SavedQueries.Versions.SavedQueryV11;

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

            VersionedSavedQuery versionedQuery = version switch
            {
                "1.0" => JsonSerializer.Deserialize<SavedQueryV10>(jdoc.RootElement.GetRawText(), options)
                    ?? throw new JsonException("Failed to deserialize SavedQueryV10."),
                "1.1" => JsonSerializer.Deserialize<SavedQueryV11>(jdoc.RootElement.GetRawText(), options)
                    ?? throw new JsonException("Failed to deserialize SavedQueryV11."),
                "1.2" => DeserializeV12(jdoc.RootElement, options),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version}).")
            };

            return versionedQuery.ToSavedQuery();
        }

        public override void Write(Utf8JsonWriter writer, SavedQuery value, JsonSerializerOptions options)
        {
            SavedQueryV12 v12 = new(value);
            JsonSerializer.Serialize(writer, v12, options);
        }

        private static SavedQueryV12 DeserializeV12(JsonElement root, JsonSerializerOptions options)
        {
            SavedQueryV12 v12 = JsonSerializer.Deserialize<SavedQueryV12>(root.GetRawText(), options) ?? throw new JsonException("Failed to deserialize SavedQueryV12.");
            if (root.TryGetProperty("Settings", out JsonElement settingsElement))
            {
                v12.Settings = JsonSerializer.Deserialize<VisualizationSettingsV11>(settingsElement.GetRawText(), options) ?? 
                    throw new JsonException("Failed to deserialize VisualizationSettingsV11 from SavedQueryV12.");
            }

            return v12;
        }
    }
}
#nullable disable

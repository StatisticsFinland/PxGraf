using System.Text.Json.Serialization;
using System.Text.Json;
using PxGraf.Models.SavedQueries.Versions;
using System;
using PxGraf.Utility.CustomJsonConverters;

namespace PxGraf.Models.SavedQueries
{
    public class ArchiveMatrixSerializer : JsonConverter<ArchiveCube>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ArchiveCube).IsAssignableFrom(typeToConvert);
        }

        public override ArchiveCube Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
            string version = "1.0"; // Default version

            if (jdoc.RootElement.TryGetProperty(nameof(ArchiveCube.Version), options, out JsonElement versionElement))
            {
                version = versionElement.GetString() ?? "1.0";
            }

            return version switch
            {
                "1.1" => JsonSerializer.Deserialize<ArchiveCubeV11>(jdoc.RootElement.GetRawText(), options)?.ToArchiveCube()
                    ?? throw new JsonException("Failed to deserialize SavedQueryV11."),
                "1.0" => JsonSerializer.Deserialize<ArchiveCubeV10>(jdoc.RootElement.GetRawText(), options)?.ToArchiveCube()
                    ?? throw new JsonException("Failed to deserialize SavedQueryV10."),
                _ => throw new NotSupportedException($"Unknown version in saved query ({version})."),
            };
        }

        public override void Write(Utf8JsonWriter writer, ArchiveCube value, JsonSerializerOptions options)
        {
            ArchiveCubeV11 v11 = new(value);
            JsonSerializer.Serialize(writer, v11, options);
        }
    }
}

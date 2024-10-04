using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PxGraf.Enums;

namespace PxGraf.Utility
{
    public class VisualizationTypeSerializer : JsonConverter<VisualizationType>
    {
        public override VisualizationType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            return Enum.TryParse<VisualizationType>(value, out var result) ? result : throw new JsonException($"Invalid VisualizationType: {value}");
        }

        public override void Write(Utf8JsonWriter writer, VisualizationType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

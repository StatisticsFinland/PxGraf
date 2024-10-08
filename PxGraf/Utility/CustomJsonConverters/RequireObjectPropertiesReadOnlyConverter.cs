using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class RequireObjectPropertiesReadOnlyConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            JsonElement jsonObject = jsonDocument.RootElement;

            foreach (PropertyInfo property in typeToConvert.GetProperties())
            {
                if (property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length == 0 &&
                    !jsonObject.TryGetProperty(property.Name, out _))
                {
                    throw new JsonException($"Missing required property: {property.Name}");
                }
            }

            return JsonSerializer.Deserialize<T>(jsonObject.GetRawText(), options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"{nameof(RequireObjectPropertiesReadOnlyConverter<T>)} only supports reading/deserialization.");
        }
    }
}

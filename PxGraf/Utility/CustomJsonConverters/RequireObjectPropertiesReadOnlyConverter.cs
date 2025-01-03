using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class RequireObjectPropertiesReadOnlyConverter<T> : JsonConverter<T> where T : class
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Json object was not valid.");
            }

            JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            JsonElement jsonObject = jsonDocument.RootElement;

            // Ensure the type is not null and has a parameterless constructor
            ArgumentNullException.ThrowIfNull(typeToConvert);

            if (typeToConvert.GetConstructor(Type.EmptyTypes) == null)
                throw new InvalidOperationException("The type must have a parameterless constructor.");

            T instance = Activator.CreateInstance(typeToConvert) as T;

            foreach (PropertyInfo property in typeToConvert.GetProperties())
            {
                if (property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0) continue;
                
                if(!jsonObject.TryGetProperty(property.Name, out JsonElement jsonProp))
                {
                    throw new JsonException($"Missing required property: {property.Name}");
                }
                else
                {
                    property.SetValue(instance, JsonSerializer.Deserialize(jsonProp.GetRawText(), property.PropertyType, options));
                }
            }

            return instance;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"{nameof(RequireObjectPropertiesReadOnlyConverter<T>)} only supports reading/deserialization.");
        }
    }
}

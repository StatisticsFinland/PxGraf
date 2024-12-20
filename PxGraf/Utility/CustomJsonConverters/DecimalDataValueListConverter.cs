using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class DecimalDataValueListConverter : JsonConverter<IReadOnlyList<DecimalDataValue>>
    {
        public override List<DecimalDataValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<DecimalDataValue> result = [];

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return result;
                }

                if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out decimal number))
                {
                    result.Add(new (number, DataValueType.Exists));
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    int index = Array.IndexOf(PxSyntaxConstants.MissingValueDotCodes, stringValue);
                    if (index < 1)
                    {
                        throw new JsonException($"Invalid missing value code: {stringValue}");
                    }
                    result.Add(new(0, (DataValueType)(index)));
                }
                else
                {
                    throw new JsonException("Unexpected token type.");
                }
            }

            throw new JsonException("Expected EndArray token.");
        }

        public override void Write(Utf8JsonWriter writer, IReadOnlyList<DecimalDataValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (DecimalDataValue item in value)
            {
                if (item.Type == DataValueType.Exists)
                {
                    writer.WriteNumberValue(item.UnsafeValue);
                }
                else
                {
                    int index = (int)item.Type;
                    if (index < 1 || index >= PxSyntaxConstants.MissingValueDotCodes.Length)
                    {
                        throw new JsonException($"Invalid DataValueType: {item.Type}");
                    }
                    writer.WriteStringValue(PxSyntaxConstants.MissingValueDotCodes[index]);
                }
            }

            writer.WriteEndArray();
        }
    }
}

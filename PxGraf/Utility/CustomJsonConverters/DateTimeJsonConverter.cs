using System.Globalization;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string dateTimeString = reader.GetString();
                return PxSyntaxConstants.ParseDateTime(dateTimeString);
            }
            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if(value.Kind is not DateTimeKind.Utc) value = value.ToUniversalTime();
            writer.WriteStringValue(value.ToString(PxSyntaxConstants.DATETIME_FORMAT_NO_MS_TS_ZERO, CultureInfo.InvariantCulture));
        }
    }
}

using System.Globalization;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string dateTimeString = reader.GetString();
                if (DateTime.TryParseExact(dateTimeString, PxSyntaxConstants.SQ_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
            }
            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(PxSyntaxConstants.SQ_DATETIME_FORMAT, CultureInfo.InvariantCulture));
        }
    }
}

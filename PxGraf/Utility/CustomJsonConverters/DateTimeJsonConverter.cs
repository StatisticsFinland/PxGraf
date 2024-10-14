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
                if (DateTime.TryParseExact(dateTimeString, PxSyntaxConstants.DATETIME_FORMAT_WITH_MS, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
                else if (DateTime.TryParseExact(dateTimeString, PxSyntaxConstants.DATETIME_FORMAT_NO_MS, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
                else if (DateTime.TryParseExact(dateTimeString, PxSyntaxConstants.DATETIME_FORMAT_NO_MS_Z, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    return dateTime;
                }
            }
            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(PxSyntaxConstants.DATETIME_FORMAT_NO_MS, CultureInfo.InvariantCulture));
        }
    }
}

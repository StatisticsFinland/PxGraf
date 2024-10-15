using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<HeaderTimeFormatsTranslation>))]
    public class HeaderTimeFormatsTranslation
    {
        public string SingleTimeValue { get; set; }
        public string TimeRange { get; set; }
    }
}

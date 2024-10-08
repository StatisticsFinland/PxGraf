using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<Translation>))]
    public class HeaderTimeFormatsTranslation
    {
        public string SingleTimeValue { get; set; }
        public string TimeRange { get; set; }
    }
}

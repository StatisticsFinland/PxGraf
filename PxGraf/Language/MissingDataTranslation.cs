using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<Translation>))]
    public class MissingDataTranslation
    {
        public string Missing { get; set; }
        public string CannotRepresent { get; set; }
        public string Confidential { get; set; }
        public string NotAcquired { get; set; }
        public string NotAsked { get; set; }
        public string Empty { get; set; }
        public string Nill { get; set; }
    }
}

using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<Translation>))]
    public class SortingOptionTranslation
    {
        public string Ascending { get; set; }
        public string Descending { get; set; }
        public string NoSorting { get; set; }
        public string Sum { get; set; }
        public string NoSortingReversed { get; set; }
    }
}

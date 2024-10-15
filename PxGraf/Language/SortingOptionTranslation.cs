using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<SortingOptionTranslation>))]
    public class SortingOptionTranslation
    {
        public string Ascending { get; set; }
        public string Descending { get; set; }
        public string NoSorting { get; set; }
        public string Sum { get; set; }
        public string NoSortingReversed { get; set; }
    }
}

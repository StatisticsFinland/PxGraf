using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<Translation>))]
    public class Translation
    {
        public string NoteDescription { get; set; }
        public string TitleVariable { get; set; }
        public string TitleVariablePlural { get; set; }
        public string Source { get; set; }
        public string Unit { get; set; }

        public RejectionReasonTranslation RejectionReasons { get; set; }
        public ChartTypeTranslation ChartTypes { get; set; }
        public SortingOptionTranslation SortingOptions { get; set; }
        public MissingDataTranslation MissingData { get; set; }
        public HeaderTimeFormatsTranslation HeaderTimeFormats { get; set; }
    }
}

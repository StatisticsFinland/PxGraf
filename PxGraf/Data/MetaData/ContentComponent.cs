using Newtonsoft.Json;
using Px.Utils.Language;
using PxGraf.Utility;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Contains information spesific to variables that are of the content type.
    /// </summary>
    public class ContentComponent
    {
        public MultilanguageString Unit { get; set; }

        public MultilanguageString Source { get; set; }

        public int NumberOfDecimals { get; set; }

        // Force JsonConverter to keep LastUpdated as a string when reading json. By default
        // these ISO 8601 formatted strings would be typed as DateTimes.
        [JsonConverter(typeof(ForceStringConverter))]
        public string LastUpdated { get; set; }

        public ContentComponent(
            MultilanguageString unit,
            MultilanguageString source,
            int numberOfDecimals,
            string lastUpdated
            )
        {
            Unit = unit;
            Source = source;
            NumberOfDecimals = numberOfDecimals;
            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Default constructor for deserialization.
        /// </summary>
        public ContentComponent()
        {

        }

        /// <summary>
        /// Produces a deep copy of the content component object.
        /// </summary>
        /// <returns></returns>
        public ContentComponent Clone()
        {
            return new ContentComponent(Unit, Source, NumberOfDecimals, LastUpdated);
        }
    }
}

using Newtonsoft.Json;
using PxGraf.Language;
using PxGraf.Utility;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Contains information spesific to variables that are of the content type.
    /// </summary>
    public class ContentComponent : IReadOnlyContentComponent
    {
        public MultiLanguageString Unit { get; set; }
        IReadOnlyMultiLanguageString IReadOnlyContentComponent.Unit => Unit;

        public MultiLanguageString Source { get; set; }
        IReadOnlyMultiLanguageString IReadOnlyContentComponent.Source => Source;

        public int NumberOfDecimals { get; set; }

        // Force JsonConverter to keep LastUpdated as a string when reading json. By default
        // these ISO 8601 formatted strings would be typed as DateTimes.
        [JsonConverter(typeof(ForceStringConverter))]
        public string LastUpdated { get; set; }

        public ContentComponent(
            IReadOnlyMultiLanguageString unit,
            IReadOnlyMultiLanguageString source,
            int numberOfDecimals,
            string lastUpdated
            )
        {
            Unit = unit.Clone();
            Source = source.Clone();
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
            return new ContentComponent(Unit.Clone(), Source.Clone(), NumberOfDecimals, LastUpdated);
        }
    }
}

using Px.Utils.Language;
using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

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

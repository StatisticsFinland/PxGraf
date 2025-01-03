
using Px.Utils.Language;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Represents the content component edition of a query.
    /// </summary>
    public class ContentComponentEdition
    {
        /// <summary>
        /// Edition of unit information
        /// </summary>
        public MultilanguageString UnitEdit { get; set; }

        /// <summary>
        /// Edition of source information
        /// </summary>
        public MultilanguageString SourceEdit { get; set; }
    }
}

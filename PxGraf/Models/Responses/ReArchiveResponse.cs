using PxGraf.Models.SavedQueries;
using Px.Utils.Language;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response object for re-archiving a saved query.
    /// </summary>
    public class ReArchiveResponse
    {
        /// <summary>
        /// Id of the new saved query.
        /// </summary>
     public string NewSqId { get; set; }

        /// <summary>
        /// The publication status of the new saved query.
        /// </summary>
        public QueryPublicationStatus PublicationStatus { get; set; }

        /// <summary>
        /// Localized publication messages from webhook response.
        /// </summary>
        public MultilanguageString PublicationMessage { get; set; }
    }
}

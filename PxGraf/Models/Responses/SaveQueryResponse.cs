using PxGraf.Models.SavedQueries;
using System.ComponentModel.DataAnnotations;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response object for saving a query
    /// </summary>
    public class SaveQueryResponse
    {
        /// <summary>
        /// The ID of the saved query
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// The publication status of the saved query
        /// </summary>
        [Required]
        public QueryPublicationStatus PublicationStatus { get; set; }
    }
}

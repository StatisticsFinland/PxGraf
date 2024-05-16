using Newtonsoft.Json;

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
        [JsonProperty]
        public string Id { get; set; }
    }
}

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// Request object for re-archiving a saved and archived query.
    /// </summary>
    public class ReArchiveRequest
    {
        /// <summary>
        /// The id of the query to re-archive.
        /// </summary>
        public string SqId { get; set; }
    }
}

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

        /// <summary>
        /// Whether to overwrite the existing file as a draft.
        /// </summary>
        public bool Draft { get; set; } = false;
    }
}

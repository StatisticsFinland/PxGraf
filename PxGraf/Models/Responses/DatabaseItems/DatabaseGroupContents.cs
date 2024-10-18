using System.Collections.Generic;

namespace PxGraf.Models.Responses.DatabaseItems
{
    /// <summary>
    /// Contains the headers and files of a database group.
    /// </summary>
    public class DatabaseGroupContents(List<DatabaseGroupHeader> headers, List<DatabaseTable> files)
    {
        /// <summary>
        /// Headers of the database group.
        /// </summary>
        public List<DatabaseGroupHeader> Headers { get; } = headers;
        /// <summary>
        /// Files of the database group.
        /// </summary>
        public List<DatabaseTable> Files { get; } = files;
    }
}

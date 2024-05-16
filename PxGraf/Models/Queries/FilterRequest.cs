using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Request object for filtering values for visualization.
    /// </summary>
    public class FilterRequest
    {
        /// <summary>
        /// Reference to the table in Px file system.
        /// </summary>
        public PxFileReference TableReference { get; set; }

        /// <summary>
        /// Dictionary that maps variable codes and their respective filters.
        /// </summary>
        public Dictionary<string, ValueFilter> Filters { get; set; }
    }
}

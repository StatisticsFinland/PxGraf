using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Represents a query for a cube, a representation of a multi-dimensional collection of data from a Px file. Contains the table reference, header text and variable queries.
    /// </summary>
    public class CubeQuery
    {
        /// <summary>
        /// Stores information about the table that the query is based on.
        /// </summary>
        public PxFileReference TableReference { get; set; }

        /// <summary>
        /// Header text of the query / visualization.
        /// </summary>
        public MultiLanguageString ChartHeaderEdit { get; set; }

        /// <summary>
        /// Variable.Code to VariableQuery
        /// </summary>
        public Dictionary<string, VariableQuery> VariableQueries { get; set; }
    }
}

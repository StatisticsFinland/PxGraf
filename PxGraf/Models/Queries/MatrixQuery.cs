using Px.Utils.Language;
using Px.Utils.Serializers.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Represents a query for a matrix, a representation of a multi-dimensional collection of data from a Px file.
    /// Contains the table reference, header text and dimension queries.
    /// </summary>
    public class MatrixQuery
    {
        /// <summary>
        /// Stores information about the table that the query is based on.
        /// </summary>
        public PxTableReference TableReference { get; set; }

        /// <summary>
        /// Header text of the query / visualization.
        /// </summary>
        [JsonConverter(typeof(MultilanguageStringConverter))]
        public MultilanguageString ChartHeaderEdit { get; set; }

        /// <summary>
        /// Dimension.Code to DimensionQuery
        /// </summary>
        [JsonPropertyName("variableQueries")] // legacy name, do not change or all the old queries break.
        public Dictionary<string, DimensionQuery> DimensionQueries { get; set; }
    }
}

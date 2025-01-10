using Px.Utils.Language;
using PxGraf.Enums;
using PxGraf.Models.Queries;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response object used by QueryMetaController, contains metadata about a saved query
    /// </summary>
    public class QueryMetaResponse
    {
        /// <summary>
        /// Header text of the visualization.
        /// </summary>
        public MultilanguageString Header { get; set; }
        /// <summary>
        /// Header text of the visualization including placeholders.
        /// </summary>
        public MultilanguageString HeaderWithPlaceholders { get; set; }
        /// <summary>
        /// Whether the query is archived.
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Whether the query is selectable.
        /// </summary>
        public bool Selectable { get; set; }
        /// <summary>
        /// Selected visualizatio type for the query.
        /// </summary>
        public VisualizationType VisualizationType { get; set; }
        /// <summary>
        /// Id of the table that the query is based on.
        /// </summary>
        public string TableId { get; set; }
        /// <summary>
        /// Reference to the table that the query is based on.
        /// </summary>
        public PxTableReference TableReference { get; set; }
        /// <summary>
        /// Last time the table was updated.
        /// </summary>
        public string LastUpdated { get; set; }
    }
}

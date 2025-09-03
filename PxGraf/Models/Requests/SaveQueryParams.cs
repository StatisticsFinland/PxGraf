using PxGraf.Models.Queries;

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// Parameters for saving a query including the query itself and its visualization settings.
    /// </summary>
    public class SaveQueryParams
    {
        /// <summary>
        /// <see cref="MatrixQuery"/> object that contains the query parameters.
        /// </summary>
        public MatrixQuery Query { get; set; }

        /// <summary>
        /// <see cref="VisualizationCreationSettings"/> object containing settings for the visualization.
        /// </summary>
        public VisualizationCreationSettings Settings { get; set; }

        /// <summary>
        /// Id of the query to be updated. If not provided, a new query will be created.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Whether the saved query is a draft. If true, the query will be overwritten when it's saved. Defaults to false.
        /// </summary>
        public bool Draft { get; set; } = false;
    }
}
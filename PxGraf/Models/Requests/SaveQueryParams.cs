using PxGraf.Models.Queries;

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// Parameters for saving a query including the query itself and its visualization settings.
    /// </summary>
    public class SaveQueryParams
    {
        /// <summary>
        /// <see cref="CubeQuery"/> object that contains the query parameters.
        /// </summary>
        public CubeQuery Query { get; set; }

        /// <summary>
        /// <see cref="VisualizationCreationSettings"/> object containing settings for the visualization.
        /// </summary>
        public VisualizationCreationSettings Settings { get; set; }
    }
}

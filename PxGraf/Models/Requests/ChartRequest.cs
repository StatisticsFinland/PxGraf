using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// Request object for creating a chart.
    /// </summary>
    public class ChartRequest
    {
        /// <summary>
        /// Query object that contains the table reference, header text and dimension queries.
        /// </summary>
        public MatrixQuery Query { get; set; }
        
        /// <summary>
        /// Language for the requested query.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Values of the currently actively selected selectable dimensions.
        /// </summary>
        public Dictionary<string, IReadOnlyList<string>> ActiveSelectableDimensionValues { get; set; }

        /// <summary>
        /// Settings for the visualization.
        /// </summary>
        public VisualizationCreationSettings VisualizationSettings { get; set; }
    }
}

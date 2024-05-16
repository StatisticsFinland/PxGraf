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
        /// Query object that contains the table reference, header text and variable queries.
        /// </summary>
        public CubeQuery Query { get; set; }
        
        /// <summary>
        /// Language for the requested query.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Values of the currently actively selected selectable variables.
        /// </summary>
        public Dictionary<string, IReadOnlyList<string>> ActiveSelectableVariableValues { get; set; }

        /// <summary>
        /// Settings for the visualization.
        /// </summary>
        public VisualizationCreationSettings VisualizationSettings { get; set; }
    }
}

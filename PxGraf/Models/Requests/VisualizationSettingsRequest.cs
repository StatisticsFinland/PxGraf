using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// Request object for changing visualization settings.
    /// </summary>
    public class VisualizationSettingsRequest
    {
        /// <summary>
        /// The visualization type selected for the visualization.
        /// </summary>
        [JsonRequired]
        public VisualizationType SelectedVisualization { get; set; }

        /// <summary>
        /// True if the user has manually pivoted the table.
        /// </summary>
        [JsonRequired]
        public bool PivotRequested { get; set; }

        /// <summary>
        /// The query object for the cube containing the header, reference to the table and the selected values.
        /// </summary>
        public MatrixQuery Query { get; set; }
    }
}

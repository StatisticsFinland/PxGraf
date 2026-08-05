using Px.Utils.Language;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Metadata required to configure and render a visualization without its data points.
    /// </summary>
    public class VisualizationMetadataResponse
    {
        public PxTableReference TableReference { get; set; }

        public IReadOnlyList<Variable> MetaData { get; set; }

        [JsonPropertyName("selectableVariableCodes")]
        public IReadOnlyList<string> SelectableDimensionCodes { get; set; }

        [JsonPropertyName("rowVariableCodes")]
        public IReadOnlyList<string> RowDimensionCodes { get; set; }

        [JsonPropertyName("columnVariableCodes")]
        public IReadOnlyList<string> ColumnDimensionCodes { get; set; }

        public MultilanguageString Header { get; set; }

        public VisualizationResponse.PxVisualizerSettings VisualizationSettings { get; set; }
    }
}
using Newtonsoft.Json;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System;
using PxGraf.Data.MetaData;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Information for a response required to render a chart visualization.
    /// </summary>
    public class VisualizationResponse
    {
        /// <summary>
        /// Information about the visualization settings to be used by the PxVisualizer package.
        /// </summary>
        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public sealed class PxVisualizerSettings
        {
            public VisualizationType VisualizationType { get; set; }
            public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; set; }
            public string MultiselectableVariableCode { get; set; }
            public TimeDimensionInterval TimeVariableIntervals { get; set; }
            public DateTime? TimeSeriesStartingPoint { get; set; }
            public bool? CutValueAxis { get; set; }
            public int? MarkerSize { get; set; }
            public bool? ShowLastLabel { get; set; }
            public string Sorting { get; set; }
            public bool? ShowDataPoints { get; set; }
        }

        /// <summary>
        /// Reference to the table that the data is from.
        /// </summary>
        public PxTableReference TableReference { get; set; }

        /// <summary>
        /// The data points to be visualized in the order determined by MetaData property.
        /// </summary>
        public IReadOnlyList<decimal?> Data { get; set; }

        /// <summary>
        /// Dictionary that contains notes for the data points, where the key is the index of the data point and the value is the note represented as a multilanguage string.
        /// </summary>
        public IReadOnlyDictionary<int, MultilanguageString> DataNotes { get; set; }

        /// <summary>
        /// Information about missing data points where key is the index of the data point. The value represents the index of the missing data point type:
        /// 1 = Missing,2 = CannotRepresent, 3 = Confidential, 4 = NotAcquired, 5 = NotAsked, UnderConstruction or Error, 6 = Empty (Actual usage unclear), 7 = Nill (Actual usage unclear)
        /// </summary>
        public IReadOnlyDictionary<int, int> MissingDataInfo { get; set; }

        /// <summary>
        /// Information about the variables and their values that define the order of the data points in the Data property.
        /// </summary>
        public IReadOnlyList<Variable> MetaData { get; set; }

        /// <summary>
        /// Variable codes for variables that have been set as selectable.
        /// </summary>
        public IReadOnlyList<string> SelectableVariableCodes { get; set; }

        /// <summary>
        /// List of variable codes to be used on rows.
        /// </summary>
        public IReadOnlyList<string> RowVariableCodes { get; set; }

        /// <summary>
        /// List of variable codes to be used on columns.
        /// </summary>
        public IReadOnlyList<string> ColumnVariableCodes { get; set; }

        /// <summary>
        /// Header text for the visualization.
        /// </summary>
        public MultilanguageString Header { get; set; }

        /// <summary>
        /// Object that contains settings for the PxVisualizer package.
        /// </summary>
        public PxVisualizerSettings VisualizationSettings { get; set; }
    }
}

#nullable enable
using Px.Utils.Language;
using PxGraf.Enums;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    public class EditorContentsResponse
    {
        /// <summary>
        /// Current size of the query.
        /// </summary>
        public required int Size { get; set; }

        /// <summary>
        /// The limit when the end user should be warned that the size of the query is getting larger than is parctical.
        /// </summary>
        public required int SizeWarningLimit { get; set; }

        /// <summary>
        /// The limit when the end user should be told that the size of the query can cause technical issues and further actios will be prevented unless the size is reduced
        /// </summary>
        public required int MaximumSupportedSize { get; set; }

        /// <summary>
        /// Default header for the query.
        /// </summary>
        public required MultilanguageString HeaderText { get; set; }

        /// <summary>
        /// Maximum length of the header text, in any language
        /// </summary>
        public required int MaximumHeaderLength { get; set; }

        /// <summary>
        /// These visualization types are valid for the query
        /// </summary>
        public required List<VisualizationOption> VisualizationOptions { get; set; }

        /// <summary>
        /// Collection of reasons why the invalid visualization types are not possible for this query.
        /// </summary>
        public required Dictionary<VisualizationType, MultilanguageString> VisualizationRejectionReasons { get; set; }
    }

    public class VisualizationOption
    {
        public required VisualizationType Type { get; set; }

        public class SortingOptionsCollection
        {
            public required List<SortingOption>? Default { get; set; }
            public required List<SortingOption>? Pivoted { get; set; }
        }

        public required bool AllowShowingDataPoints { get; set; }
        public required bool AllowCuttingYAxis { get; set; } 
        public required bool AllowMatchXLabelsToEnd { get; set; }
        public required bool AllowSetMarkerScale { get; set; }

        /// <summary>
        /// Whether manual ordering of dimensions is allowed.
        /// </summary>
        public required bool AllowManualPivot { get; set; }

        /// <summary>
        /// List of available sorting options.
        /// </summary>
        public required SortingOptionsCollection SortingOptions { get; set; }

        /// <summary>
        /// Whether the user is allowed to define dimensions with multiple selectable values displayed simultaneously.
        /// </summary>
        public required bool allowMultiselect { get; set; }
    }
}
#nullable disable

using PxGraf.Enums;
using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Rules for how a chart can be visualized.
    /// </summary>
    public class VisualizationRules
    {
        /// <summary>
        /// Rules for a specific visualization type.
        /// </summary>
        public class TypeSpecificVisualizationRules
        {
            public bool AllowShowingDataPoints { get; }
            public bool AllowCuttingYAxis { get; }
            public bool AllowMatchXLabelsToEnd { get; }
            public bool AllowSetMarkerScale { get; }

            /// <summary>
            /// Constructor for the rules for a specific chart visualization type.
            /// </summary>
            /// <param name="selectedVisualization">Selected visualization type.</param>
            public TypeSpecificVisualizationRules(VisualizationType selectedVisualization)
            {
                AllowShowingDataPoints = selectedVisualization != VisualizationType.Table && selectedVisualization != VisualizationType.ScatterPlot;
                AllowCuttingYAxis = selectedVisualization == VisualizationType.LineChart || selectedVisualization == VisualizationType.ScatterPlot;
                AllowMatchXLabelsToEnd = selectedVisualization == VisualizationType.VerticalBarChart || selectedVisualization == VisualizationType.GroupVerticalBarChart || selectedVisualization == VisualizationType.PercentVerticalBarChart || selectedVisualization == VisualizationType.StackedVerticalBarChart;
                AllowSetMarkerScale = selectedVisualization == VisualizationType.ScatterPlot;
            }
        }

        /// <summary>
        /// Whether manual ordering of variables is allowed.
        /// </summary>
        public bool AllowManualPivot { get; }

        /// <summary>
        /// List of available sorting options.
        /// </summary>
        public IReadOnlyList<SortingOption> SortingOptions { get; }

        /// <summary>
        /// Whether the user is allowed to define variables with multiple selectable values displayed simultaneously.
        /// </summary>
        public bool MultiselectVariableAllowed { get; }

        /// <summary>
        /// Rules for a specific visualization type.
        /// </summary>
        public TypeSpecificVisualizationRules VisualizationTypeSpecificRules { get; }

        /// <summary>
        /// Constructor for the rules for a chart visualization.
        /// </summary>
        /// <param name="pivotAllowed">Whether manual ordering of variables is allowed.</param>
        /// <param name="multiselectAllowed">Whether the user is allowed to define variables with multiple selectable values displayed simultaneously.</param>
        /// <param name="typeSpecificVisualizationRules">Rules for a specific visualization type.</param>
        /// <param name="sortingOptions">List of available sorting options.</param>
        public VisualizationRules(bool pivotAllowed, bool multiselectAllowed, TypeSpecificVisualizationRules typeSpecificVisualizationRules = null, IReadOnlyList<SortingOption> sortingOptions = null)
        {
            AllowManualPivot = pivotAllowed;
            SortingOptions = sortingOptions;
            MultiselectVariableAllowed = multiselectAllowed;
            VisualizationTypeSpecificRules = typeSpecificVisualizationRules;
        }
    }

    /// <summary>
    /// A sorting option for a chart visualization.
    /// </summary>
    public class SortingOption
    {
        /// <summary>
        /// Identifying code for the sorting option.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Display name for the sorting option.
        /// </summary>
        public IReadOnlyMultiLanguageString Description { get; }

        /// <summary>
        /// Constructor for a sorting option for a chart visualization.
        /// </summary>
        /// <param name="code">Value code for the sorting option.</param>
        /// <param name="description">Display name for the sorting option.</param>
        public SortingOption(string code, IReadOnlyMultiLanguageString description)
        {
            Code = code;
            Description = description;
        }
    }
}

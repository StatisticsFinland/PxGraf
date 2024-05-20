using PxGraf.Enums;
using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Rules for how a chart can be visualized.
    /// </summary>
    /// <remarks>
    /// Constructor for the rules for a chart visualization.
    /// </remarks>
    /// <param name="pivotAllowed">Whether manual ordering of variables is allowed.</param>
    /// <param name="multiselectAllowed">Whether the user is allowed to define variables with multiple selectable values displayed simultaneously.</param>
    /// <param name="typeSpecificVisualizationRules">Rules for a specific visualization type.</param>
    /// <param name="sortingOptions">List of available sorting options.</param>
    public class VisualizationRules(bool pivotAllowed, bool multiselectAllowed, VisualizationRules.TypeSpecificVisualizationRules typeSpecificVisualizationRules = null, IReadOnlyList<SortingOption> sortingOptions = null)
    {
        /// <summary>
        /// Rules for a specific visualization type.
        /// </summary>
        /// <remarks>
        /// Constructor for the rules for a specific chart visualization type.
        /// </remarks>
        /// <param name="selectedVisualization">Selected visualization type.</param>
        public class TypeSpecificVisualizationRules(VisualizationType selectedVisualization)
        {
            public bool AllowShowingDataPoints { get; } = 
                selectedVisualization != VisualizationType.Table &&
                selectedVisualization != VisualizationType.ScatterPlot;
            public bool AllowCuttingYAxis { get; } = 
                selectedVisualization == VisualizationType.LineChart ||
                selectedVisualization == VisualizationType.ScatterPlot;
            public bool AllowMatchXLabelsToEnd { get; } = 
                selectedVisualization == VisualizationType.VerticalBarChart ||
                selectedVisualization == VisualizationType.GroupVerticalBarChart ||
                selectedVisualization == VisualizationType.PercentVerticalBarChart ||
                selectedVisualization == VisualizationType.StackedVerticalBarChart;
            public bool AllowSetMarkerScale { get; } = selectedVisualization == VisualizationType.ScatterPlot;
        }

        /// <summary>
        /// Whether manual ordering of variables is allowed.
        /// </summary>
        public bool AllowManualPivot { get; } = pivotAllowed;

        /// <summary>
        /// List of available sorting options.
        /// </summary>
        public IReadOnlyList<SortingOption> SortingOptions { get; } = sortingOptions;

        /// <summary>
        /// Whether the user is allowed to define variables with multiple selectable values displayed simultaneously.
        /// </summary>
        public bool MultiselectVariableAllowed { get; } = multiselectAllowed;

        /// <summary>
        /// Rules for a specific visualization type.
        /// </summary>
        public TypeSpecificVisualizationRules VisualizationTypeSpecificRules { get; } = typeSpecificVisualizationRules;
    }

    /// <summary>
    /// A sorting option for a chart visualization.
    /// </summary>
    /// <remarks>
    /// Constructor for a sorting option for a chart visualization.
    /// </remarks>
    /// <param name="code">Value code for the sorting option.</param>
    /// <param name="description">Display name for the sorting option.</param>
    public class SortingOption(string code, IReadOnlyMultiLanguageString description)
    {
        /// <summary>
        /// Identifying code for the sorting option.
        /// </summary>
        public string Code { get; } = code;

        /// <summary>
        /// Display name for the sorting option.
        /// </summary>
        public IReadOnlyMultiLanguageString Description { get; } = description;
    }
}

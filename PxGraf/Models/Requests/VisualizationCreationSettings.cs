using Px.Utils.Models.Metadata;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Requests
{
    /// <summary>
    /// This class provides separation between the internal visualization settings and the API response.
    /// So that changes to one do not force changes to the other.
    /// </summary>
    public class VisualizationCreationSettings
    {
        /// <summary>
        /// Currently selected visualization type. (various charts, table, text)
        /// </summary>    
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VisualizationType SelectedVisualization { get; set; }

        public IReadOnlyList<string> RowVariableCodes { get; set; }

        public IReadOnlyList<string> ColumnVariableCodes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? CutYAxis { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MarkerSize { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MultiselectableVariableCode { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MatchXLabelsToEnd { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? XLabelInterval { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? PivotRequested { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Sorting { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowDataPoints { get; set; } = false;

        #region Visualization settings conversion

        /// <summary>
        /// Converts settings from a saved query to visualization creation settings.
        /// </summary>
        /// <param name="savedQuery">Object that represents the saved query.</param>
        /// <param name="meta">Metadata of the cube subject for visualization.</param>
        /// <returns>Converted settings in <see cref="VisualizationCreationSettings"/> format.</returns>
        public static VisualizationCreationSettings FromVisualizationSettings(SavedQuery savedQuery, IReadOnlyMatrixMetadata meta)
        {
            VisualizationSettings settings = savedQuery.Settings;
            MatrixQuery query = savedQuery.Query;
            Layout layout = LayoutRules.GetPivotBasedLayout(settings.VisualizationType, meta, query);

            bool? pivotRequested;
            if (settings.VisualizationType == VisualizationType.Table)
            {
                pivotRequested = null;
            }
            else if (settings.Layout is not null)
            {
                pivotRequested = !layout.Equals(settings.Layout);
            }
            else
            {
                savedQuery.LegacyProperties.TryGetValue("PivotRequested", out object legacyPivotRequested);
                pivotRequested = legacyPivotRequested is bool b && b;
            }

            return new VisualizationCreationSettings()
            {
                SelectedVisualization = settings.VisualizationType,
                RowVariableCodes = settings.VisualizationType == VisualizationType.Table ? settings.Layout?.RowVariableCodes : null,
                ColumnVariableCodes = settings.VisualizationType == VisualizationType.Table ? settings.Layout?.ColumnVariableCodes : null,
                CutYAxis = settings.CutYAxis,
                MarkerSize = settings.MarkerSize,
                MultiselectableVariableCode = settings.MultiselectableVariableCode,
                MatchXLabelsToEnd = settings.MatchXLabelsToEnd,
                XLabelInterval = settings.XLabelInterval,
                PivotRequested = pivotRequested,
                Sorting = settings.Sorting,
                DefaultSelectableVariableCodes = settings.DefaultSelectableVariableCodes,
                ShowDataPoints = settings.ShowDataPoints
            };
        }

        /// <summary>
        /// Converts the settings for visualization to an <see cref="VisualizationSettings"/> object."/>
        /// </summary>
        /// <param name="meta">Metadata for the cube that's subject to the visualization.</param>
        /// <param name="query">Object that contains query settings for the cube.</param>
        /// <returns><see cref="VisualizationSettings"/> object with the converted settings.</returns>
        public VisualizationSettings ToVisualizationSettings(IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            switch (SelectedVisualization)
            {
                case VisualizationType.VerticalBarChart:
                    {
                        return new VerticalBarChartVisualizationSettings(
                            LayoutRules.GetOneDimensionalLayout(meta, query),
                            MatchXLabelsToEnd ?? false,
                            XLabelInterval ?? 1,
                            DefaultSelectableVariableCodes,
                            ShowDataPoints ?? false);
                    }
                case VisualizationType.GroupVerticalBarChart:
                    {
                        return new GroupVerticalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                    PivotRequested ?? false,
                                    VisualizationType.GroupVerticalBarChart,
                                    meta,
                                    query),
                                MatchXLabelsToEnd ?? false,
                                XLabelInterval ?? 1,
                                DefaultSelectableVariableCodes);
                    }
                case VisualizationType.StackedVerticalBarChart:
                    {
                        return new StackedVerticalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.StackedVerticalBarChart,
                                meta,
                                query),
                            MatchXLabelsToEnd ?? false,
                            XLabelInterval ?? 1,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PercentVerticalBarChart:
                    {
                        return new PercentVerticalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.PercentVerticalBarChart,
                                meta,
                                query),
                            MatchXLabelsToEnd ?? false,
                            XLabelInterval ?? 1,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.HorizontalBarChart:
                    {
                        return new HorizontalBarChartVisualizationSettings(
                            LayoutRules.GetOneDimensionalLayout(meta, query),
                            Sorting,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.GroupHorizontalBarChart:
                    {
                        return new GroupHorizontalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.GroupHorizontalBarChart,
                                meta,
                                query),
                            Sorting,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.StackedHorizontalBarChart:
                    {
                        return new StackedHorizontalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.StackedHorizontalBarChart,
                                meta,
                                query),
                            Sorting,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PercentHorizontalBarChart:
                    {
                        return new PercentHorizontalBarChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.PercentHorizontalBarChart,
                                meta,
                                query),
                            Sorting,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PyramidChart:
                    {
                        return new PyramidChartVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.PyramidChart,
                                meta,
                                query),
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PieChart:
                    {
                        return new PieChartVisualizationSettings(
                            LayoutRules.GetOneDimensionalLayout(meta, query),
                            Sorting,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.LineChart:
                    {
                        return new LineChartVisualizationSettings(
                            LayoutRules.GetLineChartLayout(meta, query),
                            CutYAxis ?? false,
                            MultiselectableVariableCode,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.ScatterPlot:
                    {
                        return new ScatterPlotVisualizationSettings(
                            LayoutRules.GetTwoDimensionalLayout(
                                PivotRequested ?? false,
                                VisualizationType.ScatterPlot,
                                meta,
                                query),
                            CutYAxis ?? false,
                            MarkerSize ?? 100,
                            DefaultSelectableVariableCodes);
                    }
                case VisualizationType.Table:
                    {
                        return new TableVisualizationSettings(
                            LayoutRules.GetTableLayout(
                                RowVariableCodes,
                                ColumnVariableCodes),
                            DefaultSelectableVariableCodes);
                    }
            }

            return null;
        }

        #endregion
    }
}

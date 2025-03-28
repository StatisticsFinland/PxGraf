﻿using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class SavedQueryV10 : VersionedSavedQuery
    {
        public VisualizationSettingsV10 Settings { get; set; }
        public class VisualizationSettingsV10
        {
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public VisualizationType SelectedVisualization { get; set; }

            [JsonPropertyName("rowVariableCodes")]
            public IReadOnlyList<string> RowDimensionCodes { get; set; }

            [JsonPropertyName("columnVariableCodes")]
            public IReadOnlyList<string> ColumnDimensionCodes { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? CutYAxis { get; set; } = false;

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? MarkerSize { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("multiselectableVariableCode")]
            public string MultiselectableDimensionCode { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? MatchXLabelsToEnd { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? XLabelInterval { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? PivotRequested { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Sorting { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("defaultSelectableVariableCodes")]
            public Dictionary<string, List<string>> DefaultSelectableDimensionCodes { get; set; }
        }

        #region Methods for converting to SavedQuery

        public override SavedQuery ToSavedQuery()
        {
            SavedQuery savedQuery = new()
            {
                Archived = Archived,
                Settings = BuildSettingsFromV10(Settings),
                CreationTime = CreationTime,
                Query = Query,
                Version = "1.0"
            };

            if (Settings.PivotRequested is bool b)
            {
                savedQuery.LegacyProperties["PivotRequested"] = b;
            }

            return savedQuery;
        }

        private static VisualizationSettings BuildSettingsFromV10(VisualizationSettingsV10 settings)
        {
            switch (settings.SelectedVisualization)
            {
                case VisualizationType.LineChart:
                    {
                        return new LineChartVisualizationSettings(
                            null,
                            settings.CutYAxis ?? false,
                            settings.MultiselectableDimensionCode,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.VerticalBarChart:
                    {
                        return new VerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.GroupVerticalBarChart:
                    {
                        return new GroupVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.StackedVerticalBarChart:
                    {
                        return new StackedVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.PercentVerticalBarChart:
                    {
                        return new PercentVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.HorizontalBarChart:
                    {
                        return new HorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.GroupHorizontalBarChart:
                    {
                        return new GroupHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.StackedHorizontalBarChart:
                    {
                        return new StackedHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.PercentHorizontalBarChart:
                    {
                        return new PercentHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.PieChart:
                    {
                        return new PieChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.PyramidChart:
                    {
                        return new PyramidChartVisualizationSettings(
                            null,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.ScatterPlot:
                    {
                        return new ScatterPlotVisualizationSettings(
                            null,
                            settings.CutYAxis ?? false,
                            settings.MarkerSize ?? 100,
                            settings.DefaultSelectableDimensionCodes);
                    }
                case VisualizationType.Table:
                    {
                        return new TableVisualizationSettings(
                            new Layout(settings.RowDimensionCodes,
                            settings.ColumnDimensionCodes),
                            settings.DefaultSelectableDimensionCodes);
                    }
                default:
                    return null;
            }
        }
        #endregion
    }
}

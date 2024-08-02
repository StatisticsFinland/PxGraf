using Newtonsoft.Json;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class SavedQueryV10 : VersionedSavedQuery
    {
        public MatrixQuery Query { get; set; }

        public DateTime CreationTime { get; set; }

        public VisualizationSettingsV10 Settings { get; set; }

        public bool Archived { get; set; }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class VisualizationSettingsV10
        {
            public VisualizationType SelectedVisualization { get; set; }

            public IReadOnlyList<string> RowVariableCodes { get; set; }

            public IReadOnlyList<string> ColumnVariableCodes { get; set; }

            public bool? CutYAxis { get; set; } = false;

            public int? MarkerSize { get; set; }

            public string MultiselectableVariableCode { get; set; }

            public bool? MatchXLabelsToEnd { get; set; }

            public int? XLabelInterval { get; set; }

            public bool? PivotRequested { get; set; }

            public string Sorting { get; set; }

            public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; set; }
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
                            settings.MultiselectableVariableCode,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.VerticalBarChart:
                    {
                        return new VerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.GroupVerticalBarChart:
                    {
                        return new GroupVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.StackedVerticalBarChart:
                    {
                        return new StackedVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PercentVerticalBarChart:
                    {
                        return new PercentVerticalBarChartVisualizationSettings(
                            null,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.HorizontalBarChart:
                    {
                        return new HorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.GroupHorizontalBarChart:
                    {
                        return new GroupHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.StackedHorizontalBarChart:
                    {
                        return new StackedHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PercentHorizontalBarChart:
                    {
                        return new PercentHorizontalBarChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PieChart:
                    {
                        return new PieChartVisualizationSettings(
                            null,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.PyramidChart:
                    {
                        return new PyramidChartVisualizationSettings(
                            null,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.ScatterPlot:
                    {
                        return new ScatterPlotVisualizationSettings(
                            null,
                            settings.CutYAxis ?? false,
                            settings.MarkerSize ?? 100,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.Table:
                    {
                        return new TableVisualizationSettings(
                            new Layout(settings.RowVariableCodes,
                            settings.ColumnVariableCodes),
                            settings.DefaultSelectableVariableCodes);
                    }
                default:
                    return null;
            }
        }
        #endregion
    }
}

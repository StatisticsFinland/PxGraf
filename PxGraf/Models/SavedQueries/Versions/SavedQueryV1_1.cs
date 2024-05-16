using PxGraf.Enums;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class SavedQueryV11 : VersionedSavedQuery
    {
        public CubeQuery Query { get; set; }

        public DateTime CreationTime { get; set; }

        public VisualizationSettingsV11 Settings { get; set; }

        public bool Archived { get; set; }

        public class VisualizationSettingsV11
        {
            public VisualizationType VisualizationType { get; set; }

            public bool? CutYAxis { get; set; } = false;

            public int? MarkerSize { get; set; }

            public string MultiselectableVariableCode { get; set; }

            public bool? MatchXLabelsToEnd { get; set; }

            public int? XLabelInterval { get; set; }

            public string Sorting { get; set; }

            public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; set; }

            public Layout Layout { get; set; }
            public bool? ShowDataPoints { get; set; } = false;
        }

        #region Methods for converting to SavedQuery

        public override SavedQuery ToSavedQuery()
        {
            return new SavedQuery()
            {
                Archived = Archived,
                Settings = BuildSettingsFromV11(Settings),
                CreationTime = CreationTime,
                Query = Query,
                Version = "1.1"
            };
        }

        private static VisualizationSettings BuildSettingsFromV11(VisualizationSettingsV11 settings)
        {
            switch (settings.VisualizationType)
            {
                case VisualizationType.LineChart:
                    {
                        return new LineChartVisualizationSettings(
                            settings.Layout,
                            settings.CutYAxis ?? false,
                            settings.MultiselectableVariableCode,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.VerticalBarChart:
                    {
                        return new VerticalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.GroupVerticalBarChart:
                    {
                        return new GroupVerticalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.StackedVerticalBarChart:
                    {
                        return new StackedVerticalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.PercentVerticalBarChart:
                    {
                        return new PercentVerticalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.MatchXLabelsToEnd ?? false,
                            settings.XLabelInterval ?? 1,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.HorizontalBarChart:
                    {
                        return new HorizontalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.GroupHorizontalBarChart:
                    {
                        return new GroupHorizontalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.StackedHorizontalBarChart:
                    {
                        return new StackedHorizontalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.PercentHorizontalBarChart:
                    {
                        return new PercentHorizontalBarChartVisualizationSettings(
                            settings.Layout,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.PieChart:
                    {
                        return new PieChartVisualizationSettings(
                            settings.Layout,
                            settings.Sorting,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.PyramidChart:
                    {
                        return new PyramidChartVisualizationSettings(
                            settings.Layout,
                            settings.DefaultSelectableVariableCodes,
                            settings.ShowDataPoints ?? false);
                    }
                case VisualizationType.ScatterPlot:
                    {
                        return new ScatterPlotVisualizationSettings(
                            settings.Layout,
                            settings.CutYAxis ?? false,
                            settings.MarkerSize ?? 100,
                            settings.DefaultSelectableVariableCodes);
                    }
                case VisualizationType.Table:
                    {
                        return new TableVisualizationSettings(
                            settings.Layout,
                            settings.DefaultSelectableVariableCodes);
                    }
                default:
                    return null;
            }
        }

        #endregion
    }
}

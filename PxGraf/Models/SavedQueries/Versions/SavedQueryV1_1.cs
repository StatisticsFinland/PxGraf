using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class SavedQueryV11 : VersionedSavedQuery
    {
        public MatrixQuery Query { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreationTime { get; set; }

        public VisualizationSettingsV11 Settings { get; set; }

        public bool Archived { get; set; }

        [JsonInclude]
        public readonly string Version = "1.1";

        public class VisualizationSettingsV11
        {
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public VisualizationType VisualizationType { get; set; }

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
            public string Sorting { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; set; }

            public Layout Layout { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? ShowDataPoints { get; set; } = false;

            [JsonConstructor]
            public VisualizationSettingsV11() { }

            public VisualizationSettingsV11(VisualizationSettings inputSettings)
            {
                VisualizationType = inputSettings.VisualizationType;
                Layout = inputSettings.Layout;
                CutYAxis = inputSettings.CutYAxis;
                MarkerSize = inputSettings.MarkerSize;
                MultiselectableVariableCode = inputSettings.MultiselectableVariableCode;
                MatchXLabelsToEnd = inputSettings.MatchXLabelsToEnd;
                XLabelInterval = inputSettings.XLabelInterval;
                Sorting = inputSettings.Sorting;
                DefaultSelectableVariableCodes = inputSettings.DefaultSelectableVariableCodes;
                ShowDataPoints = inputSettings.ShowDataPoints;
            }
        }

        #region Constructors

        [JsonConstructor]
        public SavedQueryV11() { }

        public SavedQueryV11(SavedQuery inputQuery)
        {
            Query = inputQuery.Query;
            CreationTime = inputQuery.CreationTime;
            Archived = inputQuery.Archived;
            Settings = new(inputQuery.Settings); 
        }

        #endregion

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

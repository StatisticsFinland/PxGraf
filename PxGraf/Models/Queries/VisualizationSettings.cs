using PxGraf.Data;
using PxGraf.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Queries
{
    public abstract class VisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableDimensionCodes)
    {
        /// <summary>
        /// Currently selected visualization type. (various charts, table, text)
        /// </summary>
        public abstract VisualizationType VisualizationType { get; }

        public Layout Layout { get; set; } = layout;

        public bool? CutYAxis { get; protected set; } = false;

        public int? MarkerSize { get; protected set; }

        [JsonPropertyName("multiSelectableVariableCode")] // legacy name, do not change or all the old queries break.
        public string MultiselectableDimensionCode { get; protected set; }

        public bool? MatchXLabelsToEnd { get; protected set; }

        public int? XLabelInterval { get; protected set; }

        public string Sorting { get; protected set; }

        [JsonPropertyName("defaultSelectableVariableCodes")] // legacy name, do not change or all the old queries break.
        public Dictionary<string, List<string>> DefaultSelectableDimensionCodes { get; } = defaultSelectableDimensionCodes;
        public bool? ShowDataPoints { get; protected set; } = false;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public class TableVisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null) : VisualizationSettings(layout, defaultSelectableDimensionCodes)
    {
        public override VisualizationType VisualizationType => VisualizationType.Table;
    }

    public class LineChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.LineChart;

        public LineChartVisualizationSettings(Layout layout, bool cutYAxis, string multiselDimCode, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            CutYAxis = cutYAxis;
            MultiselectableDimensionCode = multiselDimCode;
            ShowDataPoints = showDataPoints;
        }
    }

    public class VerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.VerticalBarChart;

        public VerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {

            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class GroupVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.GroupVerticalBarChart;

        public GroupVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class StackedVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.StackedVerticalBarChart;

        public StackedVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)

        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PercentVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PercentVerticalBarChart;

        public PercentVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class HorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.HorizontalBarChart;
        public HorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            if (sorting == "descending" || sorting == "decending") Sorting = CubeSorting.DESCENDING;
            else
            {
                Sorting = sorting ?? CubeSorting.NO_SORTING;
            }
            ShowDataPoints = showDataPoints;
        }
    }

    public class GroupHorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.GroupHorizontalBarChart;
        public GroupHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class StackedHorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.StackedHorizontalBarChart;
        public StackedHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PercentHorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PercentHorizontalBarChart;
        public PercentHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PyramidChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PyramidChart;

        public PyramidChartVisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            ShowDataPoints = showDataPoints;
        }
    }

    public class PieChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PieChart;
        public PieChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableDimensionCodes)
        {
            if (sorting == "descending" || sorting == "decending") Sorting = CubeSorting.DESCENDING;
            else
            {
                Sorting = sorting ?? CubeSorting.NO_SORTING;
            }
            ShowDataPoints = showDataPoints;
        }
    }

    public class ScatterPlotVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.ScatterPlot;
        public ScatterPlotVisualizationSettings(Layout layout, bool cutYAxis, int markerSize, Dictionary<string, List<string>> defaultSelectableDimensionCodes = null)
            : base(layout, defaultSelectableDimensionCodes)
        {
            CutYAxis = cutYAxis;
            MarkerSize = markerSize;
        }
    }
}
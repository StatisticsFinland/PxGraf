using Newtonsoft.Json;
using PxGraf.Data;
using PxGraf.Enums;
using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class VisualizationSettings
    {
        /// <summary>
        /// Currently selected visualization type. (various charts, table, text)
        /// </summary>
        public abstract VisualizationType VisualizationType { get; }

        public Layout Layout { get; set; }

        public bool? CutYAxis { get; protected set; } = false;

        public int? MarkerSize { get; protected set; }

        public string MultiselectableVariableCode { get; protected set; }

        public bool? MatchXLabelsToEnd { get; protected set; }

        public int? XLabelInterval { get; protected set; }

        public string Sorting { get; protected set; }

        public Dictionary<string, List<string>> DefaultSelectableVariableCodes { get; }
        public bool? ShowDataPoints { get; protected set; } = false;

        protected VisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableVariableCodes)
        {
            Layout = layout;
            DefaultSelectableVariableCodes = defaultSelectableVariableCodes;
        }
    }

    public class TableVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.Table;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TableVisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableVariableCodes = null)
            : base(layout, defaultSelectableVariableCodes) { }
    }

    public class LineChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.LineChart;

        public LineChartVisualizationSettings(Layout layout, bool cutYAxis, string multiselVarCode, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            CutYAxis = cutYAxis;
            MultiselectableVariableCode = multiselVarCode;
            ShowDataPoints = showDataPoints;
        }
    }

    public class VerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.VerticalBarChart;

        public VerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {

            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class GroupVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.GroupVerticalBarChart;

        public GroupVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class StackedVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.StackedVerticalBarChart;

        public StackedVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)

        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PercentVerticalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PercentVerticalBarChart;

        public PercentVerticalBarChartVisualizationSettings(Layout layout, bool matchLabelsToEnd, int xLabelInterval, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            MatchXLabelsToEnd = matchLabelsToEnd;
            XLabelInterval = xLabelInterval;
            ShowDataPoints = showDataPoints;
        }
    }

    public class HorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.HorizontalBarChart;
        public HorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
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
        public GroupHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class StackedHorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.StackedHorizontalBarChart;
        public StackedHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PercentHorizontalBarChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PercentHorizontalBarChart;
        public PercentHorizontalBarChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            Sorting = sorting ?? CubeSorting.NO_SORTING;
            ShowDataPoints = showDataPoints;
        }
    }

    public class PyramidChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PyramidChart;

        public PyramidChartVisualizationSettings(Layout layout, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
        {
            ShowDataPoints = showDataPoints;
        }
    }

    public class PieChartVisualizationSettings : VisualizationSettings
    {
        public override VisualizationType VisualizationType => VisualizationType.PieChart;
        public PieChartVisualizationSettings(Layout layout, string sorting = CubeSorting.NO_SORTING, Dictionary<string, List<string>> defaultSelectableVariableCodes = null, bool showDataPoints = false)
            : base(layout, defaultSelectableVariableCodes)
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
        public ScatterPlotVisualizationSettings(Layout layout, bool cutYAxis, int markerSize, Dictionary<string, List<string>> defaultSelectableVariableCodes = null)
            : base(layout, defaultSelectableVariableCodes)
        {
            CutYAxis = cutYAxis;
            MarkerSize = markerSize;
        }
    }
}
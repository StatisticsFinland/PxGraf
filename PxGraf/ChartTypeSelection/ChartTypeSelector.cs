using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Determines which kind of charts can be drawn from the data. The limits for each chart type are read from json.
    /// </summary>
    public class ChartTypeSelector : IChartTypeSelector
    {
        /// <summary>
        /// Provides the selector instance.
        /// </summary>
        public static IChartTypeSelector Selector
        {
            get
            {
                _selector ??= new ChartTypeSelector(new ChartSelectionLimits());
                return _selector;
            }
        }

        private static IChartTypeSelector _selector = null;

        /// <summary>
        /// Validity information about basic vertical bar chart with the given query
        /// </summary>
        private VerticalBarChartCheck VerticalBarChart { get; }

        /// <summary>
        /// Validity information about group vertical bar chart with the given query
        /// </summary>
        private GroupVerticalBarChartCheck GroupVerticalBarChart { get; }

        /// <summary>
        /// Validity information about stacked vertical bar chart with the given query
        /// </summary>
        private StackedVerticalBarChartCheck StackedVerticalBarChart { get; }

        /// <summary>
        /// Validity information about percent vertical bar chart with the given query.
        /// </summary>
        private PercentVerticalBarChartCheck PercentVerticalBarChart { get; }

        /// <summary>
        /// Validity information about basic horizontal bar chart with the given query
        /// </summary>
        private HorizontalBarChartCheck HorizontalBarChart { get; }

        /// <summary>
        /// Validity information about group horizontal bar chart with the given query
        /// </summary>
        private GroupHorizontalBarChartCheck GroupHorizontalBarChart { get; }

        /// <summary>
        /// Validity information about stacked horizontal bar chart with the given query
        /// </summary>
        private StackedHorizontalBarChartCheck StackedHorizontalBarChart { get; }

        /// <summary>
        /// Validity information about percent horizontal bar chart with the given query
        /// </summary>
        private PercentHorizontalBarChartCheck PercentHorizontalBarChart { get; }

        /// <summary>
        /// Validity information about pie chart with the given query
        /// </summary>
        private PieChartCheck PieChart { get; }

        /// <summary>
        /// Validity information about line chart with the given query
        /// </summary>
        private LineChartCheck LineChart { get; }

        /// <summary>
        /// Validity information about scatter plot with the given query
        /// </summary>
        private ScatterPlotCheck ScatterPlot { get; }

        /// <summary>
        /// Validity information about pyramid chart with the given query
        /// </summary>
        private PyramidChartCheck PyramidChart { get; }

        /// <summary>
        /// Validity information about tables with the given query
        /// </summary>
        private TableCheck Table { get; }

        /// <summary>
        /// Validity information about all possible chart types
        /// </summary>
        private List<ChartRulesCheck> AllLimitChecks { get; }

        /// <summary>
        /// Construct the type selector with a set of limit objects and a logger
        /// </summary>
        /// <param name="limits"></param>
        private ChartTypeSelector(IChartSelectionLimits limits)
        {
            VerticalBarChart = new VerticalBarChartCheck(limits.VerticalBarChartLimits);
            GroupVerticalBarChart = new GroupVerticalBarChartCheck(limits.GroupVerticalBarChartLimits);
            StackedVerticalBarChart = new StackedVerticalBarChartCheck(limits.StackedVerticalBarChartLimits);
            PercentVerticalBarChart = new PercentVerticalBarChartCheck(limits.StackedVerticalBarChartLimits); //TODO: new rule set for percent even if it is identical

            HorizontalBarChart = new HorizontalBarChartCheck(limits.HorizontalBarChartLimits);
            GroupHorizontalBarChart = new GroupHorizontalBarChartCheck(limits.GroupHorizontalBarChartLimits);
            StackedHorizontalBarChart = new StackedHorizontalBarChartCheck(limits.StackedHorizontalBarChartLimits);
            PercentHorizontalBarChart = new PercentHorizontalBarChartCheck(limits.StackedHorizontalBarChartLimits); //TODO: new rule set for percent even if it is identical

            PieChart = new PieChartCheck(limits.PieChartLimits);
            LineChart = new LineChartCheck(limits.LineChartLimits);
            ScatterPlot = new ScatterPlotCheck(limits.ScatterPlotLimits);
            PyramidChart = new PyramidChartCheck(limits.PyramidChartLimits);

            Table = new TableCheck(limits.TableLimits);

            AllLimitChecks = new List<ChartRulesCheck>()
            {
                LineChart,
                HorizontalBarChart,
                GroupHorizontalBarChart,
                StackedHorizontalBarChart,
                PercentHorizontalBarChart,
                VerticalBarChart,
                GroupVerticalBarChart,
                StackedVerticalBarChart,
                PercentVerticalBarChart,
                PieChart,
                PyramidChart,
                ScatterPlot,
                Table
            };
        }

        public IReadOnlyList<VisualizationType> GetValidChartTypes(DataCube cube)
        {
            List<VisualizationType> validTypes = new();
            foreach (var check in AllLimitChecks)
            {
                if (!check.CheckValidity(VisualizationTypeSelectionObject.FromCube(cube)).Any())
                {
                    validTypes.Add(check.Type);
                }
            }

            return validTypes;
        }

        public IReadOnlyList<VisualizationType> GetValidChartTypes(CubeQuery query, DataCube cube)
        {
            List<VisualizationType> validTypes = new();
            foreach (var check in AllLimitChecks)
            {
                if (!check.CheckValidity(VisualizationTypeSelectionObject.FromQueryAndCube(query, cube)).Any())
                {
                    validTypes.Add(check.Type);
                }
            }

            return validTypes;
        }

        public IReadOnlyList<VisualizationType> GetValidChartTypes(CubeQuery query, ArchiveCube cube)
        {
            List<VisualizationType> validTypes = new();
            foreach (var check in AllLimitChecks)
            {
                if (!check.CheckValidity(VisualizationTypeSelectionObject.FromQueryAndCube(query, cube)).Any())
                {
                    validTypes.Add(check.Type);
                }
            }

            return validTypes;
        }

        public IReadOnlyDictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> GetRejectionReasons(CubeQuery query, DataCube cube)
        {
            Dictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> rejectionReasons = new();
            foreach (var check in AllLimitChecks)
            {
                rejectionReasons[check.Type] = check.CheckValidity(VisualizationTypeSelectionObject.FromQueryAndCube(query, cube));
            }

            return rejectionReasons;
        }
    }
}

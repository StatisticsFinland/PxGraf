using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;

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
            PercentVerticalBarChart = new PercentVerticalBarChartCheck(limits.StackedVerticalBarChartLimits); // Percent vertical should have its own rule set

            HorizontalBarChart = new HorizontalBarChartCheck(limits.HorizontalBarChartLimits);
            GroupHorizontalBarChart = new GroupHorizontalBarChartCheck(limits.GroupHorizontalBarChartLimits);
            StackedHorizontalBarChart = new StackedHorizontalBarChartCheck(limits.StackedHorizontalBarChartLimits);
            PercentHorizontalBarChart = new PercentHorizontalBarChartCheck(limits.StackedHorizontalBarChartLimits); // Percent horizontal should have its own rule set

            PieChart = new PieChartCheck(limits.PieChartLimits);
            LineChart = new LineChartCheck(limits.LineChartLimits);
            ScatterPlot = new ScatterPlotCheck(limits.ScatterPlotLimits);
            PyramidChart = new PyramidChartCheck(limits.PyramidChartLimits);

            Table = new TableCheck(limits.TableLimits);

            AllLimitChecks =
            [
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
            ];
        }

        public IReadOnlyList<VisualizationType> GetValidChartTypes(MatrixQuery query, Matrix<DecimalDataValue> matrix)
        {
            List<VisualizationType> validTypes = [];
            foreach (ChartRulesCheck check in AllLimitChecks)
            {
                if (check.CheckValidity(VisualizationTypeSelectionObject.FromQueryAndMatrix(query, matrix)).Count == 0)
                {
                    validTypes.Add(check.Type);
                }
            }

            return validTypes;
        }

        public IReadOnlyDictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> GetRejectionReasons(MatrixQuery query, Matrix<DecimalDataValue> matrix)
        {
            Dictionary<VisualizationType, IReadOnlyList<ChartRejectionInfo>> rejectionReasons = [];
            foreach (ChartRulesCheck check in AllLimitChecks)
            {
                rejectionReasons[check.Type] = check.CheckValidity(VisualizationTypeSelectionObject.FromQueryAndMatrix(query, matrix));
            }

            return rejectionReasons;
        }
    }
}

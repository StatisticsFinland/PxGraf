using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data
{
    /// <summary>
    /// Contains the business logic determining when the order of the variables should be reversed
    /// and the data transposed for draving some spesific chart.
    /// </summary>
    public static class AutoPivotRules
    {
        /// <summary>
        /// Delegate representing an auto pivot rule.
        /// </summary>
        /// <returns></returns>
        public delegate bool AutoPivotRule(IReadOnlyCubeMeta meta);

        /// <summary>
        /// Determines the need for autopivoting for basic vertical bar chart.
        /// </summary>
        public static bool BasicVerticalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() > 1)
            {
                var errMsg = $"Cannot determine autopivot for basic vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one variable can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Returns true if the data must be pivoted before a group vertical bar chart can be drwn.
        /// If both multiselect dimensions have maximum of 4 values, time or progressive dimensions is the outer dimension.
        /// The dimension with fewer values is the inner variable by default
        /// </summary>
        /// <returns></returns>
        public static bool GroupVerticalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2)
            {
                var errMsg = "Cannot determine autopivot for the group vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but exactly 2 are required.";
                throw new ArgumentException(errMsg);
            }

            var multiselects = meta.GetMultivalueVariables();

            if ((multiselects[0].Type == VariableType.Time || multiselects[0].Type == VariableType.Ordinal) &&
                    !(multiselects[1].Type == VariableType.Time || multiselects[1].Type == VariableType.Ordinal) &&
                    (multiselects[0].IncludedValues.Count < 5 && multiselects[1].IncludedValues.Count < 5))
            {
                return true;
            }
            return multiselects[0].IncludedValues.Count > multiselects[1].IncludedValues.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a stacked vertical bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool StackedVerticalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2)
            {
                var errMsg = "Cannot determine autopivot for the stacked vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but exactly 2 are required.";
                throw new ArgumentException(errMsg);
            }

            var multiselects = meta.GetMultivalueVariables();

            bool rowVarIsOrdinal = multiselects[0].Type == VariableType.Time || multiselects[0].Type == VariableType.Ordinal;
            bool colVarIsOrdinal = multiselects[1].Type == VariableType.Time || multiselects[1].Type == VariableType.Ordinal;

            if (rowVarIsOrdinal && !colVarIsOrdinal) return true;
            else if (colVarIsOrdinal && !rowVarIsOrdinal) return false;
            else return multiselects[0].IncludedValues.Count > multiselects[1].IncludedValues.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a percent vertical bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool PercentVerticalBarChart(IReadOnlyCubeMeta meta) => StackedVerticalBarChart(meta);

        /// <summary>
        /// Determines the need for autopivoting for a basic horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool BasicHorizontalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() > 1)
            {
                var errMsg = $"Cannot determine autopivot for basic horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one variable can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Determines the need for autopivoting for a group horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool GroupHorizontalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2)
            {
                var errMsg = $"Cannot determine autopivot for the group horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but exactly 2 are required";
                throw new ArgumentException(errMsg);
            }

            var multiselects = meta.GetMultivalueVariables();
            // If the time variable has two values it is the inner variable
            // The variable with fewer values is the inner variable.
            return (multiselects[1].Type == VariableType.Time && multiselects[1].IncludedValues.Count == 2) ||
                multiselects[1].IncludedValues.Count < multiselects[0].IncludedValues.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a stacked horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool StackedHorizontalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2)
            {
                var errMsg = $"Cannot determine autopivot for the stacked horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but exactly 2 are required";
                throw new ArgumentException(errMsg);
            }

            var multiselects = meta.GetMultivalueVariables();

            // If the time variable has two values it is the inner variable
            // The variable with fewer values is the inner variable.
            return multiselects[1].IncludedValues.Count < multiselects[0].IncludedValues.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a percent horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool PercentHorizontalBarChart(IReadOnlyCubeMeta meta) => StackedHorizontalBarChart(meta);

        /// <summary>
        /// Determines the need for autopivoting for a line chart.
        /// </summary>
        /// <returns></returns>
        public static bool LineChart(IReadOnlyCubeMeta meta)
        {
            // If the query contains a time variable it is set as the outer variable.
            // If the query contains multiple progressive variables, the one with most values selected is set as the outer variable.
            var timeOrLargestOrdinal = meta.GetMultivalueTimeOrLargestOrdinal();
            if (timeOrLargestOrdinal == null)
            {
                var errMsg = $"Cannot determine autopivot for the line chart, query doest not contain a multiselect time or ordinal dimension.";
                throw new ArgumentException(errMsg);
            }

            return meta.GetNumberOfMultivalueVariables() == 2 && meta.GetMultivalueVariables()[0] == timeOrLargestOrdinal;
        }

        /// <summary>
        /// Determines the need for autopivoting for a pie chart.
        /// </summary>
        /// <returns></returns>
        public static bool PieChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 1)
            {
                var errMsg = $"Cannot determine autopivot for the pie chart," +
                    $" the query has {meta.GetNumberOfMultivalueVariables()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one variable can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Determines the need for autopivoting for a pyramid chart.
        /// </summary>
        /// <returns></returns>
        public static bool PyramidChart(IReadOnlyCubeMeta meta)
        {
            // The variable with exactly two values selected must be the inner variable.
            return meta.GetMultivalueVariables()[0].IncludedValues.Count > 2;
        }

        /// <summary>
        /// Determines the need for autopivoting for a scatter plot.
        /// </summary>
        /// <returns></returns>
        public static bool ScatterPlot(IReadOnlyCubeMeta meta)
        {
            // The content variable with two selected values must be the inner variable.
            return meta.GetMultivalueVariables()[0].IncludedValues.Count > 2 ||
                meta.GetMultivalueVariables()[0].Type != VariableType.Content;

        }

        /// <summary>
        /// Checks the autopivoting for the specified visualization type.
        /// </summary>
        public static bool GetAutoPivot(VisualizationType visualization, IReadOnlyCubeMeta meta)
        {
            return visualization switch
            {
                VisualizationType.VerticalBarChart => BasicVerticalBarChart(meta),
                VisualizationType.GroupVerticalBarChart => GroupVerticalBarChart(meta),
                VisualizationType.StackedVerticalBarChart => StackedVerticalBarChart(meta),
                VisualizationType.PercentVerticalBarChart => PercentVerticalBarChart(meta),
                VisualizationType.HorizontalBarChart => BasicHorizontalBarChart(meta),
                VisualizationType.GroupHorizontalBarChart => GroupHorizontalBarChart(meta),
                VisualizationType.StackedHorizontalBarChart => StackedHorizontalBarChart(meta),
                VisualizationType.PercentHorizontalBarChart => PercentHorizontalBarChart(meta),
                VisualizationType.PyramidChart => PyramidChart(meta),
                VisualizationType.PieChart => PieChart(meta),
                VisualizationType.LineChart => LineChart(meta),
                VisualizationType.ScatterPlot => ScatterPlot(meta),
                VisualizationType.Table => false,
                _ => false,
            };
        }

        /// <summary>
        /// Checks the autopivoting for a specified visualization type, selectable variables are collapsed to one value.
        /// </summary>
        public static bool GetAutoPivot(VisualizationType visualization, IReadOnlyCubeMeta meta, CubeQuery query)
        {
            var varList = meta.BuildMap().ToList();
            var resultList = new List<VariableMap>();
            foreach (var varMap in varList)
            {
                // Selectable varaibles always have a size of 1 and for purposes of this class the actual value does not matter, just the size.
                if (query.VariableQueries[varMap.Code].Selectable)
                {
                    resultList.Add(new VariableMap(varMap.Code, [varMap.ValueCodes[0]]));
                }
                else
                {
                    resultList.Add(varMap);
                }
            }

            var newMata = meta.GetTransform(new CubeMap(resultList));

            return GetAutoPivot(visualization, newMata);
        }
    }
}

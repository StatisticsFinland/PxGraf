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
    /// Assuming the query is valid for a given chart type,
    /// returns the manual pivotability for the given chart type with the provided query.
    /// </summary>
    public static class ManualPivotRules
    {
        /// <summary>
        /// Delegate representing a manual pivot rule.
        /// </summary>
        /// <returns></returns>
        public delegate bool ManualPivotRule(IReadOnlyCubeMeta meta);

        /// <summary>
        /// Determines if the basic vertical bar chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool BasicVerticalBarChart(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Determines if the group vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool GroupVerticalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2) throw new ArgumentException("Can not determine the pivotability of a GroupVerticalBarChart with the provided cubemeta.");
            IReadOnlyVariable largestMultivalueVar = meta.GetLargestMultivalueVariable();
            IReadOnlyVariable smallerMultiValueVar = meta.GetSmallerMultivalueVariable();
            if (largestMultivalueVar == null || smallerMultiValueVar == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining the pivotability with GroupVerticalBarChart.";
                throw new ArgumentException(errMsg);
            }
            return largestMultivalueVar.IncludedValues.Count <= 4 && smallerMultiValueVar.IncludedValues.Count <= 4;
        }

        /// <summary>
        /// Determines if the stacked vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool StackedVerticalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2) throw new ArgumentException("Can not determine the pivotability of a StackedVerticalBarChart with the provided cubemeta.");
            bool noMultivalueTime = meta.GetTimeVariable().IncludedValues.Count == 1;
            IReadOnlyVariable largestMultivalueVar = meta.GetLargestMultivalueVariable();
            IReadOnlyVariable smallerMultivalueVar = meta.GetSmallerMultivalueVariable();
            if (largestMultivalueVar == null || smallerMultivalueVar == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with StackedVerticalBarChart";
                throw new ArgumentException(errMsg);
            }
            return noMultivalueTime && largestMultivalueVar.IncludedValues.Count < 10 && smallerMultivalueVar.IncludedValues.Count < 10;
        }

        /// <summary>
        /// Determines if the percent vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool PercentVerticalBarChart(IReadOnlyCubeMeta meta) => StackedVerticalBarChart(meta);

        /// <summary>
        /// Determines if the basic horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool BasicHorizontalBarChart(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Determines if the group horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool GroupHorizontalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2) throw new ArgumentException("Can not determine the pivotability of a GroupHorizontalBarChart with the provided cubemeta.");
            IReadOnlyVariable timeVar = meta.Variables.FirstOrDefault(v => v.Type == VariableType.Time);
            IReadOnlyVariable largestMultivalueVar = meta.GetLargestMultivalueVariable();
            IReadOnlyVariable smallerMultivalueVar = meta.GetSmallerMultivalueVariable();
            if (largestMultivalueVar == null || smallerMultivalueVar == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with GroupHorizontalBarChart";
                throw new ArgumentException(errMsg);
            }
            return ((timeVar == null || timeVar.IncludedValues.Count <= 1) &&
                    largestMultivalueVar.IncludedValues.Count <= 4 &&
                    smallerMultivalueVar.IncludedValues.Count <= 4);
        }

        /// <summary>
        /// Determines if the group horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool StackedHorizontalBarChart(IReadOnlyCubeMeta meta)
        {
            if (meta.GetNumberOfMultivalueVariables() != 2) throw new ArgumentException("Can not determine the pivotability of a StackedHorizontalBarChart with the provided cubemeta.");
            IReadOnlyVariable largestMultivalueVar = meta.GetLargestMultivalueVariable();
            IReadOnlyVariable smallerMultivalueVar = meta.GetSmallerMultivalueVariable();
            if (largestMultivalueVar == null || smallerMultivalueVar == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with StackedHorizontalBarChart";
                throw new ArgumentException(errMsg);
            }
            return largestMultivalueVar.IncludedValues.Count < 10 && smallerMultivalueVar.IncludedValues.Count < 10;
        }

        /// <summary>
        /// Determines if the percent horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool PercentHorizontalBarChart(IReadOnlyCubeMeta meta) => StackedHorizontalBarChart(meta);

        /// <summary>
        /// Determines if the line chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool LineChart(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Determines if the pie chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool PieChart(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Determines if the scatter plot can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool ScatterPlot(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Determines if the pyramid chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool PyramidChart(IReadOnlyCubeMeta _) => false;

        /// <summary>
        /// Checks is manual pivoting is possible for a specified visualization type.
        /// </summary>
        public static bool GetManualPivotability(VisualizationType visualization, IReadOnlyCubeMeta meta)
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
        /// Checks is manual pivoting is possible for a specified visualization type, selectable variables are collapsed to one value.
        /// </summary>
        public static bool GetManualPivotability(VisualizationType visualization, IReadOnlyCubeMeta meta, CubeQuery query)
        {
            List<VariableMap> varList = [.. meta.BuildMap()];
            List<VariableMap> resultList = [];
            foreach (VariableMap varMap in varList)
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

            CubeMeta newMata = meta.GetTransform(new CubeMap(resultList));

            return GetManualPivotability(visualization, newMata);
        }
    }
}

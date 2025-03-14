﻿using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
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
        public delegate bool ManualPivotRule(IReadOnlyMatrixMetadata meta);

        /// <summary>
        /// Determines if the basic vertical bar chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool BasicVerticalBarChart(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Determines if the group vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool GroupVerticalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2) throw new ArgumentException("Can not determine the pivotability of a GroupVerticalBarChart with the provided cubemeta.");
            IReadOnlyDimension largestMultivalueDim = meta.GetLargestMultivalueDimension();
            IReadOnlyDimension smallerMultiValueDim = meta.GetSmallerMultivalueDimension();
            if (largestMultivalueDim == null || smallerMultiValueDim == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining the pivotability with GroupVerticalBarChart.";
                throw new ArgumentException(errMsg);
            }
            return largestMultivalueDim.Values.Count <= 4 && smallerMultiValueDim.Values.Count <= 4;
        }

        /// <summary>
        /// Determines if the stacked vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool StackedVerticalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2) throw new ArgumentException("Can not determine the pivotability of a StackedVerticalBarChart with the provided cubemeta.");
            bool noMultivalueTime = meta.GetTimeDimension().Values.Count == 1;
            IReadOnlyDimension largestMultivalueDim = meta.GetLargestMultivalueDimension();
            IReadOnlyDimension smallerMultivalueDim = meta.GetSmallerMultivalueDimension();
            if (largestMultivalueDim == null || smallerMultivalueDim == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with StackedVerticalBarChart";
                throw new ArgumentException(errMsg);
            }
            return noMultivalueTime && largestMultivalueDim.Values.Count < 10 && smallerMultivalueDim.Values.Count < 10;
        }

        /// <summary>
        /// Determines if the percent vertical bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool PercentVerticalBarChart(IReadOnlyMatrixMetadata meta) => StackedVerticalBarChart(meta);

        /// <summary>
        /// Determines if the basic horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool BasicHorizontalBarChart(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Determines if the group horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool GroupHorizontalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2) throw new ArgumentException("Can not determine the pivotability of a GroupHorizontalBarChart with the provided cubemeta.");
            IReadOnlyDimension timeDim = meta.Dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
            IReadOnlyDimension largestMultivalueDim = meta.GetLargestMultivalueDimension();
            IReadOnlyDimension smallerMultivalueDim = meta.GetSmallerMultivalueDimension();
            if (largestMultivalueDim == null || smallerMultivalueDim == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with GroupHorizontalBarChart";
                throw new ArgumentException(errMsg);
            }
            return ((timeDim == null || timeDim.Values.Count <= 1) &&
                    largestMultivalueDim.Values.Count <= 4 &&
                    smallerMultivalueDim.Values.Count <= 4);
        }

        /// <summary>
        /// Determines if the group horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool StackedHorizontalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2) throw new ArgumentException("Can not determine the pivotability of a StackedHorizontalBarChart with the provided cubemeta.");
            IReadOnlyDimension largestMultivalueDim = meta.GetLargestMultivalueDimension();
            IReadOnlyDimension smallerMultivalueDim = meta.GetSmallerMultivalueDimension();
            if (largestMultivalueDim == null || smallerMultivalueDim == null)
            {
                string errMsg = $"Provided chart selections limits are not compatible with determining pivotability with StackedHorizontalBarChart";
                throw new ArgumentException(errMsg);
            }
            return largestMultivalueDim.Values.Count < 10 && smallerMultivalueDim.Values.Count < 10;
        }

        /// <summary>
        /// Determines if the percent horizontal bar chart can be manually pivoted.
        /// </summary>
        /// <returns></returns>
        public static bool PercentHorizontalBarChart(IReadOnlyMatrixMetadata meta) => StackedHorizontalBarChart(meta);

        /// <summary>
        /// Determines if the line chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool LineChart(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Determines if the pie chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool PieChart(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Determines if the scatter plot can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool ScatterPlot(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Determines if the pyramid chart can be manually pivoted.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool PyramidChart(IReadOnlyMatrixMetadata _) => false;

        /// <summary>
        /// Checks is manual pivoting is possible for a specified visualization type.
        /// </summary>
        public static bool GetManualPivotability(VisualizationType visualization, IReadOnlyMatrixMetadata meta)
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
        /// Checks is manual pivoting is possible for a specified visualization type, selectable dimensions are collapsed to one value.
        /// </summary>
        public static bool GetManualPivotability(VisualizationType visualization, IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            List<IDimensionMap> resultList = [];
            foreach (IDimensionMap dimMap in meta.DimensionMaps)
            {
                // Selectable dimensions always have a size of 1 and for purposes of this class the actual value does not matter, just the size.
                if (query.DimensionQueries[dimMap.Code].Selectable)
                {
                    resultList.Add(new DimensionMap(dimMap.Code, [dimMap.ValueCodes[0]]));
                }
                else
                {
                    resultList.Add(dimMap);
                }
            }

            IReadOnlyMatrixMetadata newMata = meta.GetTransform(new MatrixMap(resultList));
            return GetManualPivotability(visualization, newMata);
        }
    }
}

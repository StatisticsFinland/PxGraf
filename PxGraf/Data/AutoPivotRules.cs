#nullable enable
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System.Collections.Generic;
using System;
using Px.Utils.Models.Metadata.Dimensions;

namespace PxGraf.Data
{
    /// <summary>
    /// Contains the business logic determining when the order of the dimensions should be reversed
    /// and the data transposed for draving some spesific chart.
    /// </summary>
    public static class AutoPivotRules
    {
        /// <summary>
        /// Delegate representing an auto pivot rule.
        /// </summary>
        /// <returns></returns>
        public delegate bool AutoPivotRule(IReadOnlyMatrixMetadata meta);

        /// <summary>
        /// Determines the need for autopivoting for basic vertical bar chart.
        /// </summary>
        public static bool BasicVerticalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() > 1)
            {
                string errMsg = $"Cannot determine autopivot for basic vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one dimension can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Returns true if the data must be pivoted before a group vertical bar chart can be drwn.
        /// If both multiselect dimensions have maximum of 4 values, time or progressive dimensions is the outer dimension.
        /// The dimension with fewer values is the inner dimension by default
        /// </summary>
        /// <returns></returns>
        public static bool GroupVerticalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2)
            {
                string errMsg = "Cannot determine autopivot for the group vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but exactly 2 are required.";
                throw new ArgumentException(errMsg);
            }

            IReadOnlyList<IReadOnlyDimension> multiselects = meta.GetMultivalueDimensions();

            if ((multiselects[0].Type == DimensionType.Time || multiselects[0].Type == DimensionType.Ordinal) &&
                    !(multiselects[1].Type == DimensionType.Time || multiselects[1].Type == DimensionType.Ordinal) &&
                    (multiselects[0].Values.Count < 5 && multiselects[1].Values.Count < 5))
            {
                return true;
            }
            return multiselects[0].Values.Count > multiselects[1].Values.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a stacked vertical bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool StackedVerticalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2)
            {
                string errMsg = "Cannot determine autopivot for the stacked vertical bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but exactly 2 are required.";
                throw new ArgumentException(errMsg);
            }

            IReadOnlyList<IReadOnlyDimension> multiselects = meta.GetMultivalueDimensions();

            bool rowDimIsOrdinal = multiselects[0].Type == DimensionType.Time || multiselects[0].Type == DimensionType.Ordinal;
            bool colDimIsOrdinal = multiselects[1].Type == DimensionType.Time || multiselects[1].Type == DimensionType.Ordinal;

            if (rowDimIsOrdinal && !colDimIsOrdinal) return true;
            else if (colDimIsOrdinal && !rowDimIsOrdinal) return false;
            else return multiselects[0].Values.Count > multiselects[1].Values.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a percent vertical bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool PercentVerticalBarChart(IReadOnlyMatrixMetadata meta) => StackedVerticalBarChart(meta);

        /// <summary>
        /// Determines the need for autopivoting for a basic horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool BasicHorizontalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() > 1)
            {
                string errMsg = $"Cannot determine autopivot for basic horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one dimension can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Determines the need for autopivoting for a group horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool GroupHorizontalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2)
            {
                string errMsg = $"Cannot determine autopivot for the group horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but exactly 2 are required";
                throw new ArgumentException(errMsg);
            }

            IReadOnlyList<IReadOnlyDimension> multiselects = meta.GetMultivalueDimensions();
            // If the time dimension has two values it is the inner dimension
            // The dimension with fewer values is the inner dimension.
            return (multiselects[1].Type == DimensionType.Time && multiselects[1].Values.Count == 2) ||
                multiselects[1].Values.Count < multiselects[0].Values.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a stacked horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool StackedHorizontalBarChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 2)
            {
                string errMsg = $"Cannot determine autopivot for the stacked horizontal bar chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but exactly 2 are required";
                throw new ArgumentException(errMsg);
            }

            IReadOnlyList<IReadOnlyDimension> multiselects = meta.GetMultivalueDimensions();

            // If the time dimension has two values it is the inner dimension
            // The dimension with fewer values is the inner dimension.
            return multiselects[1].Values.Count < multiselects[0].Values.Count;
        }

        /// <summary>
        /// Determines the need for autopivoting for a percent horizontal bar chart.
        /// </summary>
        /// <returns></returns>
        public static bool PercentHorizontalBarChart(IReadOnlyMatrixMetadata meta) => StackedHorizontalBarChart(meta);

        /// <summary>
        /// Determines the need for autopivoting for a line chart.
        /// </summary>
        /// <returns></returns>
        public static bool LineChart(IReadOnlyMatrixMetadata meta)
        {
            // If the query contains a time dimension it is set as the outer dimension.
            // If the query contains multiple progressive dimensions, the one with most values selected is set as the outer dimension.
            IReadOnlyDimension? timeOrLargestOrdinal = meta.GetMultivalueTimeOrLargestOrdinal();
            if (timeOrLargestOrdinal == null)
            {
                string errMsg = $"Cannot determine autopivot for the line chart, query doest not contain a multiselect time or ordinal dimension.";
                throw new ArgumentException(errMsg);
            }

            return meta.GetNumberOfMultivalueDimensions() == 2 && meta.GetMultivalueDimensions()[0] == timeOrLargestOrdinal;
        }

        /// <summary>
        /// Determines the need for autopivoting for a pie chart.
        /// </summary>
        /// <returns></returns>
        public static bool PieChart(IReadOnlyMatrixMetadata meta)
        {
            if (meta.GetNumberOfMultivalueDimensions() != 1)
            {
                string errMsg = $"Cannot determine autopivot for the pie chart," +
                    $" the query has {meta.GetNumberOfMultivalueDimensions()} multiselect dimensions, but the maximum allowed is 1";
                throw new ArgumentException(errMsg);
            }

            // Pivoting is never required here, one dimension can only be placed is one order.
            return false;
        }

        /// <summary>
        /// Determines the need for autopivoting for a pyramid chart.
        /// </summary>
        /// <returns></returns>
        public static bool PyramidChart(IReadOnlyMatrixMetadata meta)
        {
            // The dimension with exactly two values selected must be the inner dimension.
            return meta.GetMultivalueDimensions()[0].Values.Count > 2;
        }

        /// <summary>
        /// Determines the need for autopivoting for a scatter plot.
        /// </summary>
        /// <returns></returns>
        public static bool ScatterPlot(IReadOnlyMatrixMetadata meta)
        {
            // The content dimension with two selected values must be the inner dimension.
            return meta.GetMultivalueDimensions()[0].Values.Count > 2 ||
                meta.GetMultivalueDimensions()[0].Type != DimensionType.Content;

        }

        /// <summary>
        /// Checks the autopivoting for the specified visualization type.
        /// </summary>
        public static bool GetAutoPivot(VisualizationType visualization, IReadOnlyMatrixMetadata meta)
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
        /// Checks the autopivoting for a specified visualization type, selectable dimensions are collapsed to one value.
        /// </summary>
        public static bool GetAutoPivot(VisualizationType visualization, IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            List<IDimensionMap> resultList = [];
            foreach (IDimensionMap dimensionMap in meta.DimensionMaps)
            {
                // Selectable dimensions always have a size of 1 and for purposes of this class the actual value does not matter, just the size.
                if (query.DimensionQueries[dimensionMap.Code].Selectable)
                {
                    resultList.Add(new DimensionMap(dimensionMap.Code, [dimensionMap.ValueCodes[0]]));
                }
                else
                {
                    resultList.Add(dimensionMap);
                }
            }

            MatrixMetadata newMata = meta.GetTransform(new MatrixMap(resultList));

            return GetAutoPivot(visualization, newMata);
        }
    }
}
#nullable disable
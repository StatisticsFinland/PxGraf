using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PxGraf.Data
{
    public static class LayoutRules
    {
        public static Layout GetOneDimensionalLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            IReadOnlyDimension[] candidates = [.. meta.GetMultivalueDimensions()
                .Where(v => !query.DimensionQueries[v.Code].Selectable)];

            if (candidates.Length == 0)
            {
                throw new InvalidOperationException(
                    "GetOneDimensionalLayout requires exactly one non-selectable multivalue dimension, but none were found.");
            }

            return new Layout(
                rowDimensionCodes: [],
                columnDimensionCodes: [candidates[0].Code]);
        }

        public static Layout GetTwoDimensionalLayout(bool pivotRequested, VisualizationType visualizationType, IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            IReadOnlyDimension[] multiValueDims = [.. meta.GetMultivalueDimensions().Where(v => !query.DimensionQueries[v.Code].Selectable)];

            if (multiValueDims.Length < 2)
            {
                throw new InvalidOperationException(
                    $"GetTwoDimensionalLayout requires exactly 2 non-selectable multivalue dimensions, but found {multiValueDims.Length}.");
            }

            bool autopivot = AutoPivotRules.GetAutoPivot(visualizationType, meta, query);
            bool manualPivotability = ManualPivotRules.GetManualPivotability(visualizationType, meta, query);
            bool transpose = (pivotRequested && manualPivotability) ^ autopivot;

            if (transpose)
            {
                return new Layout(
                    rowDimensionCodes: [multiValueDims[1].Code],
                    columnDimensionCodes: [multiValueDims[0].Code]);
            }
            else
            {
                return new Layout(
                    rowDimensionCodes: [multiValueDims[0].Code],
                    columnDimensionCodes: [multiValueDims[1].Code]);
            }
        }

        public static Layout GetLineChartLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            IReadOnlyDimension[] multiValueDims = [.. meta.GetMultivalueDimensions().Where(v => !query.DimensionQueries[v.Code].Selectable)];

            if (multiValueDims.Length == 0)
            {
                throw new InvalidOperationException(
                    "GetLineChartLayout requires at least one non-selectable multivalue dimension, but none were found.");
            }

            // Prefer time dimension over ordinal dimension
            IReadOnlyDimension multiValueTimeDimension = Array.Find(multiValueDims, v => v.Type == DimensionType.Time);
            string columnDimensionCode = multiValueTimeDimension is not null
                ? multiValueTimeDimension.Code
                : multiValueDims.OrderByDescending(d => d.Values.Count)
                    .First(v => v.Type == DimensionType.Ordinal).Code;

            return new Layout(
                [.. multiValueDims.Select(d => d.Code).Where(vc => vc != columnDimensionCode)],
                [columnDimensionCode]);
        }

        public static Layout GetTableLayout(IReadOnlyList<string> rowCodes, IReadOnlyList<string> columnCodes)
        {
            return new Layout(rowCodes, columnCodes);
        }

        public static Layout GetPivotBasedLayout(VisualizationType type, IReadOnlyMatrixMetadata meta, MatrixQuery query, bool pivotRequested = false)
        {
            return type switch
            {
                VisualizationType.VerticalBarChart =>
                    GetOneDimensionalLayout(meta, query),
                VisualizationType.GroupVerticalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.StackedVerticalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.PercentVerticalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.HorizontalBarChart =>
                    GetOneDimensionalLayout(meta, query),
                VisualizationType.GroupHorizontalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.StackedHorizontalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.PercentHorizontalBarChart =>
                    GetTwoDimensionalLayout(pivotRequested, type, meta, query),
                VisualizationType.PyramidChart =>
                    GetTwoDimensionalLayout(false, type, meta, query),
                VisualizationType.PieChart =>
                    GetOneDimensionalLayout(meta, query),
                VisualizationType.LineChart =>
                    GetLineChartLayout(meta, query),
                VisualizationType.ScatterPlot =>
                    GetTwoDimensionalLayout(false, type, meta, query),
                _ => new Layout(null, null),
            };
        }
    }
}

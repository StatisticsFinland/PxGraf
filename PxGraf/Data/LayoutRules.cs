using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace PxGraf.Data
{
    public static class LayoutRules
    {
        public static Layout GetOneDimensionalLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            return new Layout(
                rowVariableCodes: [],
                columnVariableCodes: [ meta.GetMultivalueDimensions()
                .Where(v => !query.DimensionQueries[v.Code].Selectable)
                .ToArray()[0].Code ]);
        }

        public static Layout GetTwoDimensionalLayout(bool pivotRequested, VisualizationType visualizationType, IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            IReadOnlyDimension[] multiValueDims = meta.GetMultivalueDimensions()
                .Where(v => !query.DimensionQueries[v.Code].Selectable)
                .ToArray();

            Debug.Assert(multiValueDims.Length == 2);

            bool autopivot = AutoPivotRules.GetAutoPivot(visualizationType, meta, query);
            bool manualPivotability = ManualPivotRules.GetManualPivotability(visualizationType, meta, query);
            bool transpose = (pivotRequested && manualPivotability) ^ autopivot;

            if (transpose)
            {
                return new Layout(
                    rowVariableCodes: [multiValueDims[1].Code],
                    columnVariableCodes: [multiValueDims[0].Code]);
            }
            else
            {
                return new Layout(
                    rowVariableCodes: [multiValueDims[0].Code],
                    columnVariableCodes: [multiValueDims[1].Code]);
            }
        }

        public static Layout GetLineChartLayout(IReadOnlyMatrixMetadata meta, MatrixQuery query)
        {
            IReadOnlyDimension[] multiValueVars = meta.GetMultivalueDimensions()
                .Where(v => !query.DimensionQueries[v.Code].Selectable)
                .ToArray();

            // Prefer time variable over ordinal variable
            IReadOnlyDimension multiValueTimeVar = Array.Find(multiValueVars, v => v.Type == DimensionType.Time);
            string columnVariableCode = multiValueTimeVar is not null
                ? multiValueTimeVar.Code
                : multiValueVars.OrderByDescending(d => d.Values.Count)
                    .First(v => v.Type == DimensionType.Ordinal).Code;

            return new Layout(
                multiValueVars.Select(d => d.Code).Where(vc => vc != columnVariableCode).ToList(),
                [columnVariableCode]);
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

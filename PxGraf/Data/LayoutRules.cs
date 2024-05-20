using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PxGraf.Data
{
    public static class LayoutRules
    {
        public static Layout GetOneDimensionalLayout(IReadOnlyCubeMeta meta, CubeQuery query)
        {
            return new Layout(
                rowVariableCodes: [],
                columnVariableCodes: [ meta.GetMultivalueVariables()
                .Where(v => !query.VariableQueries[v.Code].Selectable)
                .ToArray()[0].Code ]);
        }

        public static Layout GetTwoDimensionalLayout(bool pivotRequested, VisualizationType visualizationType, IReadOnlyCubeMeta meta, CubeQuery query)
        {
            IReadOnlyVariable[] multiValueVars = meta.GetMultivalueVariables()
                .Where(v => !query.VariableQueries[v.Code].Selectable)
                .ToArray();

            Debug.Assert(multiValueVars.Length == 2);

            bool autopivot = AutoPivotRules.GetAutoPivot(visualizationType, meta, query);
            bool manualPivotability = ManualPivotRules.GetManualPivotability(visualizationType, meta, query);
            bool transpose = (pivotRequested && manualPivotability) ^ autopivot;

            if (transpose)
            {
                return new Layout(
                    rowVariableCodes: [multiValueVars[1].Code],
                    columnVariableCodes: [multiValueVars[0].Code]);
            }
            else
            {
                return new Layout(
                    rowVariableCodes: [multiValueVars[0].Code],
                    columnVariableCodes: [multiValueVars[1].Code]);
            }
        }

        public static Layout GetLineChartLayout(IReadOnlyCubeMeta meta, CubeQuery query)
        {
            IReadOnlyVariable[] multiValueVars = meta.GetMultivalueVariables()
                .Where(v => !query.VariableQueries[v.Code].Selectable)
                .ToArray();

            // Prefer time variable over ordinal variable
            IReadOnlyVariable multiValueTimeVar = Array.Find(multiValueVars, v => v.Type == VariableType.Time);
            string columnVariableCode = multiValueTimeVar is not null
                ? multiValueTimeVar.Code
                : multiValueVars.OrderByDescending(v => v.IncludedValues.Count)
                    .First(v => v.Type == VariableType.Ordinal).Code;

            return new Layout(
                multiValueVars.Select(v => v.Code).Where(vc => vc != columnVariableCode).ToList(),
                [columnVariableCode]);
        }

        public static Layout GetTableLayout(IReadOnlyList<string> rowCodes, IReadOnlyList<string> columnCodes)
        {
            return new Layout(rowCodes, columnCodes);
        }

        public static Layout GetPivotBasedLayout(VisualizationType type, IReadOnlyCubeMeta meta, CubeQuery query, bool pivotRequested = false)
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

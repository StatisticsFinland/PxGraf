using PxGraf.Enums;
using PxGraf.Exceptions;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    /// <summary>
    /// A static class with methods for converting ChartType enums to strings and strings to enums
    /// </summary>
    public static class ChartTypeEnumConverter
    {
        /// <summary>
        /// Converts chart type enums to their string prepresentations
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(VisualizationType type)
        {
            return type switch
            {
                VisualizationType.StackedVerticalBarChart => "vbstack",
                VisualizationType.PercentVerticalBarChart => "vbpr100",
                VisualizationType.HorizontalBarChart => "hbar",
                VisualizationType.GroupHorizontalBarChart => "hbargrp",
                VisualizationType.StackedHorizontalBarChart => "hbstack",
                VisualizationType.PercentHorizontalBarChart => "hbpr100",
                VisualizationType.PyramidChart => "pyra",
                VisualizationType.PieChart => "pie",
                VisualizationType.LineChart => "line",
                VisualizationType.ScatterPlot => "scatter",
                VisualizationType.VerticalBarChart => "vbar",
                VisualizationType.GroupVerticalBarChart => "vbargrp",
                VisualizationType.Table => "table",
                VisualizationType.KeyFigure => "keyfig",
                _ => throw new UnknownChartTypeException("Given chart type does not have a string representation", type)
            };
        }

        /// <summary>
        /// Converts a collection of chart type enums to strings.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToString(IEnumerable<VisualizationType> types)
        {
            foreach (VisualizationType type in types) yield return ToString(type);
        }

        /// <summary>
        /// Converts chart type string to ChartType enums
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static VisualizationType? ToEnum(string typeString)
        {
            return typeString switch
            {
                "vbstack" => VisualizationType.StackedVerticalBarChart,
                "vbpr100" => VisualizationType.PercentVerticalBarChart,
                "hbar" => VisualizationType.HorizontalBarChart,
                "hbargrp" => VisualizationType.GroupHorizontalBarChart,
                "hbstack" => VisualizationType.StackedHorizontalBarChart,
                "hbpr100" => VisualizationType.PercentHorizontalBarChart,
                "pyra" => VisualizationType.PyramidChart,
                "pie" => VisualizationType.PieChart,
                "line" => VisualizationType.LineChart,
                "scatter" => VisualizationType.ScatterPlot,
                "vbar" => VisualizationType.VerticalBarChart,
                "vbargrp" => VisualizationType.GroupVerticalBarChart,
                "table" => VisualizationType.Table,
                "keyfig" => VisualizationType.KeyFigure,
                _ => null
            };
        }

        public static string ToJsonString(VisualizationType type)
        {
            return type switch
            {
                VisualizationType.VerticalBarChart => "VerticalBarChart",
                VisualizationType.GroupVerticalBarChart => "GroupVerticalBarChart",
                VisualizationType.StackedVerticalBarChart => "StackedVerticalBarChart",
                VisualizationType.PercentVerticalBarChart => "PercentVerticalBarChart",

                VisualizationType.HorizontalBarChart => "HorizontalBarChart",
                VisualizationType.GroupHorizontalBarChart => "GroupHorizontalBarChart",
                VisualizationType.StackedHorizontalBarChart => "StackedHorizontalBarChart",
                VisualizationType.PercentHorizontalBarChart => "PercentHorizontalBarChart",

                VisualizationType.PyramidChart => "PyramidChart",
                VisualizationType.PieChart => "PieChart",
                VisualizationType.LineChart => "LineChart",
                VisualizationType.ScatterPlot => "ScatterPlot",
                VisualizationType.Table => "Table",
                VisualizationType.KeyFigure => "KeyFigure",

                _ => throw new UnknownChartTypeException("Given chart type does not have a string representation", type),
            };
        }

        public static VisualizationType FromJsonString(string typeString)
        {
            return typeString.ToLowerInvariant() switch
            {
                "verticalbarchart" => VisualizationType.VerticalBarChart,
                "groupverticalbarchart" => VisualizationType.GroupVerticalBarChart,
                "stackedverticalbarchart" => VisualizationType.StackedVerticalBarChart,
                "percentverticalbarchart" => VisualizationType.PercentVerticalBarChart,

                "horizontalbarchart" => VisualizationType.HorizontalBarChart,
                "grouphorizontalbarchart" => VisualizationType.GroupHorizontalBarChart,
                "stackedhorizontalbarchart" => VisualizationType.StackedHorizontalBarChart,
                "percenthorizontalbarchart" => VisualizationType.PercentHorizontalBarChart,

                "pyramidchart" => VisualizationType.PyramidChart,
                "piechart" => VisualizationType.PieChart,
                "linechart" => VisualizationType.LineChart,
                "scatterplot" => VisualizationType.ScatterPlot,
                "table" => VisualizationType.Table,
                "keyfigure" => VisualizationType.KeyFigure,

                _ => throw new UnknownChartTypeException("Given string does not have a chart type representation"),
            };
        }
    }
}

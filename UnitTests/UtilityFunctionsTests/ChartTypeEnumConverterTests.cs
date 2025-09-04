using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Exceptions;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.UtilityFunctionsTests
{
    [TestFixture]
    internal class ChartTypeEnumConverterTests
    {
        [TestCase(VisualizationType.VerticalBarChart, "vbar")]
        [TestCase(VisualizationType.GroupVerticalBarChart, "vbargrp")]
        [TestCase(VisualizationType.StackedVerticalBarChart, "vbstack")]
        [TestCase(VisualizationType.PercentVerticalBarChart, "vbpr100")]
        [TestCase(VisualizationType.HorizontalBarChart, "hbar")]
        [TestCase(VisualizationType.GroupHorizontalBarChart, "hbargrp")]
        [TestCase(VisualizationType.StackedHorizontalBarChart, "hbstack")]
        [TestCase(VisualizationType.PercentHorizontalBarChart, "hbpr100")]
        [TestCase(VisualizationType.PyramidChart, "pyra")]
        [TestCase(VisualizationType.PieChart, "pie")]
        [TestCase(VisualizationType.LineChart, "line")]
        [TestCase(VisualizationType.ScatterPlot, "scatter")]
        [TestCase(VisualizationType.Table, "table")]
        [TestCase(VisualizationType.KeyFigure, "keyfig")]
        public void ToString_ReturnsCorrectString(VisualizationType type, string expectedResult)
        {
            string result = ChartTypeEnumConverter.ToString(type);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void ToString_WithInvalidEnum_ThrowsException()
        {
            Assert.Throws<UnknownChartTypeException>(() => 
                ChartTypeEnumConverter.ToString((VisualizationType)999));
        }

        [Test]
        public void ToString_WithMultipleTypes_ReturnsCorrectStrings()
        {
            List<VisualizationType> types =
            [
                VisualizationType.VerticalBarChart,
                VisualizationType.PieChart,
                VisualizationType.LineChart
            ];

            IEnumerable<string> results = ChartTypeEnumConverter.ToString(types);
            
            Assert.Multiple(() =>
            {
                Assert.That(results.Count(), Is.EqualTo(3));
                Assert.That(results.ElementAt(0), Is.EqualTo("vbar"));
                Assert.That(results.ElementAt(1), Is.EqualTo("pie"));
                Assert.That(results.ElementAt(2), Is.EqualTo("line"));
            });
        }

        [TestCase("vbar", VisualizationType.VerticalBarChart)]
        [TestCase("vbargrp", VisualizationType.GroupVerticalBarChart)]
        [TestCase("vbstack", VisualizationType.StackedVerticalBarChart)]
        [TestCase("vbpr100", VisualizationType.PercentVerticalBarChart)]
        [TestCase("hbar", VisualizationType.HorizontalBarChart)]
        [TestCase("hbargrp", VisualizationType.GroupHorizontalBarChart)]
        [TestCase("hbstack", VisualizationType.StackedHorizontalBarChart)]
        [TestCase("hbpr100", VisualizationType.PercentHorizontalBarChart)]
        [TestCase("pyra", VisualizationType.PyramidChart)]
        [TestCase("pie", VisualizationType.PieChart)]
        [TestCase("line", VisualizationType.LineChart)]
        [TestCase("scatter", VisualizationType.ScatterPlot)]
        [TestCase("table", VisualizationType.Table)]
        [TestCase("keyfig", VisualizationType.KeyFigure)]
        public void ToEnum_ReturnsCorrectType(string typeString, VisualizationType expectedType)
        {
            VisualizationType? result = ChartTypeEnumConverter.ToEnum(typeString);
            Assert.That(result, Is.EqualTo(expectedType));
        }

        [TestCase("invalid")]
        [TestCase("")]
        [TestCase(null)]
        public void ToEnum_WithInvalidString_ReturnsNull(string invalidTypeString)
        {
            VisualizationType? result = ChartTypeEnumConverter.ToEnum(invalidTypeString);
            Assert.That(result, Is.Null);
        }

        [TestCase(VisualizationType.VerticalBarChart, "VerticalBarChart")]
        [TestCase(VisualizationType.GroupVerticalBarChart, "GroupVerticalBarChart")]
        [TestCase(VisualizationType.StackedVerticalBarChart, "StackedVerticalBarChart")]
        [TestCase(VisualizationType.PercentVerticalBarChart, "PercentVerticalBarChart")]
        [TestCase(VisualizationType.HorizontalBarChart, "HorizontalBarChart")]
        [TestCase(VisualizationType.GroupHorizontalBarChart, "GroupHorizontalBarChart")]
        [TestCase(VisualizationType.StackedHorizontalBarChart, "StackedHorizontalBarChart")]
        [TestCase(VisualizationType.PercentHorizontalBarChart, "PercentHorizontalBarChart")]
        [TestCase(VisualizationType.PyramidChart, "PyramidChart")]
        [TestCase(VisualizationType.PieChart, "PieChart")]
        [TestCase(VisualizationType.LineChart, "LineChart")]
        [TestCase(VisualizationType.ScatterPlot, "ScatterPlot")]
        [TestCase(VisualizationType.Table, "Table")]
        [TestCase(VisualizationType.KeyFigure, "KeyFigure")]
        public void ToJsonString_ReturnsCorrectString(VisualizationType type, string expectedResult)
        {
            string result = ChartTypeEnumConverter.ToJsonString(type);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void ToJsonString_WithInvalidEnum_ThrowsException()
        {
            Assert.Throws<UnknownChartTypeException>(() =>
                ChartTypeEnumConverter.ToJsonString((VisualizationType)999));
        }

        [TestCase("VerticalBarChart", VisualizationType.VerticalBarChart)]
        [TestCase("verticalbarchart", VisualizationType.VerticalBarChart)]
        [TestCase("VERTICALBARCHART", VisualizationType.VerticalBarChart)]
        [TestCase("GroupVerticalBarChart", VisualizationType.GroupVerticalBarChart)]
        [TestCase("groupverticalbarchart", VisualizationType.GroupVerticalBarChart)]
        [TestCase("StackedVerticalBarChart", VisualizationType.StackedVerticalBarChart)]
        [TestCase("stackedverticalbarchart", VisualizationType.StackedVerticalBarChart)]
        [TestCase("PercentVerticalBarChart", VisualizationType.PercentVerticalBarChart)]
        [TestCase("percentverticalbarchart", VisualizationType.PercentVerticalBarChart)]
        [TestCase("HorizontalBarChart", VisualizationType.HorizontalBarChart)]
        [TestCase("horizontalbarchart", VisualizationType.HorizontalBarChart)]
        [TestCase("GroupHorizontalBarChart", VisualizationType.GroupHorizontalBarChart)]
        [TestCase("grouphorizontalbarchart", VisualizationType.GroupHorizontalBarChart)]
        [TestCase("StackedHorizontalBarChart", VisualizationType.StackedHorizontalBarChart)]
        [TestCase("stackedhorizontalbarchart", VisualizationType.StackedHorizontalBarChart)]
        [TestCase("PercentHorizontalBarChart", VisualizationType.PercentHorizontalBarChart)]
        [TestCase("percenthorizontalbarchart", VisualizationType.PercentHorizontalBarChart)]
        [TestCase("PyramidChart", VisualizationType.PyramidChart)]
        [TestCase("pyramidchart", VisualizationType.PyramidChart)]
        [TestCase("PieChart", VisualizationType.PieChart)]
        [TestCase("piechart", VisualizationType.PieChart)]
        [TestCase("LineChart", VisualizationType.LineChart)]
        [TestCase("linechart", VisualizationType.LineChart)]
        [TestCase("ScatterPlot", VisualizationType.ScatterPlot)]
        [TestCase("scatterplot", VisualizationType.ScatterPlot)]
        [TestCase("Table", VisualizationType.Table)]
        [TestCase("table", VisualizationType.Table)]
        [TestCase("KeyFigure", VisualizationType.KeyFigure)]
        [TestCase("keyfigure", VisualizationType.KeyFigure)]
        public void FromJsonString_ReturnsCorrectType(string jsonString, VisualizationType expectedType)
        {
            VisualizationType result = ChartTypeEnumConverter.FromJsonString(jsonString);
            Assert.That(result, Is.EqualTo(expectedType));
        }

        [TestCase("invalid")]
        [TestCase("")]
        public void FromJsonString_WithInvalidString_ThrowsException(string invalidJsonString)
        {
            Assert.Throws<UnknownChartTypeException>(() =>
                ChartTypeEnumConverter.FromJsonString(invalidJsonString));
        }

        [Test]
        public void FromJsonString_WithNullString_ThrowsException()
        {
            Assert.Throws<NullReferenceException>(() =>
                ChartTypeEnumConverter.FromJsonString(null));
        }

        [Test]
        public void RoundtripTest_ToString_ToEnum()
        {
            Assert.Multiple(() =>
            {
                foreach (VisualizationType type in GetAllVisualizationTypes())
                {
                    string typeString = ChartTypeEnumConverter.ToString(type);
                    VisualizationType? convertedBack = ChartTypeEnumConverter.ToEnum(typeString);
                    Assert.That(convertedBack, Is.EqualTo(type), $"Failed for {type}");
                }
            });
        }

        [Test]
        public void RoundtripTest_ToJsonString_FromJsonString()
        {
            Assert.Multiple(() =>
            {
                foreach (VisualizationType type in GetAllVisualizationTypes())
                {
                    string jsonString = ChartTypeEnumConverter.ToJsonString(type);
                    VisualizationType convertedBack = ChartTypeEnumConverter.FromJsonString(jsonString);
                    Assert.That(convertedBack, Is.EqualTo(type), $"Failed for {type}");
                }
            });
        }

        private static IEnumerable<VisualizationType> GetAllVisualizationTypes()
        {
            return Enum.GetValues(typeof(VisualizationType))
                .Cast<VisualizationType>();
        }
    }
}

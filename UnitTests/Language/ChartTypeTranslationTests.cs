using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Language;
using System;

namespace UnitTests.Language
{
    [TestFixture]
    internal class ChartTypeTranslationTests
    {
        private ChartTypeTranslation translation;

        [SetUp]
        public void Setup()
        {
            // Initialize with test translations
            translation = new ChartTypeTranslation
            {
                VerticalBar = "Vertical Bar",
                GroupVerticalBar = "Group Vertical Bar",
                StackedVerticalBar = "Stacked Vertical Bar",
                PercentVerticalBar = "Percent Vertical Bar",
                HorizontalBar = "Horizontal Bar",
                GroupHorizontalBar = "Group Horizontal Bar",
                StackedHorizontalBar = "Stacked Horizontal Bar",
                PercentHorizontalBar = "Percent Horizontal Bar",
                Pyramid = "Pyramid",
                Pie = "Pie",
                Line = "Line",
                ScatterPlot = "Scatter Plot",
                Table = "Table",
                KeyFigure = "Key Figure"
            };
        }

        [TestCase(VisualizationType.VerticalBarChart, "Vertical Bar")]
        [TestCase(VisualizationType.GroupVerticalBarChart, "Group Vertical Bar")]
        [TestCase(VisualizationType.StackedVerticalBarChart, "Stacked Vertical Bar")]
        [TestCase(VisualizationType.PercentVerticalBarChart, "Percent Vertical Bar")]
        [TestCase(VisualizationType.HorizontalBarChart, "Horizontal Bar")]
        [TestCase(VisualizationType.GroupHorizontalBarChart, "Group Horizontal Bar")]
        [TestCase(VisualizationType.StackedHorizontalBarChart, "Stacked Horizontal Bar")]
        [TestCase(VisualizationType.PercentHorizontalBarChart, "Percent Horizontal Bar")]
        [TestCase(VisualizationType.PyramidChart, "Pyramid")]
        [TestCase(VisualizationType.PieChart, "Pie")]
        [TestCase(VisualizationType.LineChart, "Line")]
        [TestCase(VisualizationType.ScatterPlot, "Scatter Plot")]
        [TestCase(VisualizationType.Table, "Table")]
        [TestCase(VisualizationType.KeyFigure, "Key Figure")]
        public void GetTranslation_ReturnsCorrectTranslation(VisualizationType chartType, string expected)
        {
            string result = translation.GetTranslation(chartType);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetTranslation_WithInvalidEnum_ThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => 
                translation.GetTranslation((VisualizationType)999));
        }

        [Test]
        public void Properties_AreCorrectlySetAndRetrieved()
        {
            ChartTypeTranslation testTranslation = new()
            {
                VerticalBar = "Test VerticalBar",
                GroupVerticalBar = "Test GroupVerticalBar",
                StackedVerticalBar = "Test StackedVerticalBar",
                PercentVerticalBar = "Test PercentVerticalBar",
                HorizontalBar = "Test HorizontalBar",
                GroupHorizontalBar = "Test GroupHorizontalBar",
                StackedHorizontalBar = "Test StackedHorizontalBar",
                PercentHorizontalBar = "Test PercentHorizontalBar",
                Pyramid = "Test Pyramid",
                Pie = "Test Pie",
                Line = "Test Line",
                ScatterPlot = "Test ScatterPlot",
                Table = "Test Table",
                KeyFigure = "Test KeyFigure"
            };

            Assert.Multiple(() =>
            {
                Assert.That(testTranslation.VerticalBar, Is.EqualTo("Test VerticalBar"));
                Assert.That(testTranslation.GroupVerticalBar, Is.EqualTo("Test GroupVerticalBar"));
                Assert.That(testTranslation.StackedVerticalBar, Is.EqualTo("Test StackedVerticalBar"));
                Assert.That(testTranslation.PercentVerticalBar, Is.EqualTo("Test PercentVerticalBar"));
                Assert.That(testTranslation.HorizontalBar, Is.EqualTo("Test HorizontalBar"));
                Assert.That(testTranslation.GroupHorizontalBar, Is.EqualTo("Test GroupHorizontalBar"));
                Assert.That(testTranslation.StackedHorizontalBar, Is.EqualTo("Test StackedHorizontalBar"));
                Assert.That(testTranslation.PercentHorizontalBar, Is.EqualTo("Test PercentHorizontalBar"));
                Assert.That(testTranslation.Pyramid, Is.EqualTo("Test Pyramid"));
                Assert.That(testTranslation.Pie, Is.EqualTo("Test Pie"));
                Assert.That(testTranslation.Line, Is.EqualTo("Test Line"));
                Assert.That(testTranslation.ScatterPlot, Is.EqualTo("Test ScatterPlot"));
                Assert.That(testTranslation.Table, Is.EqualTo("Test Table"));
                Assert.That(testTranslation.KeyFigure, Is.EqualTo("Test KeyFigure"));
            });
        }

        [Test]
        public void AllVisualizationTypes_AreHandled()
        {
            foreach (VisualizationType chartType in Enum.GetValues(typeof(VisualizationType)))
            {
                Assert.DoesNotThrow(() => translation.GetTranslation(chartType));
            }
        }
    }
}

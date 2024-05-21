using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace LayoutTests
{
    class AutoPivotTests
    {
        [Test]
        public void BasicVertical1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.BasicVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupVertical1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 2),
                new VariableParameters(VariableType.Time, 5),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupVertical3()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3),
                new VariableParameters(VariableType.Unknown, 3)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Unknown, 4)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical5()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3),
                new VariableParameters(VariableType.Ordinal, 4)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 2),
                new VariableParameters(VariableType.Time, 5),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical3()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 6)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 6)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical_ColumnTimeSmallerThanOtherClassificatory_Pivot()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical_RowTimeSmallerThanOtherClassificatory_DoNotPivot()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.Time, 5),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical_RowTimeSmallerThanOrdinal_Pivot()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 7),
                new VariableParameters(VariableType.Time, 5),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void BasicHorizontal1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.BasicHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupHorizontal1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Time, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupHorizontal2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupHorizontal3()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Ordinal, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupHorizontal4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedHorizontal1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedHorizontal2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 2),
                new VariableParameters(VariableType.Unknown, 6),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void PieChart1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PieChart(meta), Is.False);
        }

        [Test]
        public void LineChart1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
                new VariableParameters(VariableType.Unknown, 4)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.True);
        }

        [Test]
        public void LineChart2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Time, 6),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void LineChart3()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Time, 6),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void LineChart4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void PyramidChart1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.Unknown, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PyramidChart(meta), Is.True);
        }

        [Test]
        public void PyramidChart2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 2),
                new VariableParameters(VariableType.Time, 12),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PyramidChart(meta), Is.False);
        }

        [Test]
        public void ScatterPlot1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Unknown, 12),
                new VariableParameters(VariableType.Content, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.True);
        }

        [Test]
        public void ScatterPlot2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 2),
                new VariableParameters(VariableType.Ordinal, 12),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.True);
        }

        [Test]
        public void ScatterPlotWithFourDataPointsAndContetFirst_NoAutopivot()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 2),
                new VariableParameters(VariableType.Time, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.False);
        }

        [Test]
        public void ScatterPlotWithFourDataPointsAndTimeFirst_Autopivot()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Content, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.True);
        }

    }
}

using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Data;
using System.Collections.Generic;
using UnitTests;

namespace LayoutTests
{
    class AutoPivotTests
    {
        [Test]
        public void BasicVertical1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 1)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.BasicVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupVertical1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 2)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 2),
                new DimensionParameters(DimensionType.Time, 5),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupVertical3()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Unknown, 3)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 3),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupVertical5()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Ordinal, 4)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 2)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 2),
                new DimensionParameters(DimensionType.Time, 5),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical3()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 6)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Unknown, 6)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical_ColumnTimeSmallerThanOtherClassificatory_Pivot()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedVertical_RowTimeSmallerThanOtherClassificatory_DoNotPivot()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Time, 5),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.False);
        }

        [Test]
        public void StackedVertical_RowTimeSmallerThanOrdinal_Pivot()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 7),
                new DimensionParameters(DimensionType.Time, 5),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedVerticalBarChart(meta), Is.True);
        }

        [Test]
        public void BasicHorizontal1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.BasicHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupHorizontal1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 3),
                new DimensionParameters(DimensionType.Time, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupHorizontal2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void GroupHorizontal3()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 3),
                new DimensionParameters(DimensionType.Ordinal, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void GroupHorizontal4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
                new DimensionParameters(DimensionType.Unknown, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.GroupHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedHorizontal1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
                new DimensionParameters(DimensionType.Unknown, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedHorizontalBarChart(meta), Is.True);
        }

        [Test]
        public void StackedHorizontal2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 2),
                new DimensionParameters(DimensionType.Unknown, 6),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.StackedHorizontalBarChart(meta), Is.False);
        }

        [Test]
        public void PieChart1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PieChart(meta), Is.False);
        }

        [Test]
        public void LineChart1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 6),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.True);
        }

        [Test]
        public void LineChart2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Time, 6),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void LineChart3()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Time, 6),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void LineChart4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 6),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.LineChart(meta), Is.False);
        }

        [Test]
        public void PyramidChart1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Unknown, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PyramidChart(meta), Is.True);
        }

        [Test]
        public void PyramidChart2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 2),
                new DimensionParameters(DimensionType.Time, 12),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.PyramidChart(meta), Is.False);
        }

        [Test]
        public void ScatterPlot1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Unknown, 12),
                new DimensionParameters(DimensionType.Content, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.True);
        }

        [Test]
        public void ScatterPlot2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 2),
                new DimensionParameters(DimensionType.Ordinal, 12),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.False);
        }

        [Test]
        public void ScatterPlotWithFourDataPointsAndContetFirst_NoAutopivot()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 2),
                new DimensionParameters(DimensionType.Time, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.False);
        }

        [Test]
        public void ScatterPlotWithFourDataPointsAndTimeFirst_Autopivot()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(AutoPivotRules.ScatterPlot(meta), Is.True);
        }

    }
}

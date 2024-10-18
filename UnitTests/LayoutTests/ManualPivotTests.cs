using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Data;
using System.Collections.Generic;

namespace UnitTests.LayoutTests
{
    public class ManualPivotTests
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.BasicVerticalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
        }

        [Test]
        public void StackedVertical1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Unknown, 2)
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.True);
        }

        [Test]
        public void StackedVertical2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 3),
                new DimensionParameters(DimensionType.Time, 11),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.False);
        }

        [Test]
        public void StackedVertical4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Unknown, 6)
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.True);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.BasicHorizontalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void GroupHorizontal4()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void GroupHorizontal5()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void StackedHorizontal2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
                new DimensionParameters(DimensionType.Unknown, 9),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void StackedHorizontal3()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 3),
                new DimensionParameters(DimensionType.Unknown, 11),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.False);
        }

        [Test]
        public void PieChart1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 6),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.PieChart(query), Is.False);
        }

        [Test]
        public void LineChart1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 6),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
        }

        [Test]
        public void LineChart2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 6),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
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

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.PyramidChart(query), Is.False);
        }

        [Test]
        public void ScatterPlot1()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Unknown, 12),
                new DimensionParameters(DimensionType.Content, 2),
            ];

            IReadOnlyMatrixMetadata query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.ScatterPlot(query), Is.False);
        }
    }
}


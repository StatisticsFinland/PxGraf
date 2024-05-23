using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace LayoutTests
{
    class ManualPivotTests
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.BasicVerticalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupVerticalBarChart(query), Is.True);
        }

        [Test]
        public void StackedVertical1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.True);
        }

        [Test]
        public void StackedVertical2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Time, 11),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.False);
        }

        [Test]
        public void StackedVertical4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 6)
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedVerticalBarChart(query), Is.True);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.BasicHorizontalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void GroupHorizontal4()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void GroupHorizontal5()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.GroupHorizontalBarChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void StackedHorizontal2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 9),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.True);
        }

        [Test]
        public void StackedHorizontal3()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 11),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.StackedHorizontalBarChart(query), Is.False);
        }

        [Test]
        public void PieChart1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.PieChart(query), Is.False);
        }

        [Test]
        public void LineChart1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
        }

        [Test]
        public void LineChart2()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
                new VariableParameters(VariableType.Unknown, 4)
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.LineChart(query), Is.False);
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

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.PyramidChart(query), Is.False);
        }

        [Test]
        public void ScatterPlot1()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Unknown, 12),
                new VariableParameters(VariableType.Content, 2),
            ];

            CubeMeta query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.That(ManualPivotRules.ScatterPlot(query), Is.False);
        }
    }
}


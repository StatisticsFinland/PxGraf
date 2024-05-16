using NUnit.Framework;
using PxGraf.Data;
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
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.BasicVerticalBarChart(query));
        }

        [Test]
        public void GroupVertical1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 2)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.GroupVerticalBarChart(query));
        }

        [Test]
        public void GroupVertical2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 2),
                new VariableParameters(VariableType.Time, 5),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.GroupVerticalBarChart(query));
        }

        [Test]
        public void GroupVertical3()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3),
                new VariableParameters(VariableType.Unknown, 3)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.GroupVerticalBarChart(query));
        }

        [Test]
        public void GroupVertical4()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.GroupVerticalBarChart(query));
        }

        [Test]
        public void GroupVertical5()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3),
                new VariableParameters(VariableType.Ordinal, 4)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.GroupVerticalBarChart(query));
        }

        [Test]
        public void StackedVertical1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 2)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.StackedVerticalBarChart(query));
        }

        [Test]
        public void StackedVertical2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Time, 11),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.StackedVerticalBarChart(query));
        }

        [Test]
        public void StackedVertical3()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 6)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.StackedVerticalBarChart(query));
        }

        [Test]
        public void StackedVertical4()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 6)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.StackedVerticalBarChart(query));
        }

        [Test]
        public void BasicHorizontal1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.BasicHorizontalBarChart(query));
        }

        [Test]
        public void GroupHorizontal1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Time, 2),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.GroupHorizontalBarChart(query));
        }

        [Test]
        public void GroupHorizontal2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.GroupHorizontalBarChart(query));
        }

        [Test]
        public void GroupHorizontal3()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Ordinal, 2),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.GroupHorizontalBarChart(query));
        }

        [Test]
        public void GroupHorizontal4()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.GroupHorizontalBarChart(query));
        }

        [Test]
        public void GroupHorizontal5()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.GroupHorizontalBarChart(query));
        }

        [Test]
        public void StackedHorizontal1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 2),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.StackedHorizontalBarChart(query));
        }

        [Test]
        public void StackedHorizontal2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
                new VariableParameters(VariableType.Unknown, 9),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsTrue(ManualPivotRules.StackedHorizontalBarChart(query));
        }

        [Test]
        public void StackedHorizontal3()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 11),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.StackedHorizontalBarChart(query));
        }

        [Test]
        public void PieChart1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 6),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.PieChart(query));
        }

        [Test]
        public void LineChart1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.LineChart(query));
        }

        [Test]
        public void LineChart2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 6),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.LineChart(query));
        }

        [Test]
        public void LineChart3()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Time, 6),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.LineChart(query));
        }

        [Test]
        public void PyramidChart1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.Unknown, 2),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.PyramidChart(query));
        }

        [Test]
        public void ScatterPlot1()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Unknown, 12),
                new VariableParameters(VariableType.Content, 2),
            };

            var query = TestDataCubeBuilder.BuildTestMeta(variables);
            Assert.IsFalse(ManualPivotRules.ScatterPlot(query));
        }
    }
}


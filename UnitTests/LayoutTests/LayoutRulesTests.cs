using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace LayoutTests
{
    internal static class LayoutRulesTests
    {
        [Test]
        public static void GetLineChartLayoutTest_OneTimeDimension_TimeIsColumnVar()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 1)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>(), new List<string>() { "variable-1" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_TwoDimensional_TimeIsColumnVarOtherIsRow()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>() { "variable-2" }, new List<string>() { "variable-1" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TimeIsColumnVarOthersAreRow()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>() { "variable-2", "variable-3" }, new List<string>() { "variable-1" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TimeIsColumnVarOthersAreRow_Order2()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.Time, 10)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>() { "variable-1", "variable-2" }, new List<string>() { "variable-3" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_OneSelectable_TimeIsColumnVarOthersAreRow()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 10)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>() { "variable-1" }, new List<string>() { "variable-3" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TwoSelectables_TimeIsColumnVarOthersAreRow()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Unknown, 5) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 10)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>() { "variable-1" }, new List<string>() { "variable-4" });

            Assert.IsTrue(result.Equals(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_TwoDimensional_TimeVarIsSelectable_OrdinalIsColumnVar()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10) { Selectable = true },
                new VariableParameters(VariableType.Ordinal, 10)
            };

            CubeQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(new List<string>(), new List<string>() { "variable-2" });

            Assert.IsTrue(result.Equals(expexted));
        }
    }
}

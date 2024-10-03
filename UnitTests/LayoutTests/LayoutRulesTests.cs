using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Data;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using UnitTests;

namespace LayoutTests
{
    internal static class LayoutRulesTests
    {
        [Test]
        public static void GetLineChartLayoutTest_OneTimeDimension_TimeIsColumnVar()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 1)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new([], ["variable-1"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_TwoDimensional_TimeIsColumnVarOtherIsRow()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(["variable-2"], ["variable-1"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TimeIsColumnVarOthersAreRow()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Other, 2)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(["variable-2", "variable-3"], ["variable-1"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TimeIsColumnVarOthersAreRow_Order2()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Time, 10)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(["variable-1", "variable-2"], ["variable-3"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_OneSelectable_TimeIsColumnVarOthersAreRow()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true },
                new DimensionParameters(DimensionType.Time, 10)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(["variable-1"], ["variable-3"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_ThreeDimensional_TwoSelectables_TimeIsColumnVarOthersAreRow()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Unknown, 5) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true },
                new DimensionParameters(DimensionType.Time, 10)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new(["variable-1"], ["variable-4"]);

            Assert.That(result, Is.EqualTo(expexted));
        }

        [Test]
        public static void GetLineChartLayoutTest_TwoDimensional_TimeVarIsSelectable_OrdinalIsColumnVar()
        {
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10) { Selectable = true },
                new DimensionParameters(DimensionType.Ordinal, 10)
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Layout result = LayoutRules.GetLineChartLayout(testMeta, testCubeQuery);
            Layout expexted = new([], ["variable-2"]);

            Assert.That(result, Is.EqualTo(expexted));
        }
    }
}

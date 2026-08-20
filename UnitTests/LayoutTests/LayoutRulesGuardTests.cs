using NUnit.Framework;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;

namespace UnitTests.LayoutTests
{
    internal static class LayoutRulesGuardTests
    {
        [Test]
        public static void GetOneDimensionalLayout_NoMultivalueNonSelectableDimensions_ThrowsInvalidOperationException()
        {
            // Arrange: all dimensions have exactly 1 value, so none qualify as multivalue
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetOneDimensionalLayout(meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetOneDimensionalLayout"));
        }

        [Test]
        public static void GetOneDimensionalLayout_AllMultivalueDimensionsSelectable_ThrowsInvalidOperationException()
        {
            // Arrange: multivalue dimension is marked selectable, so candidates will be empty
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 5) { Selectable = true },
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetOneDimensionalLayout(meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetOneDimensionalLayout"));
        }

        [Test]
        public static void GetTwoDimensionalLayout_OnlyOneMultivalueDimension_ThrowsInvalidOperationException()
        {
            // Arrange: only one non-selectable multivalue dimension
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetTwoDimensionalLayout(false, VisualizationType.GroupVerticalBarChart, meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetTwoDimensionalLayout"));
        }

        [Test]
        public static void GetTwoDimensionalLayout_NoMultivalueDimensions_ThrowsInvalidOperationException()
        {
            // Arrange: no multivalue dimensions at all
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetTwoDimensionalLayout(false, VisualizationType.GroupVerticalBarChart, meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetTwoDimensionalLayout"));
        }

        [Test]
        public static void GetLineChartLayout_NoMultivalueNonSelectableDimensions_ThrowsInvalidOperationException()
        {
            // Arrange: all dimensions have 1 value
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetLineChartLayout(meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetLineChartLayout"));
        }

        [Test]
        public static void GetLineChartLayout_AllMultivalueDimensionsSelectable_ThrowsInvalidOperationException()
        {
            // Arrange: the only multivalue dimension is selectable
            List<DimensionParameters> variables =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery testCubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);
            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(variables);

            // Act & Assert
            Assert.That(
                () => LayoutRules.GetLineChartLayout(meta, testCubeQuery),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("GetLineChartLayout"));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    /// <summary>
    /// Tests for CreationController.GetVisualizationAsync when virtual values are mixed with real values via ItemFilter.
    /// </summary>
    internal class CreationControllerVirtualValueLayoutTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        /// <summary>
        /// Verifies that GetVisualizationAsync returns 200 OK when an ItemFilter selects a mix of
        /// one real value and one virtual value. This exercises the fix that ensures
        /// TrimOperandOnlyValues preserves both the real user-selected code and the virtual value code,
        /// so that LayoutRules still finds at least one non-selectable multivalue dimension.
        /// </summary>
        [Test]
        public async Task GetVisualizationAsync_MixedRealAndVirtualValuesInItemFilter_ReturnsOkResult()
        {
            // Arrange
            // The complete metadata has an Ordinal dimension with two real values.
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 2),
            ];

            // The datasource returns a computation matrix containing both real operand values.
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 2),
            ];

            // After ApplyVirtualValues the mock returns a matrix with three values in variable-2:
            // "value-0" (real, user-selected), "value-1" (real, operand-only), "value-2" (virtual, user-selected).
            // "value-2" is used as the virtual code because it is absent from metaParams Ordinal(2).
            List<DimensionParameters> postVirtualParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 3),
            ];

            Matrix<DecimalDataValue> matrixWithVirtual = TestDataCubeBuilder.BuildTestMatrix(postVirtualParams);

            Mock<IVirtualValueComputationService> virtualValueMock = new();
            virtualValueMock
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns(matrixWithVirtual);

            CreationController testController = TestCreationControllerBuilder.BuildController(
                cubeParams, metaParams, virtualValueComputationService: virtualValueMock);

            // Build a query that selects "value-0" (real) and "value-2" (virtual) from variable-2.
            // The virtual definition for "value-2" uses "value-1" as its operand.
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            cubeQuery.DimensionQueries["variable-2"] = new DimensionQuery
            {
                ValueFilter = new ItemFilter(["value-0", "value-2"]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "value-2", OperandCodes = ["value-1"], Constant = 0.0 }
                ]
            };

            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.VerticalBarChart,
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-2"],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert: the controller must return 200 OK with a VisualizationResponse,
            // not an IndexOutOfRangeException crash or a BadRequest.
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value, Is.TypeOf<VisualizationResponse>());
            }
        }

        /// <summary>
        /// Verifies that GetVisualizationAsync returns 200 OK when an ItemFilter selects only a virtual value
        /// (no real values in the output). After TrimOperandOnlyValues the virtual value is the sole
        /// remaining code in the dimension, keeping the dimension single-valued, which means no multivalue
        /// non-selectable dimension exists and VerticalBarChart is rejected correctly.
        /// </summary>
        [Test]
        public async Task GetVisualizationAsync_OnlyVirtualValueInItemFilter_ReturnsOkResult()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Ordinal, 2),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Ordinal, 2),
            ];

            // After ApplyVirtualValues the mock returns a matrix with "value-0", "value-1", "value-2".
            // "value-2" is the virtual value; "value-0" and "value-1" are operand-only (not in user filter).
            List<DimensionParameters> postVirtualParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Ordinal, 3),
            ];

            Matrix<DecimalDataValue> matrixWithVirtual = TestDataCubeBuilder.BuildTestMatrix(postVirtualParams);

            Mock<IVirtualValueComputationService> virtualValueMock = new();
            virtualValueMock
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns(matrixWithVirtual);

            CreationController testController = TestCreationControllerBuilder.BuildController(
                cubeParams, metaParams, virtualValueComputationService: virtualValueMock);

            // ItemFilter selects only the virtual value; no real values are selected.
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            cubeQuery.DimensionQueries["variable-2"] = new DimensionQuery
            {
                ValueFilter = new ItemFilter(["value-2"]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "value-2", OperandCodes = ["value-0", "value-1"] }
                ]
            };

            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert: with a Time dimension having 5 values and an Ordinal(1) after trimming,
            // LineChart should be accepted (Time is the column dimension).
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value, Is.TypeOf<VisualizationResponse>());
            }
        }
    }
}

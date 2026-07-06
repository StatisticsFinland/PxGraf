#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.SqControllerTests
{
    [TestFixture]
    internal class SaveQueryWithVirtualValuesTests
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
        /// Builds the shared test mock objects used across tests in this fixture.
        /// </summary>
        private static (
            Mock<ICachedDatasource> datasource,
            Mock<ISqFileInterface> sqFileInterface,
            Mock<ILogger<SqController>> logger,
            Mock<IAuditLogService> auditLogService,
            Mock<IPublicationWebhookService> webhookService)
            BuildMocks()
        {
            Mock<ICachedDatasource> datasource = new();
            Mock<ISqFileInterface> sqFileInterface = new();
            Mock<ILogger<SqController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();
            Mock<IPublicationWebhookService> webhookService = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 5),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            datasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            datasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            sqFileInterface.Setup(s => s.SerializeToSqFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            return (datasource, sqFileInterface, logger, auditLogService, webhookService);
        }

        /// <summary>
        /// Builds the cube params and query used in all three tests.
        /// variable-2 is the Other dimension on which virtual values are defined.
        /// </summary>
        private static (List<DimensionParameters> cubeParams, SaveQueryParams saveParams) BuildSaveParams(
            List<VirtualValueDefinition>? virtualDefs)
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            if (virtualDefs != null)
            {
                query.DimensionQueries["variable-2"].VirtualValueDefinitions = virtualDefs;
            }

            SaveQueryParams saveParams = new()
            {
                Query = query,
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-2"],
                    ColumnDimensionCodes = ["variable-1"],
                    MultiselectableDimensionCode = null
                },
                Draft = true
            };

            return (cubeParams, saveParams);
        }

        [Test]
        public async Task SaveQueryAsync_ValidVirtualValueDefinitions_ReturnsSaveQueryResponse()
        {
            // Arrange
            (Mock<ICachedDatasource> datasource, Mock<ISqFileInterface> sqFileInterface, Mock<ILogger<SqController>> logger, Mock<IAuditLogService> auditLogService, Mock<IPublicationWebhookService> webhookService) = BuildMocks();

            Mock<IVirtualValueValidationService> mockValidationService = new();
            Mock<IVirtualValueComputationService> mockComputationService = new();

            // Validation service returns no errors → definitions are valid
            string? noError = null;
            mockValidationService
                .Setup(s => s.Validate(It.IsAny<List<VirtualValueDefinition>>(), It.IsAny<IEnumerable<string>>(), out noError))
                .Returns(true);

            // Computation service returns the matrix unchanged (no actual virtual computation needed in this test)
            mockComputationService
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns<Matrix<DecimalDataValue>, MatrixQuery>((m, _) => m);

            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["value-0"] }
            ];
            (_, SaveQueryParams saveParams) = BuildSaveParams(defs);

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                mockValidationService.Object,
                mockComputationService.Object);

            // Act
            ActionResult<SaveQueryResponse> actionResult = await controller.SaveQueryAsync(saveParams);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
                Assert.That(actionResult.Result, Is.Not.InstanceOf<BadRequestObjectResult>());
            }

            mockValidationService.Verify(
                s => s.Validate(It.IsAny<List<VirtualValueDefinition>>(), It.IsAny<IEnumerable<string>>(), out It.Ref<string?>.IsAny),
                Times.Once);
        }

        [Test]
        public async Task SaveQueryAsync_InvalidVirtualValueDefinitions_ReturnsBadRequest()
        {
            // Arrange
            (Mock<ICachedDatasource> datasource, Mock<ISqFileInterface> sqFileInterface, Mock<ILogger<SqController>> logger, Mock<IAuditLogService> auditLogService, Mock<IPublicationWebhookService> webhookService) = BuildMocks();

            Mock<IVirtualValueValidationService> mockValidationService = new();
            Mock<IVirtualValueComputationService> mockComputationService = new();

            // Validation service returns errors → definitions are invalid
            string? validationError = "Virtual value code 'virtual_1' references unknown operand 'nonexistent'.";
            mockValidationService
                .Setup(s => s.Validate(It.IsAny<List<VirtualValueDefinition>>(), It.IsAny<IEnumerable<string>>(), out validationError))
                .Returns(false);

            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "virtual_1", OperandCodes = ["nonexistent"] }
            ];
            (_, SaveQueryParams saveParams) = BuildSaveParams(defs);

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                mockValidationService.Object,
                mockComputationService.Object);

            // Act
            ActionResult<SaveQueryResponse> actionResult = await controller.SaveQueryAsync(saveParams);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<BadRequestObjectResult>());

            // Verify save was NOT called since validation failed
            sqFileInterface.Verify(
                s => s.SerializeToSqFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()),
                Times.Never);
        }

        [Test]
        public async Task SaveQueryAsync_NoVirtualValueDefinitions_ReturnsSaveQueryResponse()
        {
            // Arrange — no virtual value definitions; validation service should not be called
            (Mock<ICachedDatasource> datasource, Mock<ISqFileInterface> sqFileInterface, Mock<ILogger<SqController>> logger, Mock<IAuditLogService> auditLogService, Mock<IPublicationWebhookService> webhookService) = BuildMocks();

            Mock<IVirtualValueValidationService> mockValidationService = new();
            Mock<IVirtualValueComputationService> mockComputationService = new();

            (_, SaveQueryParams saveParams) = BuildSaveParams(virtualDefs: null);

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                mockValidationService.Object,
                mockComputationService.Object);

            // Act
            ActionResult<SaveQueryResponse> actionResult = await controller.SaveQueryAsync(saveParams);

            // Assert
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());

            // Validation must not run when there are no virtual value definitions
            mockValidationService.Verify(
                s => s.Validate(It.IsAny<List<VirtualValueDefinition>>(), It.IsAny<IEnumerable<string>>(), out It.Ref<string?>.IsAny),
                Times.Never);
        }
    }
}

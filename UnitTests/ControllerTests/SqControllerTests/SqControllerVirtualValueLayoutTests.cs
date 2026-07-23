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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.SqControllerTests
{
    /// <summary>
    /// Integration tests for SqController covering the mixed real+virtual value scenario
    /// that previously caused IndexOutOfRangeException in the virtual value pipeline.
    /// </summary>
    [TestFixture]
    internal class SqControllerVirtualValueLayoutTests
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
        /// Builds the shared mock objects used across all tests in this fixture.
        /// The matrix mock returns a matrix whose shape matches the fetch-meta expansion:
        /// variable-1 (Time) has 3 values (2 real selected + 1 operand). The real computation
        /// service appends virtual_time_1, giving 4 Time values; GetTransform then keeps the
        /// 3 output values (2 real + 1 virtual). VerticalBarChart is valid because the single
        /// multivalue dimension is a Time dimension.
        /// </summary>
        private static (
            Mock<ICachedDatasource> datasource,
            Mock<ISqFileInterface> sqFileInterface,
            Mock<ILogger<SqController>> logger,
            Mock<IAuditLogService> auditLogService,
            Mock<IPublicationWebhookService> webhookService,
            Mock<IVirtualValueValidationService> validationService,
            Mock<IVirtualValueComputationService> computationService)
            BuildMocks()
        {
            Mock<ICachedDatasource> datasource = new();
            Mock<ISqFileInterface> sqFileInterface = new();
            Mock<ILogger<SqController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();
            Mock<IPublicationWebhookService> webhookService = new();
            Mock<IVirtualValueValidationService> validationService = new();
            Mock<IVirtualValueComputationService> computationService = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 5),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            // Matrix mock returns the fetch-meta shape:
            // Content(1), Time(3 = 2 real selected + 1 operand), Other(1), Other(1).
            // The real computation service appends virtual_time_1, giving Time(4).
            // GetTransform then reduces it to the 3 output codes (2000, 2001, virtual_time_1),
            // making it the single multivalue dimension VerticalBarChart requires.
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            datasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            datasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            sqFileInterface.Setup(s => s.SerializeToSqFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            sqFileInterface.Setup(s => s.SerializeToArchiveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ArchiveCube>()))
                .Returns(Task.CompletedTask);

            // Validation returns no errors.
            string? noError = null;
            validationService
                .Setup(s => s.Validate(It.IsAny<List<VirtualValueDefinition>>(), It.IsAny<IEnumerable<string>>(), out noError))
                .Returns(true);

            // Use the real computation service so virtual values are properly appended to the matrix.
            Mock<ILogger<VirtualValueComputationService>> computationLogger = new();
            VirtualValueComputationService realComputationService = new(computationLogger.Object);
            computationService
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns<Matrix<DecimalDataValue>, MatrixQuery>(
                    (m, query) => realComputationService.ApplyVirtualValues(m, query));

            return (datasource, sqFileInterface, logger, auditLogService, webhookService, validationService, computationService);
        }

        /// <summary>
        /// Builds the query and SaveQueryParams for the mixed real+virtual scenario.
        /// variable-1 (Time) uses ItemFilter selecting 2 real time values plus virtual "virtual_time_1"
        /// (operand: "2002"). After the pipeline the Time dimension has 3 output values
        /// (2000, 2001, virtual_time_1) and is the sole multivalue dimension, which is valid
        /// for VerticalBarChart.
        /// </summary>
        private static SaveQueryParams BuildMixedVirtualSaveParams()
        {
            List<DimensionParameters> queryParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(queryParams);

            // Override variable-1 (Time) to include a virtual value alongside the 2 real selected values.
            // The operand "2002" is fetched for computation and excluded from the output by GetTransform.
            query.DimensionQueries["variable-1"].ValueFilter = new ItemFilter(["2000", "2001", "virtual_time_1"]);
            query.DimensionQueries["variable-1"].VirtualValueDefinitions =
            [
                new SumDefinition { Code = "virtual_time_1", OperandCodes = ["2002"], Constant = 0.0 }
            ];

            return new SaveQueryParams()
            {
                Query = query,
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.VerticalBarChart
                },
                Draft = true
            };
        }

        /// <summary>
        /// Builds a SavedQuery suitable for GetSavedQueryAsync testing.
        /// The settings use the post-pipeline matrix metadata to derive the correct VerticalBarChart layout.
        /// variable-1 (Time) has 3 output values (2 real + 1 virtual) making it the multivalue axis.
        /// </summary>
        private static SavedQuery BuildMixedVirtualSavedQuery()
        {
            List<DimensionParameters> queryParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(queryParams);

            query.DimensionQueries["variable-1"].ValueFilter = new ItemFilter(["2000", "2001", "virtual_time_1"]);
            query.DimensionQueries["variable-1"].VirtualValueDefinitions =
            [
                new SumDefinition { Code = "virtual_time_1", OperandCodes = ["2002"], Constant = 0.0 }
            ];

            // Build the matrix with the post-pipeline shape (3 time values: 2 real + 1 virtual).
            // This is used solely to derive the correct layout for the saved visualization settings.
            List<DimensionParameters> outputParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];
            Matrix<DecimalDataValue> outputMatrix = TestDataCubeBuilder.BuildTestMatrix(outputParams);

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.VerticalBarChart
            };
            VisualizationSettings vizSettings = creationSettings.ToVisualizationSettings(outputMatrix.Metadata, query);

            return new SavedQuery(query, archived: false, vizSettings, DateTime.Now, draft: false);
        }

        [Test]
        public async Task SaveQueryAsync_MixedRealAndVirtualItemFilter_ReturnsSuccess()
        {
            // Arrange
            (Mock<ICachedDatasource> datasource,
             Mock<ISqFileInterface> sqFileInterface,
             Mock<ILogger<SqController>> logger,
             Mock<IAuditLogService> auditLogService,
             Mock<IPublicationWebhookService> webhookService,
             Mock<IVirtualValueValidationService> validationService,
             Mock<IVirtualValueComputationService> computationService) = BuildMocks();

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                validationService.Object,
                computationService.Object);

            SaveQueryParams saveParams = BuildMixedVirtualSaveParams();

            // Act
            ActionResult<SaveQueryResponse> actionResult = await controller.SaveQueryAsync(saveParams);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
                Assert.That(actionResult.Result, Is.Not.InstanceOf<BadRequestObjectResult>());
            }
        }

        [Test]
        public async Task ArchiveQueryAsync_MixedRealAndVirtualItemFilter_ReturnSuccessAndWritesBothFiles()
        {
            // Arrange
            (Mock<ICachedDatasource> datasource,
             Mock<ISqFileInterface> sqFileInterface,
             Mock<ILogger<SqController>> logger,
             Mock<IAuditLogService> auditLogService,
             Mock<IPublicationWebhookService> webhookService,
             Mock<IVirtualValueValidationService> validationService,
             Mock<IVirtualValueComputationService> computationService) = BuildMocks();

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                validationService.Object,
                computationService.Object);

            SaveQueryParams saveParams = BuildMixedVirtualSaveParams();

            // Act
            ActionResult<SaveQueryResponse> actionResult = await controller.ArchiveQueryAsync(saveParams);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
                Assert.That(actionResult.Result, Is.Not.InstanceOf<BadRequestObjectResult>());
            }

            sqFileInterface.Verify(
                s => s.SerializeToSqFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()),
                Times.Once);
            sqFileInterface.Verify(
                s => s.SerializeToArchiveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ArchiveCube>()),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsync_SavedQueryWithVirtualValues_ReturnsSaveQueryParams()
        {
            // Arrange
            (Mock<ICachedDatasource> datasource,
             Mock<ISqFileInterface> sqFileInterface,
             Mock<ILogger<SqController>> logger,
             Mock<IAuditLogService> auditLogService,
             Mock<IPublicationWebhookService> webhookService,
             Mock<IVirtualValueValidationService> validationService,
             Mock<IVirtualValueComputationService> computationService) = BuildMocks();

            const string testQueryId = "test-virtual-layout-001";

            SavedQuery savedQueryObj = BuildMixedVirtualSavedQuery();

            sqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(true);
            sqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(savedQueryObj);

            SqController controller = new(
                datasource.Object,
                sqFileInterface.Object,
                logger.Object,
                auditLogService.Object,
                webhookService.Object,
                validationService.Object,
                computationService.Object);

            // Act
            ActionResult<SaveQueryParams> actionResult = await controller.GetSavedQueryAsync(testQueryId);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryParams>());
                Assert.That(actionResult.Result, Is.Not.InstanceOf<BadRequestResult>());
            }
        }
    }
}

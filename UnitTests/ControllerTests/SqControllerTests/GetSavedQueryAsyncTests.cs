using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.SqControllerTests
{
    internal class GetSavedQueryAsyncTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> inMemorySettings = new()
            {
                {"DatabaseConfig:Type", "PxWeb"},
                {"DatabaseConfig:PxWebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_Return_SaveQueryParams_With_Valid_Id()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7)
            ];

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null))));

            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object, mockVirtualValueValidationService.Object, mockVirtualValueComputationService.Object);

            // Act
            ActionResult<SaveQueryParams> result = await metaController.GetSavedQueryAsync(testQueryId);

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<SaveQueryParams>>());
            Assert.That(result.Value!.RecoveredWithChanges, Is.False);
            
            // Verify audit log was called with the correct parameters
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_InvalidVisualizationType_ReturnsValidFallback()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7)
            ];

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new HorizontalBarChartVisualizationSettings(null))));

            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object, mockVirtualValueValidationService.Object, mockVirtualValueComputationService.Object);
            
            // Act
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync(testQueryId);

            // Assert
            Assert.That(actionResult.Value, Is.Not.Null);
            Assert.That(actionResult.Value!.Settings.SelectedVisualization, Is.Not.EqualTo(VisualizationType.HorizontalBarChart));
            Assert.That(actionResult.Value.RecoveredWithChanges, Is.True);
            
            // Verify audit log was called with the correct parameters
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_Return_NotFound_When_Query_Does_Not_Exist()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            string testQueryId = "aaa-bbb-111-222-333";

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(false);

            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object, mockVirtualValueValidationService.Object, mockVirtualValueComputationService.Object);
            
            // Act
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync(testQueryId);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundResult>());
            
            // Verify audit log was called with INVALID_OR_MISSING_SQID for not found queries
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_OR_MISSING_SQID)),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_MalformedQueryId_ReturnsBadRequestBeforeCacheLookup()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object, mockVirtualValueValidationService.Object, mockVirtualValueComputationService.Object);

            // Act
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync("invalid/id");

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<BadRequestResult>());
            mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            mockCachedDatasource.Verify(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()), Times.Never());
            mockSqFileInterface.Verify(x => x.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_CalledWithZeroSizedDimension_ThrowsBadRequest()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 0)
            ];

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));
            mockCachedDatasource.Setup(x => x.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(metaParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(metaParams, false, new HorizontalBarChartVisualizationSettings(null))));

            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object, mockVirtualValueValidationService.Object, mockVirtualValueComputationService.Object);
            
            // Act
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync(testQueryId);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<BadRequestResult>());
            
            // Verify audit log was called with the correct parameters
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_StaleTableLayout_SetsRecoveredWithChanges()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();
            Mock<IVirtualValueValidationService> mockVirtualValueValidationService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensionParameters =
            [
                new DimensionParameters(DimensionType.Content, 2) { Name = "content" },
                new DimensionParameters(DimensionType.Time, 2) { Name = "time" },
                new DimensionParameters(DimensionType.Other, 2) { Name = "other" }
            ];
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(
                dimensionParameters,
                false,
                new TableVisualizationSettings(new Layout(["obsolete"], ["content"])));

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(TestDataCubeBuilder.BuildTestMeta(dimensionParameters));
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(TestDataCubeBuilder.BuildTestMatrix(dimensionParameters));
            mockSqFileInterface.Setup(x => x.SavedQueryExists(testQueryId, It.IsAny<string>())).ReturnsAsync(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(testQueryId, It.IsAny<string>())).ReturnsAsync(savedQuery);

            SqController controller = new(
                mockCachedDatasource.Object,
                mockSqFileInterface.Object,
                mockLogger.Object,
                mockAuditLogService.Object,
                mockWebhookService.Object,
                mockVirtualValueValidationService.Object,
                mockVirtualValueComputationService.Object);

            ActionResult<SaveQueryParams> result = await controller.GetSavedQueryAsync(testQueryId);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.RecoveredWithChanges, Is.True);
            Assert.That(result.Value.Settings.RowDimensionCodes, Does.Not.Contain("obsolete"));
            Assert.That(result.Value.Settings.RowDimensionCodes.Concat(result.Value.Settings.ColumnDimensionCodes),
                Is.EquivalentTo(new[] { "content", "time", "other" }));
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_ChangedDimensions_PreservesValidDataAndResetsBrokenDimensions()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();
            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();
            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> savedParams =
            [
                new DimensionParameters(DimensionType.Content, 2) { Name = "content" },
                new DimensionParameters(DimensionType.Time, 2) { Name = "time" },
                new DimensionParameters(DimensionType.Other, 2) { Name = "changed" },
                new DimensionParameters(DimensionType.Other, 2) { Name = "from-changed" },
                new DimensionParameters(DimensionType.Other, 2) { Name = "virtual-changed" },
                new DimensionParameters(DimensionType.Other, 1) { Name = "removed" }
            ];
            List<DimensionParameters> currentParams =
            [
                new DimensionParameters(DimensionType.Content, 2) { Name = "content" },
                new DimensionParameters(DimensionType.Time, 2) { Name = "time" },
                new DimensionParameters(DimensionType.Other, 1) { Name = "changed" },
                new DimensionParameters(DimensionType.Other, 1) { Name = "from-changed" },
                new DimensionParameters(DimensionType.Other, 1) { Name = "virtual-changed" },
                new DimensionParameters(DimensionType.Other, 1) { Name = "added" }
            ];
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(
                savedParams,
                false,
                new LineChartVisualizationSettings(
                    null,
                    false,
                    "changed",
                    new Dictionary<string, List<string>>
                    {
                        ["content"] = ["value-0"],
                        ["changed"] = ["value-1"]
                    }));
            savedQuery.Query.ChartHeaderEdit = new("Preserved header", "fi");
            savedQuery.Query.DimensionQueries["content"].NameEdit = new("Preserved content", "fi");
            savedQuery.Query.DimensionQueries["content"].ValueEdits["value-0"] = new DimensionQuery.DimensionValueEdition
            {
                NameEdit = new("Preserved value", "fi")
            };
            savedQuery.Query.DimensionQueries["content"].Selectable = true;
            savedQuery.Query.DimensionQueries["changed"].NameEdit = new("Stale edit", "fi");
            savedQuery.Query.DimensionQueries["changed"].ValueEdits["value-0"] = new DimensionQuery.DimensionValueEdition
            {
                NameEdit = new("Stale value", "fi")
            };
            savedQuery.Query.DimensionQueries["changed"].ValueFilter = new ItemFilter(["value-0", "value-1"]);
            savedQuery.Query.DimensionQueries["from-changed"].NameEdit = new("Stale from edit", "fi");
            savedQuery.Query.DimensionQueries["from-changed"].ValueFilter = new FromFilter("value-1");
            savedQuery.Query.DimensionQueries["virtual-changed"].NameEdit = new("Stale virtual edit", "fi");
            savedQuery.Query.DimensionQueries["virtual-changed"].VirtualValueDefinitions =
            [
                new SumDefinition
                {
                    Code = "virtual-stale",
                    OperandCodes = ["value-0", "value-1"]
                }
            ];
            savedQuery.Query.DimensionQueries["virtual-changed"].ValueFilter = new ItemFilter(["virtual-stale"]);

            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(TestDataCubeBuilder.BuildTestMeta(currentParams));
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(TestDataCubeBuilder.BuildTestMatrix(
                [
                    new DimensionParameters(DimensionType.Content, 2) { Name = "content" },
                    new DimensionParameters(DimensionType.Time, 2) { Name = "time" },
                    new DimensionParameters(DimensionType.Other, 1) { Name = "changed" },
                    new DimensionParameters(DimensionType.Other, 1) { Name = "from-changed" },
                    new DimensionParameters(DimensionType.Other, 1) { Name = "virtual-changed" },
                    new DimensionParameters(DimensionType.Other, 1) { Name = "added" }
                ]));
            mockSqFileInterface.Setup(x => x.SavedQueryExists(testQueryId, It.IsAny<string>())).ReturnsAsync(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(testQueryId, It.IsAny<string>())).ReturnsAsync(savedQuery);

            SqController controller = new(
                mockCachedDatasource.Object,
                mockSqFileInterface.Object,
                mockLogger.Object,
                mockAuditLogService.Object,
                mockWebhookService.Object,
                new VirtualValueValidationService(),
                mockVirtualValueComputationService.Object);

            ActionResult<SaveQueryParams> result = await controller.GetSavedQueryAsync(testQueryId);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Query.DimensionQueries.Keys, Is.EquivalentTo(new[] { "content", "time", "changed", "from-changed", "virtual-changed", "added" }));
            Assert.That(result.Value.Query.ChartHeaderEdit, Is.SameAs(savedQuery.Query.ChartHeaderEdit));
            Assert.That(result.Value.Query.DimensionQueries["content"].NameEdit, Is.Not.Null);
            Assert.That(result.Value.Query.DimensionQueries["content"].ValueEdits, Contains.Key("value-0"));
            Assert.That(result.Value.Query.DimensionQueries["content"].Selectable, Is.True);
            Assert.That(result.Value.Query.DimensionQueries["changed"].NameEdit, Is.Null);
            Assert.That(result.Value.Query.DimensionQueries["changed"].ValueEdits, Is.Empty);
            Assert.That(result.Value.Query.DimensionQueries["changed"].ValueFilter, Is.TypeOf<ItemFilter>());
            Assert.That(((ItemFilter)result.Value.Query.DimensionQueries["changed"].ValueFilter).Codes, Is.EqualTo(new[] { "value-0" }));
            Assert.That(result.Value.Query.DimensionQueries["from-changed"].NameEdit, Is.Null);
            Assert.That(((ItemFilter)result.Value.Query.DimensionQueries["from-changed"].ValueFilter).Codes, Is.EqualTo(new[] { "value-0" }));
            Assert.That(result.Value.Query.DimensionQueries["virtual-changed"].NameEdit, Is.Null);
            Assert.That(result.Value.Query.DimensionQueries["virtual-changed"].VirtualValueDefinitions, Is.Empty);
            Assert.That(result.Value.Settings.MultiselectableDimensionCode, Is.Null);
            Assert.That(result.Value.Settings.DefaultSelectableDimensionCodes, Contains.Key("content"));
            Assert.That(result.Value.Settings.DefaultSelectableDimensionCodes, Does.Not.ContainKey("changed"));
            Assert.That(result.Value.RecoveredWithChanges, Is.True);
            Assert.That(result.Value.Query.DimensionQueries["added"].ValueFilter, Is.TypeOf<ItemFilter>());
            Assert.That(savedQuery.Query.DimensionQueries.Keys, Does.Contain("removed"));
        }
    }
}

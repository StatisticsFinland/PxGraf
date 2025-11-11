using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
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
    internal class SaveQueryAsyncTest
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

        [Test]
        public async Task ValidSaveRequestReturnsSaveQueryResponseAndCallsSerializeToFile()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            mockWebhookService.Setup(s => s.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, MetaProperty>>()))
                .ReturnsAsync(new WebhookPublicationResult { Status = QueryPublicationStatus.Success, Messages = new MultilanguageString([])});

            SaveQueryParams testInput = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-2"],
                    ColumnDimensionCodes = ["variable-1"],
                    MultiselectableDimensionCode = null
                }
            };

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);
            ActionResult<SaveQueryResponse> actionResult = await testController.SaveQueryAsync(testInput);
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Once);
            mockWebhookService.Verify(
                w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, MetaProperty>>()),
                Times.Once);
        }

        [Test]
        public async Task SaveQueryAsync_NonDraftQuery_CallsWebhookService()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            mockWebhookService.Setup(w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()))
                .ReturnsAsync(new WebhookPublicationResult { Status = QueryPublicationStatus.Success, Messages = new MultilanguageString([])});

            SaveQueryParams testInput = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-2"],
                    ColumnDimensionCodes = ["variable-1"],
                    MultiselectableDimensionCode = null
                },
                Draft = false // Non-draft query
            };

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> actionResult = await testController.SaveQueryAsync(testInput);

            // Assert
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
            Assert.That(actionResult.Value.PublicationStatus, Is.EqualTo(QueryPublicationStatus.Success));

            // Verify webhook service was called
            mockWebhookService.Verify(
                w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()),
                Times.Once);
        }

        [Test]
        public async Task SaveQueryAsync_DraftQuery_DoesNotCallWebhookService()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SaveQueryParams testInput = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-2"],
                    ColumnDimensionCodes = ["variable-1"],
                    MultiselectableDimensionCode = null
                },
                Draft = true // Draft query
            };

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> actionResult = await testController.SaveQueryAsync(testInput);

            // Assert
            Assert.That(actionResult.Value, Is.InstanceOf<SaveQueryResponse>());
            Assert.That(actionResult.Value.PublicationStatus, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify webhook service was NOT called
            mockWebhookService.Verify(
                w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()),
                Times.Never);
        }
    }
}

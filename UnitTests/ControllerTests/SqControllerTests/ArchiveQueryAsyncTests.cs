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
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using PxGraf.Services;

namespace UnitTests.ControllerTests.SqControllerTests
{
    public class ArchiveQueryAsyncTests
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
        public async Task ArhiveQueryAsyncReturnsSavedQueryResponse()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
                {
                    ValueFilter = new TopFilter(1)
                },
                new DimensionParameters(DimensionType.Time, 10)
                {
                    ValueFilter = new AllFilter()
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new FromFilter("value-0")
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new ItemFilter(["value-0", "value-1"])
                }
            ];

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

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Value, Is.InstanceOf<SaveQueryResponse>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Once);

            // Verify audit log was called with the correct parameters
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/archive"),
                    It.Is<string>(resource => resource == result.Value.Id)),
                Times.Once);
        }

        [Test]
        public async Task ArchiveQueryAsync_NonDraftQuery_CallsWebhookService()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
                {
                    ValueFilter = new TopFilter(1)
                },
                new DimensionParameters(DimensionType.Time, 10)
                {
                    ValueFilter = new AllFilter()
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new FromFilter("value-0")
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new ItemFilter(["value-0", "value-1"])
                }
            ];

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

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            mockWebhookService.Setup(w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()))
                .ReturnsAsync(QueryPublicationStatus.Success);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Value, Is.InstanceOf<SaveQueryResponse>());
            Assert.That(result.Value.PublicationStatus, Is.EqualTo(QueryPublicationStatus.Success));

            // Verify webhook service was called
            mockWebhookService.Verify(
                w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()),
                Times.Once);
        }

        [Test]
        public async Task ArchiveQueryAsync_DraftQuery_DoesNotCallWebhookService()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
                {
                    ValueFilter = new TopFilter(1)
                },
                new DimensionParameters(DimensionType.Time, 10)
                {
                    ValueFilter = new AllFilter()
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new FromFilter("value-0")
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new ItemFilter(["value-0", "value-1"])
                }
            ];

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

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Value, Is.InstanceOf<SaveQueryResponse>());
            Assert.That(result.Value.PublicationStatus, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify webhook service was NOT called
            mockWebhookService.Verify(
                w => w.TriggerWebhookAsync(It.IsAny<string>(), It.IsAny<SavedQuery>(), It.IsAny<IReadOnlyDictionary<string, Px.Utils.Models.Metadata.MetaProperties.MetaProperty>>()),
                Times.Never);
        }

        [Test]
        public async Task ArhiveQueryAsyncWithInvalidVisualizationTypeReturnsSavedQueryResponse()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 2)
                {
                    ValueFilter = new TopFilter(1)
                },
                new DimensionParameters(DimensionType.Time, 10)
                {
                    ValueFilter = new AllFilter()
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new FromFilter("value-0")
                },
                new DimensionParameters(DimensionType.Other, 1)
                {
                    ValueFilter = new ItemFilter(["value-0", "value-1"])
                }
            ];

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

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());

            // Verify audit log was called with invalid request
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/archive"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_VISUALIZATION)),
                Times.Once);
        }

        [Test]
        public async Task ArchiveQueryAsyncCalledWithZeroSizeQueryThrowsBadRequest()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 0),
            ];


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

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Never);

            // Verify audit log was called with invalid request
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/archive"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_VISUALIZATION)),
                Times.Once);
        }

        [Test]
        public async Task ArchiveQueryAsyncCalledWithInvalidVisualizationTypeReturnsBadRequest()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();
            Mock<IPublicationWebhookService> mockWebhookService = new();

            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            SaveQueryParams testInput = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                Settings = new VisualizationCreationSettings()
                {
                    SelectedVisualization = VisualizationType.HorizontalBarChart,
                    RowDimensionCodes = ["variable-2"],
                    ColumnDimensionCodes = ["variable-1"],
                    MultiselectableDimensionCode = null
                }
            };

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(cubeParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            SqController testController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object, mockWebhookService.Object);

            // Act
            ActionResult<SaveQueryResponse> result = await testController.ArchiveQueryAsync(testInput);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            mockSqFileInterface.Verify(
                s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()), Times.Never);

            // Verify audit log was called with invalid request
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/archive"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_VISUALIZATION)),
                Times.Once);
        }
    }
}

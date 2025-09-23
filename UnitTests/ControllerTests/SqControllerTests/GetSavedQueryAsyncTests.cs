using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
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
                {"pxwebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"}
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
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null))));

            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object);
            
            // Act
            ActionResult<SaveQueryParams> result = await metaController.GetSavedQueryAsync(testQueryId);

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<SaveQueryParams>>());
            
            // Verify audit log was called with the correct parameters
            mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_Return_BadRequest_With_Invalid_Query_Id()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();

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
            mockCachedDatasource.Setup(x => x.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new HorizontalBarChartVisualizationSettings(null))));

            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object);
            
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
        public async Task GetSavedQueryAsyncTest_Return_NotFound_When_Query_Does_Not_Exist()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();

            string testQueryId = "aaa-bbb-111-222-333";

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(false);

            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object);
            
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
        public async Task GetSavedQueryAsyncTest_CalledWithZeroSizedDimension_ThrowsBadRequest()
        {
            // Arrange
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();
            Mock<IAuditLogService> mockAuditLogService = new();

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
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(metaParams, false, new HorizontalBarChartVisualizationSettings(null))));

            SqController metaController = new(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object, mockAuditLogService.Object);
            
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
    }
}

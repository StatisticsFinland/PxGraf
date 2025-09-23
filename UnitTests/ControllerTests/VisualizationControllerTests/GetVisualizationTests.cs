using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.VisualizationControllerTests
{
    internal class GetVisualizationTests
    {
        private Mock<ICachedDatasource> _mockCachedDatasource;
        private Mock<ISqFileInterface> _mockSqFileInterface;
        private Mock<IMultiStateMemoryTaskCache> _mockTaskCache;
        private Mock<ILogger<VisualizationController>> _mockLogger;
        private Mock<IAuditLogService> _mockAuditLogService;

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
                {"LocalFileSystemDatabaseConfig:Encoding", "latin1"},
                {"CacheOptions:Visualization:SlidingExpirationMinutes", "15" },
                {"CacheOptions:Visualization:AbsoluteExpirationMinutes", "720" },
                {"CacheOptions:Visualization:ItemAmountLimit", "1000" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [SetUp]
        public void Setup()
        {
            _mockCachedDatasource = new Mock<ICachedDatasource>();
            _mockSqFileInterface = new Mock<ISqFileInterface>();
            _mockTaskCache = new Mock<IMultiStateMemoryTaskCache>();
            _mockLogger = new Mock<ILogger<VisualizationController>>();
            _mockAuditLogService = new Mock<IAuditLogService>();
        }

        private VisualizationController BuildController(
            List<DimensionParameters> cubeParams,
            List<DimensionParameters> metaParams,
            string testQueryId,
            MultiStateMemoryTaskCache.CacheEntryState entryState,
            bool savedQueryFound = true,
            bool archived = false)
        {
            _mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMeta(metaParams));
            
            _mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));
            
            _mockCachedDatasource.Setup(x => x.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            _mockTaskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<VisualizationResponse>>.IsAny))
                .Returns((string key, out Task<VisualizationResponse> value) =>
                {
                    value = Task.FromResult(new VisualizationResponse());
                    return entryState;
                });

            _mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .Returns(savedQueryFound);
            
            _mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, archived, new LineChartVisualizationSettings(null, false, null)));
            
            _mockSqFileInterface.Setup(x => x.ArchiveCubeExists(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .Returns(true);
            
            _mockSqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            VisualizationController controller = new(
                _mockSqFileInterface.Object, 
                _mockTaskCache.Object, 
                _mockCachedDatasource.Object, 
                _mockLogger.Object,
                _mockAuditLogService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            return controller;
        }

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            // Arrange
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

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            ContentDimensionValue cdv = meta.Dimensions.Find(v => v.Type == DimensionType.Content).Values[0] as ContentDimensionValue;
            ContentDimensionValue newCdv = new(
                cdv.Code,
                cdv.Name,
                cdv.Unit,
                PxSyntaxConstants.ParseDateTime("2008-09-01T00:00:00.000Z"),
                cdv.Precision);
            foreach (var prop in cdv.AdditionalProperties)
            {
                newCdv.AdditionalProperties.Add(prop.Key, prop.Value);
            }
            ContentDimension contentDimension = meta.Dimensions.Find(v => v.Type == DimensionType.Content) as ContentDimension;
            meta.Dimensions[meta.Dimensions.IndexOf(contentDimension)] =
                new ContentDimension(
                    contentDimension.Code,
                    contentDimension.Name,
                    contentDimension.AdditionalProperties,
                    new ContentValueList([cdv]));

            VisualizationController vController = BuildController(
                cubeParams,
                metaParams, 
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Fresh);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            _mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetVisualizationTest_Stale_Data_Is_Returned_And_Update_Is_Triggered()
        {
            // Arrange
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

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            ContentDimensionValue cdv = meta.Dimensions.Find(v => v.Type == DimensionType.Content).Values[0] as ContentDimensionValue;
            ContentDimensionValue newCdv = new(
                cdv.Code,
                cdv.Name,
                cdv.Unit,
                PxSyntaxConstants.ParseDateTime("2008-09-01T00:00:00.000Z"),
                cdv.Precision);
            foreach (var prop in cdv.AdditionalProperties)
            {
                newCdv.AdditionalProperties.Add(prop.Key, prop.Value);
            }
            ContentDimension contentDimension = meta.Dimensions.Find(v => v.Type == DimensionType.Content) as ContentDimension;
            meta.Dimensions[meta.Dimensions.IndexOf(contentDimension)] =
                new ContentDimension(
                    contentDimension.Code,
                    contentDimension.Name,
                    contentDimension.AdditionalProperties,
                    new ContentValueList([cdv]));

            VisualizationController vController = BuildController(
                cubeParams,
                metaParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Stale);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            _mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Once());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetVisualizationTest_Null_Data_202_Is_Returned_And_Update_Is_Triggered()
        {
            // Arrange
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

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            ContentDimensionValue cdv = meta.Dimensions.Find(v => v.Type == DimensionType.Content).Values[0] as ContentDimensionValue;
            ContentDimensionValue newCdv = new(
                cdv.Code,
                cdv.Name,
                cdv.Unit,
                PxSyntaxConstants.ParseDateTime("2008-09-01T00:00:00.000Z"),
                cdv.Precision);
            foreach (var prop in cdv.AdditionalProperties)
            {
                newCdv.AdditionalProperties.Add(prop.Key, prop.Value);
            }
            ContentDimension contentDimension = meta.Dimensions.Find(v => v.Type == DimensionType.Content) as ContentDimension;
            meta.Dimensions[meta.Dimensions.IndexOf(contentDimension)] =
                new ContentDimension(
                    contentDimension.Code,
                    contentDimension.Name,
                    contentDimension.AdditionalProperties,
                    new ContentValueList([cdv]));

            VisualizationController vController = BuildController(
                cubeParams,
                metaParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            _mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Once()); 
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetVisualizationTest_Faulty_Task_400_Is_Returned_No_Refetch_Is_Triggered()
        {
            // Arrange
            string testQueryId = "aaa-bbb-111-222-333";

            VisualizationController vController = BuildController(
                [],
                [],
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Error);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            _mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }

        [Test]
        public async Task GetVisualizationTest_WithFaultyQueryId_Returns_NotFound()
        {
            // Arrange
            string testQueryId = "foo";

            VisualizationController vController = BuildController(
                [],
                [],
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                false);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            
            // Verify audit log was called with INVALID_OR_MISSING_SQID for not found queries
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_OR_MISSING_SQID)),
                Times.Once);
        }

        [Test]
        public async Task GetVisualizationTest_WithArchivedQuery_ReturnsArchivedResponse()
        {
            // Arrange
            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            VisualizationController vController = BuildController(
                metaParams,
                metaParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                true,
                true);

            // Act
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            // Assert
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/visualization"),
                    It.Is<string>(resource => resource == testQueryId)),
                Times.Once);
        }
    }
}

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Models.Queries;
using UnitTests;
using PxGraf.Datasource;
using PxGraf.Data.MetaData;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.Metadata;
using PxGraf.Datasource.Cache;


namespace ControllerTests
{
    internal class GetVisualizationTests
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

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

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
            CubeMeta cubeMeta = meta.ToCubeMeta();
            ContentComponent contentClone = cubeMeta.Variables.Find(v => v.Type == DimensionType.Content).IncludedValues[0].ContentComponent.Clone();
            contentClone.LastUpdated = "2008-09-01T00:00:00.000Z";
            cubeMeta.Variables.Find(v => v.Type == DimensionType.Content).IncludedValues[0].ContentComponent = contentClone;
            VisualizationResponse mockResponse = new()
            {
                MetaData = cubeMeta.Variables
            };

            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                cubeParams,
                metaParams, 
                mockResponse,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Fresh);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Stale_Data_Is_Returned_And_Update_Is_Triggered()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

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
            CubeMeta cubeMeta = meta.ToCubeMeta();
            ContentComponent contentClone = cubeMeta.Variables.Find(v => v.Type == DimensionType.Content).IncludedValues[0].ContentComponent.Clone();
            contentClone.LastUpdated = "2008-09-01T00:00:00.000Z";
            VisualizationResponse mockResponse = new()
            {
                MetaData = cubeMeta.Variables
            };

            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                cubeParams,
                metaParams,
                mockResponse,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Stale);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Once());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Null_Data_202_Is_Returned_And_Update_Is_Triggered()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

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
            CubeMeta cubeMeta = meta.ToCubeMeta();
            ContentComponent contentClone = cubeMeta.Variables.Find(v => v.Type == DimensionType.Content).IncludedValues[0].ContentComponent.Clone();
            contentClone.LastUpdated = "2008-09-01T00:00:00.000Z";
            VisualizationResponse mockResponse = new()
            {
                MetaData = cubeMeta.Variables
            };

            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                cubeParams,
                metaParams,
                mockResponse,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Once()); 
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Faulty_Task_400_Is_Returned_No_Refetch_Is_Triggered()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

            string testQueryId = "aaa-bbb-111-222-333";

            VisualizationResponse mockResponse = default;


            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                [],
                [],
                mockResponse,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Error);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }
    }
}

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
using PxGraf.Datasource;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.Metadata;
using PxGraf.Datasource.Cache;
using System.Linq;
using Px.Utils.Models.Metadata.Dimensions;
using PxGraf.Utility;
using PxGraf.Models.SavedQueries;

namespace UnitTests.ControllerTests.VisualizationControllerTests
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

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationResponse mockResponse = new()
            {
                MetaData = meta.Dimensions.Select(d => d.ConvertToVariable(query.DimensionQueries, meta)).ToList()
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
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationResponse mockResponse = new()
            {
                MetaData = meta.Dimensions.Select(d => d.ConvertToVariable(query.DimensionQueries, meta)).ToList()
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
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationResponse mockResponse = new()
            {
                MetaData = meta.Dimensions.Select(d => d.ConvertToVariable(query.DimensionQueries, meta)).ToList()
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

        [Test]
        public async Task GetVisualizationTest_WithFaultyQueryId_Returns_NotFound()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

            string testQueryId = "foo";

            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                [],
                [],
                null,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                false);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetVisualizationTest_WithArchivedQuery_ReturnsArchivedResponse()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();

            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            ArchiveCube ac = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(metaParams, true, new LineChartVisualizationSettings(null, false, null));
            
            VisualizationResponse mockResponse = new()
            {
                MetaData = meta.Dimensions.Select(d => d.ConvertToVariable(query.DimensionQueries, meta)).ToList()
            };

            Mock<ISqFileInterface> sqFileInterface = new();
            sqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => sq);
            sqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => ac);

            VisualizationController vController = TestVisualizationControllerBuilder.BuildController(
                [],
                metaParams,
                mockResponse,
                testQueryId,
                mockCachedDatasource,
                MultiStateMemoryTaskCache.CacheEntryState.Fresh);

            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }
    }
}

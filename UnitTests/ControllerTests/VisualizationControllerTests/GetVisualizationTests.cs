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
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                {"DatabaseConfig:Type", "PxWeb"},
                {"DatabaseConfig:PxWebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"QueryOptions:MaxHeaderLength", "120"},
                {"QueryOptions:MaxQuerySize", "100000"},
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

            _mockTaskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<JsonStat2>>.IsAny))
                .Returns((string key, out Task<JsonStat2> value) =>
                {
                    value = null!;
                    return MultiStateMemoryTaskCache.CacheEntryState.Null;
                });

            _mockTaskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<IReadOnlyMatrixMetadata>>.IsAny))
                .Returns((string key, out Task<IReadOnlyMatrixMetadata> value) =>
                {
                    value = null!;
                    return MultiStateMemoryTaskCache.CacheEntryState.Null;
                });

            _mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(savedQueryFound);

            _mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, archived, new LineChartVisualizationSettings(null, false, null)));

            _mockSqFileInterface.Setup(x => x.ArchiveCubeExists(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(true);
            
            _mockSqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.Is<string>(id => id == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            Mock<IVirtualValueComputationService> mockVirtualValueComputationService = new();

            VisualizationController controller = new(
                _mockSqFileInterface.Object, 
                _mockTaskCache.Object, 
                _mockCachedDatasource.Object, 
                _mockLogger.Object,
                _mockAuditLogService.Object,
                mockVirtualValueComputationService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            return controller;
        }

        [Test]
        public void VisualizationController_UsesCollisionFreeRoutes()
        {
            RouteAttribute controllerRoute = typeof(VisualizationController)
                .GetCustomAttribute<RouteAttribute>()!;
            HttpGetAttribute visualizationRoute = typeof(VisualizationController)
                .GetMethod(nameof(VisualizationController.GetVisualization))!
                .GetCustomAttribute<HttpGetAttribute>()!;
            HttpGetAttribute jsonStatRoute = typeof(VisualizationController)
                .GetMethod(nameof(VisualizationController.GetJsonStat2VisualizationAsync))!
                .GetCustomAttribute<HttpGetAttribute>()!;
            HttpGetAttribute visualizationMetadataRoute = typeof(VisualizationController)
                .GetMethod(nameof(VisualizationController.GetVisualizationMetadataAsync))!
                .GetCustomAttribute<HttpGetAttribute>()!;
            HttpGetAttribute jsonStatMetadataRoute = typeof(VisualizationController)
                .GetMethod(nameof(VisualizationController.GetJsonStat2MetadataAsync))!
                .GetCustomAttribute<HttpGetAttribute>()!;

            Assert.Multiple(() =>
            {
                Assert.That(controllerRoute.Template, Is.EqualTo("api/sq"));
                Assert.That(visualizationRoute.Template, Is.EqualTo("visualization/{sqId}"));
                Assert.That(jsonStatRoute.Template, Is.EqualTo("jsonstat/{sqId}"));
                Assert.That(visualizationMetadataRoute.Template, Is.EqualTo("visualization/{sqId}/metadata"));
                Assert.That(jsonStatMetadataRoute.Template, Is.EqualTo("jsonstat/{sqId}/metadata"));
            });
        }

        [Test]
        public async Task GetVisualizationMetadata_ReturnsMetadataWithoutFetchingOrCachingData()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<VisualizationMetadataResponse> result = await controller.GetVisualizationMetadataAsync(testQueryId);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.MetaData, Has.Count.EqualTo(3));
            Assert.That(result.Value.VisualizationSettings.VisualizationType, Is.EqualTo(PxGraf.Enums.VisualizationType.LineChart));
            _mockCachedDatasource.Verify(
                datasource => datasource.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()),
                Times.Never());
            _mockCachedDatasource.Verify(
                datasource => datasource.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()),
                Times.Never());
            _mockTaskCache.Verify(
                cache => cache.TryGet(It.IsAny<string>(), out It.Ref<Task<VisualizationResponse>>.IsAny),
                Times.Never());
        }

        [Test]
        public void GetVisualizationMetadata_UnexpectedMetadataFailurePropagates()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            _mockCachedDatasource.Setup(datasource => datasource.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ThrowsAsync(new InvalidOperationException("Unexpected metadata failure."));

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await controller.GetVisualizationMetadataAsync(testQueryId));
        }

        [Test]
        public async Task GetJsonStat2Metadata_ReturnsEmptyValuesAndDoesNotFetchData()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2MetadataAsync(testQueryId, "fi");

            JsonResult jsonResult = (JsonResult)result.Result!;
            JsonStat2 dataset = (JsonStat2)jsonResult.Value!;
            using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(dataset, GlobalJsonConverterOptions.Default));
            JsonElement values = document.RootElement.GetProperty("value");
            Assert.Multiple(() =>
            {
                Assert.That(jsonResult.ContentType, Is.EqualTo("application/vnd.jsonstat2+json"));
                Assert.That(dataset.Dimensions, Has.Count.EqualTo(3));
                Assert.That(values.ValueKind, Is.EqualTo(JsonValueKind.Array));
                Assert.That(values.GetArrayLength(), Is.Zero);
                Assert.That(document.RootElement.TryGetProperty("status", out _), Is.False);
            });
            _mockCachedDatasource.Verify(
                datasource => datasource.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()),
                Times.Never());
            _mockCachedDatasource.Verify(
                datasource => datasource.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()),
                Times.Never());
            _mockTaskCache.Verify(
                cache => cache.TryGet(It.IsAny<string>(), out It.Ref<Task<JsonStat2>>.IsAny),
                Times.Never());
        }

        [Test]
        public void GetJsonStat2Metadata_UnexpectedMetadataFailurePropagates()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            _mockCachedDatasource.Setup(datasource => datasource.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ThrowsAsync(new InvalidOperationException("Unexpected metadata failure."));

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await controller.GetJsonStat2MetadataAsync(testQueryId, "fi"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetMetadata_WithInvalidSavedQueryId_ReturnsBadRequestBeforeFileLookup(bool jsonStat)
        {
            VisualizationController controller = BuildController(
                [],
                [],
                "valid-id",
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            IActionResult result = jsonStat
                ? (await controller.GetJsonStat2MetadataAsync("invalid/id", "fi")).Result!
                : (await controller.GetVisualizationMetadataAsync("invalid/id")).Result!;

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockSqFileInterface.Verify(
                files => files.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetMetadata_WithMissingSavedQuery_ReturnsNotFound(bool jsonStat)
        {
            const string testQueryId = "valid-id";
            VisualizationController controller = BuildController(
                [],
                [],
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                savedQueryFound: false);

            IActionResult result = jsonStat
                ? (await controller.GetJsonStat2MetadataAsync(testQueryId, "fi")).Result!
                : (await controller.GetVisualizationMetadataAsync(testQueryId)).Result!;

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetMetadata_WithEmptyDimension_ReturnsBadRequest(bool jsonStat)
        {
            const string testQueryId = "valid-id";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 0)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            IActionResult result = jsonStat
                ? (await controller.GetJsonStat2MetadataAsync(testQueryId, "fi")).Result!
                : (await controller.GetVisualizationMetadataAsync(testQueryId)).Result!;

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetJsonStat2Metadata_WithUnsupportedLanguage_ReturnsBadRequest()
        {
            const string testQueryId = "valid-id";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2MetadataAsync(testQueryId, "de");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetJsonStat2Metadata_WithoutLanguage_UsesDefaultLanguage()
        {
            const string testQueryId = "valid-id";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            JsonStat2 defaultLanguageDataset = (JsonStat2)((JsonResult)
                (await controller.GetJsonStat2MetadataAsync(testQueryId, null)).Result!).Value!;
            JsonStat2 explicitLanguageDataset = (JsonStat2)((JsonResult)
                (await controller.GetJsonStat2MetadataAsync(testQueryId, "fi")).Result!).Value!;

            Assert.That(
                JsonSerializer.Serialize(defaultLanguageDataset, GlobalJsonConverterOptions.Default),
                Is.EqualTo(JsonSerializer.Serialize(explicitLanguageDataset, GlobalJsonConverterOptions.Default)));
        }

        [Test]
        public async Task GetVisualizationMetadata_ArchivedCacheMiss_ReadsAndCachesArchiveMetadata()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: true);

            ActionResult<VisualizationMetadataResponse> result = await controller.GetVisualizationMetadataAsync(testQueryId);

            Assert.That(result.Value, Is.Not.Null);
            _mockSqFileInterface.Verify(
                files => files.ReadArchiveCubeFromFile(testQueryId, It.IsAny<string>()),
                Times.Once());
            _mockTaskCache.Verify(
                cache => cache.Set(
                    $"archived-metadata:{testQueryId}",
                    It.IsAny<Task<IReadOnlyMatrixMetadata>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()),
                Times.Once());
        }

        [Test]
        public async Task GetJsonStat2Metadata_ArchivedFreshCacheHit_DoesNotReadArchive()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            IReadOnlyMatrixMetadata cachedMetadata = TestDataCubeBuilder.BuildTestMeta(dimensions);
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: true);
            _mockTaskCache.Setup(cache => cache.TryGet(
                    $"archived-metadata:{testQueryId}",
                    out It.Ref<Task<IReadOnlyMatrixMetadata>>.IsAny))
                .Returns((string key, out Task<IReadOnlyMatrixMetadata> value) =>
                {
                    value = Task.FromResult(cachedMetadata);
                    return MultiStateMemoryTaskCache.CacheEntryState.Fresh;
                });

            ActionResult<JsonStat2> result = await controller.GetJsonStat2MetadataAsync(testQueryId, "fi");

            Assert.That(result.Result, Is.InstanceOf<JsonResult>());
            _mockSqFileInterface.Verify(
                files => files.ReadArchiveCubeFromFile(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
            _mockTaskCache.Verify(
                cache => cache.Set(
                    It.IsAny<string>(),
                    It.IsAny<Task<IReadOnlyMatrixMetadata>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()),
                Times.Never());
        }

        [Test]
        public async Task GetVisualizationMetadata_ArchivedStaleCacheHit_RefreshesBeforeReturning()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            IReadOnlyMatrixMetadata staleMetadata = TestDataCubeBuilder.BuildTestMeta(
                [
                    new DimensionParameters(DimensionType.Content, 1),
                    new DimensionParameters(DimensionType.Time, 2)
                ]);
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: true);
            _mockTaskCache.Setup(cache => cache.TryGet(
                    $"archived-metadata:{testQueryId}",
                    out It.Ref<Task<IReadOnlyMatrixMetadata>>.IsAny))
                .Returns((string key, out Task<IReadOnlyMatrixMetadata> value) =>
                {
                    value = Task.FromResult(staleMetadata);
                    return MultiStateMemoryTaskCache.CacheEntryState.Stale;
                });

            ActionResult<VisualizationMetadataResponse> result = await controller.GetVisualizationMetadataAsync(testQueryId);

            Assert.That(result.Value!.MetaData.Single(variable => variable.DimensionType == DimensionType.Time).Values, Has.Count.EqualTo(3));
            _mockSqFileInterface.Verify(
                files => files.ReadArchiveCubeFromFile(testQueryId, It.IsAny<string>()),
                Times.Once());
            _mockTaskCache.Verify(
                cache => cache.Set(
                    $"archived-metadata:{testQueryId}",
                    It.IsAny<Task<IReadOnlyMatrixMetadata>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()),
                Times.Once());
        }

        [Test]
        public async Task GetVisualizationMetadata_ArchivedCacheError_RemovesAndReloadsMetadata()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: true);
            _mockTaskCache.Setup(cache => cache.TryGet(
                    $"archived-metadata:{testQueryId}",
                    out It.Ref<Task<IReadOnlyMatrixMetadata>>.IsAny))
                .Returns((string key, out Task<IReadOnlyMatrixMetadata> value) =>
                {
                    value = Task.FromException<IReadOnlyMatrixMetadata>(new InvalidOperationException());
                    return MultiStateMemoryTaskCache.CacheEntryState.Error;
                });

            ActionResult<VisualizationMetadataResponse> result = await controller.GetVisualizationMetadataAsync(testQueryId);

            Assert.That(result.Value, Is.Not.Null);
            _mockTaskCache.Verify(cache => cache.TryRemove($"archived-metadata:{testQueryId}"), Times.Once());
            _mockSqFileInterface.Verify(
                files => files.ReadArchiveCubeFromFile(testQueryId, It.IsAny<string>()),
                Times.Once());
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetVisualizationMetadata_MatchesFullResponseWithoutData(bool archived)
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Other, 3) { Selectable = true }
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: archived);

            VisualizationResponse fullResponse = (await controller.GetVisualization(testQueryId)).Value!;
            VisualizationMetadataResponse metadataResponse = (await controller.GetVisualizationMetadataAsync(testQueryId)).Value!;
            JsonObject expected = SerializeToObject(fullResponse);
            expected.Remove("data");
            expected.Remove("dataNotes");
            expected.Remove("missingDataInfo");

            Assert.Multiple(() =>
            {
                Assert.That(JsonNode.DeepEquals(expected, SerializeToObject(metadataResponse)), Is.True);
                Assert.That(metadataResponse.SelectableDimensionCodes, Is.EqualTo(new[] { "variable-2" }));
            });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetJsonStat2Metadata_MatchesFullResponseWithEmptyValuesAndNoStatus(bool archived)
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Other, 3) { Selectable = true }
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                archived: archived);

            JsonStat2 fullResponse = (JsonStat2)((JsonResult)
                (await controller.GetJsonStat2VisualizationAsync(testQueryId, "fi")).Result!).Value!;
            JsonStat2 metadataResponse = (JsonStat2)((JsonResult)
                (await controller.GetJsonStat2MetadataAsync(testQueryId, "fi")).Result!).Value!;
            JsonObject expected = SerializeToObject(fullResponse);
            expected["value"] = new JsonArray();
            expected.Remove("status");

            Assert.That(JsonNode.DeepEquals(expected, SerializeToObject(metadataResponse)), Is.True);
        }

        [Test]
        public async Task GetVisualizationMetadata_WithVirtualValue_ReturnsComputedMetadataWithoutFetchingData()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 2),
                new DimensionParameters(DimensionType.Time, 2)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            PxGraf.Models.SavedQueries.SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(
                dimensions,
                false,
                new LineChartVisualizationSettings(null, false, null));
            DimensionQuery contentQuery = savedQuery.Query.DimensionQueries["variable-0"];
            contentQuery.ValueFilter = new ItemFilter(["value-0", "virtual-sum"]);
            contentQuery.VirtualValueDefinitions =
            [
                new SumDefinition
                {
                    Code = "virtual-sum",
                    OperandCodes = ["value-0", "value-1"]
                }
            ];
            _mockSqFileInterface.Setup(files => files.ReadSavedQueryFromFile(testQueryId, It.IsAny<string>()))
                .ReturnsAsync(savedQuery);

            VisualizationMetadataResponse response = (await controller.GetVisualizationMetadataAsync(testQueryId)).Value!;

            Assert.That(
                response.MetaData.Single(variable => variable.Code == "variable-0").Values.Select(value => value.Code),
                Is.EqualTo(new[] { "value-0", "virtual-sum" }));
            _mockCachedDatasource.Verify(
                datasource => datasource.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()),
                Times.Never());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GetMetadata_WithCircularVirtualValues_PropagatesInvalidOperationException(bool jsonStat)
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 2),
                new DimensionParameters(DimensionType.Time, 2)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            PxGraf.Models.SavedQueries.SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(
                dimensions,
                false,
                new LineChartVisualizationSettings(null, false, null));
            DimensionQuery contentQuery = savedQuery.Query.DimensionQueries["variable-0"];
            contentQuery.VirtualValueDefinitions =
            [
                new SumDefinition { Code = "virtual-1", OperandCodes = ["virtual-2", "value-0"] },
                new SumDefinition { Code = "virtual-2", OperandCodes = ["virtual-1", "value-1"] }
            ];
            _mockSqFileInterface.Setup(files => files.ReadSavedQueryFromFile(testQueryId, It.IsAny<string>()))
                .ReturnsAsync(savedQuery);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                if (jsonStat)
                {
                    await controller.GetJsonStat2MetadataAsync(testQueryId, "fi");
                }
                else
                {
                    await controller.GetVisualizationMetadataAsync(testQueryId);
                }
            });
        }

        private static JsonObject SerializeToObject(object value)
        {
            return JsonSerializer.SerializeToNode(value, GlobalJsonConverterOptions.Default)!.AsObject();
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

        [Test]
        public async Task GetJsonStat2VisualizationTest_ReturnsJsonStatDataset()
        {
            string testQueryId = "aaa-bbb-111-222-333";

            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];

            VisualizationController controller = BuildController(
                cubeParams,
                cubeParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Fresh);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(testQueryId, null);

            Assert.That(result.Result, Is.InstanceOf<JsonResult>());
            JsonResult jsonResult = (JsonResult)result.Result!;
            Assert.That(jsonResult.ContentType, Is.EqualTo("application/vnd.jsonstat2+json"));
            Assert.That(jsonResult.Value, Is.InstanceOf<JsonStat2>());
            JsonStat2 dataset = (JsonStat2)jsonResult.Value;
            Assert.That(dataset.Extension.VisualizationSettings, Is.InstanceOf<VisualizationResponse.PxVisualizerSettings>());
            Assert.That(dataset.Extension.VisualizationSettings.VisualizationType, Is.EqualTo(PxGraf.Enums.VisualizationType.LineChart));
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_CacheMiss_CachesDatasetUsingLanguageSpecificKey()
        {
            string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];
            VisualizationController controller = BuildController(
                cubeParams,
                cubeParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(testQueryId, "fi");

            Assert.That(result.Result, Is.InstanceOf<JsonResult>());
            Assert.That(controller.Response.Headers.CacheControl.ToString(), Is.EqualTo($"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}"));
            _mockTaskCache.Verify(x => x.Set(
                "jsonstat:aaa-bbb-111-222-333:fi",
                It.IsAny<Task<JsonStat2>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_FreshCache_ReturnsCachedDataset()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            JsonStat2 cachedDataset = new();
            VisualizationController controller = BuildController(
                [],
                [],
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            _mockTaskCache.Setup(x => x.TryGet(
                    "jsonstat:aaa-bbb-111-222-333:fi",
                    out It.Ref<Task<JsonStat2>>.IsAny))
                .Returns((string key, out Task<JsonStat2> value) =>
                {
                    value = Task.FromResult(cachedDataset);
                    return MultiStateMemoryTaskCache.CacheEntryState.Fresh;
                });

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(testQueryId, "fi");

            Assert.That(((JsonResult)result.Result!).Value, Is.SameAs(cachedDataset));
            Assert.That(controller.Response.Headers.CacheControl.ToString(), Is.EqualTo($"max-age={Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds}"));
            _mockSqFileInterface.Verify(x => x.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_StaleCache_ReturnsCachedDatasetWithNoMaxAge()
        {
            const string testQueryId = "aaa-bbb-111-222-333";
            JsonStat2 cachedDataset = new();
            VisualizationController controller = BuildController(
                [],
                [],
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);
            _mockTaskCache.Setup(x => x.TryGet(
                    "jsonstat:aaa-bbb-111-222-333:fi",
                    out It.Ref<Task<JsonStat2>>.IsAny))
                .Returns((string key, out Task<JsonStat2> value) =>
                {
                    value = Task.FromResult(cachedDataset);
                    return MultiStateMemoryTaskCache.CacheEntryState.Stale;
                });

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(testQueryId, "fi");

            Assert.That(((JsonResult)result.Result!).Value, Is.SameAs(cachedDataset));
            Assert.That(controller.Response.Headers.CacheControl.ToString(), Is.EqualTo("max-age=0"));
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithUnsupportedLanguage_ReturnsBadRequest()
        {
            string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];

            VisualizationController controller = BuildController(
                cubeParams,
                cubeParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Fresh);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(testQueryId, "de");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithInvalidSavedQueryId_ReturnsBadRequestBeforeFileLookup()
        {
            VisualizationController controller = BuildController(
                [],
                [],
                "valid-id",
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync("invalid/id", "fi");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            _mockSqFileInterface.Verify(x => x.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithMissingSavedQuery_ReturnsNotFound()
        {
            VisualizationController controller = BuildController(
                [],
                [],
                "valid-id",
                MultiStateMemoryTaskCache.CacheEntryState.Null,
                savedQueryFound: false);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync("valid-id", "fi");

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithEmptyDimension_ReturnsBadRequest()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 0)
            ];
            VisualizationController controller = BuildController(
                dimensions,
                dimensions,
                "valid-id",
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync("valid-id", "fi");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetVisualization_WithInvalidSavedQueryId_ReturnsBadRequestBeforeCacheLookup()
        {
            VisualizationController controller = BuildController(
                [],
                [],
                "valid-id",
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            ActionResult<VisualizationResponse> result = await controller.GetVisualization("invalid/id");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            _mockTaskCache.Verify(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<VisualizationResponse>>.IsAny), Times.Never());
            _mockCachedDatasource.Verify(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()), Times.Never());
            _mockSqFileInterface.Verify(x => x.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetVisualizationTest_WithAcceptHeader_ReturnsVisualizationResponse()
        {
            string testQueryId = "aaa-bbb-111-222-333";
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Other, 2)
            ];

            VisualizationController controller = BuildController(
                cubeParams,
                cubeParams,
                testQueryId,
                MultiStateMemoryTaskCache.CacheEntryState.Null);

            controller.ControllerContext.HttpContext.Request.Headers.Accept = "application/xml";

            ActionResult<VisualizationResponse> result = await controller.GetVisualization(testQueryId);

            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }
    }
}

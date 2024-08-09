﻿using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using UnitTests;
using PxGraf.Datasource;
using PxGraf.Data.MetaData;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Models.Responses;


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
                {"archiveFileDirectory", "goesNowhere"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        // TODO: Fix tests
         /*
        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ICachedDatasource> mockVisualizationResponseCache = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();

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
            mockCachedDatasource.Setup(x => x.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => meta);
            mockCachedDatasource.Setup(x => x.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            Variable contetClone = meta.GetContentDimension().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = [contetClone]
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Fresh); //OBS: Fresh

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedDatasource.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedDatasource.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()), Times.Never());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Stale_Data_Is_Returned_And_Update_Is_Triggered()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();
            Mock<IVisualizationResponseCache> mockVisualizationResponseCache = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();

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

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            Variable contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = [contetClone]
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Stale); //OBS: Stale

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()), Times.Once());
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_202_Is_Returned_When_Fetch_Is_Pending()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();
            Mock<IVisualizationResponseCache> mockVisualizationResponseCache = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();

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

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            Variable contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = [contetClone]
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Pending); //OBS: Pending

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()), Times.Never());
            Assert.That(result.Result, Is.InstanceOf<AcceptedResult>());
        }

        [Test]
        public async Task GetVisualizationTest_Null_Data_202_Is_Returned_And_Update_Is_Triggered()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();
            Mock<IVisualizationResponseCache> mockVisualizationResponseCache = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();

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

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams));

            VisualizationResponse mockResult = null;
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Null); //OBS: Null

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));


            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()), Times.Once());
            Assert.That(result.Result, Is.InstanceOf<AcceptedResult>());
        }

        [Test]
        public async Task GetVisualizationTest_Faulty_Task_400_Is_Returned_No_Refetch_Is_Triggered()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();
            Mock<IVisualizationResponseCache> mockVisualizationResponseCache = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();

            string testQueryId = "aaa-bbb-111-222-333";

            VisualizationResponse mockResult = default;
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Error);

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<MatrixQuery>()), Times.Never());
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }
         */
    }
}

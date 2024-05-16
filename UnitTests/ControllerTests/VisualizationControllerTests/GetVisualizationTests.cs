﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Caching;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.PxWebInterface;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace ControllerTests
{
    internal class GetVisualizationTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));

            var inMemorySettings = new Dictionary<string, string> {
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

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockVisualizationResponseCache = new Mock<IVisualizationResponseCache>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = new List<IReadOnlyVariable>() { contetClone }
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Fresh); //OBS: Fresh

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));


            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Never());
            Assert.IsInstanceOf<VisualizationResponse>(result.Value);
        }

        [Test]
        public async Task GetVisualizationTest_Stale_Data_Is_Returned_And_Update_Is_Triggered()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockVisualizationResponseCache = new Mock<IVisualizationResponseCache>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = new List<IReadOnlyVariable>() { contetClone }
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Stale); //OBS: Stale

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Once());
            Assert.IsInstanceOf<VisualizationResponse>(result.Value);
        }

        [Test]
        public async Task GetVisualizationTest_202_Is_Returned_When_Fetch_Is_Pending()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockVisualizationResponseCache = new Mock<IVisualizationResponseCache>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");
            VisualizationResponse mockResult = new()
            {
                MetaData = new List<IReadOnlyVariable>() { contetClone }
            };
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Pending); //OBS: Pending

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Never());
            Assert.IsInstanceOf<AcceptedResult>(result.Result);
        }

        [Test]
        public async Task GetVisualizationTest_Null_Data_202_Is_Returned_And_Update_Is_Triggered()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockVisualizationResponseCache = new Mock<IVisualizationResponseCache>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ReturnsAsync(() => meta);
            mockCachedPxWebConnection.Setup(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams));

            VisualizationResponse mockResult = null;
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Null); //OBS: Null

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null)));


            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Once());
            Assert.IsInstanceOf<AcceptedResult>(result.Result);
        }

        [Test]
        public async Task GetVisualizationTest_Faulty_Task_400_Is_Returned_No_Refetch_Is_Triggered()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockVisualizationResponseCache = new Mock<IVisualizationResponseCache>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            VisualizationResponse mockResult = default;
            mockVisualizationResponseCache.Setup(x => x.TryGet(It.IsAny<string>(), out mockResult))
                .Returns(VisualizationResponseCache.CacheEntryState.Error);

            VisualizationController vController = new(mockSqFileInterface.Object, mockVisualizationResponseCache.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<VisualizationController>>().Object);
            ActionResult<VisualizationResponse> result = await vController.GetVisualization(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Never());
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }
    }
}

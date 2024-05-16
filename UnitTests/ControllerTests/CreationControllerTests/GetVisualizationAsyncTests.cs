using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
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

namespace CreationControllerTests
{
    internal class GetVisualizationAsyncTests
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
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");

            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            VisualizationCreationSettings testSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart,

            };

            CreationController cController = new(mockCachedPxWebConnection.Object, new Mock<ILogger<CreationController>>().Object);
            ActionResult<VisualizationResponse> result = await cController.GetVisualizationAsync(
                new ChartRequest()
                {
                    VisualizationSettings = testSettings,
                    Query = testQuery
                });

            mockCachedPxWebConnection.Verify(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Once());
            Assert.IsInstanceOf<VisualizationResponse>(result.Value);
        }

        [Test]
        public async Task GetVisualizationTest_Volume_0_Cube_Returns_BadRequest()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();

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
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");

            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            testQuery.VariableQueries["variable-1"].ValueFilter = new ItemFilter(new List<string>());

            CreationController cController = new(mockCachedPxWebConnection.Object, new Mock<ILogger<CreationController>>().Object);
            ActionResult<VisualizationResponse> result = await cController.GetVisualizationAsync(
                new ChartRequest()
                {
                    VisualizationSettings = new VisualizationCreationSettings(),
                    Query = testQuery
                });

            mockCachedPxWebConnection.Verify(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Never());
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }

        [Test]
        public async Task GetVisualizationTest_Valid_VisualizationType_DoesNotReturn_BadRequest()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();

            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new() // mitä koko taulun metarakenne
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            var contentClone = meta.GetContentVariable().Clone();
            contentClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");

            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            VisualizationCreationSettings testSettings = new()
            {
                CutYAxis = false,
                SelectedVisualization = VisualizationType.LineChart
            };

            CreationController cController = new(mockCachedPxWebConnection.Object, new Mock<ILogger<CreationController>>().Object);
            ActionResult<VisualizationResponse> result = await cController.GetVisualizationAsync(
                new ChartRequest()
                {
                    VisualizationSettings = testSettings,
                    Query = testQuery
                });

            mockCachedPxWebConnection.Verify(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Once());
            Assert.IsNotInstanceOf<BadRequestResult>(result.Result);
        }

        [Test]
        public async Task GetVisualizationTest_Invalid_VisualizationType_Returns_BadRequest()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();

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
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            var contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");

            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            VisualizationCreationSettings testSettings = new()
            {
                CutYAxis = false,
                SelectedVisualization = VisualizationType.PieChart
            };

            CreationController cController = new(mockCachedPxWebConnection.Object, new Mock<ILogger<CreationController>>().Object);
            ActionResult<VisualizationResponse> result = await cController.GetVisualizationAsync(
                new ChartRequest()
                {
                    VisualizationSettings = testSettings,
                    Query = testQuery
                });

            mockCachedPxWebConnection.Verify(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Once());
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }
    }
}

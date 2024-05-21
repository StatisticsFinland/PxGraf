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
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace CreationControllerTests
{
    internal class GetVisualizationAsyncTests
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

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();

            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            Variable contetClone = meta.GetContentVariable().Clone();
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
            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Volume_0_Cube_Returns_BadRequest()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();

            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            Variable contetClone = meta.GetContentVariable().Clone();
            contetClone.IncludedValues.ForEach(cv => cv.ContentComponent.LastUpdated = "2008-09-01T00:00:00.000Z");

            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            testQuery.VariableQueries["variable-1"].ValueFilter = new ItemFilter([]);

            CreationController cController = new(mockCachedPxWebConnection.Object, new Mock<ILogger<CreationController>>().Object);
            ActionResult<VisualizationResponse> result = await cController.GetVisualizationAsync(
                new ChartRequest()
                {
                    VisualizationSettings = new VisualizationCreationSettings(),
                    Query = testQuery
                });

            mockCachedPxWebConnection.Verify(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Never());
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetVisualizationTest_Valid_VisualizationType_DoesNotReturn_BadRequest()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();

            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            Variable contentClone = meta.GetContentVariable().Clone();
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
            Assert.That(result.Result, Is.Not.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetVisualizationTest_Invalid_VisualizationType_Returns_BadRequest()
        {
            Mock<ICachedPxWebConnection> mockCachedPxWebConnection = new();

            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)meta));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            Variable contetClone = meta.GetContentVariable().Clone();
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
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }
    }
}

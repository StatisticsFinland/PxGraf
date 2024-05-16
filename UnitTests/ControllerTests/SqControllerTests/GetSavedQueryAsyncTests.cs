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
    internal class GetSavedQueryAsyncTests
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
        public async Task GetSavedQueryAsyncTest_Return_SaveQueryParams_With_Valid_Id()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
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

            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)TestDataCubeBuilder.BuildTestMeta(metaParams)));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null))));

            SqController metaController = new(mockCachedPxWebConnection.Object, mockSqFileInterface.Object, new Mock<ILogger<SqController>>().Object);
            ActionResult<SaveQueryParams> result = await metaController.GetSavedQueryAsync(testQueryId);

            Assert.IsInstanceOf<ActionResult<SaveQueryParams>>(result);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_Return_BadRequest_With_Invalid_Query_Id()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
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

            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .Returns(Task.Run(() => (IReadOnlyCubeMeta)TestDataCubeBuilder.BuildTestMeta(metaParams)));
            mockCachedPxWebConnection.Setup(x => x.GetDataCubeCachedAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestDataCube(cubeParams)));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new HorizontalBarChartVisualizationSettings(null))));

            SqController metaController = new(mockCachedPxWebConnection.Object, mockSqFileInterface.Object, new Mock<ILogger<SqController>>().Object);
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync(testQueryId);

            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);
        }

        [Test]
        public async Task GetSavedQueryAsyncTest_Return_NotFound_When_Query_Does_Not_Exist()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(false);

            SqController metaController = new(mockCachedPxWebConnection.Object, mockSqFileInterface.Object, new Mock<ILogger<SqController>>().Object);
            ActionResult<SaveQueryParams> actionResult = await metaController.GetSavedQueryAsync(testQueryId);

            Assert.IsInstanceOf<NotFoundResult>(actionResult.Result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PxGraf.Controllers;
using PxGraf.Enums;
using PxGraf.Exceptions;
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
    internal class GetQueryMetaTests
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
        public async Task GetQueryMetaTest_ReturnValidMeta()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
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

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, false, new LineChartVisualizationSettings(null, false, null)));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Never());

            Assert.AreEqual("value-0, value-0 2000-2009 muuttujana variable-2", result.Value.Header["fi"]);
            Assert.AreEqual("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2", result.Value.HeaderWithPlaceholders["fi"]);
            Assert.IsFalse(result.Value.Archived);
            Assert.IsFalse(result.Value.Selectable);
            Assert.AreEqual(VisualizationType.LineChart, result.Value.VisualizationType);
            Assert.AreEqual("TestPxFile.px", result.Value.TableId);
            Assert.AreEqual("Test note", result.Value.Description["fi"]);
            Assert.AreEqual("2009-09-01T00:00:00.000Z", result.Value.LastUpdated);
            Assert.AreEqual("TestPxFile.px", result.Value.TableReference.Name);

            var expectedHierarchy = new List<string>() { "testpath", "to", "test", "file" };
            Assert.AreEqual(expectedHierarchy, result.Value.TableReference.Hierarchy);  
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnSelectableTrue()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 1)
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ReturnsAsync(() => meta);

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, false, new LineChartVisualizationSettings(null, false, null)));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta(testQueryId);

            mockCachedPxWebConnection.Verify(x => x.BuildDataCubeCachedAsync(It.IsAny<CubeQuery>()), Times.Never());

            Assert.IsTrue(result.Value.Selectable);
        }

        [Test]
        public async Task GetQueryMetaTest_NotFound()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
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

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(false);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, false, new LineChartVisualizationSettings(null, false, null)));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta(testQueryId);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetQueryMetaTest_ArchivedQuery()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ThrowsAsync(new BadPxWebResponseException(System.Net.HttpStatusCode.BadRequest, "Foobar"));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, true, new LineChartVisualizationSettings(null, false, null)));

            mockSqFileInterface.Setup(x => x.ArchiveCubeExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            Assert.DoesNotThrowAsync(() => controller.GetQueryMeta(testQueryId));
            
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta(testQueryId);
            mockCachedPxWebConnection.Verify(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()), Times.Never());

            Assert.AreEqual("value-0, value-0 2000-2009 muuttujana variable-2", result.Value.Header["fi"]);
            Assert.AreEqual("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2", result.Value.HeaderWithPlaceholders["fi"]);
            Assert.IsTrue(result.Value.Archived);
            Assert.IsFalse(result.Value.Selectable);
            Assert.AreEqual(VisualizationType.LineChart, result.Value.VisualizationType);
            Assert.AreEqual("TestPxFile.px", result.Value.TableId);
            Assert.AreEqual("Test note", result.Value.Description["fi"]);
            Assert.AreEqual("2009-09-01T00:00:00.000Z", result.Value.LastUpdated);
            Assert.AreEqual("TestPxFile.px", result.Value.TableReference.Name);

            var expectedHierarchy = new List<string>() { "testpath", "to", "test", "file" };
            Assert.AreEqual(expectedHierarchy, result.Value.TableReference.Hierarchy);

        }

        [Test]
        public void GetQueryMetaTest_BadPxWebResponse_When_No_TableFound()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ThrowsAsync(new BadPxWebResponseException(System.Net.HttpStatusCode.BadRequest, "Foobar"));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, false, new LineChartVisualizationSettings(null, false, null)));

            mockSqFileInterface.Setup(x => x.ArchiveCubeExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            Assert.ThrowsAsync<BadPxWebResponseException>(() => controller.GetQueryMeta(testQueryId));
            mockCachedPxWebConnection.Verify(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()), Times.Once());

        }

        [Test]
        public void GetQueryMetaTest_ArchiveFileNotFound()
        {
            var mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            var mockSqFileInterface = new Mock<ISqFileInterface>();

            var testQueryId = "aaa-bbb-111-222-333";

            List<VariableParameters> queryParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7)
            };

            mockCachedPxWebConnection.Setup(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()))
                .ThrowsAsync(new BadPxWebResponseException(System.Net.HttpStatusCode.BadRequest, "Foobar"));

            mockSqFileInterface.Setup(x => x.SavedQueryExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(true);
            mockSqFileInterface.Setup(x => x.ReadSavedQueryFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestSavedQuery(queryParams, true, new LineChartVisualizationSettings(null, false, null)));

            mockSqFileInterface.Setup(x => x.ArchiveCubeExists(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .Returns(false);
            mockSqFileInterface.Setup(x => x.ReadArchiveCubeFromFile(It.Is<string>(s => s == testQueryId), It.IsAny<string>()))
                .ReturnsAsync(() => TestDataCubeBuilder.BuildTestArchiveCube(metaParams));

            QueryMetaController controller = new(mockSqFileInterface.Object, mockCachedPxWebConnection.Object, new Mock<ILogger<QueryMetaController>>().Object);
            Assert.ThrowsAsync<FileNotFoundException>(() => controller.GetQueryMeta(testQueryId));
            mockCachedPxWebConnection.Verify(x => x.GetCubeMetaCachedAsync(It.IsAny<PxFileReference>()), Times.Never());
        }
    }
}

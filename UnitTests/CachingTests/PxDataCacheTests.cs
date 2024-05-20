using Moq;
using NUnit.Framework;
using PxGraf.Data.MetaData;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.Caching;
using PxGraf.PxWebInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PxGraf.Data;
using PxGraf.Enums;
using UnitTests.TestDummies.DummyQueries;
using UnitTests.TestDummies;
using PxGraf.Language;
using PxGraf.Models.SavedQueries;
using Microsoft.Extensions.Configuration;
using UnitTests.Fixtures;
using PxGraf.Settings;

namespace CachingTests
{
    internal static class PxDataCacheTests
    {
        [OneTimeSetUp]
        public static void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public static void PxDataEntryIsRemovedFromCacheIfLastFetchThrewException()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            var fileRef = new PxFileReference(["test1"], "testName");
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Task<DataCube> faultyMetaTask = Task.FromException<DataCube>(new BadPxWebResponseException(HttpStatusCode.BadRequest, "This is a faulty task"));
            mockMemoryCache.Setup(x => x.TryGetData(It.IsAny<IReadOnlyCubeMeta>(), out faultyMetaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.RemoveData(It.IsAny<IReadOnlyCubeMeta>()));

            CachedPxWebConnection cachedConnection = new (mockPxWebApi.Object, mockMemoryCache.Object);
            AggregateException ex = Assert.ThrowsAsync<AggregateException>(async () => await cachedConnection.GetDataCubeCachedAsync(fileRef, testMeta));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<BadPxWebResponseException>());

            mockMemoryCache.Verify(x => x.RemoveData(It.IsAny<IReadOnlyCubeMeta>()), Times.Once);
        }

        [Test]
        public static async Task PxDataFetchTriggersIfNoValidDataInCache()
        {
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetPxTableDataAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()))
                .Returns(Task.Factory.StartNew(() => TestDataCubeBuilder.BuildTestDataCube(variables)));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            Task<DataCube> mockParam = default;
            mockMemoryCache.Setup(x => x.TryGetData(It.IsAny<IReadOnlyCubeMeta>(), out mockParam)).Returns(false);

            PxFileReference fileRef = new(["test1"], "testName");

            CachedPxWebConnection cachedConnection = new (mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataCubeCachedAsync(fileRef, testMeta);

            mockPxWebApi.Verify(x => x.GetPxTableDataAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Once);
        }


        [Test]
        public static async Task PxDataFetchDoesNotTriggerIfValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);

            Task<DataCube> mockParam = Task.Factory.StartNew(() => TestDataCubeBuilder.BuildTestDataCube(variables));
            mockMemoryCache.Setup(x => x.TryGetData(It.IsAny<IReadOnlyCubeMeta>(), out mockParam)).Returns(true);

            PxFileReference fileRef = new(["test1"], "testName");

            CachedPxWebConnection cachedConnection = new (mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataCubeCachedAsync(fileRef, testMeta);

            mockPxWebApi.Verify(x => x.GetPxTableDataAsync(It.IsAny<PxFileReference>(), It.IsAny<IReadOnlyCubeMeta>()), Times.Never);
        }

        [Test]
        public static async Task BuildDataCubeCachedAsyncTest()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);
            DataCube testData = TestDataCubeBuilder.BuildTestDataCube(variables);
            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);

            var mockMetaTask = Task.Factory.StartNew(() => testMeta);
            var mockDataTask = Task.Factory.StartNew(() => testData);

            mockMemoryCache.Setup(x => x.TryGetMeta(It.IsAny<PxFileReference>(), out mockMetaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(true);
            mockMemoryCache.Setup(x => x.TryGetData(It.IsAny<IReadOnlyCubeMeta>(), out mockDataTask)).Returns(true);

            CachedPxWebConnection cachedConnection = new (mockPxWebApi.Object, mockMemoryCache.Object);
            DataCube result = await cachedConnection.BuildDataCubeCachedAsync(testQuery);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Meta.Variables.Count);
            Assert.AreEqual(testData.Data.Length, result.Data.Length);
        }

        [Test]
        public static async Task BuildArchiveCubeCachedAsyncTest()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            List<VariableParameters> variables =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            IReadOnlyCubeMeta testMeta = TestDataCubeBuilder.BuildTestMeta(variables);
            DataCube testData = TestDataCubeBuilder.BuildTestDataCube(variables);
            CubeQuery testQuery = TestDataCubeBuilder.BuildTestCubeQuery(variables);

            var mockMetaTask = Task.Factory.StartNew(() => testMeta);
            var mockDataTask = Task.Factory.StartNew(() => testData);

            mockMemoryCache.Setup(x => x.TryGetMeta(It.IsAny<PxFileReference>(), out mockMetaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(true);
            mockMemoryCache.Setup(x => x.TryGetData(It.IsAny<IReadOnlyCubeMeta>(), out mockDataTask)).Returns(true);

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            ArchiveCube result = await cachedConnection.BuildArchiveCubeCachedAsync(testQuery);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Meta.Variables.Count);
            Assert.AreEqual(testData.Data.Length, result.Data.Count);
        }
    }
}

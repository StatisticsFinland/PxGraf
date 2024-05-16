using Moq;
using NUnit.Framework;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.Caching;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace CachingTests
{
    internal static class PxMetaCacheTests
    {
        [Test]
        public static async Task PxMetaFetchTriggersIfNoValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()))
                .Returns(Task.Factory.StartNew(() => (IReadOnlyCubeMeta)new CubeMeta()));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            var fileRef = new PxFileReference(new List<string>() { "test1" }, "testName");

            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(false);
            var cachedConnection = new CachedPxWebConnection(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetCubeMetaCachedAsync(fileRef);

            mockPxWebApi.Verify(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()), Times.Once);
        }

        [Test]
        public static void PxMetaEntryIsRemovedFromCacheIfLastFetchThrewException()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            var fileRef = new PxFileReference(new List<string>() { "test1" }, "testName");

            Task<IReadOnlyCubeMeta> faultyMetaTask = Task.FromException<IReadOnlyCubeMeta>(new BadPxWebResponseException(HttpStatusCode.BadRequest, "This is a faulty task"));
            mockMemoryCache.Setup(x => x.TryGetMeta(fileRef, out faultyMetaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(true);
            mockMemoryCache.Setup(x => x.RemoveMeta(It.IsAny<PxFileReference>()));

            var cachedConnection = new CachedPxWebConnection(mockPxWebApi.Object, mockMemoryCache.Object);
            var ex = Assert.ThrowsAsync<AggregateException>(async () => await cachedConnection.GetCubeMetaCachedAsync(fileRef));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<BadPxWebResponseException>());

            mockMemoryCache.Verify(x => x.RemoveMeta(It.IsAny<PxFileReference>()), Times.Once);
        }

        [Test]
        public static async Task PxMetaFetchDoesNotTriggerIfValidFreshDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()))
                .Returns(Task.Factory.StartNew(() => (IReadOnlyCubeMeta)new CubeMeta()));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            var fileRef = new PxFileReference(new List<string>() { "test1" }, "testName");

            Task<IReadOnlyCubeMeta> metaTask = Task.Factory.StartNew(() => (IReadOnlyCubeMeta)new CubeMeta());
            mockMemoryCache.Setup(x => x.TryGetMeta(fileRef, out metaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(true);
            var cachedConnection = new CachedPxWebConnection(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetCubeMetaCachedAsync(fileRef);

            mockPxWebApi.Verify(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()), Times.Never);
        }

        [Test]
        public static async Task PxMetaFetchTriggersIfUnfreshDataInCache()
        {
            List<VariableParameters> variables = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(variables);
            var fileRef = new PxFileReference(new List<string>() { "test1" }, "testName");

            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()))
                .Returns(Task.Factory.StartNew(() => (IReadOnlyCubeMeta)new CubeMeta()));
            mockPxWebApi.Setup(x => x.GetPxFileUpdateTimeAsync(fileRef, meta))
                .Returns(Task.Factory.StartNew(() => "2009-09-01T00:00:00.000Z"));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();

            Task<IReadOnlyCubeMeta> metaTask = Task.Factory.StartNew(() => (IReadOnlyCubeMeta)meta);
            mockMemoryCache.Setup(x => x.TryGetMeta(fileRef, out metaTask)).Returns(true);
            mockMemoryCache.Setup(x => x.CheckMetaCacheFreshness(It.IsAny<PxFileReference>())).Returns(false);
            var cachedConnection = new CachedPxWebConnection(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetCubeMetaCachedAsync(fileRef);

            mockPxWebApi.Verify(x => x.GetPxTableMetaAsync(It.IsAny<PxFileReference>(), It.IsAny<List<string>>()), Times.Never);
            mockMemoryCache.Verify(x => x.UpdateMetaCacheFreshness(It.IsAny<PxFileReference>()), Times.Once);
        }
    }
}

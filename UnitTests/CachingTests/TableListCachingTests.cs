using Moq;
using NUnit.Framework;
using PxGraf.PxWebInterface.Caching;
using PxGraf.PxWebInterface;
using System.Collections.Generic;
using System.Threading.Tasks;
using PxGraf.Exceptions;
using PxGraf.PxWebInterface.SerializationModels;
using System.Net;
using System;

namespace CachingTests
{
    internal static class TableListCachingTests
    {
        [Test]
        public static void DataBaseListingIsRemovedFromCacheIfLastFetchThrewException()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();

            Task<List<DataBaseListResponseItem>> faultyTask = Task
                .FromException<List<DataBaseListResponseItem>>(new BadPxWebResponseException(HttpStatusCode.BadRequest, "This is a faulty task"));
            mockMemoryCache.Setup(x => x.TryGetDataBases(It.IsAny<string>(), out faultyTask)).Returns(true);
            mockMemoryCache.Setup(x => x.RemoveDataBases(It.IsAny<string>()));

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            AggregateException ex = Assert.ThrowsAsync<AggregateException>(async () => await cachedConnection.GetDataBaseListingAsync("en"));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<BadPxWebResponseException>());

            mockMemoryCache.Verify(x => x.TryGetDataBases(It.IsAny<string>(), out It.Ref<Task<List<DataBaseListResponseItem>>>.IsAny), Times.Once);
            mockMemoryCache.Verify(x => x.RemoveDataBases("en"), Times.Once);
            mockPxWebApi.Verify(x => x.GetDataBaseListingAsync("en"), Times.Never);
        }

        [Test]
        public static async Task DataBaseListingFetchTriggersIfNoValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetDataBaseListingAsync(It.IsAny<string>()))
                .Returns(Task.Factory.StartNew(() => new List<DataBaseListResponseItem>()));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            Task<List<DataBaseListResponseItem>> mockParam = null;
            mockMemoryCache.Setup(x => x.TryGetDataBases(It.IsAny<string>(), out mockParam)).Returns(false);

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataBaseListingAsync("en");

            mockMemoryCache.Verify(x => x.TryGetDataBases(It.IsAny<string>(), out It.Ref<Task<List<DataBaseListResponseItem>>>.IsAny), Times.Once);
            mockPxWebApi.Verify(x => x.GetDataBaseListingAsync("en"), Times.Once);
        }

        [Test]
        public static async Task PxDataFetchDoesNotTriggerIfValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();

            Task<List<DataBaseListResponseItem>> mockTask = Task.Factory.StartNew(() => new List<DataBaseListResponseItem>());
            mockMemoryCache.Setup(x => x.TryGetDataBases(It.IsAny<string>(), out mockTask)).Returns(true);

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataBaseListingAsync("en");

            mockMemoryCache.Verify(x => x.TryGetDataBases(It.IsAny<string>(), out It.Ref<Task<List<DataBaseListResponseItem>>>.IsAny), Times.Once);
            mockPxWebApi.Verify(x => x.GetDataBaseListingAsync("en"), Times.Never);
        }

        [Test]
        public static void DataTableItemListingIsRemovedFromCacheIfLastFetchThrewException()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            Mock<IPxWebApiResponseCache> mockMemoryCache = new();

            Task<List<TableListResponseItem>> faultyTask = Task
                .FromException<List<TableListResponseItem>>(new BadPxWebResponseException(HttpStatusCode.BadRequest, "This is a faulty task"));
            mockMemoryCache.Setup(x => x.TryGetTableItems(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), out faultyTask)).Returns(true);
            mockMemoryCache.Setup(x => x.RemoveTableItems(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()));

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            AggregateException ex = Assert.ThrowsAsync<AggregateException>(async () => await cachedConnection.GetDataTableItemListingAsync("en", new List<string>()));
            Assert.That(ex.InnerExceptions[0], Is.TypeOf<BadPxWebResponseException>());

            mockMemoryCache.Verify(x => x.TryGetTableItems("en", It.IsAny<IReadOnlyList<string>>(), out It.Ref<Task<List<TableListResponseItem>>>.IsAny), Times.Once);
            mockMemoryCache.Verify(x => x.RemoveTableItems("en", It.IsAny<IReadOnlyList<string>>()), Times.Once);
            mockPxWebApi.Verify(x => x.GetTableItemListingAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()), Times.Never);
        }

        [Test]
        public static async Task DataTableItemListingFetchTriggersIfNoValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetTableItemListingAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()))
                .Returns(Task.Factory.StartNew(() => new List<TableListResponseItem>()));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            mockMemoryCache.Setup(x => x.TryGetTableItems(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), out It.Ref<Task<List<TableListResponseItem>>>.IsAny))
                .Returns(false);

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataTableItemListingAsync("en", new List<string>());

            mockMemoryCache.Verify(x => x.TryGetTableItems("en", It.IsAny<IReadOnlyList<string>>(), out It.Ref<Task<List<TableListResponseItem>>>.IsAny), Times.Once);
            mockPxWebApi.Verify(x => x.GetTableItemListingAsync("en", It.IsAny<IReadOnlyList<string>>()), Times.Once());
        }

        [Test]
        public static async Task DataTableItemListingFetchDoesNotTriggerIfValidDataInCache()
        {
            Mock<IPxWebApiInterface> mockPxWebApi = new();
            mockPxWebApi.Setup(x => x.GetTableItemListingAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()))
                .Returns(Task.Factory.StartNew(() => new List<TableListResponseItem>()));

            Mock<IPxWebApiResponseCache> mockMemoryCache = new();
            Task<List<TableListResponseItem>> mockTask = Task.Factory.StartNew(() => new List<TableListResponseItem>());
            mockMemoryCache.Setup(x => x.TryGetTableItems(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), out mockTask))
                .Returns(true);

            CachedPxWebConnection cachedConnection = new(mockPxWebApi.Object, mockMemoryCache.Object);
            await cachedConnection.GetDataTableItemListingAsync("en", new List<string>());

            mockMemoryCache.Verify(x => x.TryGetTableItems("en", It.IsAny<IReadOnlyList<string>>(), out It.Ref<Task<List<TableListResponseItem>>>.IsAny), Times.Once);
            mockPxWebApi.Verify(x => x.GetTableItemListingAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()), Times.Never());
        }
    }
}

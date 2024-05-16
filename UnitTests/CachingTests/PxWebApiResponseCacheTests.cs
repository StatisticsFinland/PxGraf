using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.Caching;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace CachingTests
{
    internal static class PxWebApiResponseCacheTests
    {
        [OneTimeSetUp]
        public static void DoSetup()
        {
            Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public static void TryGetMetaReturnsTrueIfMetaIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new (mockMemoryCache.Object);
            Assert.IsTrue(cache.TryGetMeta(new PxFileReference(new System.Collections.Generic.List<string>(), "testName"), out Task<IReadOnlyCubeMeta> _));
        }

        [Test]
        public static void TryGetMetaReturnsFalseIfMetaIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsFalse(cache.TryGetMeta(new PxFileReference(new System.Collections.Generic.List<string>(), "testName"), out Task<IReadOnlyCubeMeta> _));
        }

        [Test]
        public static void CacheMetaTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheMeta(new PxFileReference(new List<string>(), "testName"), Task.FromResult<IReadOnlyCubeMeta>(new CubeMeta()));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Exactly(2));
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Exactly(2));
        }

        [Test]
        public static void UpdateMetaCacheFreshnessTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.UpdateMetaCacheFreshness(new PxFileReference(new List<string>(), "testName"));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once());
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once());
        }

        [Test]
        public static void CheckMetaCacheFreshnessReturnsTrueIfFresh()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.IsTrue(cache.CheckMetaCacheFreshness(new PxFileReference(new List<string>(), "testName")));
        }

        [Test]
        public static void CheckMetaCacheFreshnessReturnsFalseIfNotFresh()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(false);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.IsFalse(cache.CheckMetaCacheFreshness(new PxFileReference(new List<string>(), "testName")));
        }

        [Test]
        public static void RemoveMetaTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveMeta(new PxFileReference(new List<string>(), "testName"));

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public static void TryGetDataReturnsTrueIfDataIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsTrue(cache.TryGetData(new CubeMeta(), out Task<DataCube> _));
        }

        [Test]
        public static void TryGetDataReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockCache = new();
            object outParam = default;
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.IsFalse(cache.TryGetData(new CubeMeta(), out Task<DataCube> _));
        }

        [Test]
        public static void CacheDataTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheData(new CubeMeta(), Task.FromResult<DataCube>(null));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public static void RemoveDataTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveData(new CubeMeta());

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public static void TryGetDataBasesReturnsTrueIfDataIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsTrue(cache.TryGetDataBases("en", out Task<List<DataBaseListResponseItem>> _));
        }

        [Test]
        public static void TryGetDataBasesReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns((ICacheEntry)null);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsFalse(cache.TryGetDataBases("en", out Task<List<DataBaseListResponseItem>> _));
        }

        [Test]
        public static void CacheDataBasesTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheDataBases("en", Task.FromResult(new List<DataBaseListResponseItem>()));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public static void RemoveDataBasesTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveDataBases("en");

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public static void TryGetTableItemsReturnsTrueIfDataIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsTrue(cache.TryGetTableItems("en", new List<string>(), out Task<List<TableListResponseItem>> _));
        }

        [Test]
        public static void TryGetTableItemsReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.IsFalse(cache.TryGetTableItems("en", new List<string>(), out Task<List<TableListResponseItem>> _));
        }

        [Test]
        public static void CacheTableItemsTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheTableItems("en", new List<string>(), Task.FromResult(new List<TableListResponseItem>()));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public static void RemoveTableItemsTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveTableItems("en", new List<string>());

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
    }
}

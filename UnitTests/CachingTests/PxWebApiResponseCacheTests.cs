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
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace CachingTests
{
    internal static class PxWebApiResponseCacheTests
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
        public static void TryGetMetaReturnsTrueIfMetaIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new (mockMemoryCache.Object);
            Assert.That(cache.TryGetMeta(new PxFileReference([], "testName"), out Task<IReadOnlyCubeMeta> _), Is.True);
        }

        [Test]
        public static void TryGetMetaReturnsFalseIfMetaIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.That(cache.TryGetMeta(new PxFileReference([], "testName"), out Task<IReadOnlyCubeMeta> _), Is.False);
        }

        [Test]
        public static void CacheMetaTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheMeta(new PxFileReference([], "testName"), Task.FromResult<IReadOnlyCubeMeta>(new CubeMeta()));

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
            cache.UpdateMetaCacheFreshness(new PxFileReference([], "testName"));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once());
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once());
        }

        [Test]
        public static void CheckMetaCacheFreshnessReturnsTrueIfFresh()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(true);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.That(cache.CheckMetaCacheFreshness(new PxFileReference([], "testName")), Is.True);
        }

        [Test]
        public static void CheckMetaCacheFreshnessReturnsFalseIfNotFresh()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(false);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.That(cache.CheckMetaCacheFreshness(new PxFileReference([], "testName")), Is.False);
        }

        [Test]
        public static void RemoveMetaTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveMeta(new PxFileReference([], "testName"));

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public static void TryGetDataReturnsTrueIfDataIsInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(true);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.That(cache.TryGetData(new CubeMeta(), out Task<DataCube> _), Is.True);
        }

        [Test]
        public static void TryGetDataReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockCache = new();
            object outParam = default;
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockCache.Object);
            Assert.That(cache.TryGetData(new CubeMeta(), out Task<DataCube> _), Is.False);
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
            Assert.That(cache.TryGetDataBases("en", out Task<List<DataBaseListResponseItem>> _), Is.True);
        }

        [Test]
        public static void TryGetDataBasesReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns((ICacheEntry)null);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.That(cache.TryGetDataBases("en", out Task<List<DataBaseListResponseItem>> _), Is.False);
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
            Assert.That(cache.TryGetTableItems("en", [], out Task<List<TableListResponseItem>> _), Is.True);
        }

        [Test]
        public static void TryGetTableItemsReturnsFalseIfDataIsNotInCache()
        {
            Mock<IMemoryCache> mockMemoryCache = new();

            object outParam = default;
            mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out outParam)).Returns(false);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            Assert.That(cache.TryGetTableItems("en", [], out Task<List<TableListResponseItem>> _), Is.False);
        }

        [Test]
        public static void CacheTableItemsTest()
        {
            Mock<IMemoryCache> mockMemoryCache = new();
            ICacheEntry cacheEntry = Mock.Of<ICacheEntry>();

            mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            PxWebApiResponseCache cache = new(mockMemoryCache.Object);
            cache.CacheTableItems("en", [], Task.FromResult(new List<TableListResponseItem>()));

            mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
            Mock.Get(cacheEntry).Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public static void RemoveTableItemsTest()
        {
            Mock<IMemoryCache> mockCache = new();
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));

            PxWebApiResponseCache cache = new(mockCache.Object);
            cache.RemoveTableItems("en", []);

            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
    }
}

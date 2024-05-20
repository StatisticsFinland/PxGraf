using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Caching;
using PxGraf.Language;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using System;
using System.IO;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace CachingTests
{
    internal class VisualizationResponseCacheTests
    {
        const string TEST_KEY = "foobar";

        [SetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public static void ConstructorTest()
        {
            VisualizationResponseCache cache = new();
            Assert.IsNotNull(cache);
        }

        [Test]
        public static void NoDelaySetAndFetchTest()
        {
            VisualizationResponseCache cache = new();
            var task = Task.Factory.StartNew(() => new VisualizationResponse());
            task.Wait();
            cache.Set(TEST_KEY, task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse response);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Fresh, state);
            Assert.IsNotNull(response);
            cache.Dispose();
        }

        [Test]
        public static void PendingSetAndFetchTest()
        {
            VisualizationResponseCache cache = new();
            cache.Set(TEST_KEY, new TaskCompletionSource<VisualizationResponse>().Task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse _);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Pending, state);
            cache.Dispose();
        }

        [Test]
        public static void StaleDataSetAndFetchTest()
        {
            Configuration.Current.CacheOptions.CacheFreshnessCheckInterval = 0;
            VisualizationResponseCache cache = new();
            var task = Task.Factory.StartNew(() => new VisualizationResponse());
            task.Wait();
            cache.Set(TEST_KEY, task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse response);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Stale, state);
            Assert.IsNotNull(response);
            cache.Dispose();
        }

        [Test]
        public static void NoResponseFoundTest()
        {
            VisualizationResponseCache cache = new();
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse _);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Null, state);
            cache.Dispose();
        }

        [Test]
        public static void FaultedTaskReturnsErrorState()
        {
            VisualizationResponseCache cache = new();
            var task = Task.FromException<VisualizationResponse>(new Exception("This is a faulty task"));
            cache.Set(TEST_KEY, task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse faultyResponse);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Error, state);
            Assert.IsNull(faultyResponse);

            // check that the entry is removed from the cache
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Null, cache.TryGet(TEST_KEY, out _));

            cache.Dispose();
        }

        [Test]
        public static void RefreshTest()
        {
            Configuration.Current.CacheOptions.CacheFreshnessCheckInterval = 0;
            VisualizationResponseCache cache = new();
            var task = Task.Factory.StartNew(() => new VisualizationResponse());
            task.Wait();
            cache.Set(TEST_KEY, task);
            var preFreshState = cache.TryGet(TEST_KEY, out VisualizationResponse response1);
            Configuration.Current.CacheOptions.CacheFreshnessCheckInterval = 45;

            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Stale, preFreshState);
            Assert.IsNotNull(response1);

            cache.Refresh(TEST_KEY);

            var freshState = cache.TryGet(TEST_KEY, out VisualizationResponse response2);
            Assert.AreEqual(VisualizationResponseCache.CacheEntryState.Fresh, freshState);
            Assert.IsNotNull(response2);

            cache.Dispose();
        }
    }
}

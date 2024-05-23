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
            Assert.That(cache, Is.Not.Null);
        }

        [Test]
        public static void NoDelaySetAndFetchTest()
        {
            VisualizationResponseCache cache = new();
            var task = Task.Factory.StartNew(() => new VisualizationResponse());
            task.Wait();
            cache.Set(TEST_KEY, task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse response);
            Assert.That(state, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Fresh));
            Assert.That(response, Is.Not.Null);
            cache.Dispose();
        }

        [Test]
        public static void PendingSetAndFetchTest()
        {
            VisualizationResponseCache cache = new();
            cache.Set(TEST_KEY, new TaskCompletionSource<VisualizationResponse>().Task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse _);
            Assert.That(state, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Pending));
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
            Assert.That(state, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Stale));
            Assert.That(response, Is.Not.Null);
            cache.Dispose();
        }

        [Test]
        public static void NoResponseFoundTest()
        {
            VisualizationResponseCache cache = new();
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse _);
            Assert.That(state, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Null));
            cache.Dispose();
        }

        [Test]
        public static void FaultedTaskReturnsErrorState()
        {
            VisualizationResponseCache cache = new();
            var task = Task.FromException<VisualizationResponse>(new Exception("This is a faulty task"));
            cache.Set(TEST_KEY, task);
            var state = cache.TryGet(TEST_KEY, out VisualizationResponse faultyResponse);
            Assert.That(state, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Error));
            Assert.That(faultyResponse, Is.Null);

            // check that the entry is removed from the cache
            Assert.That(cache.TryGet(TEST_KEY, out _), Is.EqualTo(VisualizationResponseCache.CacheEntryState.Null));

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

            Assert.That(preFreshState, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Stale));
            Assert.That(response1, Is.Not.Null);

            cache.Refresh(TEST_KEY);

            var freshState = cache.TryGet(TEST_KEY, out VisualizationResponse response2);
            Assert.That(freshState, Is.EqualTo(VisualizationResponseCache.CacheEntryState.Fresh));
            Assert.That(response2, Is.Not.Null);

            cache.Dispose();
        }
    }
}

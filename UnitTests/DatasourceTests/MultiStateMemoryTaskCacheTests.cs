using NUnit.Framework;
using PxGraf.Datasource.Cache;
using System;
using System.Threading.Tasks;

namespace UnitTests.DatasourceTests
{
    class MultiStateMemoryTaskCacheTests
    {
        [Test]
        public async Task TryGetTest_FreshEntry_ReturnsFresh()
        {
            const string TEST_KEY = "abcd-1234";

            // Arrange
            MultiStateMemoryTaskCache cache = new(1, TimeSpan.FromMinutes(1));
            Task<string> task = Task.FromResult("test"); // Is completed already
            cache.Set(TEST_KEY, task, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            // Act
            MultiStateMemoryTaskCache.CacheEntryState state = cache.TryGet(TEST_KEY, out Task<string> value);

            // Assert
            Assert.That(state, Is.EqualTo(MultiStateMemoryTaskCache.CacheEntryState.Fresh));

            await value;
            Assert.That(value.Result, Is.EqualTo(task.Result));
        }

        [Test]
        public async Task TryGetTest_StaleEntry_ReturnsStale()
        {
            const string TEST_KEY = "abcd-1234";

            // Arrange
            MultiStateMemoryTaskCache cache = new(1, TimeSpan.FromMilliseconds(1));
            Task<string> task = Task.FromResult("test"); // Is completed already
            cache.Set(TEST_KEY, task, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            await Task.Delay(2); // Wait 2ms for the entry to go stale. The cache does not accept 0 length durations.

            // Act
            MultiStateMemoryTaskCache.CacheEntryState state = cache.TryGet(TEST_KEY, out Task<string> value);
            

            // Assert
            Assert.That(state, Is.EqualTo(MultiStateMemoryTaskCache.CacheEntryState.Stale));

            await value;
            Assert.That(value.Result, Is.EqualTo(task.Result));
        }

        [Test]
        public void TryGetTest_NullEntry_ReturnsNull()
        {
            const string TEST_KEY = "abcd-1234";

            // Arrange
            MultiStateMemoryTaskCache cache = new(1, TimeSpan.FromMinutes(1));

            // Act
            MultiStateMemoryTaskCache.CacheEntryState state = cache.TryGet(TEST_KEY, out Task<string> value);

            // Assert
            Assert.That(state, Is.EqualTo(MultiStateMemoryTaskCache.CacheEntryState.Null));
            Assert.That(value, Is.Null);
        }

        [Test]
        public void TryGetTest_ErrorEntry_ReturnsError()
        {
            const string TEST_KEY = "abcd-1234";

            // Arrange
            MultiStateMemoryTaskCache cache = new(1, TimeSpan.FromMinutes(1));
            Task<string> task = Task.FromException<string>(new Exception("Test exception"));
            cache.Set(TEST_KEY, task, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            // Act
            MultiStateMemoryTaskCache.CacheEntryState state = cache.TryGet(TEST_KEY, out Task<string> value);

            // Assert
            Assert.That(state, Is.EqualTo(MultiStateMemoryTaskCache.CacheEntryState.Error));
            Assert.ThrowsAsync<Exception>(() => value);
            Assert.That(value.Exception.Message.Contains("Test exception"));
        }
    }
}

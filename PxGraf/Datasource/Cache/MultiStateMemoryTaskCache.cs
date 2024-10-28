using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PxGraf.Datasource.Cache
{
    public sealed class MultiStateMemoryTaskCache : IMultiStateMemoryTaskCache, IDisposable 
    {
        public enum CacheEntryState
        {
            /// <summary>
            /// Data can be used as is
            /// </summary>
            Fresh,
            /// <summary>
            /// Data can be used, but it can be outdated
            /// </summary>
            Stale,
            /// <summary>
            /// The key was not present in the cache
            /// </summary>
            Null,
            /// <summary>
            /// An error occurred while fetching the data
            /// </summary>
            Error
        }

        private readonly MemoryCache _cache;
        private readonly TimeSpan _freshnessDuration;
        private const string ENTRY_KEY_HASH_SEED = "pNlNfoAkxtzCDkNaFqx9";
        private const string FRESHNESS_TOKEN_HASH_SEED = "fUHaFE3je5kl13j4sdc";

        public MultiStateMemoryTaskCache(int cacheSizeLimit, TimeSpan itemFreshnessDuration)
        {
            MemoryCacheOptions cacheOptions = new()
            {
                SizeLimit = cacheSizeLimit
            };
            _cache = new MemoryCache(cacheOptions);
            _freshnessDuration = itemFreshnessDuration;
        }

        public void Set<ItemType>(string key, Task<ItemType> task, TimeSpan slidingExpiration, TimeSpan absoluteExpiration)
        {
            MemoryCacheEntryOptions freshnessCheckEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_freshnessDuration)
                .SetSize(0);

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(slidingExpiration)
                .SetAbsoluteExpiration(absoluteExpiration)
                .SetSize(1);

            _cache.Set(GenerateCacheKeyHash(key, FRESHNESS_TOKEN_HASH_SEED), task, freshnessCheckEntryOptions);
            _cache.Set(GenerateCacheKeyHash(key, ENTRY_KEY_HASH_SEED), task, cacheEntryOptions);
        }

        public CacheEntryState TryGet<ItemType>(string key, out Task<ItemType> value)
        {
            string freshnessKey = GenerateCacheKeyHash(key, FRESHNESS_TOKEN_HASH_SEED);
            string entryKey = GenerateCacheKeyHash(key, ENTRY_KEY_HASH_SEED);
            if (_cache.TryGetValue(freshnessKey, out value))
            {
                if (value.IsFaulted)
                {
                    _cache.Remove(freshnessKey);
                    _cache.Remove(entryKey);
                    return CacheEntryState.Error;
                }

                return CacheEntryState.Fresh;
            }
            else if (_cache.TryGetValue(entryKey, out value))
            {
                if (value.IsFaulted)
                {
                    _cache.Remove(entryKey);
                    return CacheEntryState.Error;
                }

                return CacheEntryState.Stale;
            }
            else
            {
                value = default;
                return CacheEntryState.Null;
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        private static string GenerateCacheKeyHash(string key, string seed)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(key + seed);
            byte[] hashBytes = MD5.HashData(inputBytes);

            return BitConverter.ToString(hashBytes);
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PxGraf.Caching
{
    public sealed class VisualizationResponseCache : IVisualizationResponseCache, IDisposable
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
            /// Data is being fetched or processed, not ready to be used
            /// </summary>
            Pending,
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
        private readonly ConcurrentDictionary<string, DateTime> _cacheValidationTimes;
        private static CacheValues CacheValues => Configuration.Current.CacheOptions.Visualization;
        private static int DataFreshnessTime => Configuration.Current.CacheOptions.CacheFreshnessCheckInterval;

        public VisualizationResponseCache()
        {
            MemoryCacheOptions cacheOptions = new()
            {
                SizeLimit = CacheValues.SizeLimit
            };
            _cache = new MemoryCache(cacheOptions);
            _cacheValidationTimes = new();
        }

        public void Set(string key, Task<VisualizationResponse> response)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CacheValues.SlidingExpiration))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheValues.AbsoluteExpiration))
                .SetSize(1);

            _cacheValidationTimes[key] = DateTime.Now;
            _cache.Set(key, response, cacheEntryOptions);
        }

        public void Refresh(string key)
        {
            _cacheValidationTimes[key] = DateTime.Now;
        }

        public CacheEntryState TryGet(string key, out VisualizationResponse value)
        {
            if (_cache.TryGetValue(key, out Task<VisualizationResponse> cachedTask))
            {
                if (cachedTask.IsFaulted)
                {
                    _cache.Remove(key);
                    value = null;
                    return CacheEntryState.Error;
                }

                if (cachedTask.IsCompleted)
                {
                    if (_cacheValidationTimes[key].AddSeconds(DataFreshnessTime) > DateTime.Now)
                    {
                        value = cachedTask.Result;
                        return CacheEntryState.Fresh;
                    }
                    else
                    {
                        value = cachedTask.Result;
                        return CacheEntryState.Stale;
                    }
                }
                else
                {
                    value = null;
                    return CacheEntryState.Pending;
                }
            }
            else
            {
                value = null;
                return CacheEntryState.Null;
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}

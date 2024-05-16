using Microsoft.Extensions.Caching.Memory;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface.Caching
{
    public class PxWebApiResponseCache : IPxWebApiResponseCache
    {
        private static CacheOptions CacheOptions => Configuration.Current.CacheOptions;
        private readonly IMemoryCache _memoryCache;
        public PxWebApiResponseCache(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        private const string META_FRESHNESS_CHECK_HASH_SEED = "meta_freshness_check_identifier";
        private const string META_FETCH_HASH_SEED = "table_check_identifier";

        public bool TryGetMeta(PxFileReference pxFileRef, out Task<IReadOnlyCubeMeta> meta)
        {
            if (_memoryCache.TryGetValue(pxFileRef.BuildCacheKeyHash(META_FETCH_HASH_SEED), out Task<IReadOnlyCubeMeta> cacheValue))
            {
                meta = cacheValue;
                return true;
            }
            else
            {
                meta = null;
                return false;
            }
        }

        public void CacheMeta(PxFileReference pxFileRef, Task<IReadOnlyCubeMeta> meta)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(CacheOptions.Meta.SlidingExpiration))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheOptions.Meta.AbsoluteExpiration));

            _memoryCache.Set(pxFileRef.BuildCacheKeyHash(META_FETCH_HASH_SEED), meta, cacheEntryOptions);
            UpdateMetaCacheFreshness(pxFileRef);
        }

        public void UpdateMetaCacheFreshness(PxFileReference pxFileRef)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheOptions.CacheFreshnessCheckInterval));
            _memoryCache.Set(pxFileRef.BuildCacheKeyHash(META_FRESHNESS_CHECK_HASH_SEED), "", cacheEntryOptions);
        }

        public bool CheckMetaCacheFreshness(PxFileReference pxFileRef)
        {
            return _memoryCache.TryGetValue(pxFileRef.BuildCacheKeyHash(META_FRESHNESS_CHECK_HASH_SEED), out _);
        }

        public void RemoveMeta(PxFileReference pxFileRef)
        {
            _memoryCache.Remove(pxFileRef.BuildCacheKeyHash(META_FETCH_HASH_SEED));
            _memoryCache.Remove(pxFileRef.BuildCacheKeyHash(META_FRESHNESS_CHECK_HASH_SEED));
        }

        public bool TryGetData(IReadOnlyCubeMeta meta, out Task<DataCube> data)
        {
            if (_memoryCache.TryGetValue(GetHashString(meta), out Task<DataCube> cacheValue))
            {
                data = cacheValue;
                return true;
            }
            else
            {
                data = null;
                return false;
            }
        }

        public void CacheData(IReadOnlyCubeMeta meta, Task<DataCube> data)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CacheOptions.Data.SlidingExpiration))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheOptions.Data.AbsoluteExpiration));

            _memoryCache.Set(GetHashString(meta), data, cacheEntryOptions);
        }

        public void RemoveData(IReadOnlyCubeMeta meta)
        {
            _memoryCache.Remove(GetHashString(meta));
        }

        public bool TryGetDataBases(string lang, out Task<List<DataBaseListResponseItem>> dataBases)
        {
            if (_memoryCache.TryGetValue("databases" + lang, out Task<List<DataBaseListResponseItem>> cacheValue))
            {
                dataBases = cacheValue;
                return true;
            }
            else
            {
                dataBases = null;
                return false;
            }
        }

        public void CacheDataBases(string lang, Task<List<DataBaseListResponseItem>> dataBases)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheOptions.Database.AbsoluteExpiration));

            _memoryCache.Set(lang, dataBases, cacheEntryOptions);
        }

        public void RemoveDataBases(string lang)
        {
            _memoryCache.Remove(lang);
        }

        public bool TryGetTableItems(string lang, IReadOnlyList<string> path, out Task<List<TableListResponseItem>> tableListItems)
        {
            if (_memoryCache.TryGetValue($"{lang}/{string.Join("/", path)}", out Task<List<TableListResponseItem>> cacheValue))
            {
                tableListItems = cacheValue;
                return true;
            }
            else
            {
                tableListItems = null;
                return false;
            }
        }

        public void CacheTableItems(string lang, IReadOnlyList<string> path, Task<List<TableListResponseItem>> tableListItems)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CacheOptions.Table.SlidingExpiration));

            _memoryCache.Set($"{lang}/{string.Join("/", path)}", tableListItems, cacheEntryOptions);
        }

        public void RemoveTableItems(string lang, IReadOnlyList<string> path)
        {
            _memoryCache.Remove($"{lang}/{string.Join("/", path)}");
        }

        private static string GetHashString(IReadOnlyCubeMeta cubeMeta)
        {
            StringBuilder builder = new();

            // Build a continuous string blob of the var codes, values and last updated strings, syntax doesent matter here just the values and ORDER.
            foreach (var variable in cubeMeta.Variables)
            {
                builder.Append($"{variable.Code}");
                foreach (var varVal in variable.IncludedValues)
                {
                    builder.Append(varVal.Code);
                    if (variable.Type == Enums.VariableType.Content) builder.Append(varVal.ContentComponent.LastUpdated);
                }
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(builder.ToString());
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

            return BitConverter.ToString(hashBytes);
        }
    }
}

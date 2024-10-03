using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Datasource
{
    public abstract class CachedDatasource(IMultiStateMemoryTaskCache taskCache) : ICachedDatasource
    {
        protected sealed class MetaCacheHousing(DateTime lastWriteTime, IReadOnlyMatrixMetadata meta)
        {
            public DateTime LastWritetime { get; } = lastWriteTime;
            public IReadOnlyMatrixMetadata Metadata { get; } = meta;
        }
        
        protected sealed class DataCacheHousing(DateTime lastWriteTime, DecimalDataValue[] data)
        {
            public DateTime LastWritetime { get; } = lastWriteTime;
            public DecimalDataValue[] Data { get; } = data;
        }

        protected readonly IMultiStateMemoryTaskCache _taskCache = taskCache;
        protected readonly static CacheOptions _cacheOptions = Configuration.Current.CacheOptions;

        public async Task<DatabaseGroupContents> GetGroupContentsCachedAsync(IReadOnlyList<string> hierarcy)
        {
            string key = GetHierarchyKey(hierarcy);
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(_cacheOptions.Database.SlidingExpiration);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(_cacheOptions.Database.AbsoluteExpiration);

            MultiStateMemoryTaskCache.CacheEntryState state = _taskCache.TryGet(key, out Task<DatabaseGroupContents> contentsTask);
            if (state == MultiStateMemoryTaskCache.CacheEntryState.Null)
            {
                contentsTask = GetGroupContentsAsync(hierarcy);
                _taskCache.Set(key, contentsTask, slidingExpiration, absoluteExpiration);
            }
            return await contentsTask;
        }

        public async Task<IReadOnlyMatrixMetadata> GetMatrixMetadataCachedAsync(PxTableReference tableReference)
        {
            string key = GetKeyForTable(tableReference);
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(_cacheOptions.Meta.SlidingExpiration);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(_cacheOptions.Meta.AbsoluteExpiration);
            MultiStateMemoryTaskCache.CacheEntryState state = _taskCache.TryGet(key, out Task<MetaCacheHousing> metaTask);
            if (state == MultiStateMemoryTaskCache.CacheEntryState.Null)
            {
                metaTask = GenerateNewMetaCacheHousingAsync(tableReference);
                _taskCache.Set(key, metaTask, slidingExpiration, absoluteExpiration);
            }
            else if (state == MultiStateMemoryTaskCache.CacheEntryState.Stale)
            {
                DateTime lastWritetime = await GetLastUpdateTimeAsync(tableReference);
                if (lastWritetime > (await metaTask).LastWritetime)
                {
                    metaTask = GenerateNewMetaCacheHousingAsync(tableReference);
                }
                _taskCache.Set(key, metaTask, slidingExpiration, absoluteExpiration);
            }

            MetaCacheHousing housing = await metaTask;
            return housing.Metadata;
        }

        public abstract Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata);

        public async Task<Matrix<DecimalDataValue>> GetMatrixCachedAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata)
        {
            CacheValues dataCacheConfig = Configuration.Current.CacheOptions.Data;
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(dataCacheConfig.SlidingExpiration);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(dataCacheConfig.AbsoluteExpiration);

            string key = GetKeyForMatrixMap(tableReference, metadata);
            MultiStateMemoryTaskCache.CacheEntryState state = _taskCache.TryGet(key, out Task<DataCacheHousing> dataTask);
            if(state == MultiStateMemoryTaskCache.CacheEntryState.Null)
            {
                dataTask = GenerateNewDataCacheHousingAsync(tableReference, metadata);
                _taskCache.Set(key, dataTask, slidingExpiration, absoluteExpiration); 
            }
            else if (state == MultiStateMemoryTaskCache.CacheEntryState.Stale)
            {
                DateTime lastWritetime = await GetLastUpdateTimeAsync(tableReference);
                if (lastWritetime > (await dataTask).LastWritetime)
                {
                    dataTask = GenerateNewDataCacheHousingAsync(tableReference, metadata);
                }
                _taskCache.Set(key, dataTask, slidingExpiration, absoluteExpiration);
            }
            DataCacheHousing housing = await dataTask;
            return new Matrix<DecimalDataValue>(metadata, housing.Data);
        }

        protected abstract Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> hierarcy);

        protected abstract Task<MetaCacheHousing> GenerateNewMetaCacheHousingAsync(PxTableReference tableReference);

        protected abstract Task<DataCacheHousing> GenerateNewDataCacheHousingAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta);

        protected abstract Task<DateTime> GetLastUpdateTimeAsync(PxTableReference tableReference);

        protected static string GetHierarchyKey(IReadOnlyList<string> hierarchy)
        {
            return $"hierarchy-{string.Join('-', hierarchy)}";
        }

        protected static string GetKeyForTable(PxTableReference table)
        {
            return $"{string.Join('-', table.Hierarchy)}-{table.Name}";
        }

        private static string GetKeyForMatrixMap(PxTableReference tableRef, IMatrixMap map)
        {
            IEnumerable<string> dimKeys = map.DimensionMaps
                .Select(dm => $"{dm.Code}-{string.Join('-', dm.ValueCodes)}");
            return $"{GetKeyForTable(tableRef)}-{string.Join('-', dimKeys)}";
        }
    }
}

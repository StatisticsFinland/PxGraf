using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PxGraf.Datasource
{
    /// <summary>
    /// Abstract class for a datasource that caches data about px files.
    /// </summary>
    public abstract class CachedDatasource(IMultiStateMemoryTaskCache taskCache) : ICachedDatasource
    {
        /// <summary>
        /// Represents a cache housing for metadata.
        /// </summary>
        public sealed class MetaCacheHousing(DateTime lastWriteTime, IReadOnlyMatrixMetadata meta)
        {
            /// <summary>
            /// Date time of the last write.
            /// </summary>
            public DateTime LastWritetime { get; } = lastWriteTime;
            /// <summary>
            /// Metadata of the table.
            /// </summary>
            public IReadOnlyMatrixMetadata Metadata { get; } = meta;
        }
        
        /// <summary>
        /// Represents a cache housing for data.
        /// </summary>
        protected sealed class DataCacheHousing(DateTime lastWriteTime, DecimalDataValue[] data)
        {
            /// <summary>
            /// Date time of the last write.
            /// </summary>
            public DateTime LastWritetime { get; } = lastWriteTime;
            /// <summary>
            /// Data of the table.
            /// </summary>
            public DecimalDataValue[] Data { get; } = data;
        }

        protected readonly IMultiStateMemoryTaskCache _taskCache = taskCache;
        protected readonly static CacheOptions _cacheOptions = Configuration.Current.CacheOptions;

        public async Task<DatabaseGroupContents> GetGroupContentsCachedAsync(IReadOnlyList<string> hierarcy)
        {
            string key = GetHierarchyKey(hierarcy);
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(_cacheOptions.Database.SlidingExpirationMinutes);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(_cacheOptions.Database.AbsoluteExpirationMinutes);

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
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(_cacheOptions.Meta.SlidingExpirationMinutes);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(_cacheOptions.Meta.AbsoluteExpirationMinutes);
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
            TimeSpan slidingExpiration = TimeSpan.FromMinutes(dataCacheConfig.SlidingExpirationMinutes);
            TimeSpan absoluteExpiration = TimeSpan.FromMinutes(dataCacheConfig.AbsoluteExpirationMinutes);

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

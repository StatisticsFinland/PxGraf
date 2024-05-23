using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface.Caching;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface
{
    /// <summary>
    /// Handles requesting data and metadata from pxweb and caches the results to reduce unnecessary requests.
    /// </summary>
    /// <remarks>
    /// Default constructor
    /// </remarks>
    /// <param name="pxWebApi">Connection to pxweb</param>
    /// <param name="apiRespCache">Memory cache to be used with this connection</param>
    public class CachedPxWebConnection(IPxWebApiInterface pxWebApi, IPxWebApiResponseCache apiRespCache) : ICachedPxWebConnection
    {
        private readonly IPxWebApiInterface _pxWebApi = pxWebApi;
        private readonly IPxWebApiResponseCache _apiResponseCache = apiRespCache;

        /// <summary>
        /// Retuns variable and variable value metadata from given px file.
        /// The metadata contains all variables and variable values.
        /// If the metadata of the px file is already in the cache, that value is returned.
        /// Otherwise requests the metadata from pxweb and stores it in the memory cache.
        /// </summary>
        /// <param name="pxFileReference"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCubeMeta> GetCubeMetaCachedAsync(PxFileReference pxFileReference)
        {
            if (_apiResponseCache.TryGetMeta(pxFileReference, out Task<IReadOnlyCubeMeta> metaTask))
            {
                if(metaTask.IsFaulted)
                {
                    _apiResponseCache.RemoveMeta(pxFileReference);
                    throw metaTask.Exception;
                }

                if (_apiResponseCache.CheckMetaCacheFreshness(pxFileReference))
                {
                    return await metaTask;
                }
                else
                {
                    _apiResponseCache.UpdateMetaCacheFreshness(pxFileReference);
                    IReadOnlyCubeMeta metaData = await metaTask;
                    string tableLastUpdated = await _pxWebApi.GetPxFileUpdateTimeAsync(pxFileReference, metaData);
                    if (tableLastUpdated == metaData.GetLastUpdated())
                    {
                        return metaData;
                    }
                }
            }

            Task<IReadOnlyCubeMeta> newMetaTask = _pxWebApi.GetPxTableMetaAsync(pxFileReference);
            _apiResponseCache.CacheMeta(pxFileReference, newMetaTask);
            return await newMetaTask;
        }

        /// <summary>
        /// Retuns data cube matching the given meta object and table reference.
        /// If the matching data is already in the cache, that data is returned.
        /// Otherwise requests the data from pxweb and stores it in the memory cache.
        /// </summary>
        /// <returns></returns>
        public async Task<DataCube> GetDataCubeCachedAsync(PxFileReference pxFile, IReadOnlyCubeMeta meta)
        {
            // This try returns false also if the last updated timestamps do not match
            if (_apiResponseCache.TryGetData(meta, out Task<DataCube> cubeTask))
            {
                if (cubeTask.IsFaulted)
                {
                    _apiResponseCache.RemoveData(meta);
                    throw cubeTask.Exception;
                }

                // A new cube is built because the meta text fields may have changed and we want to keep those changes
                // The structural similarity of the meta objects is guaranteed be the cache, the try fails if there is any difference.
                DataCube cube = await cubeTask;
                return new DataCube(meta, cube.Data);
            }
            else
            {
                Task<DataCube> cubeFetchTask = _pxWebApi.GetPxTableDataAsync(pxFile, meta);
                _apiResponseCache.CacheData(meta, cubeFetchTask);
                return await cubeFetchTask;
            }
        }

        public async Task<List<DataBaseListResponseItem>> GetDataBaseListingAsync(string lang)
        {
            if (_apiResponseCache.TryGetDataBases(lang, out Task<List<DataBaseListResponseItem>> dataBasesTask))
            {
                if (dataBasesTask.IsFaulted)
                {
                    _apiResponseCache.RemoveDataBases(lang);
                    throw dataBasesTask.Exception;
                }

                return await dataBasesTask;
            }
            else
            {
                var dataBasesFetchTask = _pxWebApi.GetDataBaseListingAsync(lang);
                _apiResponseCache.CacheDataBases(lang, dataBasesFetchTask);
                return await dataBasesFetchTask;
            }
        }

        public async Task<List<TableListResponseItem>> GetDataTableItemListingAsync(string lang, IReadOnlyList<string> path)
        {
            if (_apiResponseCache.TryGetTableItems(lang, path, out Task<List<TableListResponseItem>> tableListItemsTask))
            {
                if (tableListItemsTask.IsFaulted)
                {
                    _apiResponseCache.RemoveTableItems(lang, path);
                    throw tableListItemsTask.Exception;
                }

                return await tableListItemsTask;
            }
            else
            {
                var tableListItemsFetchTask = _pxWebApi.GetTableItemListingAsync(lang, path);
                _apiResponseCache.CacheTableItems(lang, path, tableListItemsFetchTask);
                return await tableListItemsFetchTask;
            }
        }

        /// <summary>
        /// Retuns DataCube objects matchinbg the provided query, data and metadata are cached separately.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DataCube> BuildDataCubeCachedAsync(CubeQuery query)
        {
            var tableMeta = await GetCubeMetaCachedAsync(query.TableReference);
            var tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(query);

            return await GetDataCubeCachedAsync(query.TableReference, tableMetaCopy);
        }

        /// <summary>
        /// Retuns an ArchiveCube object matchinbg the provided query, data and metadata are cached separately.
        /// </summary>
        public async Task<ArchiveCube> BuildArchiveCubeCachedAsync(CubeQuery query)
        {
            var tableMeta = await GetCubeMetaCachedAsync(query.TableReference);
            var tableMetaCopy = tableMeta.Clone();
            tableMetaCopy.ApplyEditionFromQuery(query);
            tableMetaCopy.Header.Truncate(Configuration.Current.QueryOptions.MaxHeaderLength);
            var dataCube = await GetDataCubeCachedAsync(query.TableReference, tableMetaCopy);
            return ArchiveCube.FromDataCube(dataCube);
        }
    }
}

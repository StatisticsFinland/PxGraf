using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface.Caching
{
    public interface IPxWebApiResponseCache
    {
        void CacheMeta(PxFileReference pxFileRef, Task<IReadOnlyCubeMeta> meta);
        bool TryGetMeta(PxFileReference pxFileRef, out Task<IReadOnlyCubeMeta> meta);
        void RemoveMeta(PxFileReference pxFileRef);

        void CacheData(IReadOnlyCubeMeta meta, Task<DataCube> data);
        bool TryGetData(IReadOnlyCubeMeta meta, out Task<DataCube> data);
        void RemoveData(IReadOnlyCubeMeta meta);

        void CacheDataBases(string lang, Task<List<DataBaseListResponseItem>> dataBases);
        bool TryGetDataBases(string lang, out Task<List<DataBaseListResponseItem>> dataBases);
        void RemoveDataBases(string lang);

        bool TryGetTableItems(string lang, IReadOnlyList<string> path, out Task<List<TableListResponseItem>> tableListItems);
        void CacheTableItems(string lang, IReadOnlyList<string> path, Task<List<TableListResponseItem>> tableListItems);
        void RemoveTableItems(string lang, IReadOnlyList<string> path);

        void UpdateMetaCacheFreshness(PxFileReference pxFileRef);
        bool CheckMetaCacheFreshness(PxFileReference pxFileRef);
    }
}
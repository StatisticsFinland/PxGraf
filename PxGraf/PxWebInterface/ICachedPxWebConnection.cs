using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface.SerializationModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface
{
    public interface ICachedPxWebConnection
    {
        Task<ArchiveCube> BuildArchiveCubeCachedAsync(CubeQuery query);
        Task<DataCube> BuildDataCubeCachedAsync(CubeQuery query);
        Task<IReadOnlyCubeMeta> GetCubeMetaCachedAsync(PxFileReference pxFileReference);
        Task<List<DataBaseListResponseItem>> GetDataBaseListingAsync(string lang);
        Task<DataCube> GetDataCubeCachedAsync(PxFileReference pxFile, IReadOnlyCubeMeta meta);
        Task<List<TableListResponseItem>> GetDataTableItemListingAsync(string lang, IReadOnlyList<string> path);
    }
}
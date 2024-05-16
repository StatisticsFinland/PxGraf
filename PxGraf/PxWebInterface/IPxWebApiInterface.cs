using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.PxWebInterface
{
    public interface IPxWebApiInterface
    {
        public Task<IReadOnlyCubeMeta> GetPxTableMetaAsync(PxFileReference tableReference, IEnumerable<string> languages = null);
        public Task<DataCube> GetPxTableDataAsync(PxFileReference tableReference, IReadOnlyCubeMeta table);

        /// <summary>
        /// Fetch a list of databases available in the given language from the PxWeb server.
        /// </summary>
        public Task<List<DataBaseListResponseItem>> GetDataBaseListingAsync(string lang);

        /// <summary>
        /// Fetch a list of directories in a database in the given language from the PxWeb server.
        /// </summary>
        public Task<List<TableListResponseItem>> GetTableItemListingAsync(string lang, IReadOnlyList<string> path);

        /// <summary>
        /// Fetches the last update time for a px file. Meta object is required for building the query object without additional requests to PxWeb.
        /// </summary>
        public Task<string> GetPxFileUpdateTimeAsync(PxFileReference pxFile, IReadOnlyCubeMeta referenceMeta);

    }
}

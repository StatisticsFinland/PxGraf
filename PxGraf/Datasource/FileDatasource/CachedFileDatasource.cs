using Microsoft.AspNetCore.Routing.Matching;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Datasource.DatabaseConnection
{
    public sealed class CachedFileDatasource(IFileDatasource datasource, IMultiStateMemoryTaskCache taskCache) : CachedDatasource(taskCache) 
    {
        private readonly IFileDatasource _datasource = datasource;

        protected override async Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> hierarcy)
        {
            List<DatabaseGroupHeader> headers = await _datasource.GetGroupHeadersAsync(hierarcy);
            List<PxTableReference> tableReferences = await _datasource.GetTablesAsync(hierarcy);
            IEnumerable<Task<DatabaseTable>> tableTasks = tableReferences.Select(GetTableListingItemAsync);
            List<DatabaseTable> tables = [];
            foreach (Task<DatabaseTable> tableTask in tableTasks) tables.Add(await tableTask);
            return new DatabaseGroupContents(headers, tables);
        }

        public override async Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata)
        {
            IReadOnlyMatrixMetadata completeTableMeta = await GetMatrixMetadataCachedAsync(tableReference);
            return await _datasource.GetMatrixAsync(tableReference, metadata, completeTableMeta); 
        }

        private async Task<DatabaseTable> GetTableListingItemAsync(PxTableReference reference)
        {
            IReadOnlyMatrixMetadata meta = await GetMatrixMetadataCachedAsync(reference);
            string tableId;
            if (meta.AdditionalProperties.TryGetValue("TABLEID", out MetaProperty tableIdProperty)) // TODO: Constant for keyword somewhere
            {
                tableId = tableIdProperty.ValueAsString('\"');
            }
            else
            {
                tableId = reference.Name.Split(Path.DirectorySeparatorChar).Last().Split('.').First();
            }
            List<string> languages = [.. meta.AvailableLanguages];
            MultilanguageString name = meta.AdditionalProperties["DESCRIPTION"].ValueAsMultilanguageString('"', languages[0]); // TODO: same
            DateTime lastUpdated = meta.GetLastUpdated() ?? await _datasource.GetLastWriteTimeAsync(reference);
            return new(tableId, name, lastUpdated, languages);
        }

        protected override async Task<MetaCacheHousing> GenerateNewMetaCacheHousingAsync(PxTableReference tableReference)
        {
            DateTime lastWritetime = await _datasource.GetLastWriteTimeAsync(tableReference);
            IReadOnlyMatrixMetadata meta = await _datasource.GetMatrixMetadataAsync(tableReference);
            return new MetaCacheHousing(lastWritetime, meta);
        }

        protected override async Task<DataCacheHousing> GenerateNewDataCacheHousingAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta)
        {
            DateTime lastWritetime = await _datasource.GetLastWriteTimeAsync(tableReference);
            Matrix<DecimalDataValue> matrix = await GetMatrixAsync(tableReference, meta);
            return new DataCacheHousing(lastWritetime, matrix.Data);
        }

        protected override async Task<DateTime> GetLastUpdateTimeAsync(PxTableReference tableReference)
        {
            return await _datasource.GetLastWriteTimeAsync(tableReference);
        }
    }
}

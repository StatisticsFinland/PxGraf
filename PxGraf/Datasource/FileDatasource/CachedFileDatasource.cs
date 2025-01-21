#nullable enable
using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Datasource.FileDatasource
{
    public sealed class CachedFileDatasource(IFileDatasource datasource, IMultiStateMemoryTaskCache taskCache, ILogger<CachedFileDatasource> logger) : CachedDatasource(taskCache)
    {
        private readonly IFileDatasource _datasource = datasource;
        private readonly ILogger<CachedFileDatasource> _logger = logger;

        protected override async Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> hierarcy)
        {
            List<DatabaseGroupHeader> headers = await _datasource.GetGroupHeadersAsync(hierarcy);
            List<PxTableReference> tableReferences = await _datasource.GetTablesAsync(hierarcy);
            IEnumerable<Task<DatabaseTable>> tableTasks = tableReferences.Select(GetTableListingItemAsync);
            List<DatabaseTable> tables = [];
            foreach (Task<DatabaseTable> tableTask in tableTasks)
            {
                DatabaseTable table = await tableTask;
                if (table != null)
                {
                    tables.Add(table);
                }
            }
            return new DatabaseGroupContents(headers, tables);
        }

        public override async Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata)
        {
            IReadOnlyMatrixMetadata completeTableMeta = await GetMatrixMetadataCachedAsync(tableReference);
            return await _datasource.GetMatrixAsync(tableReference, metadata, completeTableMeta);
        }

        private async Task<DatabaseTable> GetTableListingItemAsync(PxTableReference reference)
        {
            try
            {
                IReadOnlyMatrixMetadata meta = await GetMatrixMetadataCachedAsync(reference);
                DateTime? lastUpdated = meta.GetLastUpdated();

                List<string> languages = [.. meta.AvailableLanguages];
                MultilanguageString name = new(languages.ToDictionary(lang => lang, lang => reference.Name));
                if (meta.AdditionalProperties.TryGetValue(PxSyntaxConstants.DESCRIPTION_KEY, out MetaProperty? descriptionProperty))
                {
                    if (descriptionProperty is StringProperty sProp) name = new(languages[0], sProp.Value);
                    else if (descriptionProperty is MultilanguageStringProperty mlsProp) name = mlsProp.Value;
                }

                if (lastUpdated is not null) return new(reference.Name, name, (DateTime)lastUpdated, languages);
                else return DatabaseTable.FromError(reference.Name, name, languages);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get table listing item for {Reference}", reference);
                MultilanguageString name = new(Configuration.Current.LanguageOptions.Available.ToDictionary(lang => lang, lang => reference.Name));
                return DatabaseTable.FromError(reference.Name, name, []);
            }
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
#nullable disable

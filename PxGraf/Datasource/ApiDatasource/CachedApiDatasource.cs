using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PxGraf.Datasource.PxWebInterface
{
    public sealed class CachedApiDatasource(IApiDatasource datasource, IMultiStateMemoryTaskCache taskCache) : CachedDatasource(taskCache)
    {
        private readonly IApiDatasource _datasource = datasource;

        public override async Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata)
        {
            return await _datasource.GetMatrixAsync(tableReference, metadata);
        }

        protected override async Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> hierarcy)
        {
            return await _datasource.GetDatabaseItemGroup(hierarcy);
        }

        protected override async Task<MetaCacheHousing> GenerateNewMetaCacheHousingAsync(PxTableReference tableReference)
        {
            IReadOnlyMatrixMetadata meta = await _datasource.GetMatrixMetadataAsync(tableReference);
            return new MetaCacheHousing(meta.GetLastUpdated(), meta);
        }

        protected override async Task<DataCacheHousing> GenerateNewDataCacheHousingAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta)
        {
            Matrix<DecimalDataValue> matrix = await GetMatrixAsync(tableReference, meta);
            return new DataCacheHousing(meta.GetLastUpdated(), matrix.Data);
        }
        
        protected override Task<DateTime> GetLastUpdateTimeAsync(PxTableReference tableReference)
        {
            return _datasource.GetLastWriteTimeAsync(tableReference);
        }
    }
}

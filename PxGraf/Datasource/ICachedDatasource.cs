using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.Datasource
{
    public interface ICachedDatasource
    {
        public Task<DatabaseGroupContents> GetGroupContentsCachedAsync(IReadOnlyList<string> hierarcy);

        public Task<IReadOnlyMatrixMetadata> GetMatrixMetadataCachedAsync(PxTableReference tableReference);
        
        public Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata);

        public Task<Matrix<DecimalDataValue>> GetMatrixCachedAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata);
    }
}

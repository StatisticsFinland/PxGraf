using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.Datasource
{
    /// <summary>
    /// Interface for a datasource that caches data about px files.
    /// </summary>
    public interface ICachedDatasource
    {
        /// <summary>
        /// Gets the contents of a database group based on a given hierarchy.
        /// </summary>
        /// <param name="hierarcy">Hierarchy of the group.</param>
        /// <returns>Contents of the group as <see cref="DatabaseGroupContents"/> object.</returns>
        public Task<DatabaseGroupContents> GetGroupContentsCachedAsync(IReadOnlyList<string> hierarcy);

        /// <summary>
        /// Gets the metadata of a table based on a given reference.
        /// </summary>
        /// <param name="tableReference">Reference to the table.</param>
        /// <returns>Metadata of the table as <see cref="IReadOnlyMatrixMetadata"/> object.</returns>
        public Task<IReadOnlyMatrixMetadata> GetMatrixMetadataCachedAsync(PxTableReference tableReference);
        
        /// <summary>
        /// Gets the data matrix of a table based on a given reference and metadata.
        /// </summary>
        /// <param name="tableReference">Reference to the table.</param>
        /// <param name="metadata">Metadata of the table.</param>
        /// <returns>Data matrix of the table as <see cref="Matrix{DecimalDataValue}"/> object.</returns>
        public Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata);

        /// <summary>
        /// Gets the data matrix of a table based on a given reference and metadata.
        /// </summary>
        /// <param name="tableReference">Reference to the table.</param>
        /// <param name="metadata">Metadata of the table.</param>
        /// <returns>Data matrix of the table as <see cref="Matrix{DecimalDataValue}"/> object.</returns>
        public Task<Matrix<DecimalDataValue>> GetMatrixCachedAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata metadata);
    }
}

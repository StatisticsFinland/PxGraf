using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace PxGraf.Datasource.PxWebInterface
{
    /// <summary>
    /// Interface for accessing data from the PxWeb API.
    /// </summary>
    public interface IApiDatasource
    {
        /// <summary>
        /// Asynchronously gets the contents of a database group.
        /// </summary>
        /// <param name="groupHierarcy">Path to the group.</param>
        /// <returns><see cref="DatabaseGroupContents"/> object containing the information of the group.</returns>
        public Task<DatabaseGroupContents> GetDatabaseItemGroup(IReadOnlyList<string> groupHierarcy);

        /// <summary>
        /// Asynchronously gets the last time a table has been changed.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The last time the table has been changed.</returns>
        public Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference);

        /// <summary>
        /// Asynchronously gets the metadata of a specified file.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The complete metadata from a file.</returns>
        public Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference);

        /// <summary>
        /// Asynchronously gets the data matching the provided metadata object and builds a <see cref="Matrix{T}"/> from the result.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <param name="meta">Gets the data defined by this metadata object and uses this to build the result <see cref="Matrix{T}"/>.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A <see cref="Matrix{T}"/> build from the privided metadata and the related data.</returns>
        public Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, CancellationToken? cancellationToken = null);
    }
}

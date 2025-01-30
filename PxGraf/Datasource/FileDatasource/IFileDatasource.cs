using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PxGraf.Datasource.DatabaseConnection
{
    public interface IFileDatasource
    {
        /// <summary>
        /// Asynchronously lists all of the tables from the specified level of the database.
        /// </summary>
        /// <param name="groupHierarchy">Defines the database level where to list the tables from.</param>
        /// <returns>The tables from a level of a database.</returns>
        public Task<List<PxTableReference>> GetTablesAsync(IReadOnlyList<string> groupHierarchy);

        /// <summary>
        /// Asynchronously lists the group header items from the specified level of the database.
        /// </summary>
        /// <param name="groupHierarchy">Defines the database level where to list the items from.</param>
        /// <returns>The subgroups from a level of a database.</returns>
        public Task<List<DatabaseGroupHeader>> GetGroupHeadersAsync(IReadOnlyList<string> groupHierarchy);

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
        /// <param name="completeTableMap">A complete map of the table metadata, file based implementations require this for reading.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A <see cref="Matrix{T}"/> build from the privided metadata and the related data.</returns>
        public Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, IMatrixMap completeTableMap, CancellationToken? cancellationToken = null);
   }
}

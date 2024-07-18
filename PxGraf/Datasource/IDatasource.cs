using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using PxGraf.Datasource.DatabaseConnection.DatabaseItems;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PxGraf.Datasource
{
    public interface IDatasource
    {
        /// <summary>
        /// Lists the items from the specified level of the database.
        /// </summary>
        /// <param name="groupHierarcy">Defines the database level where to list the items from.</param>
        /// <returns>The subgroups and tables from a level of a database.</returns>
        public DatabaseGroupContents GetGroupContents(IReadOnlyList<string> groupHierarcy);

        /// <summary>
        /// Asynchronously lists the items from the specified level of the database.
        /// </summary>
        /// <param name="groupHierarcy">Defines the database level where to list the items from.</param>
        /// <returns>The subgroups and tables from a level of a database.</returns>
        public Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> groupHierarcy);

        /// <summary>
        /// Gets the last time a table has been changed. 
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The last time a table has been changed.</returns>
        public DateTime GetLastWriteTime(PxTableReference tableReference);

        /// <summary>
        /// Asynchronously gets the last time a table has been changed.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The last time the table has been changed.</returns>
        public Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference);

        /// <summary>
        /// Gets the metadata of a specified file.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The complete metadata from a file.</returns>
        public IReadOnlyMatrixMetadata GetMatrixMetadata(PxTableReference tableReference);

        /// <summary>
        /// Asynchronously gets the metadata of a specified file.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The complete metadata from a file.</returns>
        public Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference);
        
        /// <summary>
        /// Gets the data matching the provided metadata object and builds a <see cref="Matrix{T}"/> from the result.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <param name="meta">Gets the data defined by this metadata object and uses this to build the result <see cref="Matrix{T}"/>.</param>
        /// <param name="completeMap">A map that difines the the data of the whole source file.</param>
        /// <returns>A <see cref="Matrix{T}"/> build from the privided metadata and the related data.</returns>
        public Matrix<DecimalDataValue> GetMatrix(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, IMatrixMap completeMap);

        /// <summary>
        /// Asynchronously gets the data matching the provided metadata object and builds a <see cref="Matrix{T}"/> from the result.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <param name="meta">Gets the data defined by this metadata object and uses this to build the result <see cref="Matrix{T}"/>.</param>
        /// <param name="completeMap">A map that difines the the data of the whole source file.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A <see cref="Matrix{T}"/> build from the privided metadata and the related data.</returns>
        public Task<Matrix<DecimalDataValue>> GetMatrixAsync(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, IMatrixMap completeMap, CancellationToken? cancellationToken = null);
    }
}

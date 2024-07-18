using Px.Utils.Models.Metadata;
using PxGraf.Models.Queries;
using System;

namespace PxGraf.Datasource.DatabaseConnection
{
    public interface ILocalFilesystemDatabaseConnection
    {
        DateTime CheckLastWritetime(PxTableReference fileReference);

        IReadOnlyMatrixMetadata ReadMatrixMetadata(PxTableReference fileReference);
    }
}
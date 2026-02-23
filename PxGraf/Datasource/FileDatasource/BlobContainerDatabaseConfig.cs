using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Datasource.FileDatasource
{
    /// <summary>
    /// Configuration for Azure Blob Storage datasource.
    /// </summary>
    /// <param name="storageAccountName">Name of the Azure Storage Account.</param>
    /// <param name="containerName">Name of the blob container containing the Px files.</param>
    /// <param name="rootPath">Optional root path within the container for Px files. Useful when the same container stores multiple types of files.</param>
    [ExcludeFromCodeCoverage]
    public class BlobContainerDatabaseConfig(string storageAccountName, string containerName, string rootPath = "") : DatabaseConfig
    {
        /// <summary>
        /// Name of the Azure Storage Account.
        /// </summary>
        public string StorageAccountName { get; } = storageAccountName;
        
        /// <summary>
        /// Name of the blob container containing the Px files.
        /// </summary>
        public string ContainerName { get; } = containerName;
        
        /// <summary>
        /// Optional root path within the container for Px files. 
        /// Useful when the same container stores multiple types of files (e.g., "database/" for Px files, "saved-queries/" for query files).
        /// </summary>
        public string RootPath { get; } = rootPath ?? "";
    }
}
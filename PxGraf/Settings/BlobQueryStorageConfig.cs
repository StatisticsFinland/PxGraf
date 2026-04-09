#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for Azure Blob Storage of saved queries and archive files.
    /// </summary>
    /// <param name="storageAccountName">Name of the Azure Storage Account.</param>
    /// <param name="containerName">Name of the blob container.</param>
    /// <param name="savedQueryPath">Optional path within container for saved query files.</param>
    /// <param name="archiveFilePath">Optional path within container for archive files.</param>
    /// <param name="managedIdentityClientId">Optional Client ID of a User-Assigned Managed Identity for authentication.</param>
    [ExcludeFromCodeCoverage]
    public class BlobQueryStorageConfig(string storageAccountName, string containerName, string? savedQueryPath, string? archiveFilePath, string? managedIdentityClientId = null) : IQueryStorageConfig
    {
        /// <summary>
        /// Name of the Azure Storage Account.
        /// </summary>
        public string StorageAccountName { get; } = storageAccountName;

        /// <summary>
        /// Name of the blob container.
        /// </summary>
        public string ContainerName { get; } = containerName;

        /// <inheritdoc/>
        public string SavedQueryPath { get; } = savedQueryPath ?? "";

        /// <inheritdoc/>
        public string ArchiveFilePath { get; } = archiveFilePath ?? "";

        /// <summary>
        /// Optional Client ID of a User-Assigned Managed Identity.
        /// When set, DefaultAzureCredential will target this specific identity instead of using the default credential chain.
        /// </summary>
        public string? ManagedIdentityClientId { get; } = managedIdentityClientId;
    }
}
#nullable restore
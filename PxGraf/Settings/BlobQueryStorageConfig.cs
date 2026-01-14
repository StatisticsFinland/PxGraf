#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for Azure Blob Storage of saved queries and archive files.
    /// </summary>
    /// <param name="enabled">Whether blob storage is enabled.</param>
    /// <param name="storageAccountName">Name of the Azure Storage Account.</param>
    /// <param name="containerName">Name of the blob container.</param>
    /// <param name="savedQueryPath">Optional path within container for saved query files.</param>
    /// <param name="archiveFilePath">Optional path within container for archive files.</param>
    [ExcludeFromCodeCoverage]
    public class BlobQueryStorageConfig(bool enabled, string storageAccountName, string containerName, string? savedQueryPath, string? archiveFilePath)
    {
        /// <summary>
        /// Whether blob storage is enabled.
        /// </summary>
        public bool Enabled { get; } = enabled;

        /// <summary>
        /// Name of the Azure Storage Account.
        /// </summary>
        public string StorageAccountName { get; } = storageAccountName;

        /// <summary>
        /// Name of the blob container.
        /// </summary>
        public string ContainerName { get; } = containerName;

        /// <summary>
        /// Optional path within container for saved query files.
        /// </summary>
        public string? SavedQueryPath { get; } = savedQueryPath;

        /// <summary>
        /// Optional path within container for archive files.
        /// </summary>
        public string? ArchiveFilePath { get; } = archiveFilePath;
    }
}
#nullable restore
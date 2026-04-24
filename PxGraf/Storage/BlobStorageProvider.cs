using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ude;

namespace PxGraf.Storage
{
    /// <summary>
    /// Azure Blob Storage implementation of IStorageProvider.
    /// </summary>
    /// <param name="storageAccountName">Name of the Azure Storage Account.</param>
    /// <param name="containerName">Name of the blob container.</param>
    /// <param name="managedIdentityClientId">Optional Client ID of a User-Assigned Managed Identity. When null, DefaultAzureCredential uses its default credential chain.</param>
    [ExcludeFromCodeCoverage(Justification = "Methods consist mostly of Azure SDK calls")]
    public class BlobStorageProvider(string storageAccountName, string containerName, string managedIdentityClientId = null) : IStorageProvider
    {
        private readonly BlobContainerClient containerClient = new(
            new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}"),
            new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = managedIdentityClientId
            })
        );

        /// <inheritdoc/>
        public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string blobName = PathNormalizer.NormalizeToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            try
            {
                return await blobClient.ExistsAsync(cancellationToken);
            }
            catch(FileNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string blobName = PathNormalizer.NormalizeToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using Stream blobStream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

            // Detect encoding
            Encoding encoding = Encoding.UTF8; // Default fallback
            CharsetDetector cdet = new();
            byte[] buffer = new byte[1024];
            int bytesRead = await blobStream.ReadAsync(buffer.AsMemory(), cancellationToken);
            cdet.Feed(buffer, 0, bytesRead);
            cdet.DataEnd();

            if (cdet.Charset != null)
            {
                encoding = Encoding.GetEncoding(cdet.Charset);
            }

            blobStream.Position = 0;
            using StreamReader sr = new(blobStream, encoding);
            return await sr.ReadToEndAsync(cancellationToken);
        }
        
        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        {
            string blobName = PathNormalizer.NormalizeToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(BinaryData.FromString(content), overwrite: true, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string blobName = PathNormalizer.NormalizeToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string fileExtension, CancellationToken cancellationToken = default)
        {
            List<string> files = [];
            string prefix = PathNormalizer.NormalizeToAzurePath(directoryPath) ?? "";
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            // Normalize file extension using shared utility
            string normalizedExtension = PathNormalizer.NormalizeFileExtension(fileExtension).ToLowerInvariant();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken))
            {
                // Check if the blob is directly in this level (not in a subdirectory)
                string relativePath = blobItem.Name[prefix.Length..];
                if (!relativePath.Contains('/') && (string.IsNullOrEmpty(normalizedExtension) ||
                        blobItem.Name.EndsWith(normalizedExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    files.Add(blobItem.Name);
                }
            }

            return files;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath, CancellationToken cancellationToken = default)
        {
            HashSet<string> directories = [];
            string prefix = PathNormalizer.NormalizeToAzurePath(directoryPath) ?? "";
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken))
            {
                string relativePath = blobItem.Name[prefix.Length..];
                int slashIndex = relativePath.IndexOf('/');
                if (slashIndex > 0)
                {
                    string directoryName = relativePath[..slashIndex];
                    string fullDirectoryPath = prefix + directoryName;
                    directories.Add(fullDirectoryPath);
                }
            }

            return directories;
        }

        /// <inheritdoc/>
        public async Task<bool> ProbeDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
        {
            string prefix = PathNormalizer.NormalizeToAzurePath(directoryPath) ?? "";
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            // Fetch the first page to verify that the container and prefix are accessible.
            // The result is discarded — any failure will surface as an exception.
            await using IAsyncEnumerator<BlobItem> enumerator = containerClient
                .GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken)
                .GetAsyncEnumerator(cancellationToken);
            await enumerator.MoveNextAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<DateTime> GetLastWriteTimeAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string blobName = PathNormalizer.NormalizeToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobProperties properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return properties.LastModified.DateTime;
        }

        /// <inheritdoc/>
        public string GetDirectoryName(string directoryPath)
        {
            string azurePath = PathNormalizer.NormalizeToAzurePath(directoryPath);
            int lastSlashIndex = azurePath.LastIndexOf('/');
            return lastSlashIndex >= 0 ? azurePath[(lastSlashIndex + 1)..] : azurePath;
        }

        /// <inheritdoc/>
        public string GetFileName(string filePath)
        {
            string azurePath = PathNormalizer.NormalizeToAzurePath(filePath);
            return Path.GetFileName(azurePath);
        }

        /// <inheritdoc/>
        public string BuildPath(string rootPath, string userPath)
        {
            // Convert both to Azure path format
            string azureRootPath = PathNormalizer.NormalizeToAzurePath(rootPath) ?? "";
            string azureUserPath = PathNormalizer.NormalizeToAzurePath(userPath) ?? "";

            // Combine paths using shared utility
            string combinedPath = PathNormalizer.CombineAndNormalizeAzurePaths(azureRootPath, azureUserPath);

            // Security check: ensure the combined path doesn't try to escape the root using ".."
            PathNormalizer.ValidatePathSecurity(combinedPath, azureRootPath);

            return combinedPath;
        }

        /// <inheritdoc/>
        public string GetRelativePath(string basePath, string targetPath)
        {
            string azureBasePath = PathNormalizer.NormalizeToAzurePath(basePath);
            string azureTargetPath = PathNormalizer.NormalizeToAzurePath(targetPath);

            // Handle empty base path case
            if (string.IsNullOrEmpty(azureBasePath))
            {
                return azureTargetPath;
            }

            // Ensure we're checking for a path boundary by appending a trailing slash to the base path
            if (!azureBasePath.EndsWith('/'))
            {
                azureBasePath += "/";
            }

            if (azureTargetPath.StartsWith(azureBasePath, StringComparison.OrdinalIgnoreCase))
            {
                return azureTargetPath[azureBasePath.Length..];
            }

            // If target path equals base path exactly (shouldn't happen for files, but handle it)
            if (azureTargetPath.Equals(azureBasePath, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return azureTargetPath;
        }

        public string CombinePath(params string[] paths) => PathNormalizer.CombineAndNormalizeAzurePaths(paths);
    }
}
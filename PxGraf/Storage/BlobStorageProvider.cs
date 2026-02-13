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
    [ExcludeFromCodeCoverage(Justification = "Methods consist mostly of Azure SDK calls")]
    public class BlobStorageProvider(string storageAccountName, string containerName) : IStorageProvider
    {
        private readonly BlobContainerClient containerClient = new(
            new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}"),
            new DefaultAzureCredential(new DefaultAzureCredentialOptions()
            {
                // TODO: Remove after testing. Exclude everything but Visual Studio
                ExcludeAzureCliCredential = true,
                ExcludeAzureDeveloperCliCredential = true,
                ExcludeEnvironmentCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeManagedIdentityCredential = true,
                ExcludeVisualStudioCredential = false,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeWorkloadIdentityCredential = true
            })
        );

        /// <inheritdoc/>
        public async Task<bool> FileExistsAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            try
            {
                return await blobClient.ExistsAsync();
            }
            catch(FileNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using Stream blobStream = await blobClient.OpenReadAsync();

            // Detect encoding
            Encoding encoding = Encoding.UTF8; // Default fallback
            CharsetDetector cdet = new();
            byte[] buffer = new byte[1024];
            int bytesRead = await blobStream.ReadAsync(buffer.AsMemory(), CancellationToken.None);
            cdet.Feed(buffer, 0, bytesRead);
            cdet.DataEnd();

            if (cdet.Charset != null)
            {
                encoding = Encoding.GetEncoding(cdet.Charset);
            }

            blobStream.Position = 0;
            using StreamReader sr = new(blobStream, encoding);
            return await sr.ReadToEndAsync();
        }        
        
        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string filePath, string content)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)), overwrite: true);
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenReadAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.OpenReadAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string fileExtension)
        {
            List<string> files = [];
            string prefix = ConvertToAzurePath(directoryPath) ?? "";
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            // Normalize file extension using shared utility
            string normalizedExtension = PathNormalizer.NormalizeFileExtension(fileExtension).ToLowerInvariant();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
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
        public async Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath)
        {
            HashSet<string> directories = [];
            string prefix = ConvertToAzurePath(directoryPath) ?? "";
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
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
        public async Task<DateTime> GetLastWriteTimeAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            return properties.LastModified.DateTime;
        }

        /// <inheritdoc/>
        public string GetDirectoryName(string directoryPath)
        {
            string azurePath = ConvertToAzurePath(directoryPath);
            int lastSlashIndex = azurePath.LastIndexOf('/');
            return lastSlashIndex >= 0 ? azurePath[(lastSlashIndex + 1)..] : azurePath;
        }

        /// <inheritdoc/>
        public string GetFileName(string filePath)
        {
            string azurePath = ConvertToAzurePath(filePath);
            return Path.GetFileName(azurePath);
        }

        /// <inheritdoc/>
        public string CombinePath(params string[] paths)
        {
            return string.Join("/", paths);
        }

        /// <inheritdoc/>
        public string BuildPath(string rootPath, string userPath)
        {
            // Convert both to Azure path format
            string azureRootPath = ConvertToAzurePath(rootPath) ?? "";
            string azureUserPath = ConvertToAzurePath(userPath) ?? "";

            // Remove any leading slashes from user path to avoid double slashes
            azureUserPath = azureUserPath.TrimStart('/');

            // Combine paths using shared utility
            string combinedPath = PathNormalizer.CombinePaths(azureRootPath, azureUserPath);

            // Security check: ensure the combined path doesn't try to escape the root using ".."
            int rootDepth = PathNormalizer.GetPathDepth(azureRootPath);
            PathNormalizer.ValidatePathSecurity(combinedPath, rootDepth);

            // Normalize the path by removing "." and resolving ".."
            return PathNormalizer.NormalizePath(combinedPath);
        }

        /// <inheritdoc/>
        public string GetRelativePath(string basePath, string targetPath)
        {
            string azureBasePath = ConvertToAzurePath(basePath);
            string azureTargetPath = ConvertToAzurePath(targetPath);

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

        /// <summary>
        /// Converts Windows-style paths to Azure blob paths.
        /// </summary>
        /// <param name="path">Path to convert.</param>
        /// <returns>Azure blob path.</returns>
        private static string ConvertToAzurePath(string path)
        {
            return path?.Replace('\\', '/') ?? string.Empty;
        }
    }
}
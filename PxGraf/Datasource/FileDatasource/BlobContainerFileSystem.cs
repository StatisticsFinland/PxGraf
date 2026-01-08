using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ude;

namespace PxGraf.Datasource.FileDatasource
{
    /// <summary>
    /// Azure Blob Storage implementation of IFileSystem.
    /// </summary>
    /// <param name="storageAccountName">Name of the Azure Storage Account.</param>
    /// <param name="containerName">Name of the blob container.</param>
    [ExcludeFromCodeCoverage] // Methods consist mostly of Azure SDK calls
    public class BlobContainerFileSystem(string storageAccountName, string containerName) : IFileSystem
    {
        private readonly BlobContainerClient containerClient = new(
            new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}"),
            new DefaultAzureCredential()
        );

        /// <summary>
        /// Asynchronously enumerates files in a directory with the specified pattern.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <param name="searchPattern">File search pattern.</param>
        /// <returns>Enumerable of file paths.</returns>
        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string searchPattern)
        {
            List<string> files = [];
            string prefix = ConvertToAzurePath(directoryPath);
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            // Extract file extension from search pattern (e.g., "*.px" -> ".px")
            string fileExtension = searchPattern.Replace("*", "").ToLowerInvariant();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                if (blobItem.Name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if the blob is directly in this level (not in a subdirectory)
                    string relativePath = blobItem.Name.Substring(prefix.Length);
                    if (!relativePath.Contains('/'))
                    {
                        files.Add(blobItem.Name);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Asynchronously enumerates subdirectories in a directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Enumerable of directory paths.</returns>
        public async Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath)
        {
            HashSet<string> directories = [];
            string prefix = ConvertToAzurePath(directoryPath);
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += "/";
            }

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                string relativePath = blobItem.Name.Substring(prefix.Length);
                int slashIndex = relativePath.IndexOf('/');
                if (slashIndex > 0)
                {
                    string directoryName = relativePath.Substring(0, slashIndex);
                    string fullDirectoryPath = prefix + directoryName;
                    directories.Add(fullDirectoryPath);
                }
            }

            return directories;
        }

        /// <summary>
        /// Asynchronously gets the last write time of a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Last write time of the file.</returns>
        public async Task<DateTime> GetLastWriteTimeAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            return properties.LastModified.DateTime;
        }

        /// <summary>
        /// Asynchronously opens a file for reading.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Stream for reading the file.</returns>
        public async Task<Stream> OpenReadAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.OpenReadAsync();
        }

        /// <summary>
        /// Asynchronously reads all text from a file with automatic encoding detection.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Text content of the file.</returns>
        public async Task<string> ReadAllTextAsync(string filePath)
        {
            string blobName = ConvertToAzurePath(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using Stream blobStream = await blobClient.OpenReadAsync();

            // Detect encoding
            Encoding encoding = Encoding.UTF8; // Default fallback
            CharsetDetector cdet = new();
            byte[] buffer = new byte[1024];
            int bytesRead = await blobStream.ReadAsync(buffer, 0, buffer.Length);
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

        /// <summary>
        /// Gets the name of a directory from its path.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Name of the directory.</returns>
        public string GetDirectoryName(string directoryPath)
        {
            string azurePath = ConvertToAzurePath(directoryPath);
            int lastSlashIndex = azurePath.LastIndexOf('/');
            return lastSlashIndex >= 0 ? azurePath.Substring(lastSlashIndex + 1) : azurePath;
        }

        /// <summary>
        /// Gets the file name from a file path.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Name of the file.</returns>
        public string GetFileName(string filePath)
        {
            string azurePath = ConvertToAzurePath(filePath);
            return Path.GetFileName(azurePath);
        }

        /// <summary>
        /// Combines path segments into a single path.
        /// </summary>
        /// <param name="paths">Path segments to combine.</param>
        /// <returns>Combined path.</returns>
        public string CombinePath(params string[] paths)
        {
            return string.Join("/", paths);
        }

        /// <summary>
        /// Gets the relative path from a base path to a target path.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        /// <param name="targetPath">Target path.</param>
        /// <returns>Relative path.</returns>
        public string GetRelativePath(string basePath, string targetPath)
        {
            string azureBasePath = ConvertToAzurePath(basePath);
            string azureTargetPath = ConvertToAzurePath(targetPath);

            if (azureTargetPath.StartsWith(azureBasePath, StringComparison.OrdinalIgnoreCase))
            {
                return azureTargetPath.Substring(azureBasePath.Length).TrimStart('/');
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
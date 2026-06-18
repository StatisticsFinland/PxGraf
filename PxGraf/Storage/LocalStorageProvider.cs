#nullable enable
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
    /// Local file system implementation of IStorageProvider.
    /// </summary>
    /// <param name="encoding">Text encoding to use for text file operations.</param>
    [ExcludeFromCodeCoverage(Justification = "Methods consist mostly of filesystem IO")]
    public class LocalStorageProvider(Encoding? encoding = null) : IStorageProvider
    {
        private readonly Encoding encoding = encoding ?? Encoding.UTF8;

        /// <inheritdoc/>
        public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return Task.FromResult(File.Exists(filePath));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.Asynchronous);
            Encoding detectedEncoding = encoding;
            CharsetDetector cdet = new();
            byte[] buffer = new byte[1024];
            int bytesRead = await fs.ReadAsync(buffer.AsMemory(), cancellationToken);
            cdet.Feed(buffer, 0, bytesRead);
            cdet.DataEnd();
            if (cdet.Charset != null)
            {
                detectedEncoding = Encoding.GetEncoding(cdet.Charset);
            }
            fs.Position = 0;
            using StreamReader sr = new(fs, detectedEncoding);
            return await sr.ReadToEndAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            await File.WriteAllTextAsync(filePath, content, encoding, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Stream> OpenReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return Task.FromResult((Stream)File.OpenRead(filePath));
            }
            catch (Exception ex)
            {
                return Task.FromException<Stream>(ex);
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string fileExtension, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                string normalizedExtension = PathNormalizer.NormalizeFileExtension(fileExtension);

                // Convert extension to search pattern for Directory.EnumerateFiles
                string searchPattern = string.IsNullOrEmpty(normalizedExtension) ? "*" : $"*{normalizedExtension}";

                return Task.FromResult(Directory.EnumerateFiles(directoryPath, searchPattern));
            }
            catch (Exception ex)
            {
                return Task.FromException<IEnumerable<string>>(ex);
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return Task.FromResult(Directory.EnumerateDirectories(directoryPath));
            }
            catch (Exception ex)
            {
                return Task.FromException<IEnumerable<string>>(ex);
            }
        }

        /// <inheritdoc/>
        public Task<bool> ProbeDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return Task.FromResult(Directory.Exists(directoryPath));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        /// <inheritdoc/>
        public Task<DateTime> GetLastWriteTimeAsync(string filePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return Task.FromResult(File.GetLastWriteTime(filePath));
            }
            catch (Exception ex)
            {
                return Task.FromException<DateTime>(ex);
            }
        }

        /// <inheritdoc/>
        public string GetDirectoryName(string directoryPath)
        {
            return new DirectoryInfo(directoryPath).Name;
        }

        /// <inheritdoc/>
        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <inheritdoc/>
        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <inheritdoc/>
        public string BuildPath(string rootPath, string userPath)
        {
            if (!string.IsNullOrEmpty(rootPath))
            {
                // Get the full path of the root folder
                rootPath = Path.GetFullPath(rootPath);
                if (!rootPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    rootPath += Path.DirectorySeparatorChar;
                }
            }

            // Combine the root folder with the user input path
            string combinedPath = Path.Combine(rootPath, userPath);

            if (string.IsNullOrEmpty(combinedPath))
            {
                return string.Empty;
            }

            // Get the full path of the combined path
            string fullPath = Path.GetFullPath(combinedPath);

            // Check if the full path starts with the root folder's full path
            if (!fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access to the path is denied.");
            }

            return fullPath;
        }

        /// <inheritdoc/>
        public string GetRelativePath(string basePath, string targetPath)
        {
            string relativePath = Path.GetRelativePath(basePath, targetPath);
            // Normalize to forward slashes for cross-platform consistency
            return relativePath.Replace('\\', '/');
        }
    }
}
#nullable restore
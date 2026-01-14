#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
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
        public async Task<bool> FileExistsAsync(string filePath)
        {
            return await Task.FromResult(File.Exists(filePath));
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await Task.Factory.StartNew(() =>
            {
                using FileStream fs = File.OpenRead(filePath);
                Encoding detectedEncoding = encoding;
                CharsetDetector cdet = new();
                cdet.Feed(fs);
                cdet.DataEnd();
                if (cdet.Charset != null)
                {
                    detectedEncoding = Encoding.GetEncoding(cdet.Charset);
                }
                fs.Position = 0;
                using StreamReader sr = new(fs, detectedEncoding);
                return sr.ReadToEnd();
            });
        }

        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string filePath, string content)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            await File.WriteAllTextAsync(filePath, content, encoding);
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenReadAsync(string filePath)
        {
            return await Task.FromResult((Stream)File.OpenRead(filePath));
        }

        /// <inheritdoc/>
        public async Task<Stream> CreateAsync(string filePath)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return await Task.FromResult((Stream)File.Create(filePath));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string searchPattern)
        {
            return await Task.FromResult(Directory.EnumerateFiles(directoryPath, searchPattern));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath)
        {
            return await Task.FromResult(Directory.EnumerateDirectories(directoryPath));
        }

        /// <inheritdoc/>
        public async Task<DateTime> GetLastWriteTimeAsync(string filePath)
        {
            return await Task.FromResult(File.GetLastWriteTime(filePath));
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
        public string GetRelativePath(string basePath, string targetPath)
        {
            return Path.GetRelativePath(basePath, targetPath);
        }
    }
}
#nullable restore
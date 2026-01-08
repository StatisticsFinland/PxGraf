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
    /// Local file system implementation of IFileSystem.
    /// </summary>
    /// <param name="rootPath">Root path for the file system operations.</param>
    /// <param name="encoding">Text encoding to use for text file operations.</param>
    [ExcludeFromCodeCoverage] // Methods consist mostly of filesystem IO
    public class LocalFileSystem(string rootPath, Encoding encoding) : IFileSystem
    {
        private readonly string rootPath = rootPath;
        private readonly Encoding encoding = encoding;

        /// <summary>
        /// Asynchronously enumerates files in a directory with the specified pattern.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <param name="searchPattern">File search pattern.</param>
        /// <returns>Enumerable of file paths.</returns>
        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string searchPattern)
        {
            return await Task.Factory.StartNew(() => Directory.EnumerateFiles(directoryPath, searchPattern));
        }

        /// <summary>
        /// Asynchronously enumerates subdirectories in a directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Enumerable of directory paths.</returns>
        public async Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath)
        {
            return await Task.Factory.StartNew(() => Directory.EnumerateDirectories(directoryPath));
        }

        /// <summary>
        /// Asynchronously gets the last write time of a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Last write time of the file.</returns>
        public async Task<DateTime> GetLastWriteTimeAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => Directory.GetLastWriteTime(filePath));
        }

        /// <summary>
        /// Asynchronously opens a file for reading.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Stream for reading the file.</returns>
        public async Task<Stream> OpenReadAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => (Stream)File.OpenRead(filePath));
        }

        /// <summary>
        /// Asynchronously reads all text from a file with automatic encoding detection.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Text content of the file.</returns>
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

        /// <summary>
        /// Gets the name of a directory from its path.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Name of the directory.</returns>
        public string GetDirectoryName(string directoryPath)
        {
            return new DirectoryInfo(directoryPath).Name;
        }

        /// <summary>
        /// Gets the file name from a file path.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Name of the file.</returns>
        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// Combines path segments into a single path.
        /// </summary>
        /// <param name="paths">Path segments to combine.</param>
        /// <returns>Combined path.</returns>
        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// Gets the relative path from a base path to a target path.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        /// <param name="targetPath">Target path.</param>
        /// <returns>Relative path.</returns>
        public string GetRelativePath(string basePath, string targetPath)
        {
            return Path.GetRelativePath(basePath, targetPath);
        }
    }
}
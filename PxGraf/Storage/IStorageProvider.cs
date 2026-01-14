using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PxGraf.Storage
{
    /// <summary>
    /// Unified abstraction for file storage operations supporting different storage backends.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        Task<bool> FileExistsAsync(string filePath);

        /// <summary>
        /// Asynchronously reads all text from a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Text content of the file.</returns>
        Task<string> ReadAllTextAsync(string filePath);

        /// <summary>
        /// Asynchronously writes all text to a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="content">Content to write to the file.</param>
        Task WriteAllTextAsync(string filePath, string content);

        /// <summary>
        /// Asynchronously opens a file for reading.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Stream for reading the file.</returns>
        Task<Stream> OpenReadAsync(string filePath);

        /// <summary>
        /// Asynchronously creates or overwrites a file and returns a writable stream.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Stream for writing to the file.</returns>
        Task<Stream> CreateAsync(string filePath);

        /// <summary>
        /// Asynchronously enumerates files in a directory with the specified pattern.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <param name="searchPattern">File search pattern.</param>
        /// <returns>Enumerable of file paths.</returns>
        Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string searchPattern);

        /// <summary>
        /// Asynchronously enumerates subdirectories in a directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Enumerable of directory paths.</returns>
        Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath);

        /// <summary>
        /// Asynchronously gets the last write time of a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Last write time of the file.</returns>
        Task<DateTime> GetLastWriteTimeAsync(string filePath);

        /// <summary>
        /// Gets the name of a directory from its path.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <returns>Name of the directory.</returns>
        string GetDirectoryName(string directoryPath);

        /// <summary>
        /// Gets the file name from a file path.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Name of the file.</returns>
        string GetFileName(string filePath);

        /// <summary>
        /// Combines path segments into a single path.
        /// </summary>
        /// <param name="paths">Path segments to combine.</param>
        /// <returns>Combined path.</returns>
        string CombinePath(params string[] paths);

        /// <summary>
        /// Gets the relative path from a base path to a target path.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        /// <param name="targetPath">Target path.</param>
        /// <returns>Relative path.</returns>
        string GetRelativePath(string basePath, string targetPath);
    }
}
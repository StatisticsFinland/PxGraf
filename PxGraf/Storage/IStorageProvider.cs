using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously reads all text from a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Text content of the file.</returns>
        Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously writes all text to a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="content">Content to write to the file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task WriteAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously opens a file for reading.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Stream for reading the file.</returns>
        Task<Stream> OpenReadAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously enumerates files in a directory with the specified file extension.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <param name="fileExtension">File extension to filter by (e.g., ".px", ".txt"). Pass empty string to get all files.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enumerable of file paths.</returns>
        Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string fileExtension, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously enumerates subdirectories in a directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enumerable of directory paths.</returns>
        Task<IEnumerable<string>> EnumerateDirectoriesAsync(string directoryPath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a lightweight check to verify that a directory or storage prefix is accessible.
        /// </summary>
        /// <param name="directoryPath">Path to the directory or prefix.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the directory is accessible, false otherwise.</returns>
        Task<bool> ProbeDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets the last write time of a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Last write time of the file.</returns>
        Task<DateTime> GetLastWriteTimeAsync(string filePath, CancellationToken cancellationToken = default);

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
        /// Builds a safe path by combining a root path with user-provided path segments.
        /// Validates that the resulting path stays within the root path boundary.
        /// </summary>
        /// <param name="rootPath">Root path that constrains the result.</param>
        /// <param name="userPath">User-provided path segment to append.</param>
        /// <returns>Validated combined path.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the resulting path would escape the root path.</exception>
        string BuildPath(string rootPath, string userPath);

        /// <summary>
        /// Gets the relative path from a base path to a target path.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        /// <param name="targetPath">Target path.</param>
        /// <returns>Relative path.</returns>
        string GetRelativePath(string basePath, string targetPath);
    }
}
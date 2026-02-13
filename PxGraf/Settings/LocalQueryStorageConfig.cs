using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for local file system storage of saved queries and archive files.
    /// </summary>
    /// <param name="savedQueryDirectory">Directory path for saved query files.</param>
    /// <param name="archiveFileDirectory">Directory path for archive files.</param>
    [ExcludeFromCodeCoverage]
    public class LocalQueryStorageConfig(string savedQueryDirectory, string archiveFileDirectory) : IQueryStorageConfig
    {
        /// <summary>
        /// Directory path for saved query files.
        /// </summary>
        public string SavedQueryDirectory { get; } = savedQueryDirectory;

        /// <summary>
        /// Directory path for archive files.
        /// </summary>
        public string ArchiveFileDirectory { get; } = archiveFileDirectory;

        /// <inheritdoc/>
        public string SavedQueryPath => SavedQueryDirectory;

        /// <inheritdoc/>
        public string ArchiveFilePath => ArchiveFileDirectory;
    }
}
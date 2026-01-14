using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for local file system storage of saved queries and archive files.
    /// </summary>
    /// <param name="enabled">Whether local file system storage is enabled.</param>
    /// <param name="savedQueryDirectory">Directory path for saved query files.</param>
    /// <param name="archiveFileDirectory">Directory path for archive files.</param>
    [ExcludeFromCodeCoverage]
    public class LocalQueryStorageConfig(bool enabled, string savedQueryDirectory, string archiveFileDirectory)
    {
        /// <summary>
        /// Whether local file system storage is enabled.
        /// </summary>
        public bool Enabled { get; } = enabled;

        /// <summary>
        /// Directory path for saved query files.
        /// </summary>
        public string SavedQueryDirectory { get; } = savedQueryDirectory;

        /// <summary>
        /// Directory path for archive files.
        /// </summary>
        public string ArchiveFileDirectory { get; } = archiveFileDirectory;
    }
}
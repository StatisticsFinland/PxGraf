using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Settings
{
    /// <summary>
    /// Abstract base class for query storage configuration.
    /// Only one query storage configuration can be active at a time.
    /// </summary>
    public interface IQueryStorageConfig
    {
        /// <summary>
        /// Path or directory for saved query files.
        /// </summary>
        public string SavedQueryPath { get; }

        /// <summary>
        /// Path or directory for archive files.
        /// </summary>
        public string ArchiveFilePath { get; }
    }
}

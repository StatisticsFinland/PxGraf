using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PxGraf.Datasource.FileDatasource
{
    /// <summary>
    /// Configuration for local file system datasource.
    /// </summary>
    /// <param name="enabled">Whether the local file system datasource is enabled.</param>
    /// <param name="databaseRootPath">Root path to the database directory.</param>
    /// <param name="encoding">Text encoding for reading files.</param>
    [ExcludeFromCodeCoverage]
    public class LocalFilesystemDatabaseConfig(bool enabled, string databaseRootPath, Encoding encoding)
    {
        /// <summary>
        /// Whether the local file system datasource is enabled.
        /// </summary>
        public bool Enabled { get; } = enabled;

        /// <summary>
        /// Root path to the database directory.
        /// </summary>
        public string DatabaseRootPath { get; } = databaseRootPath;

        /// <summary>
        /// Text encoding for reading files.
        /// </summary>
        public Encoding Encoding { get; } = encoding;
    }
}

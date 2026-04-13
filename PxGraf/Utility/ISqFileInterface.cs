using PxGraf.Models.SavedQueries;
using System.Threading.Tasks;

namespace PxGraf.Utility
{
    /// <summary>
    /// Interface for interacting with the sq and sqa files.
    /// </summary>
    public interface ISqFileInterface
    {
        /// <summary>
        /// Returns true if a saved query file with the given sq id exists within the specified saved query file location.
        /// </summary>
        public Task<bool> SavedQueryExists(string id, string savedQueryDirectory);

        /// <summary>
        /// Returns true if an archive query file with the given sq id exists within the specified archive query file location.
        /// </summary>
        public Task<bool> ArchiveCubeExists(string id, string archiveDirectory);

        /// <summary>
        /// Reads serialized SavedQuery object from a file.
        /// </summary>
        /// <returns></returns>
        public Task<SavedQuery> ReadSavedQueryFromFile(string id, string savedQueryDirectory);

        /// <summary>
        /// Reads serialized ArchiveCube object from a file.
        /// </summary>
        /// <returns></returns>
        public Task<ArchiveCube> ReadArchiveCubeFromFile(string id, string archiveDirectory);

        /// <summary>
        /// Serialize a json serializable query object and write it to a saved query file.
        /// </summary>
        public Task SerializeToSqFileAsync(string fileName, string filePath, object input);

        /// <summary>
        /// Serialize a json serializable query archive object and write it to an archive file.
        /// </summary>
        public Task SerializeToArchiveFileAsync(string fileName, string filePath, object input);

        /// <summary>
        /// Checks whether the saved query storage directory is accessible.
        /// </summary>
        /// <param name="directory">The saved query directory path.</param>
        /// <returns>True if the directory can be accessed, false otherwise.</returns>
        public Task<bool> CanAccessSavedQueriesAsync(string directory);

        /// <summary>
        /// Checks whether the archive file storage directory is accessible.
        /// </summary>
        /// <param name="directory">The archive file directory path.</param>
        /// <returns>True if the directory can be accessed, false otherwise.</returns>
        public Task<bool> CanAccessArchivesAsync(string directory);
    }
}

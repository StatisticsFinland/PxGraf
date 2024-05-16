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
        /// Returns true if a saved query file with the given sq id exists withing the specified saved query file location.
        /// </summary>
        public bool SavedQueryExists(string id, string savedQueryDirectory);

        /// <summary>
        /// Returns true if an archive query file with the given sq id exists withing the specified archive query file location.
        /// </summary>
        public bool ArchiveCubeExists(string id, string archiveDirectory);

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
        /// Serialize a json serializable object and write it to a file.
        /// </summary>
        public Task SerializeToFile(string fileName, string filePath, object input);
    }
}

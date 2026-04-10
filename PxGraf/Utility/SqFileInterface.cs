using System.Text.Json;
using PxGraf.Models.SavedQueries;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using PxGraf.Settings;
using PxGraf.Storage;
using System.IO;

namespace PxGraf.Utility
{
    /// <summary>
    /// For interacting with the sq and sqa files using configurable storage providers.
    /// </summary>
    /// <remarks>
    /// Constructor for SqFileInterface with configurable storage providers.
    /// </remarks>
    /// <param name="savedQueryStorage">Storage provider for saved query files.</param>
    /// <param name="archiveStorage">Storage provider for archive files.</param>
    [ExcludeFromCodeCoverage] // Not worth it to build abstraction over storage IO in order to test such simple functionality
    public class SqFileInterface(IStorageProvider savedQueryStorage, IStorageProvider archiveStorage) : ISqFileInterface
    {
        private readonly static LockByKey lockScope = new(StringComparer.OrdinalIgnoreCase);
        private readonly IStorageProvider savedQueryStorage = savedQueryStorage ?? throw new ArgumentNullException(nameof(savedQueryStorage));
        private readonly IStorageProvider archiveStorage = archiveStorage ?? throw new ArgumentNullException(nameof(archiveStorage));

        /// <summary>
        /// Returns true if a saved query file with the given sq id exists within the specified saved query file location.
        /// </summary>
        public async Task<bool> SavedQueryExists(string id, string savedQueryDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                string filePath = savedQueryStorage.CombinePath(savedQueryDirectory, id + ".sq");
                return await savedQueryStorage.FileExistsAsync(filePath);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if an archive query file with the given sq id exists within the specified archive query file location.
        /// </summary>
        public async Task<bool> ArchiveCubeExists(string id, string archiveDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                string filePath = archiveStorage.CombinePath(archiveDirectory, id + ".sqa");
                return await archiveStorage.FileExistsAsync(filePath);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reads serialized SavedQuery object from a file.
        /// </summary>
        public async Task<SavedQuery> ReadSavedQueryFromFile(string id, string savedQueryDirectory)
        {
            string filePath = savedQueryStorage.CombinePath(savedQueryDirectory, id + ".sq");
            return await lockScope.RunLockedAsync(
                filePath,
                () => ReadJsonObjectFromFileImpl<SavedQuery>(savedQueryStorage, filePath)
            );
        }

        /// <summary>
        /// Reads serialized ArchiveCube object from a file.
        /// </summary>
        public async Task<ArchiveCube> ReadArchiveCubeFromFile(string id, string archiveDirectory)
        {
            string filePath = archiveStorage.CombinePath(archiveDirectory, id + ".sqa");
            return await lockScope.RunLockedAsync(
                filePath,
                () => ReadJsonObjectFromFileImpl<ArchiveCube>(archiveStorage, filePath)
            );
        }

        /// <summary>
        /// Implementation for reading serialized objects from a file using storage provider.
        /// </summary>
        /// <returns></returns>
        private static async Task<T> ReadJsonObjectFromFileImpl<T>(IStorageProvider storage, string path)
        {
            using Stream stream = await storage.OpenReadAsync(path);
            return await JsonSerializer.DeserializeAsync<T>(stream, GlobalJsonConverterOptions.Default)
                ?? throw new JsonException($"Failed to deserialize object from {path}");
        }

        /// <summary>
        /// Asynchronous function for serializing query objects to a file.
        /// This function handles locking the file.
        /// </summary>
        /// <param name="fileName">Name of the file to be created</param>
        /// <param name="filePath">Target file location</param>
        /// <param name="input">This will be serialized to the file</param>
        /// <returns>Serialization task</returns>
        public async Task SerializeToSqFileAsync(string fileName, string filePath, object input)
        {
            string fullPath = savedQueryStorage.CombinePath(filePath, fileName);
            await lockScope.RunLockedAsync(
                fullPath,
                () => SerializeToFileImplAsync(savedQueryStorage, fullPath, input)
            );
        }

        /// <summary>
        /// Asynchronous function for serializing query archive objects to a file.
        /// This function handles locking the file.
        /// </summary>
        /// <param name="fileName">Name of the file to be created</param>
        /// <param name="filePath">Target file location</param>
        /// <param name="input">This will be serialized to the file</param>
        /// <returns>Serialization task</returns>
        public async Task SerializeToArchiveFileAsync(string fileName, string filePath, object input)
        {
            string fullPath = archiveStorage.CombinePath(filePath, fileName);
            await lockScope.RunLockedAsync(
                fullPath,
                () => SerializeToFileImplAsync(archiveStorage, fullPath, input)
            );
        }

        private static async Task SerializeToFileImplAsync(IStorageProvider storage, string fullPath, object input)
        {
            string json = JsonSerializer.Serialize(input, GlobalJsonConverterOptions.Default);
            await storage.WriteAllTextAsync(fullPath, json);
        }
    }
}

using System.Text.Json;
using PxGraf.Models.SavedQueries;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using PxGraf.Settings;
using PxGraf.Storage;

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
        public bool SavedQueryExists(string id, string savedQueryDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                string filePath = savedQueryStorage.CombinePath(savedQueryDirectory, id + ".sq");
                return savedQueryStorage.FileExistsAsync(filePath).GetAwaiter().GetResult();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if an archive query file with the given sq id exists within the specified archive query file location.
        /// </summary>
        public bool ArchiveCubeExists(string id, string archiveDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                string filePath = archiveStorage.CombinePath(archiveDirectory, id + ".sqa");
                return archiveStorage.FileExistsAsync(filePath).GetAwaiter().GetResult();
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
            return await lockScope.RunLocked(
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
            return await lockScope.RunLocked(
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
            string respdata = await storage.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<T>(respdata, GlobalJsonConverterOptions.Default)
                ?? throw new JsonException($"Failed to deserialize object from {path}");
        }

        /// <summary>
        /// Asynchronous function for serializing json serializable objects to a file.
        /// This function handles locking the file.
        /// </summary>
        /// <param name="fileName">Name of the file to be created</param>
        /// <param name="filePath">Target file location</param>
        /// <param name="input">This will be serialized to the file</param>
        /// <returns>Serialization task</returns>
        public async Task SerializeToFile(string fileName, string filePath, object input)
        {
            await Task.Factory.StartNew(() =>
            {
                lockScope.RunLocked(fileName, () => SerializeToFileImpl(fileName, filePath, input));
            });
        }

        private void SerializeToFileImpl(string fileName, string filePath, object input)
        {
            string fullPath = savedQueryStorage.CombinePath(filePath, fileName);
            string json = JsonSerializer.Serialize(input, GlobalJsonConverterOptions.Default);
            savedQueryStorage.WriteAllTextAsync(fullPath, json).GetAwaiter().GetResult();
        }
    }
}

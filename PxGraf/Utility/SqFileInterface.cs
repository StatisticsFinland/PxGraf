using Newtonsoft.Json;
using PxGraf.Models.SavedQueries;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace PxGraf.Utility
{
    /// <summary>
    /// For interacting with the sq and sqa files.
    /// </summary>
    [ExcludeFromCodeCoverage] // Not worth it to build abstraction over Filesystem IO in order to test such a simple functionality
    public class SqFileInterface : ISqFileInterface
    {
        private readonly static LockByKey lockScope = new(StringComparer.OrdinalIgnoreCase);

        private static readonly JsonSerializerSettings _serializerSettings = new();

        /// <summary>
        /// Returns true if a saved query file with the given sq id exists withing the specified saved query file location.
        /// </summary>
        public bool SavedQueryExists(string id, string savedQueryDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                return File.Exists(Path.Combine(savedQueryDirectory, id + ".sq"));
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Returns true if an archive query file with the given sq id exists withing the specified archive query file location.
        /// </summary>
        public bool ArchiveCubeExists(string id, string archiveDirectory)
        {
            if (InputValidation.ValidateSqIdString(id))
            {
                return File.Exists(Path.Combine(archiveDirectory, id + ".sqa"));
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
            string path = Path.Combine(savedQueryDirectory, id + ".sq");
            return await lockScope.RunLocked(
                path,
                () => ReadJsonObjectFromFileImpl<SavedQuery>(path)
            );
        }

        /// <summary>
        /// Reads serialized ArchiveCube object from a file.
        /// </summary>
        public async Task<ArchiveCube> ReadArchiveCubeFromFile(string id, string archiveDirectory)
        {
            string path = Path.Combine(archiveDirectory, id + ".sqa");
            return await lockScope.RunLocked(
                path,
                () => ReadJsonObjectFromFileImpl<ArchiveCube>(path)
            );
        }

        /// <summary>
        /// Implementation for reading serialized objects from a file.
        /// </summary>
        /// <returns></returns>
        private static async Task<T> ReadJsonObjectFromFileImpl<T>(string path)
        {
            var respdata = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(respdata);
        }

        /// <summary>
        /// Asyncronous function for serializing json serializable objects to a file.
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

        private static void SerializeToFileImpl(string fileName, string filePath, object input)
        {
            System.IO.Directory.CreateDirectory(filePath); //If the directory does not exist, create it.
            using StreamWriter outputFile = new(Path.Combine(filePath, fileName));
            using JsonTextWriter jsonWriter = new(outputFile);
            JsonSerializer ser = JsonSerializer.Create(_serializerSettings);
            ser.Serialize(jsonWriter, input);
            jsonWriter.Flush();
        }
    }
}

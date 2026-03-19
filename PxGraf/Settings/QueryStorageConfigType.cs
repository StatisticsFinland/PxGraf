namespace PxGraf.Settings
{
    /// <summary>
    /// Determines which storage backend is used for saved queries and archive files.
    /// </summary>
    public enum QueryStorageConfigType
    {
        /// <summary>
        /// Use local file system for query storage.
        /// </summary>
        LocalFileSystem,

        /// <summary>
        /// Use Azure Blob Storage for query storage.
        /// </summary>
        BlobContainer
    }
}

namespace PxGraf.Datasource
{
    /// <summary>
    /// Determines which data source backend is used for Px file access.
    /// </summary>
    public enum DatabaseConfigType
    {
        /// <summary>
        /// Use PxWeb API as the data source.
        /// </summary>
        PxWeb,

        /// <summary>
        /// Use local file system as the data source via Px.Utils.
        /// </summary>
        LocalFileSystem,

        /// <summary>
        /// Use Azure Blob Storage as the data source.
        /// </summary>
        BlobContainer
    }
}

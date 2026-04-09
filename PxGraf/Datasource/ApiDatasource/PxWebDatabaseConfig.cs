using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Datasource.ApiDatasource
{
    /// <summary>
    /// Configuration for PxWeb API data source.
    /// </summary>
    /// <param name="pxWebUrl">URL of the PxWeb API endpoint.</param>
    [ExcludeFromCodeCoverage]
    public class PxWebDatabaseConfig(string pxWebUrl) : DatabaseConfig
    {
        /// <summary>
        /// URL of the PxWeb API endpoint.
        /// </summary>
        public string PxWebUrl { get; } = pxWebUrl;
    }
}

using System.Net.Http;
using System.Threading.Tasks;

namespace PxGraf.Datasource.PxWebInterface
{
    /// <summary>
    /// Interface for a connection to PxWeb.
    /// </summary>
    public interface IPxWebConnection
    {
        /// <summary>
        /// Sends a GET request to PxWeb.
        /// </summary>
        /// <param name="path">Path of the request.</param>
        /// <param name="queryString">Query string of the request.</param>
        /// <returns>Response from the request.</returns>
        Task<HttpResponseMessage> GetAsync(string path, string queryString = "");

        /// <summary>
        /// Sends a POST request to PxWeb.
        /// </summary>
        /// <param name="path">Path of the request.</param>
        /// <param name="json">JSON data to be sent.</param>
        /// <returns>Response from the request.</returns>
        Task<HttpResponseMessage> PostAsync(string path, string json);
    }
}

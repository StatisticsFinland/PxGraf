using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Represents the overall health status of the application.
    /// </summary>
    /// <param name="Status">Overall health status: "healthy" or "unhealthy".</param>
    /// <param name="Databases">Health status of each configured database connection.</param>
    /// <param name="Services">Health status of each configured supporting service.</param>
    public record HealthResponse(string Status, List<DatabaseHealthStatus> Databases, List<ServiceHealthStatus> Services)
    {
        /// <summary>
        /// Returns true if the overall status is healthy.
        /// </summary>
        [JsonIgnore]
        public bool IsHealthy => Status == "healthy";
    }

    /// <summary>
    /// Represents the health status of a single database connection.
    /// </summary>
    /// <param name="Id">The database identifier.</param>
    /// <param name="Status">Connection status: "healthy" or "unhealthy".</param>
    public record DatabaseHealthStatus(string Id, string Status);

    /// <summary>
    /// Represents the health status of a single supporting service.
    /// </summary>
    /// <param name="Id">The service identifier.</param>
    /// <param name="Status">Service status: "healthy" or "unhealthy".</param>
    public record ServiceHealthStatus(string Id, string Status);
}

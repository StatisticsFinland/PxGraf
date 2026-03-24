using PxGraf.Models.Responses;
using System.Threading.Tasks;

namespace PxGraf.Services
{
    /// <summary>
    /// Interface for the health check orchestrator that probes all configured dependencies.
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// Runs all configured health probes and returns an aggregated health response.
        /// </summary>
        /// <returns>A <see cref="HealthResponse"/> containing overall and per-component health status.</returns>
        Task<HealthResponse> CheckHealthAsync();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PxGraf.Models.Responses;
using PxGraf.Services;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Handles health check requests for the application.
    /// </summary>
    /// <param name="healthCheckService">Service that orchestrates all health probes.</param>
    /// <param name="logger"><see cref="ILogger"/> instance used for logging.</param>
    [ApiController]
    [Route("api/health")]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status503ServiceUnavailable)]
    public class HealthController(IHealthCheckService healthCheckService, ILogger<HealthController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns the health status of all configured dependencies.
        /// Returns 200 if all dependencies are healthy, 503 if any are unhealthy.
        /// </summary>
        /// <returns>A <see cref="HealthResponse"/> with overall and per-component health status.</returns>
        [HttpGet]
        public async Task<IActionResult> GetHealthAsync()
        {
            logger.LogDebug("api/health: Health check requested");
            HealthResponse response = await healthCheckService.CheckHealthAsync();

            if (response.IsHealthy)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
    }
}

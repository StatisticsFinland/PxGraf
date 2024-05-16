using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Handles requests for API info.
    /// </summary>
    [ApiController]
    [Route("api/info")]
    public class InfoController : ControllerBase
    {
        private readonly string _name;
        private readonly string _environment;
        private readonly string _version;
        private readonly ILogger<InfoController> _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="env">Instance of a <see cref="IWebHostEnvironment"/> object. Used for getting information about the API.</param>
        /// <param name="logger"><see cref="ILogger"/> instance used for logging API calls.</param>
        public InfoController(IWebHostEnvironment env, ILogger<InfoController> logger)
        {
            _name = env.ApplicationName;
            _environment = env.EnvironmentName;
            _version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            _logger = logger;
        }

        /// <summary>
        /// Returns information about the API including the name of the application, the environment it's running in, and version of the API.
        /// </summary>
        /// <returns>Information about the API - name, environment and version.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogDebug("api/info: API info requested: Name: {Name}, Environment: {Environment}, Version: {Version}", _name, _environment, _version);
            return Ok(new
            {
                Name = _name,
                Environment = _environment,
                Version = _version
            });
        }
    }
}

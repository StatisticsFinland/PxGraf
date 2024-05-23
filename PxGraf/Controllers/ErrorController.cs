using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Controller for logging errors.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    /// <param name="logger"><see cref="ILogger"/> instance for writing logs.</param>
    [ApiController]
    [Route("api/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger = logger;

        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error == null)
            {
                //No actual exception? User may have requested error path manually
                _logger.LogError("Error handler hitted without exception.");
                return BadRequest();
            }
            if (exceptionHandlerPathFeature.Error is IOException)
            {
                _logger.LogCritical(exceptionHandlerPathFeature.Error, "An IO error occurred in {Path}", exceptionHandlerPathFeature.Path);
            }
            else
            {
                _logger.LogError(exceptionHandlerPathFeature.Error, "api/error: Error in {Path}", exceptionHandlerPathFeature.Path);
            }

            return BadRequest();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Services
{
    public interface IAuditLogService
    {
        void LogAuditEvent(string action, string resource);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IHttpContextAccessor httpContextAccessor, ILogger<AuditLogService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void LogAuditEvent(string action, string resource)
        {
            if (!Configuration.Current.AuditLoggingEnabled) return;

            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            // Extract only the configured headers
            Dictionary<string, string> context = Configuration.Current.AuditLogHeaders
                .Where(httpContext.Request.Headers.ContainsKey)
                .ToDictionary(
                    h => h,
                    h => httpContext.Request.Headers[h].ToString()
                );

            context.Add("Category", "Audit");

            using (_logger.BeginScope(context))
            {
                _logger.LogInformation("Audit Event: action={Action}, resource={Resource}, user={User}, clientIP={ClientIP}",
                    action,
                    resource,
                    httpContext.User.Identity?.Name ?? "Anonymous",
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    );
            }
        }
    }
}

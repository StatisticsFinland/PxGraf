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

    public class AuditLogService(IHttpContextAccessor httpContextAccessor, ILogger<AuditLogService> logger) : IAuditLogService
    {
        public void LogAuditEvent(string action, string resource)
        {
            if (!Configuration.Current.AuditLoggingEnabled) return;

            HttpContext httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            // Extract only the configured headers
            Dictionary<string, string> context = Configuration.Current.AuditLogHeaders
                .Where(httpContext.Request.Headers.ContainsKey)
                .ToDictionary(
                    h => h,
                    h => httpContext.Request.Headers[h].ToString()
                );

            context.Add("Category", "Audit");

            using (logger.BeginScope(context))
            {
                logger.LogInformation("Audit Event: action={Action}, resource={Resource}, user={User}, clientIP={ClientIP}",
                    action,
                    resource,
                    httpContext.User.Identity?.Name ?? "Anonymous",
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    );
            }
        }
    }
}

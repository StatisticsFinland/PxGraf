using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PxGraf
{
    [ExcludeFromCodeCoverage] // Testing of these methods should be done with integration tests
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Setup NLog configuration
            Logger nlogConfig = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();
            
            nlogConfig.Info("Starting application");

            try
            {
                nlogConfig.Info("Starting host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                nlogConfig.Fatal(ex, "Service terminated unexpectedly");
            }
            finally
            {
                nlogConfig.Info("Shutting down");
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();
                     
                     // Configure logging for standard app logs and audit logs
                     logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
                     logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Warning);
                     
                     // Add filter to separate audit logs
                     logging.AddFilter("PxGraf.Services.AuditLogService", Microsoft.Extensions.Logging.LogLevel.Information);
                 })
                 .UseNLog()
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.Sources.Clear();
                     config.SetBasePath(Directory.GetCurrentDirectory());
                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                     config.AddEnvironmentVariables();
                     if (args != null)
                     {
                         config.AddCommandLine(args);
                     }
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

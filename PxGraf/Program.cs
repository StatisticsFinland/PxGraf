using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace PxGraf
{
    [ExcludeFromCodeCoverage] // Testing of these methods should be done with integration tests
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info("Starting application");

            try
            {
                _ = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                    .Build();

                logger.Info("Starting host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Service terminated unexpectedly");
            }
            finally
            {
                logger.Info("Shutting down");
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();
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

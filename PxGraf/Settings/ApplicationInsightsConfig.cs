using Microsoft.Extensions.Configuration;
using System;

namespace PxGraf.Settings
{

    /// <summary>
    /// Configuration for Application Insights integration.
    /// </summary>
    public class ApplicationInsightsConfig
    {
        /// <summary>
        /// Application Insights connection string.
        /// Can be overridden by the APPLICATIONINSIGHTS_CONNECTION_STRING environment variable.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Whether adaptive sampling is enabled.
        /// Defaults to false to ensure all configured logs are captured.
        /// </summary>
        public bool EnableAdaptiveSampling { get; }

        /// <summary>
        /// Initializes ApplicationInsights configuration from the provided configuration section.
        /// </summary>
        /// <param name="configurationSection">Configuration section containing ApplicationInsights settings.</param>
        /// <param name="envKey">Optional environment variable key for the connection string, default is "APPLICATIONINSIGHTS_CONNECTION_STRING".</param>
        public ApplicationInsightsConfig(IConfigurationSection configurationSection, string envKey = "APPLICATIONINSIGHTS_CONNECTION_STRING" )
        {
            // Check environment variable first, then config
            ConnectionString = Environment.GetEnvironmentVariable(envKey)
               ?? configurationSection.GetValue<string>(nameof(ConnectionString));

            // Get adaptive sampling setting, default to false
            EnableAdaptiveSampling = configurationSection.GetValue<bool>(nameof(EnableAdaptiveSampling), false);
        }

        /// <summary>
        /// Returns true if Application Insights is configured (connection string is available).
        /// </summary>
        public bool IsEnabled => !string.IsNullOrEmpty(ConnectionString);
    }
}

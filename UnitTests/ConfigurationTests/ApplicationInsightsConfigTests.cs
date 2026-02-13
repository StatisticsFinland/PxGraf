using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PxGraf.Settings;
using System;
using System.Collections.Generic;

namespace UnitTests.ConfigurationTests
{

    [TestFixture]
    public class ApplicationInsightsConfigTests
    {
        [Test]
        public void Constructor_WhenNoConfigurationProvided_ShouldBeDisabledWithDefaults()
        {
            // Arrange
            Dictionary<string, string> configData = [];
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.False);
                Assert.That(config.ConnectionString, Is.Null);
                Assert.That(config.MinimumLevel, Is.EqualTo(LogLevel.Information));
                Assert.That(config.EnableAdaptiveSampling, Is.False);
            });
        }

        [Test]
        public void Constructor_WhenConnectionStringInConfig_ShouldBeEnabledWithConnectionString()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=test-key;IngestionEndpoint=https://test.com"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.True);
                Assert.That(config.ConnectionString, Is.EqualTo("InstrumentationKey=test-key;IngestionEndpoint=https://test.com"));
                Assert.That(config.MinimumLevel, Is.EqualTo(LogLevel.Information));
                Assert.That(config.EnableAdaptiveSampling, Is.False);
            });
        }

        [Test]
        public void Constructor_WhenEnvironmentVariableSet_ShouldUseEnvironmentVariable()
        {
            // Arrange
            const string envVarName = "APPLICATIONINSIGHTS_CONNECTION_STRING";
            const string envConnectionString = "InstrumentationKey=env-key;IngestionEndpoint=https://test.com/env";

            Environment.SetEnvironmentVariable(envVarName, envConnectionString);

            try
            {
                Dictionary<string, string> configData = new()
                {
                    ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=config-key;IngestionEndpoint=https://test.com/config"
                };
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(configData)
                    .Build();
                IConfigurationSection section = configuration.GetSection("ApplicationInsights");

                // Act
                ApplicationInsightsConfig config = new(section);

                // Assert - Environment variable should take priority
                Assert.Multiple(() =>
                {
                    Assert.That(config.IsEnabled, Is.True);
                    Assert.That(config.ConnectionString, Is.EqualTo(envConnectionString));
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable(envVarName, null);
            }
        }

        [Test]
        public void Constructor_WhenOnlyEnvironmentVariableSet_ShouldUseEnvironmentVariable()
        {
            // Arrange
            const string envVarName = "APPLICATIONINSIGHTS_CONNECTION_STRING";
            const string envConnectionString = "InstrumentationKey=env-only-key;IngestionEndpoint=https://env-only.com";

            Environment.SetEnvironmentVariable(envVarName, envConnectionString);

            try
            {
                Dictionary<string, string> configData = [];
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(configData)
                    .Build();
                IConfigurationSection section = configuration.GetSection("ApplicationInsights");

                // Act
                ApplicationInsightsConfig config = new(section);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(config.IsEnabled, Is.True);
                    Assert.That(config.ConnectionString, Is.EqualTo(envConnectionString));
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable(envVarName, null);
            }
        }

        [Test]
        public void Constructor_WhenEmptyConnectionStringInConfig_ShouldBeDisabled()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = ""
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.False);
                Assert.That(config.ConnectionString, Is.EqualTo(""));
            });
        }

        [Test]
        public void Constructor_WhenMinimumLevelSet_ShouldUseConfiguredLevel()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=test-key",
                ["ApplicationInsights:MinimumLevel"] = "Debug"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.That(config.MinimumLevel, Is.EqualTo(LogLevel.Debug));
        }

        [Test]
        public void Constructor_WhenInvalidMinimumLevel_ShouldUseDefaultInformation()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=test-key",
                ["ApplicationInsights:MinimumLevel"] = "InvalidLevel"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.That(config.MinimumLevel, Is.EqualTo(LogLevel.Information));
        }

        [Test]
        public void Constructor_WhenAdaptiveSamplingEnabled_ShouldUseConfiguredValue()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=test-key",
                ["ApplicationInsights:EnableAdaptiveSampling"] = "true"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.That(config.EnableAdaptiveSampling, Is.True);
        }

        [Test]
        public void Constructor_WhenAllSettingsProvided_ShouldUseAllConfiguredValues()
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=full-test-key",
                ["ApplicationInsights:MinimumLevel"] = "Warning",
                ["ApplicationInsights:EnableAdaptiveSampling"] = "true"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                 .AddInMemoryCollection(configData)
                 .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.True);
                Assert.That(config.ConnectionString, Is.EqualTo("InstrumentationKey=full-test-key"));
                Assert.That(config.MinimumLevel, Is.EqualTo(LogLevel.Warning));
                Assert.That(config.EnableAdaptiveSampling, Is.True);
            });
        }

        [TestCase("Debug", LogLevel.Debug)]
        [TestCase("Information", LogLevel.Information)]
        [TestCase("Warning", LogLevel.Warning)]
        [TestCase("Error", LogLevel.Error)]
        [TestCase("Critical", LogLevel.Critical)]
        [TestCase("debug", LogLevel.Debug)] // Case insensitive
        [TestCase("INFORMATION", LogLevel.Information)] // Case insensitive
        public void Constructor_WhenValidMinimumLevelProvided_ShouldParseCorrectly(string levelString, LogLevel expectedLevel)
        {
            // Arrange
            Dictionary<string, string> configData = new()
            {
                ["ApplicationInsights:ConnectionString"] = "InstrumentationKey=test-key",
                ["ApplicationInsights:MinimumLevel"] = levelString
            };
            IConfiguration configuration = new ConfigurationBuilder()
                 .AddInMemoryCollection(configData)
                 .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section);

            // Assert
            Assert.That(config.MinimumLevel, Is.EqualTo(expectedLevel));
        }
    }
}

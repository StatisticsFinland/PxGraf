using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Settings;
using System;
using System.Collections.Generic;

namespace UnitTests.ConfigurationTests
{

    [TestFixture]
    public class ApplicationInsightsConfigTests
    {
        private const string EnvVarName = "TEST_APPLICATIONINSIGHTS_CONNECTION_STRING";

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
            ApplicationInsightsConfig config = new(section, EnvVarName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.False);
                Assert.That(config.ConnectionString, Is.Null);
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
            ApplicationInsightsConfig config = new(section, EnvVarName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.True);
                Assert.That(config.ConnectionString, Is.EqualTo("InstrumentationKey=test-key;IngestionEndpoint=https://test.com"));
                Assert.That(config.EnableAdaptiveSampling, Is.False);
            });
        }

        [Test]
        public void Constructor_WhenEnvironmentVariableSet_ShouldUseEnvironmentVariable()
        {
            // Arrange
            const string envConnectionString = "InstrumentationKey=env-key;IngestionEndpoint=https://test.com/env";

            Environment.SetEnvironmentVariable(EnvVarName, envConnectionString);

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
                ApplicationInsightsConfig config = new(section, EnvVarName);

                // Assert - Environment variable should take priority
                Assert.Multiple(() =>
                {
                    Assert.That(config.IsEnabled, Is.True);
                    Assert.That(config.ConnectionString, Is.EqualTo(envConnectionString));
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable(EnvVarName, null);
            }
        }

        [Test]
        public void Constructor_WhenOnlyEnvironmentVariableSet_ShouldUseEnvironmentVariable()
        {
            // Arrange
            const string envConnectionString = "InstrumentationKey=env-only-key;IngestionEndpoint=https://env-only.com";

            Environment.SetEnvironmentVariable(EnvVarName, envConnectionString);

            try
            {
                Dictionary<string, string> configData = [];
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(configData)
                    .Build();
                IConfigurationSection section = configuration.GetSection("ApplicationInsights");

                // Act
                ApplicationInsightsConfig config = new(section, EnvVarName);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(config.IsEnabled, Is.True);
                    Assert.That(config.ConnectionString, Is.EqualTo(envConnectionString));
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable(EnvVarName, null);
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
            ApplicationInsightsConfig config = new(section, EnvVarName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.False);
                Assert.That(config.ConnectionString, Is.EqualTo(""));
            });
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
            ApplicationInsightsConfig config = new(section, EnvVarName);

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
                ["ApplicationInsights:EnableAdaptiveSampling"] = "true"
            };
            IConfiguration configuration = new ConfigurationBuilder()
                 .AddInMemoryCollection(configData)
                 .Build();
            IConfigurationSection section = configuration.GetSection("ApplicationInsights");

            // Act
            ApplicationInsightsConfig config = new(section, EnvVarName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(config.IsEnabled, Is.True);
                Assert.That(config.ConnectionString, Is.EqualTo("InstrumentationKey=full-test-key"));
                Assert.That(config.EnableAdaptiveSampling, Is.True);
            });
        }

    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PxGraf.Exceptions;
using PxGraf.Language;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTests.Fixtures;

namespace UnitTests.ConfigurationTests
{
    public class ConfigurationTests
    {

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void ConfigurationLoadTest_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.EqualTo("http://pxwebtesturl:12345/"));
        }

        [Test]
        public void ConfigurationLoadTest_WithoutConfiguration_ThrowsException()
        {
            Assert.Throws<NullReferenceException>(() => Configuration.Load(null));
        }

        [Test]
        public void ConfigurationLoadTest_WithoutLocalDatabase_SuccessWithNullLocalDatabaseCOnfig()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", "http://pxwebtesturl:12345/"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.EqualTo("http://pxwebtesturl:12345/"));
            Assert.That(Configuration.Current.LocalFilesystemDatabaseConfig, Is.Null);
        }

        [Test]
        public void ConfigurationLoadTest_WithoutPxWebUrlWithLocalDatabaseConfig_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"LocalFilesystemDatabaseConfig:Enabled", "true"},
                        {"LocalFilesystemDatabaseConfig:Encoding", "latin1"},
                        {"LocalFilesystemDatabaseConfig:DatabaseRootPath", "path/to/database"}
                    })
                .Build();
            Configuration.Load(configuration);
            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.Null);
            Assert.That(Configuration.Current.LocalFilesystemDatabaseConfig, Is.Not.Null);
            Assert.That(Configuration.Current.LocalFilesystemDatabaseConfig.Enabled, Is.True);
            Assert.That(Configuration.Current.LocalFilesystemDatabaseConfig.Encoding, Is.EqualTo(Encoding.Latin1));
        }

        [Test]
        public void ConfigurationLoadTest_WithoutPxWebUrlAndLocalDatabase_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", null},
                        {"LocalFilesystemDatabaseConfig", null}
                    })
                .Build();
            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_WithMissingLocalDatabaseFields_SuccessWitNullLocalDatabaseConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", "http://pxwebtesturl:12345/"},
                        {"LocalFilesystemDatabaseConfig:Enabled", "true"}
                    })
                .Build();
            Configuration.Load(configuration);
            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.EqualTo("http://pxwebtesturl:12345/"));
            Assert.That(Configuration.Current.LocalFilesystemDatabaseConfig, Is.Null);
        }

        [Test]
        public void AppSettings_WhenApplicationInsightsConfigurationNotProvided_ShouldLoadWithDisabledApplicationInsights()
        {
            // Arrange
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"LocalFilesystemDatabaseConfig:Enabled", "true"},
                        {"LocalFilesystemDatabaseConfig:Encoding", "latin1"},
                        {"LocalFilesystemDatabaseConfig:DatabaseRootPath", "path/to/database"}
                    })
                .Build();

            // Act
            Configuration.Load(configuration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(Configuration.Current.ApplicationInsights.IsEnabled, Is.False);
                Assert.That(Configuration.Current.ApplicationInsights.ConnectionString, Is.Null);
                Assert.That(Configuration.Current.ApplicationInsights.MinimumLevel, Is.EqualTo(LogLevel.Information));
                Assert.That(Configuration.Current.ApplicationInsights.EnableAdaptiveSampling, Is.False);
            });
        }

        [Test]
        public void AppSettings_WhenApplicationInsightsConfigurationProvided_ShouldLoadApplicationInsightsSettings()
        {
            // Arrange
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", "http://pxwebtesturl:12345/"},
                        {"ApplicationInsights:ConnectionString", "InstrumentationKey=test-key;IngestionEndpoint=https://test.com"},
                        {"ApplicationInsights:MinimumLevel", "Debug"},
                        {"ApplicationInsights:EnableAdaptiveSampling", "true"}
                    })
                .Build();

            // Act
            Configuration.Load(configuration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(Configuration.Current.ApplicationInsights.IsEnabled, Is.True);
                Assert.That(Configuration.Current.ApplicationInsights.ConnectionString, Is.EqualTo("InstrumentationKey=test-key;IngestionEndpoint=https://test.com"));
                Assert.That(Configuration.Current.ApplicationInsights.MinimumLevel, Is.EqualTo(LogLevel.Debug));
                Assert.That(Configuration.Current.ApplicationInsights.EnableAdaptiveSampling, Is.True);
            });
        }

        [Test]
        public void AppSettings_WhenApplicationInsightsEnvironmentVariableSet_ShouldPrioritizeEnvironmentVariable()
        {
            // Arrange
            const string envVarName = "APPLICATIONINSIGHTS_CONNECTION_STRING";
            const string envConnectionString = "InstrumentationKey=env-key;IngestionEndpoint=https://test.com";

            Environment.SetEnvironmentVariable(envVarName, envConnectionString);

            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            {"pxwebUrl", "http://pxwebtesturl:12345/"},
                            {"ApplicationInsights:ConnectionString", "InstrumentationKey=config-key;IngestionEndpoint=https://test.com"},
                            {"ApplicationInsights:MinimumLevel", "Warning"}
                        })
                    .Build();

                // Act
                Configuration.Load(configuration);

                // Assert - Environment variable should take priority over config
                Assert.Multiple(() =>
                {
                    Assert.That(Configuration.Current.ApplicationInsights.IsEnabled, Is.True);
                    Assert.That(Configuration.Current.ApplicationInsights.ConnectionString, Is.EqualTo(envConnectionString));
                    Assert.That(Configuration.Current.ApplicationInsights.MinimumLevel, Is.EqualTo(LogLevel.Warning)); // Other settings from config should still work
                });
            }
            finally
            {
                Environment.SetEnvironmentVariable(envVarName, null);
            }
        }
    }
}

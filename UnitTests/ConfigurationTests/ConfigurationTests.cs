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
        public void ConfigurationLoadTest_WithBlobContainerConfig_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"BlobContainerDatabaseConfig:Enabled", "true"},
                        {"BlobContainerDatabaseConfig:StorageAccountName", "teststorageaccount"},
                        {"BlobContainerDatabaseConfig:ContainerName", "testcontainer"},
                        {"BlobContainerDatabaseConfig:RootPath", "database/"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig, Is.Not.Null);
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig.Enabled, Is.True);
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig.StorageAccountName, Is.EqualTo("teststorageaccount"));
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig.ContainerName, Is.EqualTo("testcontainer"));
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig.RootPath, Is.EqualTo("database/"));
        }

        [Test]
        public void ConfigurationLoadTest_WithBlobContainerConfigWithoutRootPath_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"BlobContainerDatabaseConfig:Enabled", "true"},
                        {"BlobContainerDatabaseConfig:StorageAccountName", "teststorageaccount"},
                        {"BlobContainerDatabaseConfig:ContainerName", "testcontainer"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig, Is.Not.Null);
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig.RootPath, Is.EqualTo(""));
        }

        [Test]
        public void ConfigurationLoadTest_WithMissingBlobContainerFields_SuccessWithNullBlobContainerConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", "http://pxwebtesturl:12345/"},
                        {"BlobContainerDatabaseConfig:Enabled", "true"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.EqualTo("http://pxwebtesturl:12345/"));
            Assert.That(Configuration.Current.BlobContainerDatabaseConfig, Is.Null);
        }

        [Test]
        public void ConfigurationLoadTest_WithoutAnyDataSource_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"pxwebUrl", null},
                        {"LocalFilesystemDatabaseConfig", null},
                        {"BlobContainerDatabaseConfig", null}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_WithLocalQueryStorageConfig_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"LocalQueryStorageConfig:Enabled", "true"},
                    {"LocalQueryStorageConfig:SavedQueryDirectory", "/path/to/queries"},
                    {"LocalQueryStorageConfig:ArchiveFileDirectory", "/path/to/archives"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.LocalQueryStorageConfig, Is.Not.Null);
            Assert.That(Configuration.Current.LocalQueryStorageConfig.Enabled, Is.True);
            Assert.That(Configuration.Current.LocalQueryStorageConfig.SavedQueryDirectory, Is.EqualTo("/path/to/queries"));
            Assert.That(Configuration.Current.LocalQueryStorageConfig.ArchiveFileDirectory, Is.EqualTo("/path/to/archives"));
        }

        [Test]
        public void ConfigurationLoadTest_WithBlobQueryStorageConfig_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"BlobQueryStorageConfig:Enabled", "true"},
                    {"BlobQueryStorageConfig:StorageAccountName", "teststorage"},
                    {"BlobQueryStorageConfig:ContainerName", "testcontainer"},
                    {"BlobQueryStorageConfig:SavedQueryPath", "custom-queries"},
                    {"BlobQueryStorageConfig:ArchiveFilePath", "custom-archives"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.BlobQueryStorageConfig, Is.Not.Null);
            Assert.That(Configuration.Current.BlobQueryStorageConfig.Enabled, Is.True);
            Assert.That(Configuration.Current.BlobQueryStorageConfig.StorageAccountName, Is.EqualTo("teststorage"));
            Assert.That(Configuration.Current.BlobQueryStorageConfig.ContainerName, Is.EqualTo("testcontainer"));
            Assert.That(Configuration.Current.BlobQueryStorageConfig.SavedQueryPath, Is.EqualTo("custom-queries"));
            Assert.That(Configuration.Current.BlobQueryStorageConfig.ArchiveFilePath, Is.EqualTo("custom-archives"));
        }

        [Test]
        public void ConfigurationLoadTest_WithBlobQueryStorageConfig_EmptyPathsReturnNull()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"BlobQueryStorageConfig:Enabled", "true"},
                    {"BlobQueryStorageConfig:StorageAccountName", "teststorage"},
                    {"BlobQueryStorageConfig:ContainerName", "testcontainer"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.BlobQueryStorageConfig, Is.Not.Null);
            Assert.That(Configuration.Current.BlobQueryStorageConfig.SavedQueryPath, Is.Null);
            Assert.That(Configuration.Current.BlobQueryStorageConfig.ArchiveFilePath, Is.Null);
        }

        [Test]
        public void SetQueryDirectoryFields_WithBlobQueryStorage_SetsLegacyFields()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"BlobQueryStorageConfig:Enabled", "true"},
                    {"BlobQueryStorageConfig:StorageAccountName", "teststorage"},
                    {"BlobQueryStorageConfig:ContainerName", "testcontainer" },
                    {"BlobQueryStorageConfig:SavedQueryPath", "custom-saved"},
                    {"BlobQueryStorageConfig:ArchiveFilePath", "custom-archive"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.SavedQueryDirectory, Is.EqualTo("custom-saved"));
            Assert.That(Configuration.Current.ArchiveFileDirectory, Is.EqualTo("custom-archive"));
        }

        [Test]
        public void SetQueryDirectoryFields_WithLocalQueryStorage_SetsLegacyFields()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"LocalQueryStorageConfig:Enabled", "true"},
                    {"LocalQueryStorageConfig:SavedQueryDirectory", "/local/saved"},
                    {"LocalQueryStorageConfig:ArchiveFileDirectory", "/local/archive"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.SavedQueryDirectory, Is.EqualTo("/local/saved"));
            Assert.That(Configuration.Current.ArchiveFileDirectory, Is.EqualTo("/local/archive"));
        }

        [Test]
        public void SetQueryDirectoryFields_WithLegacyStorage_SetsLegacyFields()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                 new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"savedQueryDirectory", "/legacy/saved"},
                    {"archiveFileDirectory", "/legacy/archive"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.SavedQueryDirectory, Is.EqualTo("/legacy/saved"));
            Assert.That(Configuration.Current.ArchiveFileDirectory, Is.EqualTo("/legacy/archive"));
        }

        [Test]
        public void SetQueryDirectoryFields_WithLegacyFallback_FromLocalQueryStorageConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"savedQueryDirectory", "/legacy/saved"},
                    {"archiveFileDirectory", "/legacy/archive"}
                })
                .Build();

            Configuration.Load(configuration);

            // Should create LocalQueryStorageConfig from legacy settings
            Assert.That(Configuration.Current.LocalQueryStorageConfig, Is.Not.Null);
            Assert.That(Configuration.Current.LocalQueryStorageConfig.Enabled, Is.True);
            Assert.That(Configuration.Current.LocalQueryStorageConfig.SavedQueryDirectory, Is.EqualTo("/legacy/saved"));
            Assert.That(Configuration.Current.LocalQueryStorageConfig.ArchiveFileDirectory, Is.EqualTo("/legacy/archive"));
            // And set legacy fields
            Assert.That(Configuration.Current.SavedQueryDirectory, Is.EqualTo("/legacy/saved"));
            Assert.That(Configuration.Current.ArchiveFileDirectory, Is.EqualTo("/legacy/archive"));
        }

        [Test]
        public void ConfigurationLoadTest_WithMissingBlobQueryStorageFields_ReturnsNull()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"BlobQueryStorageConfig:Enabled", "true"},
                    {"BlobQueryStorageConfig:StorageAccountName", "teststorage"}
                    // ContainerName missing
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.BlobQueryStorageConfig, Is.Null);
        }

        [Test]
        public void ConfigurationLoadTest_WithMissingLocalQueryStorageFields_ReturnsNull()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"pxwebUrl", "http://test.com"},
                    {"LocalQueryStorageConfig:Enabled", "true"},
                    {"LocalQueryStorageConfig:SavedQueryDirectory", "/path/to/queries"}
                      // ArchiveFileDirectory missing
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.LocalQueryStorageConfig, Is.Null);
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

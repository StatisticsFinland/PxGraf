using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PxGraf.Datasource.ApiDatasource;
using PxGraf.Datasource.FileDatasource;
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
        public void ConfigurationLoadTest_PxWebType_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "PxWeb"},
                        {"DatabaseConfig:PxWebUrl", "http://pxwebtesturl:12345/"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.EqualTo("http://pxwebtesturl:12345/"));
            Assert.That(Configuration.Current.DatabaseConfig, Is.InstanceOf<PxWebDatabaseConfig>());
        }

        [Test]
        public void ConfigurationLoadTest_PxWebType_MissingUrl_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "PxWeb"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_LocalFileSystemType_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "LocalFileSystem"},
                        {"DatabaseConfig:DatabaseRootPath", "path/to/database"},
                        {"DatabaseConfig:Encoding", "latin1"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.PxWebUrl, Is.Null);
            Assert.That(Configuration.Current.DatabaseConfig, Is.InstanceOf<LocalFilesystemDatabaseConfig>());
            LocalFilesystemDatabaseConfig localConfig = (LocalFilesystemDatabaseConfig)Configuration.Current.DatabaseConfig;
            Assert.That(localConfig.DatabaseRootPath, Is.EqualTo("path/to/database"));
            Assert.That(localConfig.Encoding, Is.EqualTo(Encoding.Latin1));
        }

        [Test]
        public void ConfigurationLoadTest_LocalFileSystemType_MissingPath_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "LocalFileSystem"},
                        {"DatabaseConfig:Encoding", "latin1"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_LocalFileSystemType_MissingEncoding_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "LocalFileSystem"},
                        {"DatabaseConfig:DatabaseRootPath", "path/to/database"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_BlobContainerType_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "BlobContainer"},
                        {"DatabaseConfig:StorageAccountName", "teststorageaccount"},
                        {"DatabaseConfig:ContainerName", "testcontainer"},
                        {"DatabaseConfig:RootPath", "database/"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.DatabaseConfig, Is.InstanceOf<BlobContainerDatabaseConfig>());
            BlobContainerDatabaseConfig blobConfig = (BlobContainerDatabaseConfig)Configuration.Current.DatabaseConfig;
            Assert.That(blobConfig.StorageAccountName, Is.EqualTo("teststorageaccount"));
            Assert.That(blobConfig.ContainerName, Is.EqualTo("testcontainer"));
            Assert.That(blobConfig.RootPath, Is.EqualTo("database/"));
        }

        [Test]
        public void ConfigurationLoadTest_BlobContainerType_WithoutRootPath_DefaultsToEmpty()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "BlobContainer"},
                        {"DatabaseConfig:StorageAccountName", "teststorageaccount"},
                        {"DatabaseConfig:ContainerName", "testcontainer"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current, Is.Not.Null);
            Assert.That(Configuration.Current.DatabaseConfig, Is.InstanceOf<BlobContainerDatabaseConfig>());
            BlobContainerDatabaseConfig blobConfig = (BlobContainerDatabaseConfig)Configuration.Current.DatabaseConfig;
            Assert.That(blobConfig.RootPath, Is.EqualTo(""));
        }

        [Test]
        public void ConfigurationLoadTest_BlobContainerType_MissingStorageAccount_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "BlobContainer"},
                        {"DatabaseConfig:ContainerName", "testcontainer"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_BlobContainerType_MissingContainerName_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "BlobContainer"},
                        {"DatabaseConfig:StorageAccountName", "teststorageaccount"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_NoDatabaseConfigSection_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"SomeOtherKey", "value"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_InvalidDatabaseType_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "InvalidType"}
                    })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_LocalQueryStorageType_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "LocalFileSystem"},
                    {"QueryStorageConfig:SavedQueryDirectory", "/path/to/queries"},
                    {"QueryStorageConfig:ArchiveFileDirectory", "/path/to/archives"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.QueryStorageConfig, Is.InstanceOf<LocalQueryStorageConfig>());
            LocalQueryStorageConfig localQConfig = (LocalQueryStorageConfig)Configuration.Current.QueryStorageConfig;
            Assert.That(localQConfig.SavedQueryDirectory, Is.EqualTo("/path/to/queries"));
            Assert.That(localQConfig.ArchiveFileDirectory, Is.EqualTo("/path/to/archives"));
        }

        [Test]
        public void ConfigurationLoadTest_LocalQueryStorageType_MissingDirectory_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "LocalFileSystem"},
                    {"QueryStorageConfig:SavedQueryDirectory", "/path/to/queries"}
                })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void ConfigurationLoadTest_BlobQueryStorageType_Success()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "BlobContainer"},
                    {"QueryStorageConfig:StorageAccountName", "teststorage"},
                    {"QueryStorageConfig:ContainerName", "testcontainer"},
                    {"QueryStorageConfig:SavedQueryPath", "custom-queries"},
                    {"QueryStorageConfig:ArchiveFilePath", "custom-archives"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.QueryStorageConfig, Is.InstanceOf<BlobQueryStorageConfig>());
            BlobQueryStorageConfig blobQConfig = (BlobQueryStorageConfig)Configuration.Current.QueryStorageConfig;
            Assert.That(blobQConfig.StorageAccountName, Is.EqualTo("teststorage"));
            Assert.That(blobQConfig.ContainerName, Is.EqualTo("testcontainer"));
            Assert.That(blobQConfig.SavedQueryPath, Is.EqualTo("custom-queries"));
            Assert.That(blobQConfig.ArchiveFilePath, Is.EqualTo("custom-archives"));
        }

        [Test]
        public void ConfigurationLoadTest_BlobQueryStorageType_EmptyPathsDefaultToEmpty()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "BlobContainer"},
                    {"QueryStorageConfig:StorageAccountName", "teststorage"},
                    {"QueryStorageConfig:ContainerName", "testcontainer"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.QueryStorageConfig, Is.InstanceOf<BlobQueryStorageConfig>());
            BlobQueryStorageConfig blobQConfig = (BlobQueryStorageConfig)Configuration.Current.QueryStorageConfig;
            Assert.That(blobQConfig.SavedQueryPath, Is.EqualTo(""));
            Assert.That(blobQConfig.ArchiveFilePath, Is.EqualTo(""));
        }

        [Test]
        public void ConfigurationLoadTest_BlobQueryStorageType_MissingStorageAccount_ThrowsException()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "BlobContainer"},
                    {"QueryStorageConfig:ContainerName", "testcontainer"}
                })
                .Build();

            Assert.Throws<InvalidConfigurationException>(() => Configuration.Load(configuration));
        }

        [Test]
        public void SetQueryDirectoryFields_WithBlobQueryStorage_SetsLegacyFields()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "BlobContainer"},
                    {"QueryStorageConfig:StorageAccountName", "teststorage"},
                    {"QueryStorageConfig:ContainerName", "testcontainer"},
                    {"QueryStorageConfig:SavedQueryPath", "custom-saved"},
                    {"QueryStorageConfig:ArchiveFilePath", "custom-archive"}
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
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"QueryStorageConfig:Type", "LocalFileSystem"},
                    {"QueryStorageConfig:SavedQueryDirectory", "/local/saved"},
                    {"QueryStorageConfig:ArchiveFileDirectory", "/local/archive"}
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
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
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
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"},
                    {"savedQueryDirectory", "/legacy/saved"},
                    {"archiveFileDirectory", "/legacy/archive"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.QueryStorageConfig, Is.InstanceOf<LocalQueryStorageConfig>());
            LocalQueryStorageConfig legacyQConfig = (LocalQueryStorageConfig)Configuration.Current.QueryStorageConfig;
            Assert.That(legacyQConfig.SavedQueryDirectory, Is.EqualTo("/legacy/saved"));
            Assert.That(legacyQConfig.ArchiveFileDirectory, Is.EqualTo("/legacy/archive"));
            Assert.That(Configuration.Current.SavedQueryDirectory, Is.EqualTo("/legacy/saved"));
            Assert.That(Configuration.Current.ArchiveFileDirectory, Is.EqualTo("/legacy/archive"));
        }

        [Test]
        public void ConfigurationLoadTest_NoQueryStorageConfig_ReturnsNull()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {"DatabaseConfig:Type", "PxWeb"},
                    {"DatabaseConfig:PxWebUrl", "http://test.com"}
                })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.QueryStorageConfig, Is.Null);
        }

        [Test]
        public void ConfigurationLoadTest_DatabaseConfigTypeCaseInsensitive()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"DatabaseConfig:Type", "pxweb"},
                        {"DatabaseConfig:PxWebUrl", "http://test.com"}
                    })
                .Build();

            Configuration.Load(configuration);

            Assert.That(Configuration.Current.DatabaseConfig, Is.InstanceOf<PxWebDatabaseConfig>());
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

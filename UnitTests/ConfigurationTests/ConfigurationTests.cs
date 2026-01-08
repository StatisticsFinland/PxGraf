using Microsoft.Extensions.Configuration;
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
    }
}

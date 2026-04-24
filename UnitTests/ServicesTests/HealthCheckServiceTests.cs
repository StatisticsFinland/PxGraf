using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Responses;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    internal class HealthCheckServiceTests
    {
        private Mock<ICachedDatasource> _mockDatasource;
        private Mock<ISqFileInterface> _mockSqFileInterface;
        private Mock<IPublicationWebhookService> _mockWebhookService;
        private Mock<ILogger<HealthCheckService>> _mockLogger;

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [SetUp]
        public void Setup()
        {
            _mockDatasource = new Mock<ICachedDatasource>();
            _mockSqFileInterface = new Mock<ISqFileInterface>();
            _mockWebhookService = new Mock<IPublicationWebhookService>();
            _mockLogger = new Mock<ILogger<HealthCheckService>>();
        }

        private static void LoadPxWebConfig(bool enableWebhook = false, bool enableWebhookHealthCheck = true, bool creationAPIEnabled = true)
        {
            Dictionary<string, string> config = TestInMemoryConfiguration.Get();
            config["FeatureManagement:CreationAPI"] = creationAPIEnabled.ToString();
            if (enableWebhook)
            {
                config["PublicationWebhookConfiguration:BaseUrl"] = "https://example.com";
                config["PublicationWebhookConfiguration:WebhookEndpointPath"] = "/webhook";
                if (enableWebhookHealthCheck)
                {
                    config["PublicationWebhookConfiguration:HealthCheckEndpointPath"] = "/info";
                }
                config["PublicationWebhookConfiguration:BodyContentPropertyNames:0"] = "id";
            }

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
            Configuration.Load(configuration);
        }

        private static void LoadLocalFilesystemConfig()
        {
            Dictionary<string, string> config = TestInMemoryConfiguration.Get();
            config["DatabaseConfig:Type"] = "LocalFileSystem";
            config["DatabaseConfig:DatabaseRootPath"] = "/test/path";
            config["DatabaseConfig:Encoding"] = "utf-8";

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
            Configuration.Load(configuration);
        }

        private static void LoadBlobContainerConfig()
        {
            Dictionary<string, string> config = TestInMemoryConfiguration.Get();
            config["DatabaseConfig:Type"] = "BlobContainer";
            config["DatabaseConfig:StorageAccountName"] = "testaccount";
            config["DatabaseConfig:ContainerName"] = "testcontainer";

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
            Configuration.Load(configuration);
        }

        private HealthCheckService BuildService()
        {
            return new HealthCheckService(
                _mockDatasource.Object,
                _mockSqFileInterface.Object,
                _mockWebhookService.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task CheckHealthAsync_AllProbesHealthy_PxWebConfig_ReturnsHealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Databases, Has.Count.EqualTo(1));
                Assert.That(result.Databases[0].Id, Is.EqualTo("database"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("healthy"));
                Assert.That(result.Services, Has.Count.EqualTo(2));
                Assert.That(result.Services.All(s => s.Status == "healthy"), Is.True);
            }
        }

        [Test]
        public async Task CheckHealthAsync_AllProbesHealthy_LocalFilesystemConfig_ReturnsHealthy()
        {
            // Arrange
            LoadLocalFilesystemConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Databases, Has.Count.EqualTo(1));
                Assert.That(result.Databases[0].Id, Is.EqualTo("database"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("healthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_AllProbesHealthy_BlobContainerConfig_ReturnsHealthy()
        {
            // Arrange
            LoadBlobContainerConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Databases, Has.Count.EqualTo(1));
                Assert.That(result.Databases[0].Id, Is.EqualTo("database"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("healthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_DatabaseProbeThrows_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ThrowsAsync(new Exception("Connection refused"));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                Assert.That(result.Databases[0].Id, Is.EqualTo("database"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_SavedQueryStorageProbeThrows_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Directory not found"));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus savedQueryStatus = result.Services.First(s => s.Id == "saved-query-storage");
                Assert.That(savedQueryStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_ArchiveStorageProbeThrows_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Blob storage unavailable"));

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus archiveStatus = result.Services.First(s => s.Id == "archive-file-storage");
                Assert.That(archiveStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_SavedQueryStorageReturnsFalse_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus savedQueryStatus = result.Services.First(s => s.Id == "saved-query-storage");
                Assert.That(savedQueryStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_ArchiveStorageReturnsFalse_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus archiveStatus = result.Services.First(s => s.Id == "archive-file-storage");
                Assert.That(archiveStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookProbeThrows_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockWebhookService
                .Setup(ws => ws.CheckWebhookReachabilityAsync())
                .ThrowsAsync(new Exception("Webhook endpoint unreachable"));

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus webhookStatus = result.Services.First(s => s.Id == "publication-webhook");
                Assert.That(webhookStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookProbeReturnsFalse_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockWebhookService
                .Setup(ws => ws.CheckWebhookReachabilityAsync())
                .ReturnsAsync(false);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                ServiceHealthStatus webhookStatus = result.Services.First(s => s.Id == "publication-webhook");
                Assert.That(webhookStatus.Status, Is.EqualTo("unhealthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookNotConfigured_NoWebhookEntry_StillHealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: false);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Services.Any(s => s.Id == "publication-webhook"), Is.False);
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookConfigured_AllHealthy_IncludesWebhookEntry()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockWebhookService
                .Setup(ws => ws.CheckWebhookReachabilityAsync())
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Services, Has.Count.EqualTo(3));
                ServiceHealthStatus webhookStatus = result.Services.First(s => s.Id == "publication-webhook");
                Assert.That(webhookStatus.Status, Is.EqualTo("healthy"));
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookEnabledButNoHealthCheckPath_NoWebhookEntry_StillHealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true, enableWebhookHealthCheck: false);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Services.Any(s => s.Id == "publication-webhook"), Is.False);
                _mockWebhookService.Verify(ws => ws.CheckWebhookReachabilityAsync(), Times.Never);
            }
        }

        [Test]
        public async Task CheckHealthAsync_WebhookEnabledButCreationAPIDisabled_NoWebhookEntry_StillHealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true, creationAPIEnabled: false);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("healthy"));
                Assert.That(result.Services.Any(s => s.Id == "publication-webhook"), Is.False);
                _mockWebhookService.Verify(ws => ws.CheckWebhookReachabilityAsync(), Times.Never);
            }
        }

        [Test]
        public async Task CheckHealthAsync_MixedHealthy_DatabaseFailsOthersSucceed_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig(enableWebhook: true);
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ThrowsAsync(new Exception("PxWeb unreachable"));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockWebhookService
                .Setup(ws => ws.CheckWebhookReachabilityAsync())
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("unhealthy"));
                Assert.That(result.Services.All(s => s.Status == "healthy"), Is.True);
            }
        }

        [Test]
        public async Task CheckHealthAsync_MixedHealthy_DatabaseSucceedsStorageFails_ReturnsUnhealthy()
        {
            // Arrange
            LoadPxWebConfig();
            _mockDatasource
                .Setup(ds => ds.GetGroupContentsCachedAsync(It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new DatabaseGroupContents([], []));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessSavedQueriesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Storage unavailable"));
            _mockSqFileInterface
                .Setup(sq => sq.CanAccessArchivesAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            HealthCheckService service = BuildService();

            // Act
            HealthResponse result = await service.CheckHealthAsync();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Status, Is.EqualTo("unhealthy"));
                Assert.That(result.Databases[0].Status, Is.EqualTo("healthy"));
                ServiceHealthStatus savedQueryStatus = result.Services.First(s => s.Id == "saved-query-storage");
                Assert.That(savedQueryStatus.Status, Is.EqualTo("unhealthy"));
            }
        }
    }
}

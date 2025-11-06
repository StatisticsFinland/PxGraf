using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    public class PublicationWebhookServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<ILogger<PublicationWebhookService>> _mockLogger;
        private Mock<ICachedDatasource> _mockDatasource;

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<PublicationWebhookService>>();
            _mockDatasource = new Mock<ICachedDatasource>();
        }

        // TODO: One line function, no need for this to be a separate method
        private HttpClient CreateHttpClient()
        {
            return new HttpClient(_mockHttpMessageHandler.Object);
        }

        // TODO: Combine this with ConfigureWebhookDisabled below with parameterization
        private static void ConfigureWebhookEnabled()
        {
            // Set up configuration with webhook enabled using TestInMemoryConfiguration as base
            Dictionary<string, string> configDict = TestInMemoryConfiguration.Get().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            configDict.Add("PublicationWebhookConfiguration:EndpointUrl", "https://example.com/webhook");
            configDict.Add("PublicationWebhookConfiguration:AccessTokenHeaderName", "Authorization");
            configDict.Add("PublicationWebhookConfiguration:AccessTokenHeaderValue", "Bearer test-token");
            configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:0", "id");
            configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:1", "archived");

            IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
            Configuration.Load(configuration);
        }

        private static void ConfigureWebhookDisabled()
        {
            // Set up configuration with webhook disabled
            Dictionary<string, string> configDict = TestInMemoryConfiguration.Get().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            IConfiguration configuration = new ConfigurationBuilder()
                      .AddInMemoryCollection(configDict)
                         .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task TriggerWebhookAsync_ConfigurationDisabled_ReturnsUnpublished()
        {
            // Arrange
            ConfigureWebhookDisabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            Dictionary<string, MetaProperty> additionalProperties = [];

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify no HTTP call was made
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_QueryIsDraft_ReturnsUnpublished()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            savedQuery.Draft = true; // Set query as draft
            Dictionary<string, MetaProperty> additionalProperties = [];

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify no HTTP call was made
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_SuccessfulResponse_ReturnsSuccess()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            savedQuery.Draft = false; // Ensure query is not a draft
            Dictionary<string, MetaProperty> additionalProperties = [];

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Success));

            // Verify HTTP call was made once
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_FailedResponse_ReturnsSuccess()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            savedQuery.Draft = false; // Ensure query is not a draft
            Dictionary<string, MetaProperty> additionalProperties = [];

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert - Note: Even failed HTTP responses return Success as per the implementation
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Success));

            // Verify HTTP call was made once
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_HttpException_ReturnsFailed()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            savedQuery.Draft = false; // Ensure query is not a draft
            Dictionary<string, MetaProperty> additionalProperties = [];

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Failed));

            // Verify HTTP call was attempted once
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_ValidCall_SendsCorrectRequest()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = CreateHttpClient();
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery([], false, null);
            savedQuery.Draft = false; // Ensure query is not a draft
            Dictionary<string, MetaProperty> additionalProperties = [];

            HttpRequestMessage capturedRequest = null;
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            QueryPublicationStatus result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, additionalProperties);

            // Assert
            Assert.That(result, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(capturedRequest.RequestUri.ToString(), Is.EqualTo("https://example.com/webhook"));
            Assert.That(capturedRequest.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
            Assert.That(capturedRequest.Headers.Contains("Authorization"), Is.True);
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
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
        private List<DimensionParameters> _queryParams = [];
        private VisualizationSettings _visualizationSettings = null;

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
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 4),
                new DimensionParameters(DimensionType.Nominal, 2)
            ];
            MatrixMetadata testMeta = TestDataCubeBuilder.BuildTestMeta(dimParams, ["fi", "en", "sv"]);
            testMeta.AdditionalProperties.Add("DESCRIPTION", new MultilanguageStringProperty(new MultilanguageString(new Dictionary<string, string>
            {
                { "fi", "Test description (fi)" },
                { "en", "Test description (en)" },
                { "sv", "Test description (sv)" }
            })));
            _mockDatasource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(testMeta);
            _queryParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Nominal, 1)
            ];
            _visualizationSettings = new TableVisualizationSettings(new Layout()
            {
                ColumnDimensionCodes = [],
                RowDimensionCodes = []
            });
        }

        private static void ConfigureWebhookEnabled(bool enableWebhook = true)
        {
            // Set up configuration with webhook enabled using TestInMemoryConfiguration as base
            Dictionary<string, string> configDict = TestInMemoryConfiguration.Get().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (enableWebhook)
            {
                configDict.Add("PublicationWebhookConfiguration:EndpointUrl", "https://example.com/webhook");
                configDict.Add("PublicationWebhookConfiguration:AccessTokenHeaderName", "Authorization");
                configDict.Add("PublicationWebhookConfiguration:AccessTokenHeaderValue", "Bearer test-token");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:0", "id");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:1", "archived");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:2", "header");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:3", "containsselectabledimensions");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:4", "visualizationtype");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:5", "version");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:6", "draft");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:7", "creationtime");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNameEdits:id", "id_test");
                configDict.Add("PublicationWebhookConfiguration:VisualizationTypeTranslations:Table", "CustomTable");
                configDict.Add("PublicationWebhookConfiguration:MetadataProperties:0", "DESCRIPTION");
            }

            IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task TriggerWebhookAsync_ConfigurationDisabled_ReturnsUnpublished()
        {
            // Arrange
            ConfigureWebhookEnabled(false);

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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

            using HttpClient httpClient = new (_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new (httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
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
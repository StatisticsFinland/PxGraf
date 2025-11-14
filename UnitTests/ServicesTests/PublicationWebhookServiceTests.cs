using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
        private MatrixMetadata _testMeta = null;

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
            _testMeta = TestDataCubeBuilder.BuildTestMeta(dimParams, ["fi", "en", "sv"]);
            _mockDatasource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync(_testMeta);
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
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNames:8", "tablereference");
                configDict.Add("PublicationWebhookConfiguration:BodyContentPropertyNameEdits:id", "id_test");
                configDict.Add("PublicationWebhookConfiguration:VisualizationTypeTranslations:Table", "CustomTable");
                configDict.Add("PublicationWebhookConfiguration:MetadataProperties:NOTE", "INFO");
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

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify no HTTP call was made
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_QueryIsDraft_ReturnsUnpublished()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = true; // Set query as draft

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Unpublished));

            // Verify no HTTP call was made
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_SuccessfulResponse_ReturnsSuccess()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            VisualizationSettings lineVisualizationSettings = new LineChartVisualizationSettings(new Layout()
            {
                ColumnDimensionCodes = [],
                RowDimensionCodes = []
            },
            false,
            null);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, lineVisualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            // Mock successful response with empty content to test null message handling
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(result.Messages, Is.Null); // Empty response should return null

            // Verify HTTP call was made once
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_FailedResponse_ReturnsFailed()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert - Note: Even failed HTTP responses return Failed as per the implementation
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Failed));

            // Verify HTTP call was made once
            _mockHttpMessageHandler.Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_HttpException_ReturnsFailed()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            _mockHttpMessageHandler.Protected()
              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Failed));
            Assert.That(result.Messages, Is.Not.Null);
            Assert.That(result.Messages.Languages.Contains("error"), Is.True);
            Assert.That(result.Messages["error"], Is.EqualTo("Network error"));

            // Verify HTTP call was attempted once
            _mockHttpMessageHandler.Protected()
             .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task TriggerWebhookAsync_ValidCall_SendsCorrectRequest_AndBody()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            MultilanguageString chartHeaderEdit = new(new Dictionary<string, string>()
            {
                ["sv"] = "Edited header sv"
            });
            savedQuery.Query.ChartHeaderEdit = chartHeaderEdit;
            savedQuery.Draft = false; // Ensure query is not a draft

            HttpRequestMessage capturedRequest = null;
            string capturedBody = null;
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    capturedRequest = request;
                    // Read body BEFORE the request gets disposed by the caller
                    capturedBody = request.Content?.ReadAsStringAsync(_).GetAwaiter().GetResult();
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert basic request properties
            Assert.Multiple(() =>
            {
                Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
                Assert.That(capturedRequest, Is.Not.Null);
                Assert.That(capturedRequest.Method, Is.EqualTo(HttpMethod.Post));
                Assert.That(capturedRequest.RequestUri.ToString(), Is.EqualTo("https://example.com/webhook"));
                Assert.That(capturedRequest.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                Assert.That(capturedRequest.Headers.Contains("Authorization"), Is.True);

                Assert.That(string.IsNullOrWhiteSpace(capturedBody), Is.False, "Webhook body should not be empty");
                using JsonDocument doc = JsonDocument.Parse(capturedBody);
                JsonElement root = doc.RootElement;
                Assert.That(root.TryGetProperty("id_test", out JsonElement idEl), Is.True);
                Assert.That(idEl.GetString(), Is.EqualTo("test-id"));
                Assert.That(root.TryGetProperty("Archived", out JsonElement archivedEl), Is.True, "Expected archived field missing");
                Assert.That(archivedEl.GetBoolean(), Is.False);
                Assert.That(root.TryGetProperty("VisualizationType", out JsonElement vizEl), Is.True, "Expected visualizationtype field missing");
                Assert.That(vizEl.GetString(), Is.EqualTo("CustomTable"));
                Assert.That(root.TryGetProperty("ContainsSelectableDimensions", out JsonElement selectableEl), Is.True, "Expected containsselectabledimensions field missing");
                Assert.That(selectableEl.GetBoolean(), Is.False);
                Assert.That(root.TryGetProperty("TableReference", out JsonElement tableRefEl), Is.True, "Expected tablereference field missing");
                Assert.That(tableRefEl.GetString().EndsWith("TestPxFile.px"), Is.True);
                Assert.That(root.TryGetProperty("Draft", out JsonElement draftEl), Is.True, "Expected draft field missing");
                Assert.That(draftEl.GetBoolean(), Is.False);
                Assert.That(root.TryGetProperty("INFO", out _), Is.True, "Expected INFO (mapped NOTE) metadata field missing");
                Assert.That(root.TryGetProperty("CreationTime", out _), Is.True, "Expected creationtime field missing");
                Assert.That(root.TryGetProperty("Version", out JsonElement versionEl), Is.True, "Expected version field missing");
                Assert.That(versionEl.GetString(), Is.EqualTo("1.2"));

                Assert.That(root.TryGetProperty("Header", out JsonElement headerEl), Is.True, "Expected header field missing");
                if (headerEl.ValueKind == JsonValueKind.Object && headerEl.TryGetProperty("result", out JsonElement headerResultEl) && headerResultEl.ValueKind == JsonValueKind.Object)
                {
                    Assert.That(headerResultEl.TryGetProperty("fi", out JsonElement fiHeader), Is.True, "Missing fi header inside header.result");
                    Assert.That(fiHeader.GetString(), Is.EqualTo("value-0 2000-2003 muuttujana variable-2"));
                    Assert.That(headerResultEl.TryGetProperty("en", out JsonElement enHeader), Is.True, "Missing en header inside header.result");
                    Assert.That(enHeader.GetString(), Is.EqualTo("value-0.en in 2000.en to 2003.en by variable-2.en"));
                    Assert.That(headerResultEl.TryGetProperty("sv", out JsonElement svHeader), Is.True, "Missing sv header inside header.result");
                    Assert.That(svHeader.GetString(), Is.EqualTo("Edited header sv"));
                }
                else
                {
                    Assert.Fail("header field is not in the expected format");
                }

                if (root.TryGetProperty("creationtime", out JsonElement creationEl) && creationEl.ValueKind == JsonValueKind.String)
                {
                    string creationStr = creationEl.GetString();
                    Assert.DoesNotThrow(() => _ = DateTime.Parse(creationStr, CultureInfo.InvariantCulture), "creationtime should be a valid DateTime string");
                }
            });
        }

        [Test]
        public async Task TriggerWebhookAsync_NullResponse_ReturnsNullMessages()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = null });

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(result.Messages, Is.Null);
        }

        [Test]
        public async Task TriggerWebhookAsync_EmptyResponse_ReturnsNullMessages()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("") });

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(result.Messages, Is.Null);
        }

        [Test]
        public async Task TriggerWebhookAsync_ValidWebhookResponse_ReturnsMessages()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            // Create a proper WebhookResponse with localized messages
            WebhookResponse webhookResponse = new ()
            {
                Messages = new MultilanguageString(new Dictionary<string, string>
                {
                    ["fi"] = "Julkaisu onnistui",
                    ["en"] = "Publication successful",
                    ["sv"] = "Publicering lyckades"
                })
            };

            string responseContent = JsonSerializer.Serialize(webhookResponse);

            _mockHttpMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
             {
                 Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
             });

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(result.Messages, Is.Not.Null);
            Assert.That(result.Messages.Languages.Count, Is.EqualTo(3));
            Assert.That(result.Messages["fi"], Is.EqualTo("Julkaisu onnistui"));
            Assert.That(result.Messages["en"], Is.EqualTo("Publication successful"));
            Assert.That(result.Messages["sv"], Is.EqualTo("Publicering lyckades"));
        }

        [Test]
        public async Task TriggerWebhookAsync_InvalidJsonResponse_ReturnsErrorMessage()
        {
            // Arrange
            ConfigureWebhookEnabled();

            using HttpClient httpClient = new(_mockHttpMessageHandler.Object);
            PublicationWebhookService webhookService = new(httpClient, _mockLogger.Object, _mockDatasource.Object);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(_queryParams, false, _visualizationSettings);
            savedQuery.Draft = false; // Ensure query is not a draft

            _mockHttpMessageHandler.Protected()
              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
              {
                  Content = new StringContent("invalid json content", System.Text.Encoding.UTF8, "application/json")
              });

            // Act
            WebhookPublicationResult result = await webhookService.TriggerWebhookAsync("test-id", savedQuery, _testMeta.AdditionalProperties);

            // Assert
            Assert.That(result.Status, Is.EqualTo(QueryPublicationStatus.Success));
            Assert.That(result.Messages, Is.Not.Null);
            Assert.That(result.Messages.Languages.Contains("error"), Is.True);
            Assert.That(result.Messages["error"], Does.StartWith("Invalid response format:"));
        }
    }
}
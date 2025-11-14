using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PxGraf.Services
{
    /// <summary>
    /// Result object containing publication status and localized messages
    /// </summary>
    public class WebhookPublicationResult
    {
        public QueryPublicationStatus Status { get; set; }
        public MultilanguageString Messages { get; set; } = new MultilanguageString([]);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PublicationPropertyType
    {
        Id,
        Header,
        Archived,
        ContainsSelectableDimensions,
        VisualizationType,
        TableReference,
        CreationTime,
        Draft,
        Version
    }

    /// <summary>
    /// Service for sending publication webhook notifications when queries are saved or archived in non-draft mode.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the PublicationWebhookService.
    /// </remarks>
    /// <param name="httpClient">HTTP client for making webhook requests.</param>
    /// <param name="logger">Logger for logging webhook activities.</param>
    /// <param name="datasource">Cached datasource for retrieving metadata.</param>
    public class PublicationWebhookService(HttpClient httpClient, ILogger<PublicationWebhookService> logger, ICachedDatasource datasource) : IPublicationWebhookService
    {
        private readonly PublicationWebhookConfiguration _config = Configuration.Current.PublicationWebhookConfig;

        /// <summary>
        /// Triggers a webhook notification for a published query.
        /// </summary>
        /// <param name="queryId">The ID of the saved query.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <param name="additionalProperties">The metadata additional properties dictionary.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<WebhookPublicationResult> TriggerWebhookAsync(string queryId, SavedQuery savedQuery, IReadOnlyDictionary<string, MetaProperty> additionalProperties)
        {
            using (logger.BeginScope(new Dictionary<string, object>()
            {
                [LoggerConstants.FUNC_NAME] = nameof(TriggerWebhookAsync),
                [LoggerConstants.SQ_ID] = queryId
            }))
            {
                if (!_config.IsEnabled)
                {
                    logger.LogDebug("Publication webhook is not configured or disabled. Skipping webhook trigger.");
                    return new WebhookPublicationResult { Status = QueryPublicationStatus.Unpublished };
                }

                if (savedQuery.Draft)
                {
                    logger.LogDebug("Query is in draft mode. Webhook will not be triggered.");
                    return new WebhookPublicationResult { Status = QueryPublicationStatus.Unpublished };
                }

                try
                {
                    Dictionary<string, object> webhookBody = BuildWebhookBody(queryId, savedQuery, additionalProperties);
                    string jsonContent = JsonSerializer.Serialize(webhookBody, GlobalJsonConverterOptions.Default);

                    using HttpRequestMessage request = new(HttpMethod.Post, _config.EndpointUrl);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    if (_config.HasAccessToken)
                    {
                        request.Headers.Add(_config.AccessTokenHeaderName, _config.AccessTokenHeaderValue);
                    }

                    logger.LogInformation("Sending publication webhook for query to {EndpointUrl}", _config.EndpointUrl);

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    MultilanguageString messages = await ExtractMessagesFromResponse(response);

                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Publication webhook for query sent successfully. Status: {StatusCode}", response.StatusCode);
                        return new WebhookPublicationResult
                        {
                            Status = QueryPublicationStatus.Success,
                            Messages = messages
                        };
                    }
                    else
                    {
                        logger.LogWarning("Publication webhook for query failed. Status: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                        return new WebhookPublicationResult
                        {
                            Status = QueryPublicationStatus.Failed,
                            Messages = messages
                        };
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send publication webhook for query");
                    return new WebhookPublicationResult
                    {
                        Status = QueryPublicationStatus.Failed,
                        Messages = null
                    };
                }
            }
        }

        /// <summary>
        /// Builds the webhook body based on the configured property names.
        /// </summary>
        /// <param name="queryId">The ID of the saved query.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <param name="additionalProperties">The metadata additional properties dictionary.</param>
        /// <returns>A dictionary containing the webhook payload.</returns>
        private Dictionary<string, object> BuildWebhookBody(string queryId, SavedQuery savedQuery, IReadOnlyDictionary<string, MetaProperty> additionalProperties)
        {
            Dictionary<string, object> body = [];

            foreach (PublicationPropertyType propertyName in _config.BodyContentPropertyNames)
            {
                // Get the custom field name if configured, otherwise use the original property name
                string fieldName = _config.BodyContentPropertyNameEdits.TryGetValue(propertyName, out string customName)
                    ? customName
                    : propertyName.ToString();

                object value = GetPropertyValue(propertyName, queryId, savedQuery);
                if (value != null)
                {
                    body[fieldName] = value;
                }
            }

            // Process metadata properties separately
            foreach (KeyValuePair<string, string> metadataProperty in _config.MetadataProperties)
            {
                string metadataKey = metadataProperty.Key;
                string fieldName = metadataProperty.Value;

                if (additionalProperties.TryGetValue(metadataKey, out MetaProperty metaProperty))
                {
                    body[fieldName] = metaProperty switch
                    {
                        StringProperty sp => sp.Value,
                        MultilanguageStringProperty mlp => mlp.Value,
                        NumericProperty np => np.Value,
                        MultilanguageStringListProperty mllp => mllp.Value,
                        StringListProperty slp => slp.Value,
                        BooleanProperty bp => bp.Value,
                        _ => metaProperty // Fallback to the object itself for serialization
                    };
                }
            }

            return body;
        }

        /// <summary>
        /// Gets the value for a specified property name from the saved query data.
        /// </summary>
        /// <param name="propertyName"><see cref="PublicationPropertyType"/> type of the property to retrieve.</param>
        /// <param name="queryId">The query ID.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <returns>The property value, or null if the property is not recognized.</returns>
        private object GetPropertyValue(PublicationPropertyType propertyName, string queryId, SavedQuery savedQuery)
        {
            return propertyName switch
            {
                PublicationPropertyType.Id => queryId,
                PublicationPropertyType.Header => GetHeader(savedQuery.Query.ChartHeaderEdit, savedQuery),
                PublicationPropertyType.Archived => savedQuery.Archived,
                PublicationPropertyType.ContainsSelectableDimensions => savedQuery.Query.DimensionQueries.Any(q => q.Value.Selectable),
                PublicationPropertyType.VisualizationType => GetVisualizationType(savedQuery.Settings.VisualizationType),
                PublicationPropertyType.TableReference => savedQuery.Query.TableReference.ToPath(),
                PublicationPropertyType.CreationTime => savedQuery.CreationTime,
                PublicationPropertyType.Draft => savedQuery.Draft,
                PublicationPropertyType.Version => savedQuery.Version,
                _ => LogUnknownPropertyAndReturnNull(propertyName.ToString())
            };
        }

#nullable enable
        private async Task<MultilanguageString> GetHeader(MultilanguageString? headerEdit, SavedQuery query)
        {
            Dictionary<string, string> headersPerLanguage = [];
            MultilanguageString? defaultHeader = null;
            foreach (string language in Configuration.Current.LanguageOptions.Available)
            {
                if (headerEdit?.Languages.Contains(language) == true)
                {
                    headersPerLanguage[language] = headerEdit[language];
                }
                else
                {
                    if (defaultHeader == null)
                    {
                        IReadOnlyMatrixMetadata metadata = await datasource.GetMatrixMetadataCachedAsync(query.Query.TableReference);
                        defaultHeader = HeaderBuildingUtilities.GetHeader(metadata, query.Query);
                    }

                    headersPerLanguage[language] = defaultHeader[language] ?? string.Empty;
                }
            }
            return new MultilanguageString(headersPerLanguage);
        }
#nullable restore

        /// <summary>
        /// Logs an unknown property name and returns null.
        /// </summary>
        /// <param name="propertyName">The unknown property name.</param>
        /// <returns>Always returns null.</returns>
        private object LogUnknownPropertyAndReturnNull(string propertyName)
        {
            logger.LogWarning("Unknown property name in webhook configuration: {PropertyName}", propertyName);
            return null;
        }

        /// <summary>
        /// Converts the visualization type enum to a string representation using configured translations.
        /// </summary>
        /// <param name="visualizationType">The visualization type enum.</param>
        /// <returns>A string representation of the visualization type.</returns>
        private string GetVisualizationType(VisualizationType visualizationType)
        {
            string enumName = visualizationType.ToString();

            // Check if there's a custom translation for this specific visualization type
            if (_config.VisualizationTypeTranslations.TryGetValue(enumName, out string customTranslation))
            {
                return customTranslation;
            }

            // Fallback to default enum name
            return enumName;
        }

        /// <summary>
        /// Extracts localized messages from the webhook response.
        /// </summary>
        /// <param name="response">The HTTP response from the webhook.</param>
        /// <returns>MultilanguageString of localized messages or error message if parsing fails.</returns>
        private async Task<MultilanguageString> ExtractMessagesFromResponse(HttpResponseMessage response)
        {
            try
            {
                if (response?.Content == null)
                {
                    logger.LogWarning("Webhook response or response content is null");
                    return null;
                }

                string responseContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    logger.LogWarning("Webhook response content is empty or whitespace");
                    return null;
                }

                WebhookResponse webhookResponse = JsonSerializer.Deserialize<WebhookResponse>(responseContent, GlobalJsonConverterOptions.Default);

                if (webhookResponse?.Messages != null && webhookResponse.Messages.Languages.Any())
                {
                    return webhookResponse.Messages;
                }

                logger.LogWarning("Webhook response content could not be parsed as valid message format. Content: {ResponseContent}", responseContent);
                return null;
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse webhook response as JSON. Response content: {ResponseContent}", response?.Content);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while extracting messages from webhook response");
                return null;
            }
        }
    }
}
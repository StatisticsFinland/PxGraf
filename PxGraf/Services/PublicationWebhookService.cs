using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Models.Metadata;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PxGraf.Services
{
    /// <summary>
    /// Service interface for publication webhooks.
    /// </summary>
    public interface IPublicationWebhookService
    {
        /// <summary>
        /// Triggers a webhook notification for a published query.
        /// </summary>
        /// <param name="queryId">The ID of the saved query.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <param name="additionalProperties">The metadata additional properties dictionary.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<QueryPublicationStatus> TriggerWebhookAsync(string queryId, SavedQuery savedQuery, IReadOnlyDictionary<string, MetaProperty> additionalProperties);
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
        public async Task<QueryPublicationStatus> TriggerWebhookAsync(string queryId, SavedQuery savedQuery, IReadOnlyDictionary<string, MetaProperty> additionalProperties)
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
                    return QueryPublicationStatus.Unpublished;
                }

                if (savedQuery.Draft)
                {
                    logger.LogDebug("Query is in draft mode. Webhook will not be triggered.");
                    return QueryPublicationStatus.Unpublished;
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

                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Publication webhook for query sent successfully. Status: {StatusCode}", response.StatusCode);
                    }
                    else
                    {
                        logger.LogWarning("Publication webhook for query failed. Status: {StatusCode}, Reason: {ReasonPhrase}",
                                 response.StatusCode, response.ReasonPhrase);
                    }

                    return QueryPublicationStatus.Success;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send publication webhook for query");
                    return QueryPublicationStatus.Failed;
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

            foreach (string propertyName in _config.BodyContentPropertyNames)
            {
                // Get the custom field name if configured, otherwise use the original property name
                string fieldName = _config.BodyContentPropertyNameEdits.TryGetValue(propertyName, out string customName)
                    ? customName
                    : propertyName;

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
        /// <param name="propertyName">The standard property name to retrieve.</param>
        /// <param name="queryId">The query ID.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <returns>The property value, or null if the property is not recognized.</returns>
        private object GetPropertyValue(string propertyName, string queryId, SavedQuery savedQuery)
        {
            return propertyName.ToLowerInvariant() switch
            {
                "id" => queryId,
                "header" => GetHeader(savedQuery.Query.ChartHeaderEdit, savedQuery),
                "archived" => savedQuery.Archived,
                "containsselectabledimensions" => savedQuery.Query.DimensionQueries.Any(q => q.Value.Selectable),
                "visualizationtype" => GetVisualizationType(savedQuery.Settings.VisualizationType),
                "tablereference" => savedQuery.Query.TableReference.ToPath(),
                "creationtime" => savedQuery.CreationTime,
                "draft" => savedQuery.Draft,
                "version" => savedQuery.Version,
                _ => LogUnknownPropertyAndReturnNull(propertyName)
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
    }
}
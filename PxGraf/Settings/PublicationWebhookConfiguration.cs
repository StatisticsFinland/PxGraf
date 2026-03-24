using PxGraf.Services;
using System.Collections.Generic;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for publication webhooks that are triggered when queries are saved or archived in non-draft mode.
    /// </summary>
    public class PublicationWebhookConfiguration
    {
        /// <summary>
        /// The base URL of the webhook service (e.g., "https://example.com").
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The path appended to <see cref="BaseUrl"/> for webhook POST requests (e.g., "/api/publish").
        /// </summary>
        public string WebhookEndpointPath { get; set; }

        /// <summary>
        /// Optional path appended to <see cref="BaseUrl"/> for health check GET requests (e.g., "/api/info").
        /// When omitted, the health endpoint will not probe the webhook service.
        /// </summary>
        public string HealthCheckEndpointPath { get; set; }

        /// <summary>
        /// Optional name of the HTTP header used for access token authentication.
        /// </summary>
        public string AccessTokenHeaderName { get; set; }

        /// <summary>
        /// Optional access token value to be included in the HTTP header.
        /// </summary>
        public string AccessTokenHeaderValue { get; set; }

        /// <summary>
        /// List of property names to include in the webhook POST body as described by <see cref="PublicationPropertyType"/> enum.
        /// Standard supported properties: Id, ChartHeaderEdit, Archived, ContainsSelectableDimensions, VisualizationType, TableReference, Note
        /// </summary>
        public PublicationPropertyType[] BodyContentPropertyNames { get; set; }

        /// <summary>
        /// Optional dictionary to map standard property names to custom field names in the webhook body.
        /// Key: Standard property name (Id, ChartHeaderEdit, Archived, ContainsSelectableDimensions, VisualizationType, TableReference, Note) as described by <see cref="PublicationPropertyType"/> enum.
        /// Value: Custom field name to use in the webhook body
        /// </summary>
        public Dictionary<PublicationPropertyType, string> BodyContentPropertyNameEdits { get; set; } = [];

        /// <summary>
        /// Optional dictionary to translate VisualizationType enum values to custom strings.
        /// Key: VisualizationType enum name (e.g., "Table", "LineChart", "VerticalBarChart")
        /// Value: Custom string representation
        /// If not specified, defaults to "Table" for Table type and "Graph" for all others.
        /// </summary>
        public Dictionary<string, string> VisualizationTypeTranslations { get; set; } = [];

        /// <summary>
        /// Optional dictionary to map px file metadata properties to webhook body field names.
        /// Key: Px file metadata property key (e.g., "DESCRIPTION", "TITLE", "SOURCE")
        /// Value: Field name to use in the webhook body for this metadata property
        /// The values will be extracted from IReadOnlyMatrixMetadata.AdditionalProperties.
        /// </summary>
        public Dictionary<string, string> MetadataProperties { get; set; } = [];

        /// <summary>
        /// Gets the full webhook POST URL by combining <see cref="BaseUrl"/> and <see cref="WebhookEndpointPath"/>.
        /// </summary>
        public string WebhookUrl => CombineUrl(BaseUrl, WebhookEndpointPath);

        /// <summary>
        /// Gets the full health check GET URL by combining <see cref="BaseUrl"/> and <see cref="HealthCheckEndpointPath"/>.
        /// </summary>
        public string HealthCheckUrl => CombineUrl(BaseUrl, HealthCheckEndpointPath);

        /// <summary>
        /// Gets a value indicating whether the webhook configuration is valid and enabled.
        /// </summary>
        public bool IsEnabled => !string.IsNullOrEmpty(BaseUrl) && !string.IsNullOrEmpty(WebhookEndpointPath) && BodyContentPropertyNames?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a health check endpoint is configured for the webhook service.
        /// </summary>
        public bool HasHealthCheckEndpoint => !string.IsNullOrEmpty(HealthCheckUrl);

        /// <summary>
        /// Gets a value indicating whether access token authentication is configured.
        /// </summary>
        public bool HasAccessToken => !string.IsNullOrEmpty(AccessTokenHeaderName) && !string.IsNullOrEmpty(AccessTokenHeaderValue);

        private static string CombineUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(path))
            {
                return null;
            }

            return baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }
    }
}
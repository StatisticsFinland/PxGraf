using System.Collections.Generic;

namespace PxGraf.Settings
{
    /// <summary>
    /// Configuration for publication webhooks that are triggered when queries are saved or archived in non-draft mode.
    /// </summary>
    public class PublicationWebhookConfiguration
    {
        /// <summary>
        /// The endpoint URL where the webhook POST request will be sent.
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        /// Optional name of the HTTP header used for access token authentication.
        /// </summary>
        public string AccessTokenHeaderName { get; set; }

        /// <summary>
        /// Optional access token value to be included in the HTTP header.
        /// </summary>
        public string AccessTokenHeaderValue { get; set; }

        /// <summary>
        /// List of property names to include in the webhook POST body.
        /// These can be either standard names or custom names defined in BodyContentPropertyNameEdits.
        /// Standard supported properties: Id, ChartHeaderEdit, Archived, ContainsSelectableDimensions, VisualizationType, TableReference, Note
        /// </summary>
        public string[] BodyContentPropertyNames { get; set; }

        /// <summary>
        /// Optional dictionary to map standard property names to custom field names in the webhook body.
        /// Key: Standard property name (Id, ChartHeaderEdit, Archived, ContainsSelectableDimensions, VisualizationType, TableReference, Note)
        /// Value: Custom field name to use in the webhook body
        /// </summary>
        public Dictionary<string, string> BodyContentPropertyNameEdits { get; set; } = [];

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
        /// Gets a value indicating whether the webhook configuration is valid and enabled.
        /// </summary>
        public bool IsEnabled => !string.IsNullOrEmpty(EndpointUrl) && BodyContentPropertyNames?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether access token authentication is configured.
        /// </summary>
        public bool HasAccessToken => !string.IsNullOrEmpty(AccessTokenHeaderName) && !string.IsNullOrEmpty(AccessTokenHeaderValue);
    }
}
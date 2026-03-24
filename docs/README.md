# PxGraf

## Overview
PxGraf is a tool developed and maintained by Statistics Finland (Tilastokeskus) for visualizing statistical data from Px tables. It supports multiple data sources including PxWeb API integration, local Px databases with the Px.Utils library, and Azure Blob Storage for cloud-native deployments. The backend, written in C# with ASP.NET Core, provides a unified storage architecture that can fetch data from various sources and store saved queries in either local file systems or Azure Blob Storage. The frontend, written in TypeScript with React, provides a user interface for selecting and previewing data for visualizations and saving them as queries. The visualizations are drawn using PxVisualizer npm package, also developed and maintained by Statistics Finland.

The software is provided as is and Statistics Finland will **not** offer any support for setting up PxGraf or solving issues related to it.

**IMPORTANT:** PxVisualizer package uses HighCharts for rendering the visualizations. Please note that commercial use of HighCharts requires a commercial license. Non-commercial use may qualify for a free educational or personal license. Read more about licenses 
in the [HighCharts shop](https://shop.highsoft.com/?utm_source=npmjs&utm_medium=referral&utm_campaign=highchartspage&utm_content=licenseinfo).

## Core functions
- **Data visualization:** PxGraf helps users create visualization from px data. It automatically decides which visualization types are available based on the user's selections and what kind of customizations can be made. The user can then choose from available options. The data and visualizations can also be exported in different formats.
- **Saved queries:** The user can create, save, overwrite, archive and load queries. Saved queries can be shared with other users or published externally. Saved queries are stored in configurable storage backends (local file system or Azure Blob Storage) and the user is provided with an ID representing the saved query. A saved query can be overwritten if it was previously saved in draft state. The frontend save dialogue prompts this as "Publish-ready", false by default. A query is saved as draft if it's not marked as publish-ready.
- **APIs:** PxGraf's APIs provide functionality for processing the data for visualization, and saving and loading queries.

## Storage Architecture

PxGraf features a unified storage architecture that separates data source storage from saved query storage, enabling flexible deployment scenarios:

### Data Sources (Px Files)
- **Azure Blob Storage**: Cloud-native storage with automatic scaling and high availability
- **Local File System**: Direct file access for on-premises or single-instance deployments  
- **PxWeb API**: Integration with existing PxWeb installations

### Saved Queries and Archives
- **Azure Blob Storage**: Scalable cloud storage with shared access across multiple instances
- **Local File System**: Traditional local directory storage with structured or legacy configuration

### Mixed Deployment Scenarios
The architecture supports various combinations:
- **Full Cloud**: Both data and queries in Azure Blob Storage
- **Hybrid**: Data in cloud, queries local (or vice versa)
- **Traditional**: Both data and queries on local file system
- **Shared Container**: Data and queries in the same Azure storage account with organized paths

## APIs
- **Creation API:** Provides endpoints for fetching database listings and Px table metadata. It also provides functionality for metadata validation and providing required contents for the visualization editor. Can be disabled in the appsettings.json file.
- **Info API:** Provides information about the application. Its single endpoint returns the application's name, version and the environment it is running in.
- **Health API:** Provides a health check endpoint that probes all configured dependencies (database connection, saved query storage, archive storage, and optionally the publication webhook) and returns an aggregated health status. Returns HTTP 200 when all dependencies are healthy, or HTTP 503 when any dependency is unhealthy.
- **Query meta API:** Provides an endpoint that returns the metadata for a saved query given its ID.
- **Saved query API:** Used for managing saved queries. Provides endpoints for fetching a saved query, saving a new query, archiving a query and re-archiving an existing query. If publication webhook is configured, the queries can be saved as draft or publish-ready. When saved as draft, the query id will be overwritten in the next save operation. Publish-ready queries can not be overwritten and saving a publish-ready query will also trigger the publication webhook. Webhook's response is expected to contain a Messages field as a MultilanguageString that will be shown in the editor UI after the save process is completed.
- **Visualization API:** Provides an endpoint for fetching visualization data for a saved query given its ID. More information about the response format can be found in VISUALIZATION_RESPONSE.md

### More information about the APIs can be found [here](API_DOCUMENTATION.md).

### Instructions for local development can be found [here](LOCAL_SETUP.md).

### Information about production setup can be found [here](PRODUCTION_SETUP.md).

## Configuration

### appsettings.json
appsettings.json contains the configuration for the backend. It contains following sections:
#### LogOptions
Configuration for logging such as file save location and logging level. Logging uses NLog. Further configuration can be done in the nlog.config file.

The following logging features are available:
- **Standard Logging**: Configurable text-based logs with customizable log level.
- **Audit Logging**: Optional feature for tracking user actions. When enabled, logs HTTP requests with configurable headers to include in the audit trail.

LogOptions structure:
```json
"LogOptions": {
  "Folder": "<path to logs>",
  "SysId": "PxGraf",
  "Level": "Information",
  "AuditLog": {
    "Enabled": false,
    "IncludedHeaders": []
  }
}
```

#### Logging
Configuration for Application Insights log levels. This section uses the standard ASP.NET Core logging provider configuration and should **only** contain the `ApplicationInsights` provider key. General log filtering (e.g., suppressing `Microsoft.*` or `System.*` namespaces) is handled in the application code, not in this section. NLog file logging levels are controlled separately via `LogOptions.Level`.

Logging structure:
```json
"Logging": {
  "ApplicationInsights": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

#### ApplicationInsights
Connection and telemetry settings for Azure Application Insights. Log levels are configured through the `Logging` section above, not here.

ApplicationInsights structure:
```json
"ApplicationInsights": {
  "ConnectionString": "<connection string>",
  "TracesPerSecond": 30
}
```

- **ConnectionString**: Azure Application Insights connection string. Can also be provided via the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable.
- **TracesPerSecond**: Limits the number of traces sent to Application Insights per second. This replaces the old `EnableAdaptiveSampling` boolean with a more flexible approach.

#### CacheOptions
Configuration for caching PxWeb data, visualization responses and more.
#### CORS
Configuration for Cross-Origin Resource Sharing. This is used determine which domains are allowed to access the API.
#### QueryOptions
Configuration for the maximum header length and the maximum number of data points that can be included in a query.
#### Language
Configuration for the default language and the available languages in the backend.
#### DatabaseConfig
Configures the Px file data source. Set `Type` to one of: `PxWeb`, `LocalFileSystem`, or `BlobContainer`. The remaining fields in the section depend on the chosen type. Missing required fields will cause a startup error.

| Type | Required fields | Optional fields |
|------|----------------|----------------|
| `PxWeb` | `PxWebUrl` | — |
| `LocalFileSystem` | `DatabaseRootPath`, `Encoding` | — |
| `BlobContainer` | `StorageAccountName`, `ContainerName` | `RootPath`, `ManagedIdentityClientId` |

Example:
```json
"DatabaseConfig": {
  "Type": "PxWeb",
  "PxWebUrl": "http://localhost:56338/"
}
```

#### FeatureManagement
Grants or denies access to the Creation API.
#### DatabaseWhitelist
List of directory or PxWeb database names that are allowed to be accessed and shown as database root directories. If empty, everything is allowed.

#### QueryStorageConfig
Configures where saved queries and archives are stored. Set `Type` to one of: `LocalFileSystem` or `BlobContainer`. Missing required fields will cause a startup error. When this section is absent, the legacy `savedQueryDirectory`/`archiveFileDirectory` top-level settings are used as fallback.

| Type | Required fields | Optional fields |
|------|----------------|----------------|
| `LocalFileSystem` | `SavedQueryDirectory`, `ArchiveFileDirectory` | — |
| `BlobContainer` | `StorageAccountName`, `ContainerName` | `SavedQueryPath`, `ArchiveFilePath`, `ManagedIdentityClientId` |

Example:
```json
"QueryStorageConfig": {
  "Type": "LocalFileSystem",
  "SavedQueryDirectory": "C:\\queries",
  "ArchiveFileDirectory": "C:\\archives"
}
```
##### savedQueryDirectory / archiveFileDirectory
**LEGACY**: These top-level settings are supported as a fallback when `QueryStorageConfig` is absent.

#### PublicationWebhookConfiguration.BaseUrl
Base URL of the webhook service (e.g., "https://example.com").
#### PublicationWebhookConfiguration.WebhookEndpointPath
Path appended to BaseUrl for webhook POST requests (e.g., "/api/publish").
#### PublicationWebhookConfiguration.HealthCheckEndpointPath
Optional path appended to BaseUrl for health check GET requests (e.g., "/api/info"). When omitted, the health endpoint will not probe the webhook service.
####  PublicationWebhookConfiguration.AccessTokenHeaderName
Optional HTTP header name for access token authentication.
####  PublicationWebhookConfiguration.AccessTokenHeaderValue
Optional access token value.
####  PublicationWebhookConfiguration.BodyContentPropertyNames
List of property names to include in the webhook body. Described by PublicationPropertyType enum.
####  PublicationWebhookConfiguration.BodyContentPropertyNameEdits
Optional mapping of standard property names to custom field names.
####  PublicationWebhookConfiguration.VisualizationTypeTranslations
Optional mapping of VisualizationType enum values to custom strings.
####  PublicationWebhookConfiguration.MetadataProperties
Optional mapping of px file metadata property keys to webhook body field names.

### Configuration Notes

- **DatabaseConfig is required**: The application will fail to start if the `DatabaseConfig` section is absent or has an invalid `Type`.
- **QueryStorageConfig is optional**: When absent, the legacy `savedQueryDirectory`/`archiveFileDirectory` top-level keys are used. If neither is present, query storage is unconfigured.
- **Type values are case-insensitive**: `"pxweb"`, `"PxWeb"`, and `"PXWEB"` are all valid.
- **ManagedIdentityClientId**: When using Azure Blob Storage (`BlobContainer`) for either `DatabaseConfig` or `QueryStorageConfig`, you can optionally specify a `ManagedIdentityClientId`.
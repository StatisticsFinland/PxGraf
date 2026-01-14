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

#### ApplicationInsights
Configuration for Azure Application Insights integration for comprehensive telemetry. Application Insights logging is handled through the ASP.NET Core logging pipeline rather than NLog.

ApplicationInsights structure:
```json
"ApplicationInsights": {
  "ConnectionString": "<connection string>",
  "EnableAdaptiveSampling": false,
  "MinLevel": "Information"
}
```

- **ConnectionString**: Azure Application Insights connection string. Can also be provided via the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable.
- **EnableAdaptiveSampling**: Whether to enable adaptive sampling (defaults to false to ensure all configured logs are captured).
- **MinLevel**: Minimum log level to send to Application Insights (defaults to Information).

#### CacheOptions
Configuration for caching PxWeb data, visualization responses and more.
#### CORS
Configuration for Cross-Origin Resource Sharing. This is used determine which domains are allowed to access the API.
#### QueryOptions
Configuration for the maximum header length and the maximum number of data points that can be included in a query.
#### Language
Configuration for the default language and the available languages in the backend.
#### pxwebUrl
The address of the PxWeb server if in use. Required when using PxWeb API as data source.
#### savedQueryDirectory
**LEGACY**: The directory where saved query files are stored. Use `LocalQueryStorageConfig` or `BlobQueryStorageConfig` for new deployments.
#### archiveFileDirectory  
**LEGACY**: The directory where archived query files are stored. Use `LocalQueryStorageConfig` or `BlobQueryStorageConfig` for new deployments.
#### FeatureManagement
Grants or denies access to the Creation API.
#### DatabaseWhitelist
List of directory or PxWeb database names that are allowed to be accessed and shown as database root directories. If empty, everything is allowed.
#### LocalFileSystemDatabaseConfig
Optional configuration for the local database if in use.
#### LocalFileSystemDatabaseConfig.Enabled
Determines whether the local database with Px.Utils or PxWeb api is used.
#### LocalFileSystemDatabaseConfig.DatabaseRootPath
The path to the database root directory.
#### LocalFileSystemDatabaseConfig.Encoding
Name of the encoding used in the database files (e.g., "utf-8", "latin1").
#### BlobContainerDatabaseConfig
Optional configuration for Azure Blob Storage database (alternative to local filesystem database).
#### BlobContainerDatabaseConfig.Enabled
Determines whether Azure Blob Storage is used as the data source.
#### BlobContainerDatabaseConfig.StorageAccountName
Name of the Azure Storage Account containing the Px files.
#### BlobContainerDatabaseConfig.ContainerName
Name of the blob container containing the Px files.
#### BlobContainerDatabaseConfig.RootPath
Optional root path within the blob container for Px files. Useful when the same container stores multiple types of files, allowing you to organize Px files under a specific path (e.g., "database/px-files/") while keeping other files like saved queries in separate paths within the same container.
#### LocalQueryStorageConfig
Configuration for local file system storage of saved queries and archive files (replaces legacy savedQueryDirectory/archiveFileDirectory).
#### LocalQueryStorageConfig.Enabled
Whether to use the structured local query storage configuration instead of legacy individual directory settings.
#### LocalQueryStorageConfig.SavedQueryDirectory
Directory path for saved query files when using local storage.
#### LocalQueryStorageConfig.ArchiveFileDirectory
Directory path for archive files when using local storage.
#### BlobQueryStorageConfig
Configuration for Azure Blob Storage of saved queries and archive files.
#### BlobQueryStorageConfig.Enabled
Determines whether Azure Blob Storage is used for saved queries and archive files.
#### BlobQueryStorageConfig.StorageAccountName
Name of the Azure Storage Account for saved queries and archive files (can be the same as data storage).
#### BlobQueryStorageConfig.ContainerName
Name of the blob container for saved queries and archive files (can be the same as data storage).
#### BlobQueryStorageConfig.SavedQueryPath
Path within the container for saved query files (default: "saved-queries").
#### BlobQueryStorageConfig.ArchiveFilePath
Path within the container for archive files (default: "archive-files").

#### PublicationWebhookConfiguration.EndpointUrl
URL for optional publication webhook that is called when a query is saved as publish-ready.
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

### Data Source Requirements

**Important**: At least one data source must be configured for PxGraf to function properly. The application will fail to start if none of the following are configured:
- `pxwebUrl` (for PxWeb API integration)
- `LocalFileSystemDatabaseConfig` with `Enabled: true` (for local Px files)  
- `BlobContainerDatabaseConfig` with `Enabled: true` (for Azure Blob Storage Px files)

### Storage Configuration Compatibility  

**Legacy Compatibility**: The system automatically populates the legacy `savedQueryDirectory` and `archiveFileDirectory` fields based on your active storage configuration:
- **BlobQueryStorageConfig enabled** → Uses `SavedQueryPath` and `ArchiveFilePath`
- **LocalQueryStorageConfig enabled** → Uses `SavedQueryDirectory` and `ArchiveFileDirectory`  
- **Legacy configuration** → Uses `savedQueryDirectory` and `archiveFileDirectory` directly

This ensures backward compatibility with existing code while enabling new storage backends.
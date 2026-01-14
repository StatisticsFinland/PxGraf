# Production setup

Because technical details or requirements of the target environment will vary, here are some high-level examples to give an idea how the PxGraf can be set up.

Statistics Finland will **not** offer any support for the production setup of PxGraf. The following examples are provided as is and are not guaranteed to work in all environments.

Some good things to know:
	- The backend is stateless, it can be scaled horizontally as long as the instances can access the query files.
	- The backend does not support authentication, if you want to limit access to the creation API for example that needs to be handled outside of PxGraf.
	- The query files are read-only once they are created. PxGraf never edits or deletes them.
	- The frontend SPA is a static website, it can be served by any web server that can serve static files. (The backend serves it by default.)

## Only one PxGraf instance

This is the simplest setup. A single instance if PxGraf that serves the frontend SPA and runs the visualization API.

![PxGraf setup with one instance](/docs/pxgraf_setup_1.png)

## Two PxGraf instances

In some cases it might be beneficial to have separate instances for the creation api and the visualization api. This enables authentication on instance basis and independent scaling for example. All of the instances must access the same query files and the data received from the data sources must be identical if there are more than one.

![PxGraf setup with two instances](/docs/pxgraf_setup_2.png)
- **Migration**: Useful for transitional scenarios when moving from PxWeb to PxGraf

## Production Logging Considerations

### Application Insights
In production environments, Application Insights provides comprehensive telemetry for monitoring:
- **Connection String**: For production, set the Application Insights connection string using environment variables rather than configuration files:
  ```
  APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=...;IngestionEndpoint=...
  ```
- **Configuration**: Application Insights is configured in the `ApplicationInsights` section of appsettings.json:
  ```json
  "ApplicationInsights": {
    "ConnectionString": "", // Leave empty in config, use environment variable instead
    "EnableAdaptiveSampling": false, // Disable to ensure all logs are captured
    "MinLevel": "Information" // Set appropriate minimum level for production
  }
  ```
- **Environment Variable Priority**: The `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable takes priority over the configuration file setting

### Audit Logging
For compliance and security tracking:
- **Included Headers**: In production, configure `LogOptions.AuditLog.IncludedHeaders` to include security-relevant headers like information about the user or request origin.

## Storage Architecture

PxGraf is built upon unified storage architecture that supports multiple storage backends for both data sources and saved queries, enabling flexible deployment scenarios from local development to cloud-native setups.

### Data Source Storage

#### Azure Blob Storage
For cloud-native deployments:
- **Authentication**: Uses Azure Managed Identity via DefaultAzureCredential in production environments
- **Security**: Supports Azure RBAC and private endpoints for secure access
- **Configuration**: Requires `BlobContainerDatabaseConfig.StorageAccountName` and `BlobContainerDatabaseConfig.ContainerName`
- **Organization**: Optional `BlobContainerDatabaseConfig.RootPath` allows organizing Px files under a specific path within the container

#### Local File System
For on-premises or VM-based deployments:
- **Performance**: Direct file access provides good performance for local/on-prem scenarios
- **Simplicity**: No external dependencies beyond the file system
- **Configuration**: Requires `LocalFileSystemDatabaseConfig.DatabaseRootPath` and encoding settings

#### PxWeb API Integration
For integration with existing PxWeb installations:
- **Compatibility**: Works with existing PxWeb infrastructure
- **Configuration**: Requires `pxwebUrl` pointing to the PxWeb API endpoint

### Saved Query Storage

#### Azure Blob Storage
Store queries and archives in the cloud:
- **Organization**: Separate paths for queries (`SavedQueryPath`) and archives (`ArchiveFilePath`) are supported
- **Authentication**: Uses the same Managed Identity approach as data sources
- **Configuration**: Can use the same or different storage account as data sources

#### Local File System
For simpler deployments or regulatory requirements:
- **Legacy Support**: Maintains backward compatibility with existing deployments  
- **Structured Configuration**: New `LocalQueryStorageConfig` provides better organization
- **Direct Access**: Fast access when PxGraf runs on the same machine as storage

### Mixed Storage Scenarios

The architecture supports flexible combinations such as:
- Data sources in Azure Blob Storage with saved queries on local file system
- Both data sources and saved queries in Azure Blob Storage for fully cloud-native deployments
- Data sources on local file system with saved queries in Azure Blob Storage
- Data sources and saved queries on separate Storage Accounts in Azure Blob Storage
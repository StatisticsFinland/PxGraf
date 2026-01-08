# Production setup

Because technical details or requirements of the target environment will vary, here are some high-level examples to give an idea how the PxGraf can be set up.

Statistics Finland will **not** offer any support for the production setup of PxGraf. The following examples are provided as is and are not guaranteed to work in all environments.

Some good things to know:
	- The backend is stateless, it can be scaled horizontally as long as the instances can access the query files.
	- The backend does not support authentication, if you want to limit access to the creation API for example that needs to be handled outside of PxGraf.
	- The query files are read-only once they are created. PxGraf never edits or deletes them.
	- The frontend SPA is a static website, it can be served by any web server that can serve static files. (The backend serves it by default.)

## Production Logging Considerations

### Application Insights
In production environments, Application Insights provides comprehensive telemetry for monitoring:
- **Connection String**: For production, set the Application Insights connection string using environment variables rather than configuration files:
  ```
  PXGRAF_APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=...;IngestionEndpoint=...
  ```
- **Disabled in Development**: Application Insights is automatically disabled in development environments (when running in DEBUG configuration)

### Audit Logging
For compliance and security tracking:
- **Included Headers**: In production, configure `LogOptions.AuditLog.IncludedHeaders` to include security-relevant headers like information about the user or request origin.

## Data Source Considerations

PxGraf supports three types of data sources:

### Azure Blob Storage
For cloud-native deployments:
- **Authentication**: Uses Azure Managed Identity via DefaultAzureCredential in production environments
- **Scalability**: Highly scalable and available through Azure's global infrastructure  
- **Security**: Supports Azure RBAC and private endpoints for secure access
- **Configuration**: Requires `BlobContainerDatabaseConfig.StorageAccountName` and `BlobContainerDatabaseConfig.ContainerName`
- **Organization**: Optional `BlobContainerDatabaseConfig.RootPath` allows organizing Px files under a specific path within the container, enabling shared containers for multiple file types

### Local File System
For on-premises or VM-based deployments:
- **Performance**: Direct file access provides good performance for local/on-prem scenarios
- **Simplicity**: No external dependencies beyond the file system
- **Configuration**: Requires `LocalFileSystemDatabaseConfig.DatabaseRootPath` and encoding settings

### PxWeb API
For integration with existing PxWeb installations:
- **Compatibility**: Works with existing PxWeb infrastructure
- **Configuration**: Requires `pxwebUrl` pointing to the PxWeb API endpoint

## Only one PxGraf instance

This is the simplest setup. A single instance if PxGraf that serves the frontend SPA and runs the visualization API.

![PxGraf setup with one instance](/docs/pxgraf_setup_1.png)

## Two PxGraf instances

In some cases it might be beneficial to have separate instances for the creation api and the visualization api. This enables authentication on instance basis and independent scaling for example. All of the instances must access the same query files and the data received from the data sources must be identical if there are more than one.

![PxGraf setup with two instances](/docs/pxgraf_setup_2.png)

## Azure Cloud Deployment

For Azure-based deployments using Blob Storage:

### Azure App Service + Blob Storage
- Deploy PxGraf as an Azure App Service
- Store Px files in Azure Blob Storage
- Use Managed Identity for authentication between App Service and Blob Storage
- Configure Application Insights for monitoring
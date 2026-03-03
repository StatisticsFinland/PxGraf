# Set up for local development

## Prerequisites

1. dotnet
2. Visual Studio (or similar solution) for writing and running ASP.NET Core solutions
3. npm
4. Git

## Data Source Configuration

PxGraf requires exactly one data source to be configured. The three options below are mutually exclusive — enable only one.

### PxWeb API

If PxWeb api is to be used, PxGraf requires a connection to PxWeb api for database access.

Development setup can be configured to use a local instance of PxWeb or a remote instance running on a server.

#### Running a local instance of PxWeb
PxWeb can be ran locally, which can be useful for debugging and easier access to the database files and application settings.
The source code for PxWeb can be found here: https://github.com/statisticssweden/PxWeb.git
PxGraf uses the PxWeb API to fetch data from the Px databases. Additional testing databases will be nessessary for development since the default testing database
that comes with pxweb lacks the necessary metadata (content and time dimensions) for building visualizations with PxGraf.

### Local Px Database with Px.Utils
PxGraf can be configured to run using the local px database using Px.Utils instead of a PxWeb server. Px.Utils provides a fast way to access Px files on the local px database.
In order to use Px.Utils, a Px file database must be created on the local system.
To enable PxGraf to use Px.Utils, set `DatabaseConfig.Type` to `LocalFileSystem` in appsettings.json, along with `DatabaseConfig.DatabaseRootPath` and `DatabaseConfig.Encoding`.

### Azure Blob Storage
PxGraf can be configured to run using Azure Blob Storage as the data source. This is useful for cloud deployments where Px files are stored in Azure Storage.
To enable PxGraf to use Azure Blob Storage, set `DatabaseConfig.Type` to `BlobContainer` in appsettings.json, along with `DatabaseConfig.StorageAccountName` and `DatabaseConfig.ContainerName`.

**Optional Root Path**: You can specify a root path within the container using `DatabaseConfig.RootPath`. This is particularly useful when the same blob container stores multiple types of files. For example:
- Set RootPath to "px-data/" to store Px files under the px-data directory
- This allows you to use other paths like "saved-queries/" for query files in the same container
- If not specified, Px files will be stored at the container root

**Authentication**: The Azure Blob Storage datasource uses Azure DefaultAzureCredential for authentication, which supports:
- Azure CLI (for local development): Run `az login` to authenticate
- Managed Identity (for Azure deployments)
- Environment variables
- Visual Studio/Visual Studio Code authentication

**User-Assigned Managed Identity**: If you need to target a specific User-Assigned Managed Identity (e.g., when multiple identities are assigned to the same resource), set the optional `DatabaseConfig.ManagedIdentityClientId` to the Client ID of the desired identity. When omitted, `DefaultAzureCredential` uses its default credential chain.

For local development, the easiest method is to use Azure CLI authentication.

## Logging Configuration

### Standard Logging
1. In your appsettings.json, set the `LogOptions.Folder` to a valid directory path where logs will be stored
2. Set `LogOptions.Level` to the desired logging level (e.g., "Information", "Debug", "Warning", etc.)

### Application Insights
To enable Application Insights telemetry:
1. Obtain an Application Insights connection string from your Azure portal
2. Configure it using one of these methods:
   - Add it to appsettings.json in the `ApplicationInsights.ConnectionString` field
   - Set the environment variable `APPLICATIONINSIGHTS_CONNECTION_STRING`
3. Configure the minimum log level for Application Insights by setting `Logging.ApplicationInsights.LogLevel.Default` in appsettings.json (defaults to "Information")
4. Optionally configure adaptive sampling by setting `ApplicationInsights.EnableAdaptiveSampling` (defaults to false)

### Audit Logging
For security and compliance tracking, enable audit logging:
1. Set `LogOptions.AuditLog.Enabled` to `true` in appsettings.json
2. Configure which HTTP headers to include in the audit logs by adding their names to the `LogOptions.AuditLog.IncludedHeaders` array

Example configuration:
```json
"Logging": {
  "ApplicationInsights": {
    "LogLevel": {
      "Default": "Information"
    }
  }
},
"LogOptions": {
  "Folder": "C:\\PxGraf\\Logs",
  "SysId": "PxGraf",
  "Level": "Information",
  "AuditLog": {
    "Enabled": true,
    "IncludedHeaders": ["testHeader", "foobar"]
  }
},
"ApplicationInsights": {
  "ConnectionString": "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://region.in.applicationinsights.azure.com/",
  "EnableAdaptiveSampling": false
}
```

## Backend

1. Make a copy of the appsettings.template.json and rename it to appsettings.json
2. Replace the placeholders in the appsettings.json file based on your environment.
3. Set up the database by setting `DatabaseConfig.Type` in appsettings.json to one of: `PxWeb` (a), `LocalFileSystem` (b), or `BlobContainer` (c). Only one type can be active.
a. Set `DatabaseConfig.Type` to `PxWeb` and `DatabaseConfig.PxWebUrl` to the address of the PxWeb server. This can be a remote server or localhost if running a local instance of PxWeb. If running a local instance, make sure to include the port number.
b. Set `DatabaseConfig.Type` to `LocalFileSystem`, `DatabaseConfig.DatabaseRootPath` to the path to the local px database, and `DatabaseConfig.Encoding` to the encoding of the Px and alias files.
c. Set `DatabaseConfig.Type` to `BlobContainer`, `DatabaseConfig.StorageAccountName` and `DatabaseConfig.ContainerName`. Optionally, set `DatabaseConfig.RootPath` to organize Px files under a specific path within the container (e.g., "database/"). Optionally, set `DatabaseConfig.ManagedIdentityClientId` to the Client ID of a User-Assigned Managed Identity if needed. Ensure you are authenticated with Azure CLI (`az login`) for local development.
4. Build the solution in Visual Studio or run `dotnet build` in the PxGraf folder (See https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build for more details).
This will also build the frontend.

## Frontend

1. Follow the steps in Backend section to build the frontend.
2. When the build process has completed, run `npm start` in the PxGraf.Frontend folder. This will start the development server for the frontend.
3. You should now be able to navigate to localhost:3000 using your browser and see the application. (Requires PxGraf backend to also be running)

## Unit tests
Both the backend and the frontend have unit tests. The backend tests can be run by running `dotnet test` in the solution folder or using Visual Studio test explorer. The frontend tests can be run by running `npm run test` in the PxGraf.Frontend folder.

Example configuration with root path:
```json
{
  "DatabaseConfig": {
    "Type": "BlobContainer",
    "StorageAccountName": "storage",
    "ContainerName": "container",
    "RootPath": "database/",
    "ManagedIdentityClientId": ""
  }
}
```

This configuration will:
- Store Px files under the "database/" directory in the "container" container
- Allow you to use other paths like "saved-queries/" or "archived-queries/" for other file types in the same container
- Provide better organization and separation of concerns within a single container

## Storage Configuration

PxGraf supports multiple storage backends for both data sources (Px files) and saved queries/archives. These are configured through `DatabaseConfig.Type` and `QueryStorageConfig.Type`.

### Data Source Storage

Set `DatabaseConfig.Type` to one of the following:

#### LocalFileSystem
For local development and on-premises deployments:
```json
{
  "DatabaseConfig": {
    "Type": "LocalFileSystem",
    "DatabaseRootPath": "D:\\path\\to\\px\\database",
    "Encoding": "latin1"
  }
}
```

#### BlobContainer
For cloud-native deployments:
```json
{
  "DatabaseConfig": {
    "Type": "BlobContainer",
    "StorageAccountName": "mycompanydata",
    "ContainerName": "database",
    "RootPath": "database/",
    "ManagedIdentityClientId": ""
  }
}
```

### Saved Query Storage

Set `QueryStorageConfig.Type` to one of the following, or omit the section to use legacy fallback.

#### Legacy (still supported)
```json
{
  "savedQueryDirectory": "C:\\queries",
  "archiveFileDirectory": "C:\\archives"
}
```

#### LocalFileSystem
```json
{
  "QueryStorageConfig": {
    "Type": "LocalFileSystem",
    "SavedQueryDirectory": "C:\\queries",
    "ArchiveFileDirectory": "C:\\archives"
  }
}
```

#### BlobContainer
```json
{
  "QueryStorageConfig": {
    "Type": "BlobContainer",
    "StorageAccountName": "mycompanydata",
    "ContainerName": "pxgraf-queries",
    "SavedQueryPath": "saved-queries",
    "ArchiveFilePath": "archive-files",
    "ManagedIdentityClientId": ""
  }
}
```

### Mixed Storage Scenarios

You can mix storage types - for example:
- **Hybrid Setup**: Px files from Azure Blob Storage + Queries stored locally
- **Shared Container**: Both Px files and queries in same container with different paths
- **Separate Accounts**: Different storage accounts for data vs queries
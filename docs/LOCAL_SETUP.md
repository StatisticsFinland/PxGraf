# Set up for local development

## Prerequisites

1. dotnet
2. Visual Studio (or similar solution) for writing and running ASP.NET Core solutions
3. npm
4. Git

## PxWeb

If PxWeb api is to be used, PxGraf requires a connection to PxWeb api for database access.

Development setup can be configured to use a local instance of PxWeb or a remote instance running on a server.

### Running a local instance of PxWeb
PxWeb can be ran locally, which can be useful for debugging and easier access to the database files and application settings.
The source code for PxWeb can be found here: https://github.com/statisticssweden/PxWeb.git
PxGraf uses the PxWeb API to fetch data from the Px databases. Additional testing databases will be nessessary for development since the default testing database
that comes with pxweb lacks the necessary metadata (content and time dimensions) for building visualizations with PxGraf.

## Running PxGraf using local px database with Px.Utils
PxGraf can be configured to run using the local px database using Px.Utils instead of a PxWeb server. Px.Utils provides a fast way to access Px files on the local px database.
In order to use Px.Utils, a Px file database must be created on the local system.
To enable PxGraf to use Px.Utils, the appsettings.json file must be updated with the path to the Px file database (LocalFileSystemDatabaseConfig.DatabaseRootPath) and LocalFileSystemDatabaseConfig.Enabled must be set to true. LocalFileSystemDatabaseConfig.Encoding should match the encoding of the Px and alias files in the database.

## Logging Configuration

### Standard Logging
1. In your appsettings.json, set the `LogOptions.Folder` to a valid directory path where logs will be stored
2. Set `LogOptions.Level` to the desired logging level (e.g., "Information", "Debug", "Warning", etc.)

### Application Insights
To enable Application Insights telemetry:
1. Obtain an Application Insights connection string from your Azure portal
2. Configure it using one of these methods:
   - Add it to appsettings.json in the `LogOptions.ApplicationInsightsConnectionString` field
   - Set the environment variable `PXGRAF_APPLICATIONINSIGHTS_CONNECTION_STRING`

### Audit Logging
For security and compliance tracking, enable audit logging:
1. Set `LogOptions.AuditLog.Enabled` to `true` in appsettings.json
2. Configure which HTTP headers to include in the audit logs by adding their names to the `LogOptions.AuditLog.IncludedHeaders` array

Example configuration:
```json
"LogOptions": {
  "Folder": "C:\\PxGraf\\Logs",
  "SysId": "PxGraf",
  "Level": "Information",
  "AuditLog": {
    "Enabled": true,
    "IncludedHeaders": ["testHeader", "foobar"]
  },
  "ApplicationInsightsConnectionString": "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://region.in.applicationinsights.azure.com/"
}
```

## Backend

1. Make a copy of the appsettings.template.json and rename it to appsettings.json
2. Replace the placeholders in the appsettings.json file based on your environment.
3. Set up the database to either use a PxWeb instance (a) or a local database using Px.Utils (b).
	a. Fill in the address of the remote pxweb server to the appsettings.json file's "pxwebUrl" field. This can be the address of the remote server running PxWeb or localhost if you're running a local instance of PxWeb. If running a local instance of PxWeb, make sure to include the port number.
	b. Fill in the path to the local px database in the appsettings.json file's "LocalFileSystemDatabaseConfig.DatabaseRootPath" field. Set "LocalFileSystemDatabaseConfig.Enabled" to true. Set "LocalFileSystemDatabaseConfig.Encoding" to the encoding of the Px and alias files in the database.
4. Build the solution in Visual Studio or run `dotnet build` in the PxGraf folder (See https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build for more details).
This will also build the frontend.

## Frontend

1. Follow the steps in Backend section to build the frontend.
2. When the build process has completed, run `npm start` in the PxGraf.Frontend folder. This will start the development server for the frontend.
3. You should now be able to navigate to localhost:3000 using your browser and see the application. (Requires PxGraf backend to also be running)

## Unit tests
Both the backend and the frontend have unit tests. The backend tests can be run by running `dotnet test` in the solution folder or using Visual Studio test explorer. The frontend tests can be run by running `npm run test` in the PxGraf.Frontend folder.
# PxGraf

## Overview
PxGraf is a tool developed and maintained by Statistics Finland (Tilastokeskus) for visualizing statistical data from Px tables using the PxWeb API or local px database with Px.Utils library. The backend, written in C# with ASP.NET Core, fetches px file data from the PxWeb API or local px database using Px.Utils library, processes it for visualizations and serves it via REST apis. The frontend, written in TypeScript with React, provides a user interface for selecting and previewing data for visualizations and saving them as queries. The visualizations are drawn using PxVisualizer npm package, also developed and maintained by Statistics Finland.

The software is provided as is and Statistics Finland will **not** offer any support for setting up PxGraf or solving issues related to it.

**IMPORTANT:** PxVisualizer package uses HighCharts for rendering the visualizations. Please note that commercial use of HighCharts requires a commercial license. Non-commercial use may qualify for a free educational or personal license. Read more about licenses 
in the [HighCharts shop](https://shop.highsoft.com/?utm_source=npmjs&utm_medium=referral&utm_campaign=highchartspage&utm_content=licenseinfo).

## Core functions
- **Data visualization:** PxGraf helps users create visualization from px data. It automatically decides which visualization types are available based on the user's selections and what kind of customizations can be made. The user can then choose from available options. The data and visualizations can also be exported in different formats.
- **Saved queries:** The user can create, save, overwrite, archive and load queries. Saved queries can be shared with other users or published externally. Saved queries are stored in the running environment's file system and the user is provided with an ID representing the saved query.
- **APIs:** PxGraf's APIs provide functionality for processing the data for visualization, and saving and loading queries.

## APIs
- **Creation API:** Provides endpoints for fetching database listings and Px table metadata. It also provides functionality for metadata validation and providing required contents for the visualization editor. Can be disabled in the appsettings.json file.
- **Info API:** Provides information about the application. Its single endpoint returns the application's name, version and the environment it is running in.
- **Query meta API:** Provides an endpoint that returns the metadata for a saved query given its ID.
- **Saved query API:** Used for managing saved queries. Provides endpoints for fetching a saved query, saving a new query, archiving a query and re-archiving an existing query.
- **Visualization API:** Provides an endpoint for fetching visualization data for a saved query given its ID. More information about the response format can be found in VISUALIZATION_RESPONSE.md

### More information about the APIs can be found [here](API_DOCUMENTATION.md).

### Instructions for local development can be found [here](LOCAL_SETUP.md).

### Information about production setup can be found [here](PRODUCTION_SETUP.md).

## Configuration

### appsettings.json
appsettings.json contains the configuration for the backend. It contains following sections:
#### LogOptions
Configuration for logging such as file save location and logging level. Logging uses NLog. Further configuration can be done in the nlog.config file.
#### CacheOptions
Configuration for caching PxWeb data, visualization responses and more.
#### CORS
Configuration for Cross-Origin Resource Sharing. This is used determine which domains are allowed to access the API.
#### QueryOptions
Configuration for the maximum header length and the maximum number of data points that can be included in a query.
#### Language
Configuration for the default language and the available languages in the backend.
#### pxwebUrl
The address of the PxWeb server if in use.
#### savedQueryDirectory
The directory where saved query files are stored.
#### archiveFileDirectory
The directory where archived query files are stored.
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
Name of the encoding used in the database.

Note: If the LocalFileSystemDatabaseConfig block is not present, the PxWeb API is used to fetch data automatically. Providing neither working PxWeb API address nor LocalFileSystemDatabaseConfig will result in an error at application startup.

## Translation files

### Backend
Backend translations are located under PxGraf/Pars/translations.json. The file contains translations for English, Swedish and Finnish. Additional languages can be added by adding a new object to the translations.json file. Available languages need also to be added under Language.AvailableLanguages in the appsettings.json file.

### Frontend
Frontend translations are located under pxgraf.frontend/src/localization. Each language has its own file with translations for the frontend. The language codes in the file names have to match the language codes in the translationsConfig.json file.
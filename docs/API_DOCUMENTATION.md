# Creation API (/api/creation/)
Creation API provides endpoints for getting information about the databases and tables and their available languages, validating tables for visualization and creating visualizations from Px file data.

## Endpoints

| Function Name | API Route | Parameters | Returns |
|---------------|-----------|------------|---------|
| GetDataBaseListingAsync | GET: data-bases/{*dbPath*} | dbPath: The path to the database level provided in the url.<br>queryParameters: Dictionary of query parameters represented by key-value pair of strings. Used by this endpoint to determined the preferred language of the response item. | List of DataBaseListingItem objects that can represent either a database, subfolder or a Px table. |
| GetLanguagesAsync | GET: languages | None | List of language code strings that are supported by the backend and supported by one or more databases. |
| GetCubeMetaAsync | GET: cube-meta/{*tablePath*} | tablePath: The path to the table provided in the url. | CubeMeta object that contains the metadata of the table. |
| ValidateTableMetaData | GET: validate-table-metadata/{*tablePath*} | tablePath: The path to the table provided in the url. | TableMetaValidationResult object that contains the validation result of the table metadata. |
| GetVariableFilterResultAsync | POST: filter-variable | filterRequest: FilterRequest object that contains the table path and selected filters for each variable. | A dictionary of variable codes and their value codes that are available based on the filter request. |
| GetDefaultHeaderAsync | POST: default-header | input: CubeQuery object that contains the table path and information about the selected variables. | A multi language string object that contains the default header for each available language. |
| GetValidVisualizationTypesAsync | POST: valid-visualizations | cubeQuery: CubeQuery object that contains the table path and information about the selected variables. | List of strings that represent the valid visualization types for the table given the query request. |
| GetQueryInfoAsync | POST: query-info | cubeQuery: CubeQuery object that contains the table path and information about the selected variables. | QueryInfoResponse object that contains the current size of the query, limits for maximum supported query size and its warning threshold, maximum header length, valid visualization types and lists of rules for why rejected visualization types were rejected. |
| GetVisualizationRulesAsync | POST: visualization-rules | rulesQuery: VisualizationSettingsRequest object that contains visualization settings including the selected visualization type and the CubeQuery object representing the data for the visualization. | VisualizationRules object that defines which settings are available for tweaking the visualization in the front end user interface such as sorting options, pivoting options, displaying data labels etc. |
| GetVisualizationAsync | POST: visualization | request: ChartRequest object that contains the CubeQuery, selected language, active selectable variables and VisualizationCreationSettings object that contains the visualization type, variable codes for rows and columns, and settings for the given visualization type. | VisualizationResponse object that contains the data that PxVisualizer needs to render the visualization in the front end user interface including the data, settings and metadata. |

# Info API (/api/info)
Info API provides information about the application through its one GET endpoint. It returns an object that contains the name and version of the application and the environment that it's running in.

# Query meta API (api/sq/meta/)
Query meta API provides an endpoint for retrieving metadata of a saved query.

## Endpoints

| Function Name | API Route | Parameters | Returns |
|---------------|-----------|------------|---------|
| GetQueryMeta | GET: {*savedQueryId*} | savedQueryId: The id of the saved query provided in the url. | QueryMetaResponse object that contains the metadata of the saved query including the header, archival status, selected visualization type, etc. |

# Saved query API (/api/sq/)
SQ api provides endpoints for retrieving, saving, archiving and re-archiving queries.

## Endpoints

| Function Name | API Route | Parameters | Returns |
|---------------|-----------|------------|---------|
| GetSavedQueryAsync | GET: {*savedQueryId*} | savedQueryId: The id of the saved query provided in the url. | SaveQueryParams object that contains the CubeQuery and VisualizationCreationSettings objects that represent the saved query and its visualization settings. |
| SaveQueryAsync | POST: save | parameters: SaveQueryParams object that contains the CubeQuery and VisualizationCreationSettings objects that represent the query and its visualization settings. | SaveQueryResponse object that contains the id of the saved query. |
| ArchiveQueryAsync | POST: archive | parameters: SaveQueryParams object that contains the CubeQuery and VisualizationCreationSettings objects that represent the query and its visualization settings. | SaveQueryResponse object that contains the id of the archived query. |
| RearchiveQueryAsync | POST: re-archive | request: ReArchiveRequest object that contains the id of the archived query that is to be re-archived. | ReArchiveResponse object that contains the id of the new archived query. |


# Visualization API (api/sq/visualization/)
Visualization API provides an endpoint for retrieving data required for rendering a saved visualization.

## Endpoints

| Function Name | API Route | Parameters | Returns |
|---------------|-----------|------------|---------|
| GetVisualization | GET: {*sqId*} | sqId: The id of the saved query provided in the url. | VisualizationResponse object that contains the data that PxVisualizer needs to render the visualization in the front end user interface including the data, settings and metadata. |
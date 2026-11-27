# Backend Architecture

Backend paths are relative to `PxGraf/`. For data sources and storage, see [data architecture](architecture.data.md).

## Entry Point & Hosting

| File | Purpose |
|---|---|
| `Program.cs` | `Main()` → NLog setup → `CreateHostBuilder` → `Host.CreateDefaultBuilder` with NLog, custom `IConfiguration`, `Startup` |
| `Startup.cs` | DI registration, CORS, Swagger, controllers, SPA static files, feature management (`CreationAPI` gate) |

The backend uses the `IHostBuilder` / `Startup` pattern (not minimal API). Static config is loaded into a singleton `Configuration` class (`Settings/Configuration.cs`).

## Controllers (API Surface)

All controller routes are under `/api/`. The `CreationAPI` feature flag gates the creation/editing endpoints.

| Controller | Route Prefix | Feature-Gated | Key Endpoints |
|---|---|---|---|
| `CreationController` | `api/creation` | ✅ CreationAPI | `GET data-bases/{*dbPath}` — browse database hierarchy; `GET cube-meta/{*tablePath}` — table metadata; `GET validate-table-metadata/{*tablePath}` — metadata validation; `POST filter-dimension` — resolve dimension filters; `POST editor-contents` — editor setup data (sizes, valid chart types, headers); `POST visualization` — render multilingual preview visualization; `POST jsonstat?lang={language}` — render a single-language JSON-stat 2.0 preview using the table default when language is omitted and validating explicit languages |
| `SqController` | `api/sq` | ✅ CreationAPI | `GET {savedQueryId}` — load saved query; `POST save` — save/draft query; `POST archive` — archive query with data snapshot; `POST re-archive` — refresh archived query |
| `VisualizationController` | `api/sq` | ❌ | `GET visualization/{sqId}` — serve the multilingual visualization response for a saved query (cached with multi-state memory cache); `GET visualization/{sqId}/metadata` — serve metadata without fetching live matrix data; `GET jsonstat/{sqId}?lang={language}` — serve a single-language JSON-stat 2.0 dataset using the table default when language is omitted and validating explicit languages; `GET jsonstat/{sqId}/metadata?lang={language}` — serve equivalent JSON-stat metadata with an empty `value` array and no `status`. Metadata-only responses are not cached, but both formats share cached deserialized metadata for archived queries. |
| `QueryMetaController` | `api/sq/meta` | ❌ | `GET {savedQueryId}` — return saved query metadata (header, archived status, visualization type) |
| `InfoController` | `api/info` | ❌ | Application info endpoint |
| `HealthController` | `api/health` | ❌ | Health check endpoint — probes all configured dependencies (database, storage, webhook) and returns 200/503 |
| `ErrorController` | `api/error` | ❌ | Global error handler |

## Chart Type Selection (`ChartTypeSelection/`)

Rule-based engine that determines which visualization types are valid for a given data query.

| Type | Purpose |
|---|---|
| `ChartTypeSelector` | Main entry: `GetValidChartTypes()`, `GetRejectionReasons()` |
| `ChartRulesCheck` | Common rule evaluation logic |
| `ChartSpecificLimits/*Check.cs` | Per-chart-type rules (VerticalBarChart, LineChart, PieChart, ScatterPlot, PyramidChart, Table, stacked/grouped/horizontal/percent variants) |
| `ChartSelectionLimits`, `ChartTypeLimits` | JSON-configurable dimension limits |

Supported chart types (enum `VisualizationType`): VerticalBarChart, GroupVerticalBarChart, StackedVerticalBarChart, PercentVerticalBarChart, HorizontalBarChart, GroupHorizontalBarChart, StackedHorizontalBarChart, PercentHorizontalBarChart, PyramidChart, PieChart, LineChart, ScatterPlot, Table.

## Models

| Folder | Key Types | Purpose |
|---|---|---|
| `Models/Queries/` | `MatrixQuery`, `DimensionQuery`, `ValueFilters`, `PxTableReference`, `FilterRequest`, `VisualizationSettings`, `Layout` | Query structure and dimension filtering |
| `Models/Requests/` | `ChartRequest`, `SaveQueryParams`, `ReArchiveRequest`, `VisualizationCreationSettings` | API request DTOs |
| `Models/Responses/` | `EditorContentsResponse`, `VisualizationResponse`, `JsonStat2Extension`, `QueryMetaResponse`, `SaveQueryResponse`, `ReArchiveResponse`, `TableMetaValidationResult`, `DatabaseGroupContents`, `DatabaseGroupHeader`, `DatabaseTable`, `HealthResponse`, `DatabaseHealthStatus`, `ServiceHealthStatus` | API response DTOs |
| `Models/SavedQueries/` | `SavedQuery`, `ArchiveCube`, versioned types (`V1_0`, `V1_1`, `V1_2`, `V10`, `V11`) | Persisted query + archive formats with version migration |
| `Models/Metadata/` | `HeaderBuildingUtilities`, `MatrixMetadataExtensions`, `DimensionExtensions`, `DimensionValueExtensions` | Metadata processing and header generation |
| `Data/MetaData/` | `CubeMeta`, `Variable`, `VariableValue`, `ContentComponent` | Legacy metadata model types |
| `Data/` | `CubeSorting`, `AutoPivotRules`, `ManualPivotRules`, `LayoutRules`, `TimeDimensionIntervalParser` | Data transformation and layout logic |

## Configuration (`Settings/`)

Singleton `Configuration` loaded from `appsettings.json` + environment variables.

Key config sections: `DatabaseConfig`, `QueryStorageConfig`, `CacheOptions`, `CorsOptions`, `QueryOptions`, `LanguageOptions`, `PublicationWebhookConfig`, `ApplicationInsightsConfig`.

## Services (`Services/`)

| Type | Purpose |
|---|---|
| `AuditLogService` / `IAuditLogService` | Structured audit logging |
| `PublicationWebhookService` / `IPublicationWebhookService` | HTTP webhook for query publication events; also exposes `CheckWebhookReachabilityAsync` for health probing |
| `HealthCheckService` / `IHealthCheckService` | Probes all configured dependencies (database, saved query storage, archive storage, publication webhook) and returns aggregated `HealthResponse` |
| `VirtualValueValidationService` / `IVirtualValueValidationService` | Validates calculated dimension value definitions against existing values |
| `VirtualValueComputationService` / `IVirtualValueComputationService` | Computes calculated dimension values when building matrices for previews, saved queries and visualization responses |

## Other Backend Folders

| Folder | Purpose |
|---|---|
| `Visualization/` | `PxVisualizerCubeAdapter` — transforms matrix data into `VisualizationResponse` for the PxVisualizer library and builds the common JSON-stat visualization settings extension; `JsonStat2DatasetBuilder` — transforms matrix data into the shared `Datasource/ApiDatasource/SerializationModels/JsonStat2` JSON-stat 2.0 DTO |
| `Language/` | `Localization`, `Translation`, per-concern translation classes (rejection reasons, sorting options, chart types, etc.) loaded from `Pars/translations.json` |
| `Utility/` | `SqFileInterface`, `InputValidation`, `LoggerConstants`, `PxSyntaxConstants`, JSON converters (`CustomJsonConverters/`) |
| `Enums/` | `VisualizationType` (ChartTypesEnum), `ChartTypeRejectionEnum`, `TimeDimensionIntervals` |
| `Exceptions/` | `BadPxWebResponseException`, `UnknownFilterTypeException`, `UnknownChartTypeException`, `InvalidConfigurationException`, `TooManyRequestsException`, `TableMetadataException` |
| `Pars/` | `translations.json` — multilingual UI/API string translations |

## External Dependencies (NuGet)

`Px.Utils` (1.4.0) — core PX file parsing/matrix library. `Azure.Identity` + `Azure.Storage.Blobs` — Azure storage. `Microsoft.FeatureManagement.AspNetCore` — feature flags. `NLog` — logging. `Swashbuckle` — Swagger/OpenAPI. `Microsoft.ApplicationInsights` — telemetry.

## Configuration & Feature Flags

- **`CreationAPI`**: Feature flag gating `CreationController` and `SqController` (including saved-query loading and saving). When disabled, visualization serving and query metadata remain available; health and info endpoints are not gated by this flag.
- **Database sources**: Local filesystem (`LocalFilesystemDatabaseConfig`), Azure Blob (`BlobContainerDatabaseConfig`), or PxWeb API (`PxWebDatabaseConfig`).
- **Query storage**: Local filesystem (`LocalQueryStorageConfig`) or Azure Blob (`BlobQueryStorageConfig`).
- **Publication webhook**: Optional HTTP webhook triggered on non-draft saves/archives.
- **Application Insights**: Optional telemetry via connection string.

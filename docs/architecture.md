# PxGraf Solution Architecture

> **Purpose**: Machine-readable architecture reference for AI coding agents. Provides high-level structural context to minimize token usage when reasoning about the codebase.

## Solution Overview

PxGraf is a statistical data visualization tool. Users browse hierarchical database tables, configure dimension filters, select a chart type, and save/publish visualizations ("saved queries"). The solution consists of a .NET 10 backend API, a React/TypeScript SPA frontend, and an NUnit test project.

**Solution file**: `PxGraf.sln`  
**Projects**:
| Project | Path | Target | Role |
|---|---|---|---|
| PxGraf | `PxGraf\PxGraf.csproj` | net10.0 | ASP.NET Core Web API + SPA host |
| PxGraf.Frontend | `PxGraf.Frontend\pxgraf.frontend.esproj` | — | React 19 SPA (Vite + TypeScript) |
| UnitTests | `UnitTests\UnitTests.csproj` | net10.0 | NUnit + Moq unit tests for backend |

---

## Backend — `PxGraf/`

### Entry Point & Hosting

| File | Purpose |
|---|---|
| `Program.cs` | `Main()` → NLog setup → `CreateHostBuilder` → `Host.CreateDefaultBuilder` with NLog, custom `IConfiguration`, `Startup` |
| `Startup.cs` | DI registration, CORS, Swagger, controllers, SPA static files, feature management (`CreationAPI` gate) |

The backend uses the `IHostBuilder` / `Startup` pattern (not minimal API). Static config is loaded into a singleton `Configuration` class (`Settings/Configuration.cs`).

### Controllers (API Surface)

All controller routes are under `/api/`. The `CreationAPI` feature flag gates the creation/editing endpoints.

| Controller | Route Prefix | Feature-Gated | Key Endpoints |
|---|---|---|---|
| `CreationController` | `api/creation` | ✅ CreationAPI | `GET data-bases/{*dbPath}` — browse database hierarchy; `GET cube-meta/{*tablePath}` — table metadata; `GET validate-table-metadata/{*tablePath}` — metadata validation; `POST filter-dimension` — resolve dimension filters; `POST editor-contents` — editor setup data (sizes, valid chart types, headers); `POST visualization` — render preview visualization |
| `SqController` | `api/sq` | ✅ CreationAPI | `GET {savedQueryId}` — load saved query; `POST save` — save/draft query; `POST archive` — archive query with data snapshot; `POST re-archive` — refresh archived query |
| `VisualizationController` | `api/sq/visualization` | ❌ | `GET {sqId}` — serve visualization for a saved query (cached with multi-state memory cache) |
| `QueryMetaController` | `api/sq/meta` | ❌ | `GET {savedQueryId}` — return saved query metadata (header, archived status, visualization type) |
| `InfoController` | `api/info` | ❌ | Application info endpoint |
| `ErrorController` | `api/error` | ❌ | Global error handler |

### Data Access Layer (`Datasource/`)

Pluggable data source architecture via `ICachedDatasource`:

```
ICachedDatasource
├── CachedFileDatasource  ← IFileDatasource (local FS or Azure Blob)
└── CachedApiDatasource   ← IApiDatasource (PxWeb V1 API)
```

| Folder | Key Types | Purpose |
|---|---|---|
| `Datasource/FileDatasource/` | `FileDatasource`, `CachedFileDatasource`, `IFileDatasource`, `LocalFilesystemDatabaseConfig`, `BlobContainerDatabaseConfig`, `PathUtils` | File-based data: local filesystem or Azure Blob Storage |
| `Datasource/ApiDatasource/` | `PxWebV1ApiInterface`, `CachedApiDatasource`, `PxWebConnection`, `IPxWebConnection` | PxWeb REST API data source |
| `Datasource/Cache/` | `MultiStateMemoryTaskCache`, `IMultiStateMemoryTaskCache` | Task-based in-memory cache with Fresh/Stale/Error states |
| `Datasource/` | `CachedDatasource` (abstract), `ICachedDatasource`, `DatabaseConfig`, `DatabaseConfigType` | Base caching layer and config types |

Configured in `Startup.ConfigureDataSourceStorage()` based on `Configuration.Current.DatabaseConfig` type.

### Storage (`Storage/`)

File I/O abstraction for saved queries and archive files:

| Type | Purpose |
|---|---|
| `IStorageProvider` | Interface for read/write/exists/list operations |
| `LocalStorageProvider` | Local filesystem implementation |
| `BlobStorageProvider` | Azure Blob Storage implementation (uses `Azure.Identity` + `Azure.Storage.Blobs`) |
| `PathNormalizer` | Normalizes file paths across storage providers |

Configured in `Startup.ConfigureQueryFileStorage()`. Saved queries use `ISqFileInterface` / `SqFileInterface` (`Utility/`).

### Chart Type Selection (`ChartTypeSelection/`)

Rule-based engine that determines which visualization types are valid for a given data query.

| Type | Purpose |
|---|---|
| `ChartTypeSelector` | Main entry: `GetValidChartTypes()`, `GetRejectionReasons()` |
| `ChartRulesCheck` | Common rule evaluation logic |
| `ChartSpecificLimits/*Check.cs` | Per-chart-type rules (VerticalBarChart, LineChart, PieChart, ScatterPlot, PyramidChart, Table, stacked/grouped/horizontal/percent variants) |
| `ChartSelectionLimits`, `ChartTypeLimits` | JSON-configurable dimension limits |

Supported chart types (enum `VisualizationType`): VerticalBarChart, GroupVerticalBarChart, StackedVerticalBarChart, PercentVerticalBarChart, HorizontalBarChart, GroupHorizontalBarChart, StackedHorizontalBarChart, PercentHorizontalBarChart, PyramidChart, PieChart, LineChart, ScatterPlot, Table.

### Models

| Folder | Key Types | Purpose |
|---|---|---|
| `Models/Queries/` | `MatrixQuery`, `DimensionQuery`, `ValueFilters`, `PxTableReference`, `FilterRequest`, `VisualizationSettings`, `Layout` | Query structure and dimension filtering |
| `Models/Requests/` | `ChartRequest`, `SaveQueryParams`, `ReArchiveRequest`, `VisualizationCreationSettings` | API request DTOs |
| `Models/Responses/` | `EditorContentsResponse`, `VisualizationResponse`, `QueryMetaResponse`, `SaveQueryResponse`, `ReArchiveResponse`, `TableMetaValidationResult`, `DatabaseGroupContents`, `DatabaseGroupHeader`, `DatabaseTable` | API response DTOs |
| `Models/SavedQueries/` | `SavedQuery`, `ArchiveCube`, versioned types (`V1_0`, `V1_1`, `V1_2`, `V10`, `V11`) | Persisted query + archive formats with version migration |
| `Models/Metadata/` | `HeaderBuildingUtilities`, `MatrixMetadataExtensions`, `DimensionExtensions`, `DimensionValueExtensions` | Metadata processing and header generation |
| `Data/MetaData/` | `CubeMeta`, `Variable`, `VariableValue`, `ContentComponent` | Legacy metadata model types |
| `Data/` | `CubeSorting`, `AutoPivotRules`, `ManualPivotRules`, `LayoutRules`, `TimeDimensionIntervalParser` | Data transformation and layout logic |

### Configuration (`Settings/`)

Singleton `Configuration` loaded from `appsettings.json` + environment variables.

Key config sections: `DatabaseConfig`, `QueryStorageConfig`, `CacheOptions`, `CorsOptions`, `QueryOptions`, `LanguageOptions`, `PublicationWebhookConfig`, `ApplicationInsightsConfig`.

### Services (`Services/`)

| Type | Purpose |
|---|---|
| `AuditLogService` / `IAuditLogService` | Structured audit logging |
| `PublicationWebhookService` / `IPublicationWebhookService` | HTTP webhook for query publication events |

### Other Backend Folders

| Folder | Purpose |
|---|---|
| `Visualization/` | `PxVisualizerCubeAdapter` — transforms matrix data into `VisualizationResponse` for the PxVisualizer library |
| `Language/` | `Localization`, `Translation`, per-concern translation classes (rejection reasons, sorting options, chart types, etc.) loaded from `Pars/translations.json` |
| `Utility/` | `SqFileInterface`, `InputValidation`, `LoggerConstants`, `PxSyntaxConstants`, JSON converters (`CustomJsonConverters/`) |
| `Enums/` | `VisualizationType` (ChartTypesEnum), `ChartTypeRejectionEnum`, `TimeDimensionIntervals` |
| `Exceptions/` | `BadPxWebResponseException`, `UnknownFilterTypeException`, `UnknownChartTypeException`, `InvalidConfigurationException`, `TooManyRequestsException`, `TableMetadataException` |
| `Pars/` | `translations.json` — multilingual UI/API string translations |

### External Dependencies (NuGet)

`Px.Utils` (1.4.0) — core PX file parsing/matrix library. `Azure.Identity` + `Azure.Storage.Blobs` — Azure storage. `Microsoft.FeatureManagement.AspNetCore` — feature flags. `NLog` — logging. `Swashbuckle` — Swagger/OpenAPI. `Microsoft.ApplicationInsights` — telemetry.

---

## Frontend — `PxGraf.Frontend/`

### Tech Stack

React 19, TypeScript, Vite 8, MUI 7, styled-components, react-router-dom 7, TanStack React Query 5, i18next, lodash, `@statisticsfinland/pxvisualizer` (chart rendering).

Testing: Jest 30, Testing Library (React + DOM + user-event), ts-jest.

### Application Shell

| File | Purpose |
|---|---|
| `src/App.tsx` | Root: `QueryClientProvider` → `UiLanguageProvider` → `NavigationProvider` → `ThemeProvider` → `ErrorBoundary` → `Router` |
| `src/Router.tsx` | Route definitions with `PageLayout` wrapper (Header + Divider + content) |

### Routes

| Path | View Component | Purpose |
|---|---|---|
| `/` | `TableTreeSelection` | Browse database hierarchy as a tree |
| `/editor/*` | `Editor` (wrapped in `EditorProvider`) | Configure dimensions, chart type, metadata; preview & save |
| `/table-list/*` | `TableListSelection` | Detail view of a folder's contents |
| `/sqid/*` | `QueryLoader` | Load & display a saved query by ID |

### Views (`src/views/`)

| View | Key Responsibilities |
|---|---|
| `Editor/Editor.tsx` | Main editor orchestrator. Loads cube metadata, resolves dimensions, determines valid visualization types. Consumes `QueryContext`, `VisualizationContext`, and `SaveContext` for state. Sub-components: `EditorFilterSection` (dimension value selection), `EditorMetaSection` (metadata/chart type editing), `EditorPreviewSection` (chart preview), `EditorFooterSection` (save actions), `EditorDialogs` (save mutation dialogs) |
| `TableTreeSelection/` | Tree view using `NestedList` for hierarchical database browsing |
| `TableListSelection/` | Flat list of `DirectoryInfo` and `TableInfo` components for a path |
| `QueryLoader/` | Fetches saved query by URL param and renders the visualization |

### Components (`src/components/`)

| Component | Purpose |
|---|---|
| `Header/Header` | Application header bar |
| `NestedList/NestedList` | Recursive tree list for database hierarchy. Uses `TableListItem` (folder) and `TableItem` (table) |
| `Preview/Preview` | Chart preview with size controls and selectable dimension menus. Consumes `QueryContext` and `VisualizationContext`. Uses `@statisticsfinland/pxvisualizer` `Chart` component |
| `ChartTypeSelector/` | UI for selecting visualization type |
| `ChartTypeRejectionReasons/` | Displays reasons a chart type is not available |
| `MetaEditor/` | Editors for dimension metadata: `MetaEditor`, `HeaderEditor`, `BasicDimensionEditor`, `ContentDimensionEditor`, `ContentDimensionValueEditor`, `DimensionEditor`, `EditorField`, `RevertButton` |
| `VariableSelection/` | Dimension filter UI: `AllDimensionSelection`, `DefaultSelectableDimensionSelection`, `DimensionSelection`, `DimensionSelectionList`, filter sub-components |
| `SelectableVariableMenus/` | `SelectableDimensionMenus`, `ValueSelect` — dropdowns for selectable dimensions in preview |
| `VisualizationSettingsControls/` | Visualization settings UI. Sub-folders: `TypeSpecificControls/` (`MultiselectableSelector`, `TablePivotSettings`) and `UtilityComponents/` (`DimensionList`, `MarkerScaler`, `SortingSelector`, `VisualizationSettingsSwitch`) |
| `SaveDialog/SaveDialog` | Save query dialog with dynamic/static and draft/publish options. Consumes `SaveContext` |
| `SavedQueryFinder/SavedQueryFinder` | Dialog to search and load existing saved queries by ID |
| `SaveResultDialog/` | Post-save result display: `SaveResultDialog`, `ErrorDialogContent`, `LoadingDialogContent`, `SuccessDialogContent` |
| `ErrorBoundary/ErrorBoundary` | React class-based error boundary wrapping the app. Catches render errors and displays a retry UI |
| `LanguageSelector/` | Language selection UI: `LanguageSelector`, `LangText` |
| `TabPanel/TabPanel` | Generic tab panel wrapper for tabbed content |
| `CellCount/` | Shows current/max query cell count |
| `InfoBubble/` | Tooltip info component |
| `DirectoryInfo/` | Card for a database subfolder |
| `TableInfo/` | Card for a database table |

### API Layer (`src/api/`)

| File | Calls Backend Endpoint | Purpose |
|---|---|---|
| `client.ts` | — | `ApiClientV2` class wrapping `fetch` with base URL from `pxGrafUrl('api/')` |
| `services/table.ts` | `GET api/creation/data-bases/{path}` | `useTableQuery` — database hierarchy listing |
| `services/cube-meta.ts` | `GET api/creation/cube-meta/{path}` | `useCubeMetaQuery` — table metadata |
| `services/validate-table-metadata.ts` | `GET api/creation/validate-table-metadata/{path}` | `useValidateTableMetadataQuery` — table validity |
| `services/filter-dimension.ts` | `POST api/creation/filter-dimension` | `useResolveDimensionFiltersQuery` — resolve dimension value codes |
| `services/editor-contents.ts` | `POST api/creation/editor-contents` | `useEditorContentsQuery` — editor setup (sizes, chart options, header) |
| `services/visualization.ts` | `POST api/creation/visualization` | `useVisualizationQuery` — preview visualization data |
| `services/queries.ts` | `POST api/sq/save`, `GET api/sq/{id}`, etc. | `useSaveMutation`, fetch saved queries |

All API hooks use TanStack React Query for caching and state management.

### State Management (`src/contexts/`)

The editor state was decomposed from a single monolithic `EditorContext` into three focused sub-contexts. The `EditorProvider` is now a composite provider that wraps all three.

| Context | Purpose |
|---|---|
| `queryContext.tsx` | `QueryContext` + `QueryProvider` — `cubeQuery` (with debounced setter, 1s), `query`, and their setters. Manages the dimension query state |
| `visualizationContext.tsx` | `VisualizationContext` + `VisualizationProvider` — `selectedVisualizationUserInput`, `visualizationSettingsUserInput`, `defaultSelectables`, and their setters |
| `saveContext.tsx` | `SaveContext` + `SaveProvider` — `saveDialogOpen`, `loadedQueryId`, `loadedQueryIsDraft`, `publicationWebhookEnabled`, and their setters |
| `editorContext.tsx` | `EditorProvider` — composite provider that nests `QueryProvider` → `VisualizationProvider` → `SaveProvider`. No longer exports a context object; components consume sub-contexts directly |
| `uiLanguageContext.tsx` | `UiLanguageContext` + `UiLanguageProvider` — UI language, content language tab, content language, available UI languages |
| `navigationContext.tsx` | `NavigationContext` + `NavigationProvider` + `useNavigationContext` hook — current table path for navigation state |

### Hooks (`src/hooks/`)

| Hook | Purpose |
|---|---|
| `useHierarchyParams` | Extracts path hierarchy from URL params |
| `useQueryParams` | URL query parameter parsing |
| `useReplaceQueryParams` | URL query parameter replacement |
| `useScrollToElement` | Scroll-to-element behavior |

### Types (`src/types/`)

| Type File | Key Types |
|---|---|
| `cubeMeta.ts` | `IDimension`, `EDimensionType`, cube metadata interfaces |
| `query.ts` | `Query`, `ICubeQuery`, `IDimensionQuery`, `IValueFilter`, `FilterType`, `IDimensionEditions` |
| `visualizationSettings.ts` | `IVisualizationSettings` |
| `visualizationType.ts` | Visualization type enum/constants |
| `editorContentsResponse.ts` | Editor contents response shape |
| `saveQuery.ts` | Save query request/response types |
| `tableListItems.ts` | `IDatabaseGroupHeader`, `IDatabaseTable` |
| `tableValidation.ts` | Table validation response |
| `visualizationResponse.ts` | Visualization response shape, `IVariable` |
| `multiLanguageString.ts` | Multilanguage string type |

### Utilities (`src/utils/`)

| File | Purpose |
|---|---|
| `ApiHelpers.ts` | `pxGrafUrl()`, `extractQuery()`, `extractCubeQuery()` |
| `editorHelpers.ts` | `getDefaultQueries()`, `getVisualizationOptionsForVisualizationType()`, `resolveDimensions()` |
| `ChartSettingHelpers.ts` | `getValidatedSettings()` |
| `sortingHelpers.ts` | `sortDatabaseItems()` |
| `dimensionSelectionHelpers.ts` | Dimension filter logic |
| `componentHelpers.ts` | Shared UI helpers |
| `metadataUtils.ts` | Metadata processing |
| `keywordConstants.ts` | String constants |

### Internationalization

`i18next` + `i18next-browser-languagedetector` + `react-i18next`. Configured in `src/i18n.ts`.

### Styling

MUI 7 theme (`src/styles/materialTheme`), styled-components for custom layout, `App.css` for global styles.

### Frontend Tests

Jest 30 + Testing Library. Tests are co-located with source files using `*.test.tsx`/`*.test.ts` naming. Context tests are in `src/contexts/__tests__/`. Snapshot tests in `__snapshots__/` subdirectories.

| Test Area | Key Test Files |
|---|---|
| Contexts | `queryContext.test.tsx`, `saveContext.test.tsx`, `visualizationContext.test.tsx`, `navigationContext.test.tsx` |
| Views | `Editor.test.tsx`, `EditorFilterSection.test.tsx`, `EditorFooterSection.test.tsx`, `EditorMetaSection.test.tsx`, `EditorPreviewSection.test.tsx` |
| Components | Co-located `*.test.tsx` files for each component (e.g., `Preview.test.tsx`, `ChartTypeSelector.test.tsx`, `SaveDialog.test.tsx`, `ErrorBoundary.test.tsx`) |
| API services | `cube-meta.test.ts`, `editor-contents.test.ts`, `filter-dimension.test.ts`, `queries.test.ts`, `table.test.ts`, `validate-table-metadata.test.ts`, `visualization.test.ts` |
| Utils | `ApiHelpers.test.ts`, `ChartSettingHelpers.test.ts`, `componentHelpers.test.ts`, `dimensionSelectionHelpers.test.ts`, `editorHelpers.test.ts`, `metadataUtils.test.ts`, `sortingHelpers.test.ts` |
| Hooks | `useHierarchyParams.test.tsx`, `useQueryParams.test.tsx`, `useReplaceQueryParams.test.tsx`, `useScrollToElement.test.tsx` |

---

## Backend Unit Tests — `UnitTests/`

NUnit 4 + Moq 4. Tests mirror backend structure.

### Test Organization

| Folder | Covers |
|---|---|
| `ControllerTests/CreationControllerTests/` | `GetDataBaseListingTests`, `GetCubeMetaTests`, `ValidateTableMetaDataTests`, `GetDimensionFilterResultAsyncTests`, `GetEditorContentsTests`, `GetVisualizationAsyncTests` |
| `ControllerTests/SqControllerTests/` | `GetSavedQueryAsyncTests`, `SaveQueryAsyncTest`, `ArchiveQueryAsyncTests`, `ReArchiveActionTests` |
| `ControllerTests/VisualizationControllerTests/` | `GetVisualizationTests` |
| `ControllerTests/QueryMetaControllerTests/` | `GetQueryMetaTests` |
| `ControllerTests/` | `ErrorControllerTests` |
| `ChartTypeSelectionTests/` | Per-chart-type tests (BasicVerticalBarChart, LineChart, PieChart, ScatterPlot, Table, etc.) |
| `DatasourceTests/` | `FileDatasourceTests`, `CachedApiDataSourceTests`, `CachedFileDatasourceTests`, `MultiStateMemoryTaskCacheTests`, `PxWebV1ApiInterfaceTests`, `PathUtilsTests` |
| `Storage/` | `LocalStorageProviderTests`, `PathNormalizerTests` |
| `SerializerTests/` | `SavedQuerySerializerTests`, `ArchiveMatrixSerializerTests`, `ConverterUtilityTests` |
| `MatrixMetadataTests/` | `HeaderBuildingUtilitiesTests`, `MatrixMetadataExtensionsTests`, `DimensionExtensionsTests`, `DimensionValueExtensionsTests` |
| `DataCubeTests/` | `CubeMetaHeaderGenerationTests`, `TimeDimensionIntervalParserTests` |
| `LayoutTests/` | `AutoPivotTests`, `ManualPivotTests`, `LayoutRulesTests`, `LayoutEqualityTests` |
| `SortingTests/` | `DimensionSortingOptionsTests` |
| `Visualization/` | `PxVisualizerCubeAdapterTests`, `DimensionOrderingTests` |
| `ModelTests/` | `ValueFilterTests` |
| `ServicesTests/` | `AuditLogServiceTests`, `PublicationWebhookServiceTests` |
| `ConfigurationTests/` | `ConfigurationTests`, `ApplicationInsightsConfigTests` |
| `UtilityFunctionsTests/` | `InputValidationTests`, `CartesianProductTest`, `PxFileReferenceTests`, `LockByKeyTests`, `MetaPropertyExtensionsTests` |
| `PxWebInterfaceTests/` | `ModelExtensionsTests` + fixtures |

### Test Helpers

| File | Purpose |
|---|---|
| `TestCreationControllerBuilder.cs` | Builder for `CreationController` with mocked dependencies |
| `TestVisualizationControllerBuilder.cs` | Builder for `VisualizationController` with mocked dependencies |
| `TestQueryMetaControllerBuilder.cs` | Builder for `QueryMetaController` with mocked dependencies |
| `TestDataCubeBuilder.cs` | Builder for test data cubes |
| `DimensionParameters.cs` | Shared dimension test parameters |
| `Fixtures/` | `SavedQueryFixtures`, `TranslationFixture`, `TestInMemoryConfiguration`, `ArchiveCubeFixtures`, response fixtures |

---

## Key Data Flows

### 1. Browse Database → Select Table
```
Frontend: TableTreeSelection → NestedList → useTableQuery
    → GET api/creation/data-bases/{path}
Backend: CreationController.GetDataBaseListingAsync → ICachedDatasource.GetGroupContentsCachedAsync
```

### 2. Open Editor for Table
```
Frontend: Editor → useCubeMetaQuery + useValidateTableMetadataQuery + useEditorContentsQuery
    → GET api/creation/cube-meta/{path}
    → GET api/creation/validate-table-metadata/{path}
    → POST api/creation/editor-contents
Backend: CreationController → ICachedDatasource → ChartTypeSelector (valid types + rejection reasons)
```

### 3. Preview Visualization
```
Frontend: Preview → useVisualizationQuery
    → POST api/creation/visualization
Backend: CreationController.GetVisualizationAsync → filter metadata → get matrix → validate chart type → PxVisualizerCubeAdapter.BuildVisualizationResponse
Frontend: Chart component from @statisticsfinland/pxvisualizer renders the response
```

### 4. Save Query
```
Frontend: EditorFooterSection → useSaveMutation
    → POST api/sq/save  (or POST api/sq/archive)
Backend: SqController.SaveQueryAsync → validate → ISqFileInterface.SerializeToSqFileAsync → optional webhook
```

### 5. View Saved Query
```
Frontend: QueryLoader → GET api/sq/visualization/{sqId}
Backend: VisualizationController → MultiStateMemoryTaskCache (Fresh/Stale/Error) → ISqFileInterface → PxVisualizerCubeAdapter
```

---

## Configuration & Feature Flags

- **`CreationAPI`**: Feature flag gating `CreationController` and `SqController` (save/archive). When disabled, only visualization serving (`VisualizationController`, `QueryMetaController`) is available.
- **Database sources**: Local filesystem (`LocalFilesystemDatabaseConfig`), Azure Blob (`BlobContainerDatabaseConfig`), or PxWeb API (`PxWebDatabaseConfig`).
- **Query storage**: Local filesystem (`LocalQueryStorageConfig`) or Azure Blob (`BlobQueryStorageConfig`).
- **Publication webhook**: Optional HTTP webhook triggered on non-draft saves/archives.
- **Application Insights**: Optional telemetry via connection string.

---

## File Naming Conventions

- **Backend**: PascalCase for files and folders. Controllers in `Controllers/`. One class per file.
- **Frontend**: PascalCase for component files (`.tsx`), camelCase for utility/hook files (`.ts`). Tests co-located with `*.test.tsx`/`*.test.ts` or in `__tests__/` subdirectory. Snapshot tests in `__snapshots__/`.
- **Tests (backend)**: Mirror source structure. Test classes named `{Feature}Tests.cs`.

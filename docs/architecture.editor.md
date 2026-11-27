# Editor Architecture

Frontend paths are relative to `PxGraf.Frontend/`.

## Views (`src/views/`)

| View | Key Responsibilities |
|---|---|
| `Editor/Editor.tsx` | Main editor orchestrator. Loads cube metadata, resolves dimensions, determines valid visualization types. Consumes `QueryContext`, `VisualizationContext`, and `SaveContext` for state. Sub-components: `EditorFilterSection` (dimension value selection), `EditorMetaSection` (chart header, type, settings and preview size), `EditorPreviewSection` (chart preview), `EditorFooterSection` (save actions), `EditorDialogs` (metadata and save dialogs) |
| `TableTreeSelection/` | Tree view using `NestedList` for hierarchical database browsing |
| `QueryLoader/` | Fetches saved query by URL param and redirects to `/editor/*`, passing the query as navigation state |

## Components (`src/components/`)

| Component | Purpose |
|---|---|
| `Header/Header` | Application header bar |
| `NestedList/NestedList` | Recursive tree list for database hierarchy. Uses `TableListItem` (folder) and `TableItem` (table) |
| `Preview/Preview` | Chart preview with selectable dimension menus; size controls are in `EditorMetaSection`. Consumes `QueryContext` and `VisualizationContext`. Uses `@statisticsfinland/pxvisualizer` `Chart` component |
| `ChartTypeSelector/` | UI for selecting visualization type |
| `ChartTypeRejectionReasons/` | Displays reasons a chart type is not available |
| `MetaEditor/` | `MetadataDialog` edits selected dimension value names, units and sources; `HeaderEditor` edits the chart header in the main editor. Also contains `BasicDimensionEditor`, `ContentDimensionEditor`, `ContentDimensionValueEditor`, `DimensionEditor`, `EditorField`, `RevertButton` |
| `VariableSelection/` | Dimension filter UI: `DefaultSelectableDimensionSelection`, `DimensionSelection`, `DimensionSelectionList`, filter sub-components and computed-value controls |
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

## API Layer (`src/api/`)

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

## State Management (`src/contexts/`)

The editor state was decomposed from a single monolithic `EditorContext` into three focused sub-contexts. The `EditorProvider` is now a composite provider that wraps all three.

| Context | Purpose |
|---|---|
| `queryContext.tsx` | `QueryContext` + `QueryProvider` — `cubeQuery` (with debounced setter, 1s), `query`, and their setters. Manages the dimension query state |
| `visualizationContext.tsx` | `VisualizationContext` + `VisualizationProvider` — `selectedVisualizationUserInput`, `visualizationSettingsUserInput`, `defaultSelectables`, and their setters |
| `saveContext.tsx` | `SaveContext` + `SaveProvider` — `saveDialogOpen`, `loadedQueryId`, `loadedQueryIsDraft`, `publicationWebhookEnabled`, and their setters |
| `editorContext.tsx` | `EditorProvider` — composite provider that nests `QueryProvider` → `VisualizationProvider` → `SaveProvider`. No longer exports a context object; components consume sub-contexts directly |
| `uiLanguageContext.tsx` | `UiLanguageContext` + `UiLanguageProvider` — UI language, content language tab, content language, available UI languages |
| `navigationContext.tsx` | `NavigationContext` + `NavigationProvider` + `useNavigationContext` hook — current table path for navigation state |

# Testing and File Conventions

## Frontend Tests

Jest 30 + Testing Library. Tests are co-located with source files using `*.test.tsx`/`*.test.ts` naming. Context tests are in `src/contexts/__tests__/`. Snapshot tests in `__snapshots__/` subdirectories.

| Test Area | Key Test Files |
|---|---|
| Contexts | `queryContext.test.tsx`, `saveContext.test.tsx`, `visualizationContext.test.tsx`, `navigationContext.test.tsx` |
| Views | `Editor.test.tsx`, `EditorFilterSection.test.tsx`, `EditorFooterSection.test.tsx`, `EditorMetaSection.test.tsx`, `EditorPreviewSection.test.tsx` |
| Components | Co-located `*.test.tsx` files for each component (e.g., `Preview.test.tsx`, `ChartTypeSelector.test.tsx`, `SaveDialog.test.tsx`, `ErrorBoundary.test.tsx`) |
| API services | `cube-meta.test.ts`, `editor-contents.test.ts`, `filter-dimension.test.ts`, `queries.test.ts`, `table.test.ts`, `validate-table-metadata.test.ts`, `visualization.test.ts` |
| Utils | `ApiHelpers.test.ts`, `ChartSettingHelpers.test.ts`, `componentHelpers.test.ts`, `dimensionSelectionHelpers.test.ts`, `editorHelpers.test.ts`, `metadataUtils.test.ts`, `sortingHelpers.test.ts` |
| Hooks | `useHierarchyParams.test.tsx`, `useQueryParams.test.tsx`, `useScrollToElement.test.tsx` |

## Backend Unit Tests — `UnitTests/`

NUnit 4 + Moq 4. Tests mirror backend structure.

### Test Organization

| Folder | Covers |
|---|---|
| `ControllerTests/CreationControllerTests/` | `GetDataBaseListingTests`, `GetCubeMetaTests`, `ValidateTableMetaDataTests`, `GetDimensionFilterResultAsyncTests`, `GetEditorContentsTests`, `GetVisualizationAsyncTests` |
| `ControllerTests/SqControllerTests/` | `GetSavedQueryAsyncTests`, `SaveQueryAsyncTest`, `ArchiveQueryAsyncTests`, `ReArchiveActionTests` |
| `ControllerTests/VisualizationControllerTests/` | `GetVisualizationTests` |
| `ControllerTests/QueryMetaControllerTests/` | `GetQueryMetaTests` |
| `ControllerTests/` | `ErrorControllerTests`, `HealthControllerTests` |
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
| `ServicesTests/` | `AuditLogServiceTests`, `PublicationWebhookServiceTests`, `HealthCheckServiceTests` |
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

## File Naming Conventions

- **Backend**: PascalCase for files and folders. Controllers in `Controllers/`. One class per file.
- **Frontend**: PascalCase for component files (`.tsx`), camelCase for utility/hook files (`.ts`). Tests co-located with `*.test.tsx`/`*.test.ts` or in `__tests__/` subdirectory. Snapshot tests in `__snapshots__/`.
- **Tests (backend)**: Mirror source structure. Test classes named `{Feature}Tests.cs`.

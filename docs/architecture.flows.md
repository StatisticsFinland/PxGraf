# Key Data Flows

## 1. Browse Database → Select Table
```
Frontend: TableTreeSelection → NestedList → useTableQuery
    → GET api/creation/data-bases/{path}
Backend: CreationController.GetDataBaseListingAsync → ICachedDatasource.GetGroupContentsCachedAsync
```

## 2. Open Editor for Table
```
Frontend: Editor → useCubeMetaQuery + useValidateTableMetadataQuery + useEditorContentsQuery
    → GET api/creation/cube-meta/{path}
    → GET api/creation/validate-table-metadata/{path}
    → POST api/creation/editor-contents
Backend: CreationController → ICachedDatasource → ChartTypeSelector (valid types + rejection reasons)
```

## 3. Preview Visualization
```
Frontend: Preview → useVisualizationQuery
    → POST api/creation/visualization
Backend: CreationController.GetVisualizationAsync → filter metadata → get matrix → validate chart type → PxVisualizerCubeAdapter.BuildVisualizationResponse
Frontend: Chart component from @statisticsfinland/pxvisualizer renders the response
```

## 4. Save Query
```
Frontend: EditorFooterSection → useSaveMutation
    → POST api/sq/save  (or POST api/sq/archive)
Backend: SqController.SaveQueryAsync → validate → ISqFileInterface.SerializeToSqFileAsync → optional webhook
```

## 5. Reopen Saved Query in Editor
```
Frontend: QueryLoader (or SavedQueryFinder) → GET api/sq/{sqId} → redirect to /editor/*
Backend: SqController.GetSavedQueryAsync → ISqFileInterface → current table metadata/data → reconcile saved query
Frontend: Editor initializes query and settings from navigation state; warns if recovery changed the query
```

## 6. Serve Saved Visualization to API Consumers
```
Client: GET api/sq/visualization/{sqId} (or GET api/sq/jsonstat/{sqId})
Backend: VisualizationController → MultiStateMemoryTaskCache → ISqFileInterface
    → archived matrix snapshot or current datasource matrix → PxVisualizerCubeAdapter / JsonStat2DatasetBuilder
```

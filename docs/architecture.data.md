# Data Sources and Storage

Backend paths are relative to `PxGraf/`.

## Data Access Layer (`Datasource/`)

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

## Storage (`Storage/`)

File I/O abstraction for saved queries and archive files:

| Type | Purpose |
|---|---|
| `IStorageProvider` | Interface for read/write/exists/list operations |
| `LocalStorageProvider` | Local filesystem implementation |
| `BlobStorageProvider` | Azure Blob Storage implementation (uses `Azure.Identity` + `Azure.Storage.Blobs`) |
| `PathNormalizer` | Normalizes file paths across storage providers |

Configured in `Startup.ConfigureQueryFileStorage()`. Saved queries use `ISqFileInterface` / `SqFileInterface` (`Utility/`).

# PxGraf Solution Architecture

PxGraf is a statistical data visualization tool. Users browse database tables, filter dimensions, select charts, and save or publish visualizations. `PxGraf.sln` comprises the .NET backend API (`PxGraf/PxGraf.csproj`), React/TypeScript frontend (`PxGraf.Frontend/pxgraf.frontend.esproj`), and NUnit backend tests (`UnitTests/UnitTests.csproj`).

## Documentation index

Choose the documents relevant to the task:

| Document | Contents |
|---|---|
| [Backend](architecture.backend.md) | Hosting, API controllers, chart rules, models, services, configuration and feature flags |
| [Data sources and storage](architecture.data.md) | Data source implementations, caching, saved-query and archive storage |
| [Frontend](architecture.frontend.md) | Application shell, routes, hooks, types, utilities, internationalization and styling |
| [Editor](architecture.editor.md) | Views, components, frontend API hooks and state management |
| [Testing and conventions](architecture.testing.md) | Frontend and backend test organization, test helpers and file naming |
| [Data flows](architecture.flows.md) | Browsing, editor setup, preview, saving, reopening and serving visualizations |

For user-facing feature names and UI terms, see the [feature map](FEATURE_MAP.md). For endpoint contracts, see the [API documentation](API_DOCUMENTATION.md).

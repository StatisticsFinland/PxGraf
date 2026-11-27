# Frontend Architecture

Frontend paths are relative to `PxGraf.Frontend/`. For editor components, API hooks, and state, see [editor architecture](architecture.editor.md).

## Tech Stack

React 19, TypeScript, Vite 8, MUI 7, styled-components, react-router-dom 7, TanStack React Query 5, i18next, lodash, `@statisticsfinland/pxvisualizer` (chart rendering).

Testing: Jest 30, Testing Library (React + DOM + user-event), ts-jest.

## Application Shell

| File | Purpose |
|---|---|
| `src/App.tsx` | Root: `QueryClientProvider` → `UiLanguageProvider` → `NavigationProvider` → `ThemeProvider` → `ErrorBoundary` → `Router` |
| `src/Router.tsx` | Route definitions using `PageLayout` for browsing/loading and `EditorRoute` for the editor; both are defined in `src/components/Layout/Layout.tsx` |

## Routes

| Path | View Component | Purpose |
|---|---|---|
| `/` | `TableTreeSelection` | Browse database hierarchy as a tree |
| `/editor/*` | `Editor` (wrapped in `EditorProvider`) | Configure dimensions, chart type, metadata; preview & save |
| `/sqid/*` | `QueryLoader` | Fetch a saved query by ID and redirect to the editor |

## Hooks (`src/hooks/`)

| Hook | Purpose |
|---|---|
| `useHierarchyParams` | Extracts path hierarchy from URL params |
| `useQueryParams` | URL query parameter parsing |
| `useScrollToElement` | Scroll-to-element behavior |

## Types (`src/types/`)

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

## Utilities (`src/utils/`)

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

## Internationalization

`i18next` + `i18next-browser-languagedetector` + `react-i18next`. Configured in `src/i18n.ts`.

## Styling

The MUI 7 theme (`src/styles/materialTheme.ts`) is the authoritative source for application UI colors, typography, surfaces, borders, shape, shadows, and interaction states. Application components should consume semantic theme roles through MUI `styled` or `sx`; reusable UI colors should not be defined directly in components. Styled-components remains available for layout-only styles that do not need theme tokens.

Visualization output is a separate styling boundary. Do not apply the application theme to HTML tables or graphs rendered by `@statisticsfinland/pxvisualizer`. The `.tk-table` rules in `src/index.css`, the public `pxgraf-styles/pxgraf-table.css` stylesheet, and pxvisualizer chart styles control visualization and export appearance independently and must be reviewed separately from application UI changes.

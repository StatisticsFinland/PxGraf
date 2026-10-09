import { handleStandaloneRequest } from './handler';
import { cubeMetadata, tableReference } from './fixture';
import { FilterType } from 'types/query';
import { VisualizationType } from 'types/visualizationType';

const query = {
    tableReference,
    variableQueries: {
        Year: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: [], valueEdits: {} },
        Area: { valueFilter: { type: FilterType.Item, query: ['north', 'south'] }, selectable: false, virtualValueDefinitions: [], valueEdits: {} },
        Measure: { valueFilter: { type: FilterType.Item, query: ['population'] }, selectable: false, virtualValueDefinitions: [], valueEdits: {} },
    },
};

describe('standalone API', () => {
    it('opens the sample hierarchy and rejects unknown tables', async () => {
        const root = await handleStandaloneRequest('GET', 'creation/data-bases/');
        expect(root).toHaveProperty('headers.0.code', 'standalone');
        const group = await handleStandaloneRequest('GET', 'creation/data-bases/standalone');
        expect(group).toHaveProperty('files.0.fileName', 'sample.px');
        expect(await handleStandaloneRequest('GET', 'creation/cube-meta/standalone/sample.px')).toEqual(cubeMetadata);
        expect(await handleStandaloneRequest('GET', 'creation/validate-table-metadata/standalone/sample.px')).toHaveProperty('tableHasTimeDimension', true);
        await expect(handleStandaloneRequest('GET', 'creation/cube-meta/unknown.px')).rejects.toThrow('Unsupported standalone request');
    });

    it('resolves filters and virtual codes from the request', async () => {
        const result = await handleStandaloneRequest('POST', 'creation/filter-dimension', JSON.stringify({
            tableReference,
            filters: { Year: { type: FilterType.Top, query: 2 }, Area: { type: FilterType.Item, query: ['south', 'total'] } },
            virtualValueDefinitions: { Area: ['total'] },
        }));
        expect(result).toEqual({ Year: ['2022', '2023'], Area: ['south', 'total'], Measure: ['population', 'households'] });
    });

    it('returns coherent editor options and preview data for the selected values', async () => {
        const contents = await handleStandaloneRequest('POST', 'creation/editor-contents', JSON.stringify(query));
        expect(contents).toMatchObject({ size: 6, publicationWebhookEnabled: true });
        expect(contents).toHaveProperty('visualizationOptions');

        const preview = await handleStandaloneRequest('POST', 'creation/visualization', JSON.stringify({
            query, language: 'en', visualizationSettings: { selectedVisualization: VisualizationType.Table, rowVariableCodes: ['Year', 'Area', 'Measure'] },
        }));
        expect(preview).toMatchObject({
            tableReference,
            data: [2000, 2320, 2085, 2405, 2170, 2490],
            rowVariableCodes: ['Year', 'Area', 'Measure'],
            columnVariableCodes: [],
            visualizationSettings: { visualizationType: VisualizationType.Table },
        });
        expect(preview).toHaveProperty('metaData.0.values.2.code', '2024');
        expect(preview).toHaveProperty('metaData.2.values.0.contentComponent.unit.en', 'people');
    });

    it('keeps sample values stable when other values are filtered out', async () => {
        const southOnly = {
            ...query,
            variableQueries: { ...query.variableQueries, Area: { ...query.variableQueries.Area, valueFilter: { type: FilterType.Item, query: ['south'] } } },
        };
        const preview = await handleStandaloneRequest('POST', 'creation/visualization', JSON.stringify({
            query: southOnly, language: 'en', visualizationSettings: { selectedVisualization: VisualizationType.Table },
        }));
        expect(preview).toHaveProperty('data', [2320, 2405, 2490]);
    });

    it('returns a localized JSON-stat dataset for the query-suffixed route', async () => {
        const result = await handleStandaloneRequest('POST', 'creation/jsonstat?lang=fi', JSON.stringify({
            query,
            language: 'en',
            visualizationSettings: { selectedVisualization: VisualizationType.Table },
        }));

        expect(result).toMatchObject({
            version: '2.0',
            class: 'dataset',
            id: ['Year', 'Area', 'Measure'],
            size: [3, 2, 1],
            label: 'Vaestoesimerkki',
            value: [2000, 2320, 2085, 2405, 2170, 2490],
            extension: {
                visualizationConfig: {
                    chartType: 'table',
                    layout: { rows: ['Year', 'Area', 'Measure'], columns: [] },
                },
            },
        });
        expect(result).toHaveProperty('dimension.Year.label', 'Vuosi');
        expect(result).toHaveProperty('dimension.Area.category.label.north', 'Pohjoinen');
    });

    it('rejects unsupported JSON-stat languages', async () => {
        await expect(handleStandaloneRequest('POST', 'creation/jsonstat?lang=xx', JSON.stringify({
            query,
            visualizationSettings: { selectedVisualization: VisualizationType.Table },
        }))).rejects.toThrow('Unsupported standalone language');
    });

    it('does not preview or save a query with an empty dimension', async () => {
        const emptyQuery = {
            ...query,
            variableQueries: { ...query.variableQueries, Area: { ...query.variableQueries.Area, valueFilter: { type: FilterType.Item, query: [] } } },
        };
        const contents = await handleStandaloneRequest('POST', 'creation/editor-contents', JSON.stringify(emptyQuery));
        expect(contents).toMatchObject({ size: 0, visualizationOptions: [] });
        await expect(handleStandaloneRequest('POST', 'creation/visualization', JSON.stringify({
            query: emptyQuery, language: 'en', visualizationSettings: { selectedVisualization: VisualizationType.Table },
        }))).rejects.toThrow('No values selected');
        await expect(handleStandaloneRequest('POST', 'sq/save', JSON.stringify({
            query: emptyQuery, settings: { selectedVisualization: VisualizationType.Table }, id: '', draft: true,
        }))).rejects.toThrow('No values selected');
    });

    it('saves, reloads and archives an editable draft without persistence', async () => {
        const settings = { selectedVisualization: VisualizationType.LineChart, showDataPoints: true };
        const saved = await handleStandaloneRequest('POST', 'sq/save', JSON.stringify({ query, settings, id: '', draft: true })) as { id: string };
        const loaded = await handleStandaloneRequest('GET', `sq/${saved.id}`);
        expect(loaded).toMatchObject({ id: saved.id, draft: true, recoveredWithChanges: false, query, settings });

        const archived = await handleStandaloneRequest('POST', 'sq/archive', JSON.stringify({ query, settings, id: saved.id, draft: false }));
        expect(archived).toHaveProperty('id', saved.id);
        expect(await handleStandaloneRequest('GET', `sq/${saved.id}`)).toHaveProperty('draft', false);
        await expect(handleStandaloneRequest('GET', 'sq/unknown')).rejects.toThrow('Unknown standalone query');
    });

    it('fails closed on unsupported paths and table references', async () => {
        await expect(handleStandaloneRequest('POST', 'creation/unsupported', '{}')).rejects.toThrow('Unsupported standalone request');
        await expect(handleStandaloneRequest('POST', 'creation/editor-contents', JSON.stringify({ ...query, tableReference: { name: 'missing.px', hierarchy: [] } }))).rejects.toThrow('Unknown standalone table');
    });
});
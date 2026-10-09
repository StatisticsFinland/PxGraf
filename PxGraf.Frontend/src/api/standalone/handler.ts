import { ETimeVariableInterval, EVariableType, EVisualizationType, IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { IFetchSavedQueryResponse, ISaveQueryResponse } from 'api/services/queries';
import { EDimensionType, IDimension, IContentDimensionValue } from 'types/cubeMeta';
import { IEditorContentsResponse } from 'types/editorContentsResponse';
import { FilterType, IValueFilter } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';
import { getChartType, IJsonStatDataset } from 'types/jsonStatChart';
import { cubeMetadata, groupContents, rootContents, tablePath, tableReference, visualizationOptions } from './fixture';

type CubeRequest = IFetchSavedQueryResponse['query'];
type FilterRequest = { tableReference: typeof tableReference; filters: { [code: string]: IValueFilter }; virtualValueDefinitions: { [code: string]: string[] } };
type PreviewRequest = { query: CubeRequest; language: string; visualizationSettings: IVisualizationSettings };
type SaveRequest = { query: CubeRequest; settings: IVisualizationSettings; id: string; draft: boolean };

const savedQueries = new Map<string, IFetchSavedQueryResponse>();
let nextQueryId = 1;

const checkTable = (reference: typeof tableReference) => {
    if (reference?.name !== tableReference.name || reference.hierarchy?.join('/') !== tableReference.hierarchy.join('/')) {
        throw new Error('Unknown standalone table');
    }
};

const selectValues = (dimension: IDimension, filter?: IValueFilter, virtualCodes: string[] = []): string[] => {
    const codes = [...dimension.values.map(value => value.code), ...virtualCodes];
    const selected = Array.isArray(filter?.query) ? filter.query : [];
    switch (filter?.type) {
        case FilterType.Item: return codes.filter(code => selected.includes(code));
        case FilterType.InverseItem: return codes.filter(code => !selected.includes(code));
        case FilterType.Top: return codes.slice(0, Number(filter.query) || 0);
        case FilterType.From: return codes.slice(Math.max(0, codes.indexOf(String(filter.query))));
        case FilterType.Regex: {
            try {
                const pattern = new RegExp(String(filter.query ?? ''), 'i');
                return codes.filter(code => pattern.test(code) || pattern.test(dimension.values.find(value => value.code === code)?.name.en ?? ''));
            } catch {
                return [];
            }
        }
        default: return codes;
    }
};

const resolveFilters = (request: FilterRequest) => {
    checkTable(request.tableReference);
    return Object.fromEntries(cubeMetadata.dimensions.map(dimension => [
        dimension.code,
        selectValues(dimension, request.filters?.[dimension.code], request.virtualValueDefinitions?.[dimension.code]),
    ]));
};

const selectedDimensions = (request: CubeRequest) => {
    checkTable(request.tableReference);
    return cubeMetadata.dimensions.map(dimension => {
        const dimensionQuery = request.variableQueries?.[dimension.code];
        const virtualCodes = dimensionQuery?.virtualValueDefinitions?.map(definition => definition.code) ?? [];
        const codes = selectValues(dimension, dimensionQuery?.valueFilter, virtualCodes);
        return { dimension, codes, selectable: dimensionQuery?.selectable ?? false };
    });
};

const editorContents = (request: CubeRequest): IEditorContentsResponse => {
    const dimensions = selectedDimensions(request);
    const size = dimensions.reduce((count, { codes }) => count * codes.length, 1);
    return {
        size,
        sizeWarningLimit: 100,
        maximumSupportedSize: 1000,
        maximumHeaderLength: 120,
        headerText: size === 0 ? { en: '', fi: '', sv: '' } : request.chartHeaderEdit ?? { en: 'Population sample', fi: 'Vaestoesimerkki', sv: 'Befolkningsexempel' },
        visualizationOptions: size === 0 ? [] : visualizationOptions,
        visualizationRejectionReasons: {},
        publicationWebhookEnabled: true,
    };
};

const visualization = (request: PreviewRequest): IQueryVisualizationResponse => {
    const selected = selectedDimensions(request.query);
    if (selected.some(({ codes }) => codes.length === 0)) throw new Error('No values selected');
    const settings = request.visualizationSettings;
    const type = settings?.selectedVisualization ?? VisualizationType.LineChart;
    if (!Object.values(VisualizationType).includes(type)) throw new Error('Unknown standalone visualization');
    const metaData: IQueryVisualizationResponse['metaData'] = selected.map(({ dimension, codes }) => ({
        code: dimension.code,
        name: dimension.name,
        note: null,
        type: dimension.type === EDimensionType.Time ? EVariableType.Time
            : dimension.type === EDimensionType.Content ? EVariableType.Content
            : EVariableType.OtherClassificatory,
        values: codes.map(code => {
            const value = dimension.values.find(item => item.code === code);
            const content = value as IContentDimensionValue | undefined;
            const edits = request.query.variableQueries?.[dimension.code]?.valueEdits?.[code];
            return {
                code,
                name: edits?.nameEdit ?? value?.name ?? { en: code, fi: code, sv: code },
                note: null,
                isSum: false,
                contentComponent: dimension.type === EDimensionType.Content ? {
                    unit: edits?.contentComponent?.unitEdit ?? content?.unit ?? { en: 'people' },
                    source: edits?.contentComponent?.sourceEdit ?? { en: 'Standalone sample', fi: 'Esimerkki', sv: 'Exempel' },
                    numberOfDecimals: content?.precision ?? 0,
                    lastUpdated: content?.lastUpdated ?? '2025-01-01T00:00:00Z',
                } : null,
            };
        }),
    }));
    const [years, areas, measures] = selected.map(item => item.codes);
    const allCodes = selected.map(({ dimension }) => [
        ...dimension.values.map(value => value.code),
        ...(request.query.variableQueries?.[dimension.code]?.virtualValueDefinitions?.map(definition => definition.code) ?? []),
    ]);
    const data = years.flatMap(year => areas.flatMap(area => measures.map(measure =>
        measure === 'households'
            ? 900 + allCodes[0].indexOf(year) * 35 + allCodes[1].indexOf(area) * 110
            : 2000 + allCodes[0].indexOf(year) * 85 + allCodes[1].indexOf(area) * 320
    )));
    const selectableVariableCodes = selected.filter(item => item.selectable && item.codes.length > 0).map(item => item.dimension.code);
    const rowVariableCodes = type === VisualizationType.Table
        ? settings.rowVariableCodes ?? ['Year', 'Area', 'Measure']
        : selected.filter(item => !item.selectable && item.dimension.type !== EDimensionType.Content).map(item => item.dimension.code);
    const columnVariableCodes = type === VisualizationType.Table ? settings.columnVariableCodes ?? [] : ['Measure'];
    return {
        tableReference,
        data,
        dataNotes: {},
        missingDataInfo: {},
        metaData,
        selectableVariableCodes,
        rowVariableCodes,
        columnVariableCodes,
        header: editorContents(request.query).headerText,
        visualizationSettings: {
            visualizationType: EVisualizationType[type],
            timeVariableIntervals: ETimeVariableInterval.Year,
            timeSeriesStartingPoint: '2022-01-01T00:00:00Z',
            sorting: settings.sorting,
            cutYAxis: settings.cutYAxis,
            showDataPoints: settings.showDataPoints,
            markerSize: settings.markerSize,
        },
    };
};

const jsonStatVisualization = (request: PreviewRequest, requestedLanguage?: string): IJsonStatDataset => {
    const language = requestedLanguage ?? cubeMetadata.defaultLanguage;
    if (!cubeMetadata.availableLanguages.includes(language)) throw new Error(`Unsupported standalone language: ${language}`);

    const response = visualization(request);
    const dimensions = Object.fromEntries(response.metaData.map(variable => [
        variable.code,
        {
            label: variable.name[language] ?? variable.name.en ?? variable.code,
            category: {
                index: Object.fromEntries(variable.values.map((value, index) => [value.code, index])),
                label: Object.fromEntries(variable.values.map(value => [
                    value.code,
                    value.name[language] ?? value.name.en ?? value.code,
                ])),
            },
        },
    ]));
    const settings = request.visualizationSettings;
    const selectableSelections = Object.fromEntries(response.selectableVariableCodes.map(code => [
        code,
        response.metaData.find(variable => variable.code === code)?.values.map(value => value.code) ?? [],
    ]));
    const visualizationType = settings.selectedVisualization ?? VisualizationType.LineChart;

    return {
        version: '2.0',
        class: 'dataset',
        label: response.header[language] ?? response.header.en ?? '',
        id: response.metaData.map(variable => variable.code),
        size: response.metaData.map(variable => variable.values.length),
        dimension: dimensions,
        value: response.data,
        role: {
            time: response.metaData.filter(variable => variable.type === EVariableType.Time).map(variable => variable.code),
            geo: response.metaData.filter(variable => variable.type === EVariableType.Geological).map(variable => variable.code),
            metric: response.metaData.filter(variable => variable.type === EVariableType.Content).map(variable => variable.code),
        },
        extension: {
            visualizationConfig: {
                chartType: getChartType(visualizationType as VisualizationType),
                layout: { rows: response.rowVariableCodes, columns: response.columnVariableCodes },
                cutValueAxis: settings.cutYAxis,
                sorting: settings.sorting,
            },
            selectableConfig: {
                selectableSelections,
                defaultSelectableSelections: settings.defaultSelectableVariableCodes ?? undefined,
                multiSelectableDimensionCode: settings.multiselectableVariableCode,
            },
        },
    };
};

export async function handleStandaloneRequest(method: 'GET' | 'POST', url: string, body?: string): Promise<unknown> {
    if (method === 'GET') {
        if (url === 'creation/data-bases/') return rootContents;
        if (url === 'creation/data-bases/standalone') return groupContents;
        if (url === `creation/cube-meta/${tablePath.join('/')}`) return cubeMetadata;
        if (url === `creation/validate-table-metadata/${tablePath.join('/')}`) {
            return { tableHasContentDimension: true, tableHasTimeDimension: true, allDimensionsContainValues: true };
        }
        if (url.startsWith('sq/')) {
            const saved = savedQueries.get(url.slice(3));
            if (saved) return saved;
            throw new Error('Unknown standalone query');
        }
    }
    if (method === 'POST') {
        const request = JSON.parse(body ?? '{}');
        if (url === 'creation/filter-dimension') return resolveFilters(request as FilterRequest);
        if (url === 'creation/editor-contents') return editorContents(request as CubeRequest);
        if (url === 'creation/visualization') return visualization(request as PreviewRequest);
        const requestUrl = new URL(url, 'http://localhost');
        if (requestUrl.pathname.replace(/^\/+/, '') === 'creation/jsonstat') {
            return jsonStatVisualization(request as PreviewRequest, requestUrl.searchParams.get('lang') ?? undefined);
        }
        if (url === 'sq/save' || url === 'sq/archive') {
            const save = request as SaveRequest;
            if (selectedDimensions(save.query).some(({ codes }) => codes.length === 0)) throw new Error('No values selected');
            const id = save.id && savedQueries.has(save.id) ? save.id : `standalone-${nextQueryId++}`;
            savedQueries.set(id, {
                id,
                query: structuredClone(save.query),
                settings: structuredClone(save.settings),
                draft: save.draft,
                recoveredWithChanges: false,
            });
            return { id, publicationStatus: 0 } satisfies ISaveQueryResponse;
        }
    }
    throw new Error(`Unsupported standalone request: ${method} ${url}`);
}
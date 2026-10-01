import { EDimensionType, IMatrixMetadata } from 'types/cubeMeta';
import { IVisualizationOptions } from 'types/editorContentsResponse';
import { IDatabaseGroupContents } from 'types/tableListItems';
import { VisualizationType } from 'types/visualizationType';

export const tablePath = ['standalone', 'sample.px'];
export const tableReference = { hierarchy: ['standalone'], name: 'sample.px' };

export const rootContents: IDatabaseGroupContents = {
    headers: [{ code: 'standalone', name: { en: 'Sample data', fi: 'Esimerkkiaineisto', sv: 'Exempeldata' }, languages: ['en', 'fi', 'sv'] }],
    files: [],
};

export const groupContents: IDatabaseGroupContents = {
    headers: [],
    files: [{ fileName: 'sample.px', name: { en: 'Population sample', fi: 'Vaestoesimerkki', sv: 'Befolkningsexempel' }, languages: ['en', 'fi', 'sv'], lastUpdated: '2025-01-01T00:00:00Z' }],
};

export const cubeMetadata: IMatrixMetadata = {
    defaultLanguage: 'en',
    availableLanguages: ['en', 'fi', 'sv'],
    additionalProperties: {},
    dimensions: [
        {
            code: 'Year', type: EDimensionType.Time, interval: 'Year',
            name: { en: 'Year', fi: 'Vuosi', sv: 'Ar' },
            values: ['2022', '2023', '2024'].map(code => ({ code, name: { en: code, fi: code, sv: code }, isVirtual: false })),
        },
        {
            code: 'Area', type: EDimensionType.Geographical,
            name: { en: 'Area', fi: 'Alue', sv: 'Omrade' },
            values: [
                { code: 'north', name: { en: 'North', fi: 'Pohjoinen', sv: 'Norr' }, isVirtual: false },
                { code: 'south', name: { en: 'South', fi: 'Etela', sv: 'Soder' }, isVirtual: false },
                { code: 'east', name: { en: 'East', fi: 'Ita', sv: 'Oster' }, isVirtual: false },
            ],
        },
        {
            code: 'Measure', type: EDimensionType.Content,
            name: { en: 'Measure', fi: 'Tieto', sv: 'Uppgift' },
            values: [
                { code: 'population', name: { en: 'Population', fi: 'Vaesto', sv: 'Befolkning' }, isVirtual: false, unit: { en: 'people', fi: 'henkiloa', sv: 'personer' }, precision: 0, lastUpdated: '2025-01-01T00:00:00Z' },
                { code: 'households', name: { en: 'Households', fi: 'Kotitaloudet', sv: 'Hushall' }, isVirtual: false, unit: { en: 'households', fi: 'kotitaloutta', sv: 'hushall' }, precision: 0, lastUpdated: '2025-01-01T00:00:00Z' },
            ],
        },
    ],
};

const sortingOptions = [
    { code: 'DESCENDING', description: { en: 'Descending', fi: 'Laskeva', sv: 'Fallande' } },
    { code: 'ASCENDING', description: { en: 'Ascending', fi: 'Nouseva', sv: 'Stigande' } },
];

export const visualizationOptions: IVisualizationOptions[] = Object.values(VisualizationType).map(type => {
    const sortable = [VisualizationType.HorizontalBarChart, VisualizationType.GroupHorizontalBarChart,
        VisualizationType.StackedHorizontalBarChart, VisualizationType.PercentHorizontalBarChart, VisualizationType.PieChart].includes(type);
    return {
        type,
        allowManualPivot: [VisualizationType.GroupVerticalBarChart, VisualizationType.StackedVerticalBarChart,
            VisualizationType.GroupHorizontalBarChart, VisualizationType.StackedHorizontalBarChart].includes(type),
        allowMultiselect: type === VisualizationType.LineChart || type === VisualizationType.Table,
        allowShowingDataPoints: type !== VisualizationType.Table,
        allowCuttingYAxis: type === VisualizationType.LineChart || type === VisualizationType.ScatterPlot,
        allowMatchXLabelsToEnd: type === VisualizationType.VerticalBarChart,
        allowSetMarkerScale: type === VisualizationType.ScatterPlot,
        sortingOptions: { default: sortable ? sortingOptions : [], pivoted: sortable ? sortingOptions : [] },
    };
});
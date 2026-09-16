import { ChartConfig, ChartType, JsonStatDataset, SelectableSelections } from '@statisticsfinland/jsonstatgraphs';
import { EDimensionType } from './cubeMeta';
import { IVariable } from './visualizationResponse';
import { IVisualizationSettings } from './visualizationSettings';
import { VisualizationType } from './visualizationType';

export interface IJsonStatChartExtension {
    jsonstatChart?: {
        sources?: {
            dimension?: Record<string, string>;
            category?: Record<string, Record<string, string>>;
        };
    };
    selectableConfig?: {
        selectableSelections?: SelectableSelections;
        defaultSelectableSelections?: SelectableSelections;
        multiSelectableDimensionCode?: string;
    };
    visualizationConfig?: {
        chartType: ChartType;
        layout?: {
            rows: string[];
            columns: string[];
        };
        cutValueAxis?: boolean;
        sorting?: string;
    };
    missingValueDescriptions?: Record<string, string>;
}

export type IJsonStatDataset = JsonStatDataset & {
    extension?: IJsonStatChartExtension;
};

export interface IJsonStatSelectable {
    dimension: IVariable;
    multiselectable: boolean;
}

const chartTypeMap: Record<VisualizationType, ChartType> = {
    [VisualizationType.VerticalBarChart]: 'verticalBar',
    [VisualizationType.GroupVerticalBarChart]: 'groupedVerticalBar',
    [VisualizationType.StackedVerticalBarChart]: 'stackedVerticalBar',
    [VisualizationType.PercentVerticalBarChart]: 'percentVerticalBar',
    [VisualizationType.HorizontalBarChart]: 'horizontalBar',
    [VisualizationType.GroupHorizontalBarChart]: 'groupedHorizontalBar',
    [VisualizationType.StackedHorizontalBarChart]: 'stackedHorizontalBar',
    [VisualizationType.PercentHorizontalBarChart]: 'percentHorizontalBar',
    [VisualizationType.PyramidChart]: 'pyramid',
    [VisualizationType.PieChart]: 'pie',
    [VisualizationType.LineChart]: 'line',
    [VisualizationType.ScatterPlot]: 'scatterPlot',
    [VisualizationType.Table]: 'table',
};

export const getChartConfig = (dataset: IJsonStatDataset, locale: string, title?: string): ChartConfig => {
    const visualizationConfig = dataset.extension?.visualizationConfig;
    const selectableConfig = dataset.extension?.selectableConfig;

    return {
        chartType: visualizationConfig?.chartType,
        layout: visualizationConfig?.layout,
        cutValueAxis: visualizationConfig?.cutValueAxis,
        sorting: visualizationConfig?.sorting,
        locale,
        title,
        defaultSelectableSelections: selectableConfig?.defaultSelectableSelections,
        multiSelectableDimensionCode: selectableConfig?.multiSelectableDimensionCode,
        showHeader: true
    };
};

export const getSelectables = (dataset?: IJsonStatDataset, visualizationSettings?: IVisualizationSettings): IJsonStatSelectable[] => {
    if (!dataset) return [];

    const selectableSelections = dataset.extension?.selectableConfig?.selectableSelections ?? {};
    const multiselectableDimensionCode = dataset.extension?.selectableConfig?.multiSelectableDimensionCode
        ?? visualizationSettings?.multiselectableVariableCode;

    return Object.keys(selectableSelections)
        .filter(code => dataset.id.includes(code) && dataset.dimension[code] != null)
        .map(code => {
            const dimension = dataset.dimension[code];
            const categoryCodes = Object.keys(dimension.category.index)
                .sort((left, right) => dimension.category.index[left] - dimension.category.index[right]);

            return {
                dimension: {
                    code,
                    name: { fi: dimension.label ?? code, sv: dimension.label ?? code, en: dimension.label ?? code },
                    note: null,
                    type: EDimensionType.Unknown,
                    values: categoryCodes.map(valueCode => ({
                        code: valueCode,
                        name: { fi: dimension.category.label[valueCode] ?? valueCode, sv: dimension.category.label[valueCode] ?? valueCode, en: dimension.category.label[valueCode] ?? valueCode },
                        note: null,
                        isSum: false,
                    })),
                },
                multiselectable: code === multiselectableDimensionCode,
            };
        });
};

export const getChartType = (visualizationType: VisualizationType): ChartType => chartTypeMap[visualizationType];

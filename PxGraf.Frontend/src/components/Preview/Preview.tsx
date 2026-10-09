import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert } from '@mui/material';
import { styled } from '@mui/material/styles';
import { Chart as PxVisualizerChart, IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { ISelectableSelections, SelectableDimensionMenus } from 'components/SelectableVariableMenus/SelectableDimensionMenus';
import { ISelectabilityInfo } from 'components/SelectableVariableMenus/selectableDimension';
import React from 'react';
import { Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { createChart, ChartInstance } from '@statisticsfinland/jsonstatgraphs';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import { IJsonStatDataset, getChartConfig, getSelectables as getJsonStatSelectables } from 'types/jsonStatChart';
import { VisualizationType } from 'types/visualizationType';
import { VisualizationLibrary } from 'utils/visualizationLibrary';
import { QueryContext } from '../../contexts/queryContext';
import { VisualizationContext } from '../../contexts/visualizationContext';
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { EPreviewSize } from 'types/previewSize';

export type { ISelectabilityInfo };

interface IPreviewProps {
    path: string[];
    query: Query;
    selectedVisualization: string;
    visualizationSettings: IVisualizationSettings;
    previewSize: EPreviewSize;
    visualizationLibrary: VisualizationLibrary;
}

const ResponseWrapper = styled('div')({
    position: 'absolute',
    width: '100%',
    height: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
});

const PreviewCanvas = styled('div', {
    shouldForwardProp: prop => prop !== 'previewSize',
})<{ previewSize: EPreviewSize }>(({ previewSize, theme }) => ({
    width: previewSize,
    height: '100%',
    maxHeight: '100%',
    minHeight: 0,
    margin: 'auto',
    padding: '20px 24px 12px',
    boxSizing: 'border-box',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden',
    backgroundColor: theme.palette.background.paper,
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    boxShadow: theme.shadows[1],
}));

const ChartContainer = styled('div')({
    width: '100%',
    minHeight: 0,
    flex: '1 1 auto',
    overflow: 'auto',
});

const PxVisualizerContainer = styled(ChartContainer)({
    '& > div': {
        width: '100%',
        height: '100% !important',
        display: 'flex',
        flexDirection: 'column',
    },
    '& > div > div:nth-of-type(2)': {
        flex: '1 1 auto',
        minHeight: 0,
        height: 'auto !important',
    },
    '& > div > div:nth-of-type(2) > div': {
        minHeight: 0,
        height: '100% !important',
    },
    '& .highcharts-container, & svg.highcharts-root': {
        width: '100% !important',
        height: '100% !important',
    },
});

export const getSelectables = getJsonStatSelectables;

export const getPxVisualizerSelectables = (data?: IQueryVisualizationResponse): ISelectabilityInfo[] => {
    if (!data) return [];

    return data.selectableVariableCodes.flatMap(code => {
        const variable = data.metaData.find(item => item.code === code);
        if (!variable) return [];

        const dimension = {
            code: variable.code,
            name: variable.name,
            values: variable.values.map(value => ({ code: value.code, name: value.name })),
        };

        return [{ dimension }];
    });
};

export const getResolvedSelections = (selectables: ISelectabilityInfo[], selections: ISelectableSelections, defaultSelectables: ISelectableSelections, multiselectableVariableCode?: string): ISelectableSelections => {
    const newSelections: ISelectableSelections = {};
    selectables.forEach((selectable) => {
        const { dimension } = selectable;
        const availableValues = new Set(dimension.values.map(value => value.code));
        const configuredSelection = selections[dimension.code] ?? defaultSelectables?.[dimension.code];
        const validSelection = configuredSelection?.filter(value => availableValues.has(value));
        const selection = validSelection && validSelection.length > 0
            ? validSelection
            : [dimension.values[0].code];
        newSelections[dimension.code] = multiselectableVariableCode === dimension.code ? selection : [selection[0]];
    });
    return newSelections;
};

/**
 * Preview component for visualizing the chart using the selected visualization type and settings.
 * Additionally, in this view the user can pick values for the selectable dimensions and choose a size for the visualization.
 * @param {string[]} path Path to the table subject to visualization in the Px file system.
 * @param {Query} query Object that represents the current query.
 * @param {string} selectedVisualization Name of the visualization type selected for the preview.
 * @param {IVisualizationSettings} visualizationSettings Visualization settings object.
 * @param {EPreviewSize} previewSize The current preview size to render the chart at.
 * @param {VisualizationLibrary} visualizationLibrary Library used to render the preview.
 */
export const Preview: React.FC<IPreviewProps> = ({ path, query, selectedVisualization, visualizationSettings, previewSize, visualizationLibrary }) => {
    const { t } = useTranslation();
    const { languageTab } = React.useContext(UiLanguageContext);
    const { cubeQuery } = React.useContext(QueryContext);
    const { defaultSelectables } = React.useContext(VisualizationContext);
    const { data, isLoading, isFetching, isError } = useVisualizationQuery(path, query, cubeQuery, languageTab, selectedVisualization, visualizationSettings, visualizationLibrary);
    const showVisualization = data && !isLoading && !isFetching && !isError;
    const { selections, setSelections } = useSelections();
    const selectables = React.useMemo(
        () => visualizationLibrary === 'jsonstatgraphs'
            ? getSelectables(data as IJsonStatDataset, visualizationSettings)
            : getPxVisualizerSelectables(data as IQueryVisualizationResponse),
        [data, visualizationLibrary, visualizationSettings]
    );
    const chartContainerRef = React.useRef<HTMLDivElement>(null);
    const chartRef = React.useRef<ChartInstance>(null);
    const jsonStatData = visualizationLibrary === 'jsonstatgraphs' ? data as IJsonStatDataset : undefined;
    const pxVisualizerData = visualizationLibrary === 'pxvisualizer' ? data as IQueryVisualizationResponse : undefined;
    const responseDefaults = visualizationLibrary === 'jsonstatgraphs'
        ? jsonStatData?.extension?.selectableConfig?.defaultSelectableSelections
        : pxVisualizerData?.visualizationSettings.defaultSelectableVariableCodes;
    const selectionDefaults = React.useMemo(
        () => ({ ...responseDefaults, ...defaultSelectables }),
        [responseDefaults, defaultSelectables]
    );

    const resolvedSelections = React.useMemo(() => {
        return getResolvedSelections(selectables, selections, selectionDefaults, visualizationSettings?.multiselectableVariableCode);
    }, [selectables, selections, selectionDefaults, visualizationSettings?.multiselectableVariableCode]);

    React.useEffect(() => {
        if (visualizationLibrary !== 'jsonstatgraphs' || !showVisualization || !chartContainerRef.current) {
            chartRef.current?.destroy();
            chartRef.current = null;
            return;
        }

        const dataset = jsonStatData as IJsonStatDataset;
        const chartConfig = getChartConfig(dataset, languageTab, dataset.label);
        if (chartRef.current) {
            chartRef.current.update(dataset, chartConfig, resolvedSelections);
        } else {
            chartRef.current = createChart(chartContainerRef.current, dataset, chartConfig, resolvedSelections);
        }
    }, [cubeQuery?.chartHeaderEdit, jsonStatData, languageTab, resolvedSelections, showVisualization, visualizationLibrary]);

    React.useEffect(() => () => {
        chartRef.current?.destroy();
        chartRef.current = null;
    }, []);

    if (isLoading || isFetching || (!data && !isError)) {
        return (
            <ResponseWrapper>
                <CircularProgress />
            </ResponseWrapper>
        );
    } else if (isError) {
        return (
            <ResponseWrapper>
                <Alert severity="error">{t("error.contentLoad")}</Alert>
            </ResponseWrapper>
        );
    }

    return (
        <PreviewCanvas previewSize={previewSize}>
            <SelectableDimensionMenus
                setSelections={setSelections}
                selections={resolvedSelections}
                selectables={selectables}
                multiselectableDimensionCode={visualizationSettings?.multiselectableVariableCode}
            />
            {showVisualization &&
                visualizationLibrary === 'jsonstatgraphs' && jsonStatData &&
                <ChartContainer
                    className='tk-table'
                    style={{ height: jsonStatData.extension?.visualizationConfig?.chartType === 'table' || selectedVisualization === VisualizationType.Table ? 'auto' : '480px' }}
                    ref={chartContainerRef}
                />}
            {showVisualization && visualizationLibrary === 'pxvisualizer' && pxVisualizerData &&
                <PxVisualizerContainer
                    className='tk-table'
                    style={{ height: '100%' }}
                >
                    <PxVisualizerChart
                        pxGraphData={pxVisualizerData}
                        locale={languageTab}
                        selectedVariableCodes={resolvedSelections}
                    />
                </PxVisualizerContainer>}
        </PreviewCanvas>
    );
}

export default Preview;
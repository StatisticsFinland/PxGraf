import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert } from '@mui/material';
import { styled } from '@mui/material/styles';
import { ISelectableSelections, SelectableDimensionMenus } from 'components/SelectableVariableMenus/SelectableDimensionMenus';
import React from 'react';
import { Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { createChart, ChartInstance } from '@statisticsfinland/jsonstatgraphs';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import { IJsonStatDataset, IJsonStatSelectable, getChartConfig, getSelectables as getJsonStatSelectables } from 'types/jsonStatChart';
import { VisualizationType } from 'types/visualizationType';
import { QueryContext } from '../../contexts/queryContext';
import { VisualizationContext } from '../../contexts/visualizationContext';
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { EPreviewSize } from 'types/previewSize';

export type ISelectabilityInfo = IJsonStatSelectable;

interface IPreviewProps {
    path: string[];
    query: Query;
    selectedVisualization: string;
    visualizationSettings: IVisualizationSettings;
    previewSize: EPreviewSize;
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
    margin: 'auto',
    padding: '20px 24px 12px',
    boxSizing: 'border-box',
    backgroundColor: theme.palette.background.paper,
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    boxShadow: theme.shadows[1],
}));

const ChartContainer = styled('div')({});

export const getSelectables = getJsonStatSelectables;

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
 * @param {string} selectedVisualization Name of the visualization type selected for the visualization.
 * @param {IVisualizationSettings} visualizationSettings Visualization settings object.
 * @param {EPreviewSize} previewSize The current preview size to render the chart at.
 */
export const Preview: React.FC<IPreviewProps> = ({ path, query, selectedVisualization, visualizationSettings, previewSize }) => {
    const { t } = useTranslation();
    const { languageTab } = React.useContext(UiLanguageContext);
    const { cubeQuery } = React.useContext(QueryContext);
    const { defaultSelectables } = React.useContext(VisualizationContext);
    const { data, isLoading, isFetching, isError } = useVisualizationQuery(path, query, cubeQuery, languageTab, selectedVisualization, visualizationSettings);
    const showVisualization = data && !isLoading && !isFetching && !isError;
    const { selections, setSelections } = useSelections();
    const selectables = React.useMemo(
        () => getSelectables(data, visualizationSettings),
        [data, visualizationSettings]
    );
    const chartContainerRef = React.useRef<HTMLDivElement>(null);
    const chartRef = React.useRef<ChartInstance>(null);

    const resolvedSelections = React.useMemo(() => {
        return getResolvedSelections(selectables, selections, defaultSelectables, visualizationSettings?.multiselectableVariableCode);
    }, [selectables, selections, defaultSelectables, visualizationSettings?.multiselectableVariableCode]);

    React.useEffect(() => {
        if (!showVisualization || !chartContainerRef.current) {
            chartRef.current?.destroy();
            chartRef.current = null;
            return;
        }

        const chartConfig = getChartConfig(data as IJsonStatDataset, languageTab, data.label);
        if (chartRef.current) {
            chartRef.current.update(data as IJsonStatDataset, chartConfig, resolvedSelections);
        } else {
            chartRef.current = createChart(chartContainerRef.current, data as IJsonStatDataset, chartConfig, resolvedSelections);
        }
    }, [cubeQuery?.chartHeaderEdit, data, languageTab, resolvedSelections, showVisualization]);

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
                <ChartContainer
                    className='tk-table'
                    style={{ height: data.extension?.visualizationConfig?.chartType === 'table' || selectedVisualization === VisualizationType.Table ? 'auto' : '480px' }}
                    ref={chartContainerRef}
                />}
        </PreviewCanvas>
    );
}

export default Preview;
import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert } from '@mui/material';
import { ISelectableSelections, SelectableDimensionMenus } from 'components/SelectableVariableMenus/SelectableDimensionMenus';
import styled from 'styled-components';
import React from 'react';
import { Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { Chart, IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import { IVariable } from '../../types/visualizationResponse';
import { QueryContext } from '../../contexts/queryContext';
import { VisualizationContext } from '../../contexts/visualizationContext';
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { EDimensionType } from '../../types/cubeMeta';
import { EPreviewSize } from 'types/previewSize';

export interface ISelectabilityInfo {
    dimension: IVariable;
    multiselectable: boolean;
}

interface IPreviewProps {
    path: string[];
    query: Query;
    selectedVisualization: string;
    visualizationSettings: IVisualizationSettings;
    previewSize: EPreviewSize;
}

const ResponseWrapper = styled.div`
  position: absolute;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
`;

interface IChartWrapperProps {
    $previewSize: EPreviewSize;
}

const ChartWrapper = styled.div<IChartWrapperProps>`
    width: ${p => p.$previewSize};
    margin: auto;
`;

export const getSelectables = (visualizationResponse: IQueryVisualizationResponse, visualizationSettings?: IVisualizationSettings): ISelectabilityInfo[] => {
    if (!visualizationResponse) return [];
    const { selectableVariableCodes, metaData } = visualizationResponse;

    return selectableVariableCodes.map((code: string) => {
        const metaDataItem = metaData.find(item => item.code === code);
        const dimension = { ...metaDataItem, type: EDimensionType[metaDataItem.type] };

        return {
            dimension,
            multiselectable: visualizationSettings?.multiselectableVariableCode === dimension.code,
        };
    }) ?? [];
};

export const getResolvedSelections = (selectables: ISelectabilityInfo[], selections: ISelectableSelections, defaultSelectables: ISelectableSelections, multiselectableVariableCode?: string): ISelectableSelections => {
    const newSelections: ISelectableSelections = {};
    selectables.forEach((selectable) => {
        const { dimension } = selectable;
        const selection = selections[dimension.code] ?? defaultSelectables?.[dimension.code] ?? [dimension.values[0].code];
        newSelections[dimension.code] = multiselectableVariableCode === dimension.code ? selection : [selection[0]];
    });
    return newSelections;
};

/**
 * Preview component for visualizing the chart using the selected visualization type and settings. Visualization is rendered using @see {@link Chart} component from the PxVisualizer library.
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
    const { data, isLoading, isError } = useVisualizationQuery(path, query, cubeQuery, languageTab, selectedVisualization, visualizationSettings);
    const showVisualization = data && !isLoading && !isError;
    const { selections, setSelections } = useSelections();
    const selectables = getSelectables(data, visualizationSettings);

    const resolvedSelections = React.useMemo(() => {
        return getResolvedSelections(selectables, selections, defaultSelectables, visualizationSettings?.multiselectableVariableCode);
    }, [selectables, selections, defaultSelectables, visualizationSettings]);

    if (isLoading || (!data && !isError)) {
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
        <>
            <SelectableDimensionMenus
                setSelections={setSelections}
                selections={resolvedSelections}
                selectables={selectables}
                multiselectableDimensionCode={visualizationSettings?.multiselectableVariableCode}
            />
            {showVisualization &&
                <ChartWrapper className='tk-table' $previewSize={previewSize}>
                    <Chart
                        locale={languageTab}
                        pxGraphData={data}
                        selectedVariableCodes={resolvedSelections}
                        showTableSources={true}
                        showLastUpdated={true}
                        showTableUnits={true}
                    />
                </ChartWrapper>}
        </>
    );
}

export default Preview;
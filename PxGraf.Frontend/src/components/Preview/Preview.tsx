import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert, ToggleButton, ToggleButtonGroup } from '@mui/material';
import { SelectableVariableMenus } from 'components/SelectableVariableMenus/SelectableVariableMenus';
import styled from 'styled-components';
import React from 'react';
import { ICubeQuery, Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { Chart } from '@statisticsfinland/pxvisualizer';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { IVariable } from '../../types/visualizationResponse';

export interface ISelectabilityInfo {
    variable: IVariable;
    multiselectable: boolean;
}

interface IPreviewProps {
    path: string[];
    query: Query;
    language: string;
    cubeQueryTextEdits: ICubeQuery;
    selectedVisualization: string;
    visualizationSettings: IVisualizationSettings;
}

const ResponseWrapper = styled.div`
  position: absolute;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
`;

const FlexContentWrapper = styled.div`
    display: flex;
    align-items: center;
    padding-bottom: 8px;
    padding-left: 8px;
    padding-right: 8px;
    margin-bottom: 10px;
`;

interface IChartWrapperProps {
    $previewSize: EPreviewSize;
}

const ChartWrapper = styled.div<IChartWrapperProps>`
    width: ${p => p.$previewSize};
    margin: auto;
`;

enum EPreviewSize {
    XL = '100%',
    L = '1200px',
    M = '992px',
    S = '768px',
    XS = '576px',
    XXS = '390px',
    XXXS = '360px',
    XXXXS = '320px'
}

/**
 * Preview component for visualizing the chart using the selected visualization type and settings. Visualization is rendered using @see {@link Chart} component from the PxVisualizer library.
 * Additionally, in this view the user can pick values for the selectable variables and choose a size for the visualization.
 * @param {string[]} path Path to the table subject to visualization in the Px file system.
 * @param {Query} query Object that represents the current query.
 * @param {string} language Content language used for displaying the visualization meta data.
 * @param {ICubeQuery} cubeQueryTextEdits Edited header and meta data texts for the query.
 * @param {string} selectedVisualization Name of the visualization type selected for the visualization.
 * @param {IVisualizationSettings} visualizationSettings Visualization settings object.
 */
export const Preview: React.FC<IPreviewProps> = ({ path, query, language, cubeQueryTextEdits, selectedVisualization, visualizationSettings }) => {
    const { t } = useTranslation();

    const { data, isLoading, isError } = useVisualizationQuery(path, query, cubeQueryTextEdits, language, selectedVisualization, visualizationSettings);

    const showVisualization = data && !isLoading && !isError;

    const { selections, setSelections } = useSelections();
    const [size, setSize] = React.useState<EPreviewSize>(EPreviewSize.XL);

    const buttons = Object.values(EPreviewSize).map((value) =>
        <ToggleButton selected={size === value} value={value} key={value} onClick={() => setSize(value)}>
            {size === value ? <b>{value}</b> : value}
        </ToggleButton>
    );

    if (isLoading) {
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
            {showVisualization &&
                <FlexContentWrapper>
                    <InfoBubble info={t("infoText.rescaleButtons")} ariaLabel={t('tooltip.visualizationSize')} />
                    <ToggleButtonGroup aria-label={t('tooltip.visualizationSize')} color={'primary'} exclusive>
                        {buttons}
                    </ToggleButtonGroup>
                </FlexContentWrapper>}
            <SelectableVariableMenus
                setSelections={setSelections}
                data={data}
                selectedVisualization={selectedVisualization}
                visualizationSettings={visualizationSettings} />
            {showVisualization && <ChartWrapper className='tk-table' $previewSize={size}><Chart locale={language} pxGraphData={data} selectedVariableCodes={selections} /></ChartWrapper>}
        </>
    );
}

export default Preview;
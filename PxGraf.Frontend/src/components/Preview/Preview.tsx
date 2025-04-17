import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert, ToggleButton, ToggleButtonGroup } from '@mui/material';
import { SelectableDimensionMenus } from 'components/SelectableVariableMenus/SelectableDimensionMenus';
import styled from 'styled-components';
import React from 'react';
import { Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { Chart } from '@statisticsfinland/pxvisualizer';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { IVariable } from '../../types/visualizationResponse';
import { EditorContext } from '../../contexts/editorContext';
import UiLanguageContext from '../../contexts/uiLanguageContext';

export interface ISelectabilityInfo {
    dimension: IVariable;
    multiselectable: boolean;
}

interface IPreviewProps {
    path: string[];
    query: Query;
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
 * Additionally, in this view the user can pick values for the selectable dimensions and choose a size for the visualization.
 * @param {string[]} path Path to the table subject to visualization in the Px file system.
 * @param {Query} query Object that represents the current query.
 * @param {string} selectedVisualization Name of the visualization type selected for the visualization.
 * @param {IVisualizationSettings} visualizationSettings Visualization settings object.
 */
export const Preview: React.FC<IPreviewProps> = ({ path, query, selectedVisualization, visualizationSettings }) => {
    const { t } = useTranslation();
    const { languageTab } = React.useContext(UiLanguageContext);
    const { cubeQuery } = React.useContext(EditorContext);

    const { data, isLoading, isError } = useVisualizationQuery(path, query, cubeQuery, languageTab, selectedVisualization, visualizationSettings);

    const showVisualization = data && !isLoading && !isError;
    const { selections, setSelections } = useSelections();
    const [size, setSize] = React.useState<EPreviewSize>(EPreviewSize.XL);

    const buttons = Object.values(EPreviewSize).map((value) =>
        <ToggleButton selected={size === value} value={value} key={value} onClick={() => setSize(value)}>
            {size === value ? <b>{value}</b> : value}
        </ToggleButton>
    );

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
            {showVisualization &&
                <FlexContentWrapper>
                    <InfoBubble info={t("infoText.rescaleButtons")} ariaLabel={t('tooltip.visualizationSize')} />
                    <ToggleButtonGroup aria-label={t('tooltip.visualizationSize')} color={'primary'} exclusive>
                        {buttons}
                    </ToggleButtonGroup>
                </FlexContentWrapper>}
            <SelectableDimensionMenus
                setSelections={setSelections}
                data={data}
                selectedVisualization={selectedVisualization}
                visualizationSettings={visualizationSettings} />
            {showVisualization && <ChartWrapper className='tk-table' $previewSize={size}><Chart locale={languageTab} pxGraphData={data} selectedVariableCodes={selections} /></ChartWrapper>}
        </>
    );
}

export default Preview;
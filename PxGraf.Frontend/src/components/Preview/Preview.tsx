import { useTranslation } from 'react-i18next';
import { CircularProgress, Alert, ToggleButton, ToggleButtonGroup } from '@mui/material';
import { ISelectableSelections, SelectableDimensionMenus } from 'components/SelectableVariableMenus/SelectableDimensionMenus';
import styled from 'styled-components';
import React from 'react';
import { Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useVisualizationQuery } from 'api/services/visualization';
import { Chart, IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import useSelections from 'components/SelectableVariableMenus/hooks/useSelections';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { IVariable } from '../../types/visualizationResponse';
import { EditorContext } from '../../contexts/editorContext';
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { EDimensionType } from '../../types/cubeMeta';

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
 */
export const Preview: React.FC<IPreviewProps> = ({ path, query, selectedVisualization, visualizationSettings }) => {
    const { t } = useTranslation();
    const { languageTab } = React.useContext(UiLanguageContext);
    const { cubeQuery, defaultSelectables } = React.useContext(EditorContext);
    const { data, isLoading, isError } = useVisualizationQuery(path, query, cubeQuery, languageTab, selectedVisualization, visualizationSettings);
    const showVisualization = data && !isLoading && !isError;
    const { selections, setSelections } = useSelections();
    const [size, setSize] = React.useState<EPreviewSize>(EPreviewSize.XL);
    const selectables = getSelectables(data, visualizationSettings);

    const resolvedSelections = React.useMemo(() => {
        return getResolvedSelections(selectables, selections, defaultSelectables, visualizationSettings?.multiselectableVariableCode);
    }, [selectables, selections, defaultSelectables, visualizationSettings]);

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
                selections={resolvedSelections}
                selectables={selectables}
                multiselectableDimensionCode={visualizationSettings?.multiselectableVariableCode}
            />
            {showVisualization &&
                <ChartWrapper className='tk-table' $previewSize={size}>
                    <Chart
                        locale={languageTab}
                        pxGraphData={data}
                        selectedVariableCodes={resolvedSelections}
                        showTableSources={true}
                        showLastUpdated={true}
                    />
                </ChartWrapper>}
        </>
    );
}

export default Preview;
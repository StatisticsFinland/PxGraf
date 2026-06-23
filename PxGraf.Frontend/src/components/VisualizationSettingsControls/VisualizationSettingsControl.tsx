import TablePivotSettings from "./TypeSpecificControls/TablePivotSettings";
import { MarkerScaler } from "./UtilityComponents/MarkerScaler";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import { IDimension, EDimensionType } from "types/cubeMeta";
import { Query } from "types/query";
import InfoBubble from "components/InfoBubble/InfoBubble";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { VisualizationSettingsSwitch } from "./UtilityComponents/VisualizationSettingsSwitch";
import SortingSelector from './UtilityComponents/SortingSelector';
import React from 'react';
import { MultiselectableSelector } from "./TypeSpecificControls/MultiselectableSelector";
import { IVisualizationOptions } from "../../types/editorContentsResponse";

export interface IVisualizationSettingControlProps {
    selectedVisualization: VisualizationType,
    dimensions: IDimension[],
    dimensionQuery: Query,
    visualizationOptions: IVisualizationOptions,
    visualizationSettings: IVisualizationSettings,
}

export interface IVisualizationSettingsProps {
    visualizationOptions: IVisualizationOptions,
    visualizationSettings: IVisualizationSettings,
}

const SettingsRow = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    padding-left: 8px;
`;

const ControlsWrapper = styled.div`
    display: flex;
    align-items: center;
    gap: 16px;
`;

export const VisualizationSettingControl: React.FC<IVisualizationSettingControlProps> = ({
    selectedVisualization,
    dimensions,
    dimensionQuery,
    visualizationOptions,
    visualizationSettings,
}) => {
    const sortingOptions = visualizationOptions?.allowManualPivot && visualizationSettings.pivotRequested ? visualizationOptions?.sortingOptions.pivoted : visualizationOptions?.sortingOptions.default;
    const showTableSettings: boolean = selectedVisualization === VisualizationType.Table;
    const showSortingOptions: boolean = (sortingOptions?.length > 0);
    const showMarkerScaler: boolean = visualizationOptions?.allowSetMarkerScale;
    const showYAxisCutting: boolean = visualizationOptions?.allowCuttingYAxis;
    const showMatchXLabelsToEnd: boolean = visualizationOptions?.allowMatchXLabelsToEnd;
    const showPivot: boolean = visualizationOptions?.allowManualPivot;
    const showDataPoints: boolean = visualizationOptions?.allowShowingDataPoints;

    const { t } = useTranslation();
    const selectableDimensions: IDimension[] = dimensions.filter(v => dimensionQuery[v.code].selectable);
    const selectableDimensionsExcludingContent: IDimension[] = selectableDimensions?.filter(fv => fv.type !== EDimensionType.Content);
    const showMultiselectableSelector: boolean = visualizationOptions?.allowMultiselect && selectableDimensionsExcludingContent.length > 0;

    return (
        <SettingsRow>
            <InfoBubble info={t('infoText.visualizationConfiguration')} ariaLabel={t('tooltip.visualizationConfig')} />
            <ControlsWrapper>
                {showTableSettings && (
                    <TablePivotSettings
                        visualizationOptions={visualizationOptions}
                        dimensions={dimensions}
                        selectableDimensions={selectableDimensions}
                        query={dimensionQuery}
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showSortingOptions && (
                    <SortingSelector
                        visualizationSettings={visualizationSettings}
                        sortingOptions={sortingOptions}
                    />
                )}
                {showMarkerScaler && (
                    <MarkerScaler
                        visualizationOptions={visualizationOptions}
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showMultiselectableSelector && (
                    <MultiselectableSelector
                        visualizationOptions={visualizationOptions}
                        dimensions={selectableDimensionsExcludingContent}
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showYAxisCutting && (
                    <VisualizationSettingsSwitch
                        selected={visualizationSettings.cutYAxis}
                        label="chartSettings.cutYAxis"
                        changeProperty="cutYAxis"
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showMatchXLabelsToEnd && (
                    <VisualizationSettingsSwitch
                        selected={visualizationSettings.matchXLabelsToEnd}
                        label="chartSettings.matchXLabelsToEnd"
                        changeProperty="matchXLabelsToEnd"
                        hidden={true}
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showPivot && (
                    <VisualizationSettingsSwitch
                        selected={visualizationSettings.pivotRequested}
                        label="chartSettings.pivot"
                        changeProperty="pivotRequested"
                        visualizationSettings={visualizationSettings}
                    />
                )}
                {showDataPoints && (
                    <VisualizationSettingsSwitch
                        selected={visualizationSettings.showDataPoints}
                        label="visualizationSettings.showDataPoints"
                        changeProperty="showDataPoints"
                        visualizationSettings={visualizationSettings}
                    />
                )}
            </ControlsWrapper>
        </SettingsRow>
    )
}

export default VisualizationSettingControl;
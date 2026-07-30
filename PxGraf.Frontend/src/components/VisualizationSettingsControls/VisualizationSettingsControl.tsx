import TablePivotSettings from "./TypeSpecificControls/TablePivotSettings";
import { MarkerScaler } from "./UtilityComponents/MarkerScaler";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import { IDimension } from "types/cubeMeta";
import { Query } from "types/query";
import styled from "styled-components";
import { VisualizationSettingsSwitch } from "./UtilityComponents/VisualizationSettingsSwitch";
import SortingSelector from './UtilityComponents/SortingSelector';
import React from 'react';
import { MultiselectableSelector } from "./TypeSpecificControls/MultiselectableSelector";
import { IVisualizationOptions } from "../../types/editorContentsResponse";
import { getVisualizationSettingVisibility } from '../../utils/editorHelpers';

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
    align-items: center;
    width: 100%;
`;

const ControlsWrapper = styled.div`
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: 8px 24px;
    width: 100%;
`;

export const VisualizationSettingControl: React.FC<IVisualizationSettingControlProps> = ({
    selectedVisualization,
    dimensions,
    dimensionQuery,
    visualizationOptions,
    visualizationSettings,
}) => {
    const selectableDimensions = dimensions.filter(dimension => dimensionQuery[dimension.code].selectable);
    const {
        sortingOptions,
        selectableDimensionsExcludingContent,
        showTableSettings,
        showSortingOptions,
        showMarkerScaler,
        showMultiselectableSelector,
        showYAxisCutting,
        showPivot,
        showDataPoints,
    } = getVisualizationSettingVisibility(selectedVisualization, dimensions, dimensionQuery, visualizationOptions, visualizationSettings);

    return (
        <SettingsRow>
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
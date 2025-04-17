import TablePivotSettings from "./TypeSpecificControls/TablePivotSettings";
import { MarkerScaler } from "./UtilityComponents/MarkerScaler";
import { IVisualizationSettings } from "types/visualizationSettings";
import { IVisualizationRules } from "types/visualizationRules";
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

export interface IVisualizationSettingControlProps {
    selectedVisualization: string,
    visualizationRules: IVisualizationRules,
    dimensions: IDimension[],
    dimensionQuery: Query,
    visualizationSettings: IVisualizationSettings
}

export interface IVisualizationSettingsProps {
    visualizationRules: IVisualizationRules,
    visualizationSettings: IVisualizationSettings,
}

const SettingsWrapper = styled.div`
    display: flex;
    padding-left: 24px;
    align-items: center;
`;

export const VisualizationSettingControl: React.FC<IVisualizationSettingControlProps> = ({
    selectedVisualization,
    visualizationRules,
    dimensions,
    dimensionQuery,
    visualizationSettings
}) => {
    const showTableSettings: boolean = selectedVisualization === VisualizationType.Table;
    const showSortingOptions: boolean = (visualizationRules.sortingOptions?.length > 0);
    const showMarkerScaler: boolean = visualizationRules.visualizationTypeSpecificRules.allowSetMarkerScale;
    const showYAxisCutting: boolean = visualizationRules.visualizationTypeSpecificRules.allowCuttingYAxis;
    const showMatchXLabelsToEnd: boolean = visualizationRules.visualizationTypeSpecificRules.allowMatchXLabelsToEnd;
    const showPivot: boolean = visualizationRules.allowManualPivot;
    const showDataPoints: boolean = visualizationRules.visualizationTypeSpecificRules.allowShowingDataPoints;

    const { t } = useTranslation();
    const selectableDimensions: IDimension[] = dimensions.filter(v => dimensionQuery[v.code].selectable);
    const selectableDimensionsExcludingContent: IDimension[] = selectableDimensions?.filter(fv => fv.type !== EDimensionType.Content);
    const showMultiselectableSelector: boolean = visualizationRules.multiselectDimensionAllowed && selectableDimensionsExcludingContent.length > 0;

    return (
        <div>
            <InfoBubble info={t('infoText.visualizationConfiguration')} ariaLabel={t('tooltip.visualizationConfig')} />
            {(showTableSettings || showSortingOptions || showMarkerScaler || showMultiselectableSelector) && (
                <SettingsWrapper>
                    {showTableSettings && (
                        <TablePivotSettings
                            visualizationRules={visualizationRules}
                            dimensions={dimensions}
                            selectableDimensions={selectableDimensions}
                            query={dimensionQuery}
                            visualizationSettings={visualizationSettings}
                        />
                    )}
                    {showSortingOptions && (
                        <SortingSelector
                            visualizationSettings={visualizationSettings}
                            sortingOptions={visualizationRules.sortingOptions}
                        />
                    )}
                    {showMarkerScaler && (
                        <MarkerScaler
                            visualizationRules={visualizationRules}
                            visualizationSettings={visualizationSettings}
                        />
                    )}
                    {showMultiselectableSelector && (
                        <MultiselectableSelector
                            visualizationRules={visualizationRules}
                            dimensions={selectableDimensionsExcludingContent}
                            visualizationSettings={visualizationSettings}
                        />
                    )}
                </SettingsWrapper>
            )}
            {(showYAxisCutting || showMatchXLabelsToEnd || showPivot || showDataPoints) && (
                <SettingsWrapper>
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
                    {(showPivot) && (
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
                </SettingsWrapper>
            )}
        </div>
    )
}

export default VisualizationSettingControl;
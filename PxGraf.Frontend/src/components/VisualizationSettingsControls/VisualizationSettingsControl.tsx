import TablePivotSettings from "./TypeSpecificControls/TablePivotSettings";
import { MarkerScaler } from "./UtilityComponents/MarkerScaler";
import { IVisualizationSettings } from "types/visualizationSettings";
import { IVisualizationRules } from "types/visualizationRules";
import { VisualizationType } from "types/visualizationType";
import { IDimension, VariableType } from "types/cubeMeta";
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
    visualizationSettings: IVisualizationSettings,
    visualizationRules: IVisualizationRules,
    settingsChangedHandler: SettingsChangedHandler,
    variables: IDimension[],
    variableQuery: Query,
}

export type SettingsChangedHandler = (newSettings: IVisualizationSettings) => void;

export interface IVisualizationSettingsProps {
    visualizationRules: IVisualizationRules,
    visualizationSettings: IVisualizationSettings
    settingsChangedHandler: SettingsChangedHandler
}

const SettingsWrapper = styled.div`
    display: flex;
    padding-left: 24px;
    align-items: center;
`;

export const VisualizationSettingControl: React.FC<IVisualizationSettingControlProps> = ({
    selectedVisualization,
    visualizationSettings,
    visualizationRules,
    settingsChangedHandler,
    variables,
    variableQuery,
}) => {

    const showTableSettings: boolean = selectedVisualization === VisualizationType.Table;
    const showSortingOptions: boolean = (visualizationRules.sortingOptions?.length > 0);
    const showMarkerScaler: boolean = visualizationRules.visualizationTypeSpecificRules.allowSetMarkerScale;
    const showYAxisCutting: boolean = visualizationRules.visualizationTypeSpecificRules.allowCuttingYAxis;
    const showMatchXLabelsToEnd: boolean = visualizationRules.visualizationTypeSpecificRules.allowMatchXLabelsToEnd;
    const showPivot: boolean = visualizationRules.allowManualPivot;
    const showDataPoints: boolean = visualizationRules.visualizationTypeSpecificRules.allowShowingDataPoints;

    const { t } = useTranslation();
    const selectableVariables: IDimension[] = variables.filter(v => variableQuery[v.Code].selectable);
    const selectableVariablesExcludingContent: IDimension[] = selectableVariables?.filter(fv => fv.Type !== VariableType.Content);
    const showMultiselectableSelector: boolean = visualizationRules.multiselectVariableAllowed && selectableVariablesExcludingContent.length > 0;

    return (
        <div>
            <InfoBubble info={t('infoText.visualizationConfiguration')} ariaLabel={t('tooltip.visualizationConfig')} />
            {(showTableSettings || showSortingOptions || showMarkerScaler || showMultiselectableSelector) && (
                <SettingsWrapper>
                    {showTableSettings && (
                        <TablePivotSettings
                            visualizationRules={visualizationRules}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                            variables={variables}
                            selectableVariables={selectableVariables}
                            query={variableQuery}
                        />
                    )}
                    {showSortingOptions && (
                        <SortingSelector
                            sortingOptions={visualizationRules.sortingOptions}
                            activeSortingCode={visualizationSettings.sorting}
                            sortingChangedHandler={(sorting) => settingsChangedHandler({ ...visualizationSettings, sorting: sorting })}
                        />
                    )}
                    {showMarkerScaler && (
                        <MarkerScaler
                            visualizationRules={visualizationRules}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                        />
                    )}
                    {showMultiselectableSelector && (
                        <MultiselectableSelector
                            visualizationRules={visualizationRules}
                            settingsChangedHandler={settingsChangedHandler}
                            visualizationSettings={visualizationSettings}
                            variables={selectableVariablesExcludingContent}
                        />
                    )}
                </SettingsWrapper>
            )}
            {(showYAxisCutting || showMatchXLabelsToEnd || showPivot || showDataPoints) && (
                <SettingsWrapper>
                    {showYAxisCutting && (
                        <VisualizationSettingsSwitch
                            selected={visualizationSettings.cutYAxis}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                            label="chartSettings.cutYAxis"
                            changeProperty="cutYAxis"
                        />
                    )}
                    {showMatchXLabelsToEnd && (
                        <VisualizationSettingsSwitch
                            selected={visualizationSettings.matchXLabelsToEnd}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                            label="chartSettings.matchXLabelsToEnd"
                            changeProperty="matchXLabelsToEnd"
                            hidden={true}
                        />
                    )}
                    {(showPivot) && (
                        <VisualizationSettingsSwitch
                            selected={visualizationSettings.pivotRequested}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                            label="chartSettings.pivot"
                            changeProperty="pivotRequested"
                        />
                    )}
                    {showDataPoints && (
                        <VisualizationSettingsSwitch
                            selected={visualizationSettings.showDataPoints}
                            visualizationSettings={visualizationSettings}
                            settingsChangedHandler={settingsChangedHandler}
                            label="visualizationSettings.showDataPoints"
                            changeProperty="showDataPoints"
                        />
                    )}
                </SettingsWrapper>
            )}
        </div>
    )
}

export default VisualizationSettingControl;
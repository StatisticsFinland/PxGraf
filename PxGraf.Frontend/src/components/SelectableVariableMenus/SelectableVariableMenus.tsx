import { Stack } from '@mui/material';
import { ValueSelect } from './ValueSelect';
import React, { Dispatch, SetStateAction, useEffect, useState } from 'react';
import { ISelectabilityInfo } from 'components/Preview/Preview';
import { IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { VariableType } from 'types/cubeMeta';
import { IVisualizationSettings } from 'types/visualizationSettings';

export interface IMenuProps {
    [key: string]: string[];
}

export interface ISelectionState {
    activeSelections: IMenuProps;
    visualization: string;
    multiselect: string;
}

interface ISelectableVariableMenusProps {
    data: IQueryVisualizationResponse;
    setSelections: Dispatch<SetStateAction<IMenuProps>>;
    selectedVisualization: string;
    visualizationSettings: IVisualizationSettings;
}

export const getSelectables = (data: IQueryVisualizationResponse, visualizationSettings?: IVisualizationSettings): ISelectabilityInfo[] => {
    if (!data) return [];

    const { selectableVariableCodes, metaData } = data;

    return selectableVariableCodes.map((code: string) => {
        const metaDataItem = metaData.find(item => item.code === code);
        const variable = { ...metaDataItem, type: VariableType[metaDataItem.type] };

        return {
            variable,
            multiselectable: visualizationSettings?.multiselectableVariableCode === variable.code,
        };
    }) ?? [];
};

export const getSelections = (
    selectables: ISelectabilityInfo[],
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    state: ISelectionState,
    setState: Dispatch<SetStateAction<ISelectionState>>
) => {
    const newState: ISelectionState = {
        activeSelections: {},
        visualization: selectedVisualization,
        multiselect: visualizationSettings?.multiselectableVariableCode
    };

    selectables.forEach(({ variable, multiselectable }) => {
        const { code } = variable;

        if (newState.visualization !== state.visualization
            || newState.multiselect !== state.multiselect
            || state.activeSelections[code] == null
            || state.activeSelections[code].length === 0) {
            newState.activeSelections[code] = [variable.values[0].code];
        } else {
            newState.activeSelections[code] = multiselectable
                ? state.activeSelections[code]
                : [state.activeSelections[code][0]];
        }
    });

    // Update the active selections only when they have changed.
    if (JSON.stringify(newState) !== JSON.stringify(state)) setState(newState);
    return newState.activeSelections;
}

export const SelectableVariableMenus: React.FC<ISelectableVariableMenusProps> = ({ data, selectedVisualization, visualizationSettings, setSelections }) => {

    const [selectionState, setSelectionState] = useState<ISelectionState>({ activeSelections: null, visualization: null, multiselect: null });

    const selectables = getSelectables(data, visualizationSettings);
    const selections = getSelections(selectables, selectedVisualization, visualizationSettings, selectionState, setSelectionState);

    useEffect(() => {
        setSelections(selections);
    }, [selectionState]);

    const onValueChanged = (newSelections) => setSelectionState({ ...selectionState, activeSelections: newSelections as IMenuProps });

    return (
        <Stack direction="row" spacing={2}>
            {selectables?.map(v => {
                return <ValueSelect
                    key={v.variable.code}
                    variable={v.variable}
                    multiselect={v.multiselectable}
                    activeSelections={selections[v.variable.code] ?? []}
                    onValueChanged={value => {
                        if (value != null && value.length > 0) {
                            onValueChanged({
                                ...selections,
                                [v.variable.code]: typeof value === 'string'
                                    ? value.split(',')
                                    : value
                            })
                        }
                    }}
                />
            })}
        </Stack>
    )
}

export default SelectableVariableMenus;
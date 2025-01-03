import {
    FormControl, Select, MenuItem, InputLabel
} from '@mui/material';
import React from 'react';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IVariable } from '../../types/visualizationResponse';

export interface IValueSelectProps {
    dimension: IVariable;
    multiselect: boolean;
    activeSelections: string | string[];
    onValueChanged: (value: string | string[]) => void;
}

export const ValueSelect: React.FC<IValueSelectProps> = ({ dimension, multiselect, activeSelections, onValueChanged }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <FormControl>
            <InputLabel id={dimension.code + "-selection-label"}>{dimension.name[uiContentLanguage]}</InputLabel>
            <Select
                labelId={dimension.code + "-selection-label"}
                id={dimension.code + "-variable-select"}
                value={activeSelections}
                label={dimension.name[uiContentLanguage]}
                onChange={(e) => onValueChanged(e.target.value)}
                multiple={multiselect}
            >
                {dimension.values.map(value => { return <MenuItem key={value.code} value={value.code}>{value.name[uiContentLanguage]}</MenuItem> })}
            </Select>
        </FormControl>
    )
}

export default ValueSelect;
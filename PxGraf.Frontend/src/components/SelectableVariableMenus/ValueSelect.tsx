import {
    FormControl, Select, MenuItem, InputLabel
} from '@mui/material';
import React from 'react';
import { IVariable } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

export interface IValueSelectProps {
    variable: IVariable;
    multiselect: boolean;
    activeSelections: string | string[];
    onValueChanged: (value: string | string[]) => void;
}

export const ValueSelect: React.FC<IValueSelectProps> = ({ variable, multiselect, activeSelections, onValueChanged }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <FormControl>
            <InputLabel id={variable.code + "-selection-label"}>{variable.name[uiContentLanguage]}</InputLabel>
            <Select
                labelId={variable.code + "-selection-label"}
                id={variable.code + "-variable-select"}
                value={activeSelections}
                label={variable.name[uiContentLanguage]}
                onChange={(e) => onValueChanged(e.target.value)}
                multiple={multiselect}
            >
                {variable.values.map(value => { return <MenuItem key={value.code} value={value.code}>{value.name[uiContentLanguage]}</MenuItem> })}
            </Select>
        </FormControl>
    )
}

export default ValueSelect;
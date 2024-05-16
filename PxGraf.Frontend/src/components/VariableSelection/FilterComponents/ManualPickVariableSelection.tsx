import React from 'react';
import { Autocomplete, TextField } from '@mui/material';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';
import { IVariableValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface ManualPickVariableSelectionProps {
    options: IVariableValue[],
    selectedValues: IVariableValue[],
    onQueryChanged: (selectedValues: string[]) => void
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    width: 90%;
    background-color: #fff;
`;

export const ManualPickVariableSelection: React.FC<ManualPickVariableSelectionProps> = ({ options, selectedValues, onQueryChanged }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const handleChange = (_evt, newValues: IVariableValue[]) => {
        const selectedCodes = newValues.map(newValue => newValue.code);
        onQueryChanged(selectedCodes);
    }

    return (
        <StyledAutocomplete
            multiple
            options={options}
            getOptionLabel={(option: IVariableValue) => option.name[uiContentLanguage] ?? option.code}
            isOptionEqualToValue={(option: IVariableValue, value: IVariableValue) => option.code === value.code}
            value={selectedValues}
            onChange={handleChange}
            openText={t("selectable.open")}
            closeText={t("selectable.close")}
            clearText={t("selectable.clear")}
            noOptionsText={t("selectable.noSelections")}
            renderInput={(params) => (
                <TextField {...params} label={t("variableSelect.itemFilter")} />
            )}
        />
    );
}

export default ManualPickVariableSelection;
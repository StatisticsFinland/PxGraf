import React from 'react';
import { Autocomplete, Chip, TextField } from '@mui/material';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';
import { IDimensionValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface ManualPickDimensionSelectionProps {
    options: IDimensionValue[],
    selectedValues: IDimensionValue[],
    onQueryChanged: (selectedValues: string[]) => void
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    width: 90%;
    background-color: #fff;
`;

export const ManualPickDimensionSelection: React.FC<ManualPickDimensionSelectionProps> = ({ options, selectedValues, onQueryChanged }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const handleChange = (_evt, newValues: IDimensionValue[]) => {
        const selectedCodes = newValues.map(newValue => newValue.code);
        onQueryChanged(selectedCodes);
    }

    return (
        <StyledAutocomplete
            multiple
            options={options}
            getOptionLabel={(option: IDimensionValue) => option.name[uiContentLanguage] ?? option.code}
            isOptionEqualToValue={(option: IDimensionValue, value: IDimensionValue) => option.code === value.code}
            value={selectedValues}
            onChange={handleChange}
            openText={t("selectable.open")}
            closeText={t("selectable.close")}
            clearText={t("selectable.clear")}
            noOptionsText={t("selectable.noSelections")}
            renderTags={(tagValue, getTagProps) => {
                return tagValue.map((option: IDimensionValue, index) => {
                    return <Chip 
                        {...getTagProps({ index })}
                        key={index + '-key'}
                        label={option.name[uiContentLanguage] ?? option.code}
                        />
                })
            }}
            renderOption={(props, option: IDimensionValue) => {
                return (
                    <li {...props} key={option.code}>
                        {option.name[uiContentLanguage] ?? option.code}
                    </li>
                );
            }}
            renderInput={(params) => (
                <TextField {...params} label={t("variableSelect.itemFilter")} />
            )}
        />
    );
}

export default ManualPickDimensionSelection;
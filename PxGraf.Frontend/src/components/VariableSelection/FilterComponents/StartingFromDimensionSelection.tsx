import React from 'react';
import { Autocomplete, TextField } from '@mui/material';
import styled from 'styled-components';
import { IDimensionValue } from 'types/cubeMeta';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IStartingFromDimensionSelectionProps {
    options: IDimensionValue[],
    startingCode: string,
    onQueryChanged: (newCode: string) => void
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    background-color: #fff;
`;

export const StartingFromDimensionSelection: React.FC<IStartingFromDimensionSelectionProps> = ({ options, startingCode, onQueryChanged }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const handleChange = (_event, newValue: IDimensionValue) => {
        onQueryChanged(newValue?.code ?? null);
    }

    return (
        <StyledAutocomplete
            options={options}
            getOptionLabel={(option: IDimensionValue) => option.name[uiContentLanguage] ?? option.code}
            isOptionEqualToValue={(option: IDimensionValue, value: IDimensionValue) => option.code === value.code}
            value={options.find((o: IDimensionValue) => o.code === startingCode)}
            onChange={handleChange}
            renderInput={(params) => (
                <TextField {...params} label={t("variableSelect.fromFilter")} />
            )}
        />
    );
}

export default StartingFromDimensionSelection;
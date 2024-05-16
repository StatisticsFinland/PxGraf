import React from 'react';
import { Autocomplete, TextField } from '@mui/material';
import styled from 'styled-components';
import { IVariableValue } from 'types/cubeMeta';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IStartingFromVariableSelectionProps {
    options: IVariableValue[],
    startingCode: string,
    onQueryChanged: (newCode: string) => void
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    background-color: #fff;
`;

export const StartingFromVariableSelection: React.FC<IStartingFromVariableSelectionProps> = ({ options, startingCode, onQueryChanged }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const handleChange = (_event, newValue: IVariableValue) => {
        onQueryChanged(newValue?.code ?? null);
    }

    return (
        <StyledAutocomplete
            options={options}
            getOptionLabel={(option: IVariableValue) => option.name[uiContentLanguage] ?? option.code}
            isOptionEqualToValue={(option: IVariableValue, value: IVariableValue) => option.code === value.code}
            value={options.find((o: IVariableValue) => o.code === startingCode)}
            onChange={handleChange}
            renderInput={(params) => (
                <TextField {...params} label={t("variableSelect.fromFilter")} />
            )}
        />
    );
}

export default StartingFromVariableSelection;
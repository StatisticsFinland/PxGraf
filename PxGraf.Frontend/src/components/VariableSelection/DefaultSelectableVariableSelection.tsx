import React from 'react';
import { Autocomplete, TextField } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';
import { IDimensionValue } from 'types/cubeMeta';
import { EditorContext } from 'contexts/editorContext';

interface IDefaultSelectableVariableSelection {
    variableCode: string;
    resolvedVariableValueCodes: string[];
    options: IDimensionValue[],
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    width: 90%;
`;


export const DefaultSelectableVariableSelection: React.FC<IDefaultSelectableVariableSelection> = ({ variableCode, resolvedVariableValueCodes, options }) => {
    const { language } = React.useContext(UiLanguageContext);
    const { defaultSelectables, setDefaultSelectables } = React.useContext(EditorContext);
    const { t } = useTranslation();

    // Note: array filtering to support selecting multiple values in the future
    const defaultOptionValues = defaultSelectables?.[variableCode] ? options.filter((option) => defaultSelectables[variableCode].includes(option.Code)) : [];
    const [value, setValue] = React.useState<IDimensionValue>(defaultOptionValues[0] ? defaultOptionValues[0] : null);

    React.useEffect(() => {
        if (value && resolvedVariableValueCodes.length > 0) {
            if (!resolvedVariableValueCodes.includes(value.Code)) {
                const defaultSelectablesCopy = { ...defaultSelectables };
                delete defaultSelectablesCopy[variableCode];
                setDefaultSelectables(defaultSelectablesCopy);
                setValue(null);
            }
        }
    }, [resolvedVariableValueCodes]);

    const handleChange = (_evt, value: IDimensionValue) => {
        if (value) {
            setDefaultSelectables({
                ...defaultSelectables,
                [variableCode]: [value.Code]
            })
        } else {
            const defaultSelectablesCopy = { ...defaultSelectables };
            delete defaultSelectablesCopy[variableCode];
            setDefaultSelectables(defaultSelectablesCopy);
        }
        setValue(value);
    };

    return (<StyledAutocomplete
        options={options.filter(option => resolvedVariableValueCodes.indexOf(option.Code) > -1)}
        getOptionLabel={(option: IDimensionValue) => option?.Name[language] ?? option.Code}
        isOptionEqualToValue={() => true}
        value={value}
        onChange={handleChange}
        noOptionsText={t("selectable.noSelections")}
        openText={t("selectable.open")}
        closeText={t("selectable.close")}
        clearText={t("selectable.clear")}
        renderInput={(params) => (
            <TextField {...params} label={t("selectable.labelText")} />
        )}
    />);

}

export default DefaultSelectableVariableSelection;
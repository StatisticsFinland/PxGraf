import React from 'react';
import { Autocomplete, TextField } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';
import { IDimensionValue } from 'types/cubeMeta';
import { EditorContext } from 'contexts/editorContext';

interface IDefaultSelectableDimensionSelection {
    dimensionCode: string;
    resolvedDimensionValueCodes: string[];
    options: IDimensionValue[],
}

const StyledAutocomplete = styled(Autocomplete)`
    flex-basis: 0;
    flex-grow: 1;
    width: 90%;
`;


export const DefaultSelectableDimensionSelection: React.FC<IDefaultSelectableDimensionSelection> = ({ dimensionCode, resolvedDimensionValueCodes, options }) => {
    const { language } = React.useContext(UiLanguageContext);
    const { defaultSelectables, setDefaultSelectables } = React.useContext(EditorContext);
    const { t } = useTranslation();

    // Note: array filtering to support selecting multiple values in the future
    const defaultOptionValues = defaultSelectables?.[dimensionCode] ? options.filter((option) => defaultSelectables[dimensionCode].includes(option.code)) : [];
    const [value, setValue] = React.useState<IDimensionValue>(defaultOptionValues[0] ? defaultOptionValues[0] : null);

    React.useEffect(() => {
        if (value && resolvedDimensionValueCodes.length > 0) {
            if (!resolvedDimensionValueCodes.includes(value.code)) {
                const defaultSelectablesCopy = { ...defaultSelectables };
                delete defaultSelectablesCopy[dimensionCode];
                setDefaultSelectables(defaultSelectablesCopy);
                setValue(null);
            }
        }
    }, [resolvedDimensionValueCodes]);

    const handleChange = (_evt, value: IDimensionValue) => {
        if (value) {
            setDefaultSelectables({
                ...defaultSelectables,
                [dimensionCode]: [value.code]
            })
        } else {
            const defaultSelectablesCopy = { ...defaultSelectables };
            delete defaultSelectablesCopy[dimensionCode];
            setDefaultSelectables(defaultSelectablesCopy);
        }
        setValue(value);
    };

    return (<StyledAutocomplete
        options={options.filter(option => resolvedDimensionValueCodes.indexOf(option.code) > -1)}
        getOptionLabel={(option: IDimensionValue) => option?.name[language] ?? option.code}
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

export default DefaultSelectableDimensionSelection;
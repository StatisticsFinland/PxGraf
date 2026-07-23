import React from 'react';
import { Autocomplete, TextField, ToggleButton, ToggleButtonGroup } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { IDimensionValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface SumOperationFormProps {
    availableValues: IDimensionValue[];
    operandCodes: string[];
    constant?: number;
    onChange: (operandCodes: string[], constant?: number) => void;
}

export const SumOperationForm: React.FC<SumOperationFormProps> = ({
    availableValues,
    operandCodes,
    constant,
    onChange,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const [useConstant, setUseConstant] = React.useState(constant !== undefined);
    const [localConstant, setLocalConstant] = React.useState(constant ?? 0);

    const selectedValues = operandCodes
        .map(code => availableValues.find(v => v.code === code))
        .filter((v): v is IDimensionValue => v !== undefined);

    const handleValueChange = (_evt: React.SyntheticEvent, newValues: IDimensionValue[]) => {
        onChange(newValues.map(v => v.code), useConstant ? localConstant : undefined);
    };

    const handleToggleConstant = (_: React.MouseEvent<HTMLElement>, val: 'value' | 'constant' | null) => {
        if (val === null) return;
        const checked = val === 'constant';
        setUseConstant(checked);
        onChange(operandCodes, checked ? localConstant : undefined);
    };

    const handleConstantChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const parsed = Number.parseFloat(event.target.value);
        const value = Number.isNaN(parsed) ? 0 : parsed;
        setLocalConstant(value);
        onChange(operandCodes, value);
    };

    return (
        <>
            <Autocomplete
                multiple
                options={availableValues}
                getOptionLabel={(option: IDimensionValue) => option.name[uiContentLanguage] ?? option.code}
                isOptionEqualToValue={(option: IDimensionValue, value: IDimensionValue) => option.code === value.code}
                value={selectedValues}
                onChange={handleValueChange}
                openText={t('selectable.open')}
                closeText={t('selectable.close')}
                clearText={t('selectable.clear')}
                noOptionsText={t('selectable.noSelections')}
                renderOption={(props, option: IDimensionValue) => (
                    <li {...props} key={option.code}>
                        {option.name[uiContentLanguage] ?? option.code}
                    </li>
                )}
                renderInput={(params) => (
                    <TextField {...params} label={t('computedValues.selectValues')} />
                )}
            />
            <ToggleButtonGroup
                exclusive
                size="small"
                color="primary"
                value={useConstant ? 'constant' : 'value'}
                onChange={handleToggleConstant}
                aria-label={t('computedValues.operandType')}
                sx={{ mt: 1 }}
            >
                <ToggleButton value="value">{t('computedValues.useValue')}</ToggleButton>
                <ToggleButton value="constant">{t('computedValues.constant')}</ToggleButton>
            </ToggleButtonGroup>
            {useConstant && (
                <TextField
                    type="number"
                    label={t('computedValues.constant')}
                    value={localConstant}
                    onChange={handleConstantChange}
                    sx={{ mt: 1 }}
                />
            )}
        </>
    );
};

export default SumOperationForm;

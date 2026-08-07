import React from 'react';
import {
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    SelectChangeEvent,
    TextField,
    ToggleButton,
    ToggleButtonGroup,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { IDimensionValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface MultiplicationOperationFormProps {
    availableValues: IDimensionValue[];
    operandCodes: string[];
    constant?: number;
    onChange: (operandCodes: string[], constant?: number) => void;
}

export const MultiplicationOperationForm: React.FC<MultiplicationOperationFormProps> = ({
    availableValues,
    operandCodes,
    constant,
    onChange,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const [useConstant, setUseConstant] = React.useState(constant !== undefined);
    const [localConstant, setLocalConstant] = React.useState(constant ?? 1);

    const valueCode = operandCodes[0] ?? '';
    const multiplyByCode = useConstant ? '' : (operandCodes[1] ?? '');

    const handleValueChange = (event: SelectChangeEvent<string>) => {
        const newValue = event.target.value;
        if (useConstant) {
            onChange([newValue].filter(Boolean), localConstant);
        } else {
            onChange([newValue, multiplyByCode].filter(Boolean), undefined);
        }
    };

    const handleMultiplyByChange = (event: SelectChangeEvent<string>) => {
        const newMultiplyBy = event.target.value;
        onChange([valueCode, newMultiplyBy].filter(Boolean), undefined);
    };

    const handleToggleConstant = (_: React.MouseEvent<HTMLElement>, val: 'value' | 'constant' | null) => {
        if (val === null) return;
        const checked = val === 'constant';
        setUseConstant(checked);
        if (checked) {
            onChange([valueCode].filter(Boolean), localConstant);
        } else {
            onChange([valueCode].filter(Boolean), undefined);
        }
    };

    const handleConstantChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const parsed = Number.parseFloat(event.target.value);
        const value = Number.isNaN(parsed) ? 0 : parsed;
        setLocalConstant(value);
        onChange([valueCode].filter(Boolean), value);
    };

    return (
        <>
            <FormControl fullWidth sx={{ mt: 1 }}>
                <InputLabel>{t('computedValues.baseValue')}</InputLabel>
                <Select
                    value={valueCode}
                    label={t('computedValues.baseValue')}
                    onChange={handleValueChange}
                >
                    {availableValues.map(v => (
                        <MenuItem key={v.code} value={v.code}>
                            {v.name[uiContentLanguage] ?? v.code}
                        </MenuItem>
                    ))}
                </Select>
            </FormControl>
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
            {useConstant ? (
                <TextField
                    type="number"
                    label={t('computedValues.constant')}
                    value={localConstant}
                    onChange={handleConstantChange}
                    sx={{ mt: 1 }}
                    fullWidth
                />
            ) : (
                <FormControl fullWidth sx={{ mt: 1 }}>
                    <InputLabel>{t('computedValues.multiplyByValue')}</InputLabel>
                    <Select
                        value={multiplyByCode}
                        label={t('computedValues.multiplyByValue')}
                        onChange={handleMultiplyByChange}
                    >
                        {availableValues.map(v => (
                            <MenuItem key={v.code} value={v.code}>
                                {v.name[uiContentLanguage] ?? v.code}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            )}
        </>
    );
};

export default MultiplicationOperationForm;

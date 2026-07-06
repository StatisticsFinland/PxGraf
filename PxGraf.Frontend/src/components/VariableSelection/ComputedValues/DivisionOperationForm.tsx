import React from 'react';
import {
    FormControl,
    FormHelperText,
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

interface DivisionOperationFormProps {
    availableValues: IDimensionValue[];
    operandCodes: string[];
    constant?: number;
    onChange: (operandCodes: string[], constant?: number) => void;
}

export const DivisionOperationForm: React.FC<DivisionOperationFormProps> = ({
    availableValues,
    operandCodes,
    constant,
    onChange,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const [useConstant, setUseConstant] = React.useState(constant !== undefined);
    const [localConstant, setLocalConstant] = React.useState(constant ?? 1);

    const dividendCode = operandCodes[0] ?? '';
    const divisorCode = useConstant ? '' : (operandCodes[1] ?? '');
    const divisionByZero = useConstant && localConstant === 0;

    const handleDividendChange = (event: SelectChangeEvent<string>) => {
        const newDividend = event.target.value;
        if (useConstant) {
            onChange([newDividend].filter(Boolean), localConstant);
        } else {
            onChange([newDividend, divisorCode].filter(Boolean), undefined);
        }
    };

    const handleDivisorChange = (event: SelectChangeEvent<string>) => {
        const newDivisor = event.target.value;
        onChange([dividendCode, newDivisor].filter(Boolean), undefined);
    };

    const handleToggleConstant = (_: React.MouseEvent<HTMLElement>, val: 'value' | 'constant' | null) => {
        if (val === null) return;
        const checked = val === 'constant';
        setUseConstant(checked);
        if (checked) {
            onChange([dividendCode].filter(Boolean), localConstant);
        } else {
            onChange([dividendCode].filter(Boolean), undefined);
        }
    };

    const handleConstantChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const parsed = Number.parseFloat(event.target.value);
        const value = Number.isNaN(parsed) ? 0 : parsed;
        setLocalConstant(value);
        onChange([dividendCode].filter(Boolean), value);
    };

    return (
        <>
            <FormControl fullWidth sx={{ mt: 1 }}>
                <InputLabel>{t('computedValues.dividendValue')}</InputLabel>
                <Select
                    value={dividendCode}
                    label={t('computedValues.dividendValue')}
                    onChange={handleDividendChange}
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
                sx={{ mt: 1 }}
            >
                <ToggleButton value="value">{t('computedValues.useValue')}</ToggleButton>
                <ToggleButton value="constant">{t('computedValues.constant')}</ToggleButton>
            </ToggleButtonGroup>
            {useConstant ? (
                <FormControl fullWidth sx={{ mt: 1 }} error={divisionByZero}>
                    <TextField
                        type="number"
                        label={t('computedValues.constant')}
                        value={localConstant}
                        onChange={handleConstantChange}
                        error={divisionByZero}
                        fullWidth
                    />
                    {divisionByZero && (
                        <FormHelperText>{t('computedValues.validationDivisionByZero')}</FormHelperText>
                    )}
                </FormControl>
            ) : (
                <FormControl fullWidth sx={{ mt: 1 }}>
                    <InputLabel>{t('computedValues.divisorValue')}</InputLabel>
                    <Select
                        value={divisorCode}
                        label={t('computedValues.divisorValue')}
                        onChange={handleDivisorChange}
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

export default DivisionOperationForm;

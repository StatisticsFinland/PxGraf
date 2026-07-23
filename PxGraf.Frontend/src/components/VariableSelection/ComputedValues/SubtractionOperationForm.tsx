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

interface SubtractionOperationFormProps {
    availableValues: IDimensionValue[];
    operandCodes: string[];
    constant?: number;
    onChange: (operandCodes: string[], constant?: number) => void;
}

export const SubtractionOperationForm: React.FC<SubtractionOperationFormProps> = ({
    availableValues,
    operandCodes,
    constant,
    onChange,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const [useConstant, setUseConstant] = React.useState(constant !== undefined);
    const [localConstant, setLocalConstant] = React.useState(constant ?? 0);

    const baseCode = operandCodes[0] ?? '';
    const subtractCode = useConstant ? '' : (operandCodes[1] ?? '');

    const handleBaseChange = (event: SelectChangeEvent<string>) => {
        const newBase = event.target.value;
        if (useConstant) {
            onChange([newBase], localConstant);
        } else {
            onChange([newBase, subtractCode].filter(Boolean), undefined);
        }
    };

    const handleSubtractValueChange = (event: SelectChangeEvent<string>) => {
        const newSubtract = event.target.value;
        onChange([baseCode, newSubtract].filter(Boolean), undefined);
    };

    const handleToggleConstant = (_: React.MouseEvent<HTMLElement>, val: 'value' | 'constant' | null) => {
        if (val === null) return;
        const checked = val === 'constant';
        setUseConstant(checked);
        if (checked) {
            onChange([baseCode].filter(Boolean), localConstant);
        } else {
            onChange([baseCode].filter(Boolean), undefined);
        }
    };

    const handleConstantChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const parsed = Number.parseFloat(event.target.value);
        const value = Number.isNaN(parsed) ? 0 : parsed;
        setLocalConstant(value);
        onChange([baseCode].filter(Boolean), value);
    };

    return (
        <>
            <FormControl fullWidth sx={{ mt: 1 }}>
                <InputLabel>{t('computedValues.baseValue')}</InputLabel>
                <Select
                    value={baseCode}
                    label={t('computedValues.baseValue')}
                    onChange={handleBaseChange}
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
                    <InputLabel>{t('computedValues.subtractValue')}</InputLabel>
                    <Select
                        value={subtractCode}
                        label={t('computedValues.subtractValue')}
                        onChange={handleSubtractValueChange}
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

export default SubtractionOperationForm;

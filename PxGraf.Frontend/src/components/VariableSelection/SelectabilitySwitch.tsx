import React from 'react';
import { useTranslation } from 'react-i18next';

import { FormControlLabel, FormControl, Switch } from '@mui/material';

interface ISelectabilitySwitchProps {
    selected: boolean,
    onChange: (newValue: boolean) => object
}

export const SelectabilitySwitch: React.FC<ISelectabilitySwitchProps> = ({ selected, onChange }) => {
    const { t } = useTranslation();

    return (
        <FormControl fullWidth>
            <FormControlLabel
                control={
                    <Switch
                        checked={selected ?? false}
                        onChange={(event) => onChange(event.target.checked)}
                    />
                }
                label={t("variableSelect.selectable")}
            />
        </FormControl>
    )
}

export default SelectabilitySwitch;
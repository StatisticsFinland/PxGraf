import React from 'react';
import { useTranslation } from 'react-i18next';
import { FormControlLabel, FormControl, Switch, Typography } from '@mui/material';
import { IVisualizationSettings } from "types/visualizationSettings";
import { SettingsChangedHandler } from '../VisualizationSettingsControl';
import styled from "styled-components";

const HiddenFormControl = styled(FormControl)`
    display: none !important;
`;

const LabelTypography = styled(Typography)`
  min-width: 125px;
  max-width: 200px;
`;

interface IVisualizationSettingsSwitchProps {
    selected: boolean,
    visualizationSettings: IVisualizationSettings,
    settingsChangedHandler: SettingsChangedHandler,
    label: string,
    changeProperty: keyof IVisualizationSettings,
    hidden?: boolean
}

export const VisualizationSettingsSwitch: React.FC<IVisualizationSettingsSwitchProps> = ({ selected, visualizationSettings, settingsChangedHandler, label, changeProperty, hidden }) => {
    const { t } = useTranslation();
    const Control = hidden ? HiddenFormControl : FormControl;

    return (
        <Control fullWidth>
            <FormControlLabel
                control={
                    <Switch
                        checked={selected}
                        onChange={(event) => settingsChangedHandler({ ...visualizationSettings, [changeProperty]: event.target.checked })}
                    />
                }
                label={<LabelTypography>{t(label)}</LabelTypography>}
            />
        </Control>
    )
}

export default VisualizationSettingsSwitch;
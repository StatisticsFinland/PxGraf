import React from 'react';
import { useTranslation } from 'react-i18next';
import { FormControlLabel, FormControl, Switch, Typography } from '@mui/material';
import { IVisualizationSettings } from "types/visualizationSettings";
import styled from "styled-components";
import { EditorContext } from '../../../contexts/editorContext';

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
    label: string,
    changeProperty: keyof IVisualizationSettings,
    hidden?: boolean
}

export const VisualizationSettingsSwitch: React.FC<IVisualizationSettingsSwitchProps> = ({ selected, visualizationSettings, label, changeProperty, hidden }) => {
    const { t } = useTranslation();
    const Control = hidden ? HiddenFormControl : FormControl;
    const { setVisualizationSettingsUserInput } = React.useContext(EditorContext);

    return (
        <Control fullWidth>
            <FormControlLabel
                control={
                    <Switch
                        checked={selected}
                        onChange={(event) => setVisualizationSettingsUserInput({ ...visualizationSettings, [changeProperty]: event.target.checked })}
                    />
                }
                label={<LabelTypography>{t(label)}</LabelTypography>}
            />
        </Control>
    )
}

export default VisualizationSettingsSwitch;
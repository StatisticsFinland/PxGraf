import { Box, Stack, Typography, Slider } from '@mui/material';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { IVisualizationSettingsProps } from '../VisualizationSettingsControl';
import styled from 'styled-components';

const FormControlWrapper = styled(Stack)`
    gap: 40px;
    align-items: center;
`;

const StyledSlider = styled(Slider)`
    min-width: 150px;
    margin-right: 16px;
`;

export const MarkerScaler: React.FC<IVisualizationSettingsProps> = ({ visualizationSettings, settingsChangedHandler }) => {
    const { t } = useTranslation();
    // We dont want to trigger the api call every time the slider moves, only when the change is committed.
    const [sliderValue, setSliderValue] = React.useState(visualizationSettings.markerSize);

    const marks = [
        {
            value: 1,
            label: '1%',
        },
        {
            value: 100,
            label: '100%',
        },
        {
            value: 200,
            label: '200%',
        }
    ];

    return (
        <FormControlWrapper direction="row">
            <Box>
                <Typography id="input-slider" gutterBottom>
                    {t("chartSettings.markerScale")}
                </Typography>
                <StyledSlider
                    min={1}
                    max={200}
                    marks={marks}
                    value={sliderValue}
                    onChange={(_event, value) => setSliderValue(typeof (value) == "number" ? value : 100)}
                    onChangeCommitted={(_event, value) => settingsChangedHandler({ ...visualizationSettings, markerSize: typeof (value) == "number" ? value : 100 })}
                    aria-label={t("chartSettings.markerScaleDefaultLabel")}
                    valueLabelDisplay="auto" />
            </Box>
        </FormControlWrapper>

    );
}

export default MarkerScaler;
import { useTranslation } from 'react-i18next';
import { ToggleButton, ToggleButtonGroup } from '@mui/material';
import React from 'react';
import { VisualizationContext } from '../../contexts/visualizationContext';

interface IChartTypeSelectorProps {
    possibleTypes: string[];
    selectedType?: string;
}

export const ChartTypeSelector: React.FC<IChartTypeSelectorProps> = ({ possibleTypes, selectedType }) => {
    const { t } = useTranslation();
    const isSelected = (type) => {
        return selectedType ? selectedType === type : type === possibleTypes[0];
    };
    const { setSelectedVisualizationUserInput } = React.useContext(VisualizationContext);

    return (
        <ToggleButtonGroup
            aria-label={t('tooltip.visualizationType')}
            value={"1"}
            exclusive
            size="small"
        >
            {!possibleTypes || possibleTypes.length < 1
                ? <span>{t("general.noPossibleVisualizations")}</span>
                : possibleTypes.map((type) => {
                    return <ToggleButton value={type}
                        key={type}
                        onClick={(_event, value) => setSelectedVisualizationUserInput(value)}
                        selected={isSelected(type)}
                    >
                        {isSelected(type) ? <b>{t('chartTypes.' + type)}</b> : t('chartTypes.' + type)}
                    </ToggleButton>
                })
            }
        </ToggleButtonGroup>
    );
}

export default ChartTypeSelector;
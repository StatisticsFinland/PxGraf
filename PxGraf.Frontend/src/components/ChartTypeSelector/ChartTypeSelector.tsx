import { useTranslation } from 'react-i18next';
import { ToggleButton, ToggleButtonGroup } from '@mui/material';
import React from 'react';

interface IChartTypeSelectorProps {
    possibleTypes: string[];
    selectedType?: string;
    onTypeSelected: (value: string) => void;
}

export const ChartTypeSelector: React.FC<IChartTypeSelectorProps> = ({ possibleTypes, selectedType, onTypeSelected }) => {
    const { t } = useTranslation();
    const isSelected = (type) => {
        return !selectedType ? type === possibleTypes[0] : selectedType === type;
    };

    return (
        <ToggleButtonGroup
            aria-label={t('tooltip.visualizationType')}
            value={"1"}
            exclusive
        >
            {!possibleTypes || possibleTypes.length < 1
                ? <span>{t("general.noPossibleVisualizations")}</span>
                : possibleTypes.map((type) => {
                    return <ToggleButton value={type}
                        key={type}
                        onClick={(_event, value) => onTypeSelected(value)}
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
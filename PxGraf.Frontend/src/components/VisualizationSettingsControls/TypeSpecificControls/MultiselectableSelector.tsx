import React from 'react';
import { FormControl, Select, InputLabel, MenuItem} from '@mui/material';
import { IVisualizationSettingsProps } from '../VisualizationSettingsControl';
import { IVariable } from "types/cubeMeta";
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { useTranslation } from 'react-i18next';

interface MultiselectableSelectorProps extends IVisualizationSettingsProps {
    variables: IVariable[]
}

export const MultiselectableSelector: React.FC<MultiselectableSelectorProps> = ({ settingsChangedHandler, visualizationSettings, variables  }) => {
    const { t } = useTranslation();
    const { language, languageTab } = React.useContext(UiLanguageContext);

    return (
        <FormControl>
            <InputLabel id="multiselectable-selector-label">{t("chartSettings.multiSelectVariable")}</InputLabel>
            <Select
                sx={{ minWidth: 210 }}
                labelId="multiselectable-selector-label"
                id="multiselectable-selector"
                label={"Monivalitaselausmuuttuja"}
                value={visualizationSettings.multiselectableVariableCode ?? "noMultiselectable"}
                defaultValue={"noMultiselectable"}
                onChange={(event) => settingsChangedHandler({
                    ...visualizationSettings,
                    multiselectableVariableCode: event.target.value !== "noMultiselectable" ? event.target.value : null
                })}
            >
                <MenuItem value={"noMultiselectable"}>{t("chartSettings.noMultiselectable")}</MenuItem>
                {variables.map(v => { return <MenuItem key={"key" + v.code} value={v.code}>{v.name[language] ?? v.name[languageTab] ?? v.code}</MenuItem> })}
            </Select>
        </FormControl>
    )
}
import React from 'react';
import { FormControl, Select, InputLabel, MenuItem} from '@mui/material';
import { IVisualizationSettingsProps } from '../VisualizationSettingsControl';
import { IDimension } from "types/cubeMeta";
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { useTranslation } from 'react-i18next';
import { EditorContext } from '../../../contexts/editorContext';

interface IMultiselectableSelectorProps extends IVisualizationSettingsProps {
    dimensions: IDimension[]
}

export const MultiselectableSelector: React.FC<IMultiselectableSelectorProps> = ({ visualizationSettings, dimensions  }) => {
    const { t } = useTranslation();
    const { language, languageTab } = React.useContext(UiLanguageContext);
    const { setVisualizationSettingsUserInput } = React.useContext(EditorContext);

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
                onBlur={(event) => setVisualizationSettingsUserInput({
                    ...visualizationSettings,
                    multiselectableVariableCode: event.target.value !== "noMultiselectable" ? event.target.value : null
                })}
            >
                <MenuItem value={"noMultiselectable"}>{t("chartSettings.noMultiselectable")}</MenuItem>
                {dimensions.map(v => { return <MenuItem key={"key" + v.code} value={v.code}>{v.name[language] ?? v.name[languageTab] ?? v.code}</MenuItem> })}
            </Select>
        </FormControl>
    )
}
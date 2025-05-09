import {
    FormControl, Select, MenuItem, InputLabel
} from '@mui/material';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IVisualizationSettings } from '../../../types/visualizationSettings';
import { EditorContext } from '../../../contexts/editorContext';
import { ISortingOption } from '../../../types/editorContentsResponse';

interface ISortingSelectorProps {
    sortingOptions: ISortingOption[],
    visualizationSettings: IVisualizationSettings
}

export const SortingSelector: React.FC<ISortingSelectorProps> = ({ sortingOptions, visualizationSettings }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const { setVisualizationSettingsUserInput } = React.useContext(EditorContext);

    return (
        <FormControl>
            <InputLabel id="sorting-selector-label">{t("chartSettings.sort")}</InputLabel>
            <Select
                sx={{ minWidth: 220 }}
                labelId="sorting-selector-label"
                id="sorting-selector"
                label={t("chartSettings.sort")}
                value={visualizationSettings.sorting}
                onBlur={(event) => setVisualizationSettingsUserInput({ ...visualizationSettings, sorting: event.target.value })}
            >
                {sortingOptions.map(v => { return <MenuItem key={"key" + v.code} value={v.code}>{v.description[uiContentLanguage]}</MenuItem> })}
            </Select>
        </FormControl>
    );
}

export default SortingSelector;
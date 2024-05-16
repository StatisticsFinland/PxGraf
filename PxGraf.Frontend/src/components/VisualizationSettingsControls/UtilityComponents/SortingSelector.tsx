import {
    FormControl, Select, MenuItem, InputLabel
} from '@mui/material';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { ISortingOption } from 'types/visualizationRules';

interface ISortingSelectorProps {
    sortingOptions: ISortingOption[],
    activeSortingCode: string,
    sortingChangedHandler: (newSortingCode: string) => void
}

export const SortingSelector: React.FC<ISortingSelectorProps> = ({ sortingOptions, activeSortingCode, sortingChangedHandler }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <FormControl>
            <InputLabel id="sorting-selector-label">{t("chartSettings.sort")}</InputLabel>
            <Select
                sx={{ minWidth: 220 }}
                labelId="sorting-selector-label"
                id="sorting-selector"
                label={t("chartSettings.sort")}
                value={activeSortingCode}
                onChange={(event) => sortingChangedHandler(event.target.value)}
            >
                {sortingOptions.map(v => { return <MenuItem key={"key" + v.code} value={v.code}>{v.description[uiContentLanguage]}</MenuItem> })}
            </Select>
        </FormControl>
    );
}

export default SortingSelector;
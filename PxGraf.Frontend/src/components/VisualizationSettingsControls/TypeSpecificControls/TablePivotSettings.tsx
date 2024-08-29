import { Stack, IconButton, FormControl } from '@mui/material';

import React, { useState } from 'react';

import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import SwapHorizIcon from '@mui/icons-material/SwapHoriz';

import { useTranslation } from 'react-i18next';
import VariableList from '../UtilityComponents/VariableList';
import { IVisualizationSettingsProps } from '../VisualizationSettingsControl';
import { IDimension } from 'types/cubeMeta';
import styled from 'styled-components';
import { Query } from "types/query";

const VariableListWrapper = styled(Stack)`
    padding: 8px;
    align-items: center;
    min-width: 375px;
`;

const IconWrapper = styled(Stack)`
    min-width: 75px;
`;

interface ITableSettingsProps extends IVisualizationSettingsProps {
    variables: IDimension[]
    selectableVariables?: IDimension[]
    query?: Query
}

export const TablePivotSettings: React.FC<ITableSettingsProps> = ({ visualizationSettings, settingsChangedHandler, variables, selectableVariables, query }) => {
    const { t } = useTranslation();
    const [selected, setSelected] = useState("");

    const upHandler = () => {
        if (selected == null) return;
        if (visualizationSettings.columnVariableCodes.includes(selected) && visualizationSettings.columnVariableCodes[0] !== selected) {
            const varsCopy = [...visualizationSettings.columnVariableCodes];
            const i = varsCopy.indexOf(selected);
            varsCopy[i] = varsCopy[i - 1];
            varsCopy[i - 1] = selected;
            settingsChangedHandler({ ...visualizationSettings, columnVariableCodes: varsCopy });
        }
        else if (visualizationSettings.rowVariableCodes.includes(selected) && visualizationSettings.rowVariableCodes[0] !== selected) {
            const varsCopy = [...visualizationSettings.rowVariableCodes];
            const i = varsCopy.indexOf(selected);
            varsCopy[i] = varsCopy[i - 1];
            varsCopy[i - 1] = selected;
            settingsChangedHandler({ ...visualizationSettings, rowVariableCodes: varsCopy });
        }
    }

    const downHandler = () => {
        if (selected == null) return;
        if (visualizationSettings.columnVariableCodes.includes(selected) && visualizationSettings.columnVariableCodes[visualizationSettings.columnVariableCodes.length - 1] !== selected) {
            const varsCopy = [...visualizationSettings.columnVariableCodes];
            const i = varsCopy.indexOf(selected);
            varsCopy[i] = varsCopy[i + 1];
            varsCopy[i + 1] = selected;
            settingsChangedHandler({ ...visualizationSettings, columnVariableCodes: varsCopy });
        }
        else if (visualizationSettings.rowVariableCodes.includes(selected) && visualizationSettings.rowVariableCodes[visualizationSettings.rowVariableCodes.length - 1] !== selected) {
            const varsCopy = [...visualizationSettings.rowVariableCodes];
            const i = varsCopy.indexOf(selected);
            varsCopy[i] = varsCopy[i + 1];
            varsCopy[i + 1] = selected;
            settingsChangedHandler({ ...visualizationSettings, rowVariableCodes: varsCopy });
        }
    }

    const swapHandler = () => {
        const rowCopy = [...visualizationSettings.rowVariableCodes];
        const columnCopy = [...visualizationSettings.columnVariableCodes];
        const rowIndex = rowCopy.indexOf(selected);
        const columnIndex = columnCopy.indexOf(selected);
        if (rowIndex > -1) {
            rowCopy.splice(rowIndex, 1);
            settingsChangedHandler({ ...visualizationSettings, rowVariableCodes: rowCopy, columnVariableCodes: [...columnCopy, selected] });
        } else if (columnIndex > -1) {
            columnCopy.splice(columnIndex, 1);
            settingsChangedHandler({ ...visualizationSettings, rowVariableCodes: [...rowCopy, selected], columnVariableCodes: columnCopy });
        }
    }

    const getMultiValueVariables = (codes): IDimension[] => {
        //Filter multi value variables and variables that are defined with either "from" or "all" value filters. Exclude selectables - except for multivalueseletable variable
        const multiValueVariableCodes = codes
            .map(c => variables.find(v => v.Code === c))
            .filter(v => variables.filter(v => query[v.Code].valueFilter.type === 'from' || query[v.Code].valueFilter.type === 'all')?.includes(v) || v.Values.length > 1)
            .filter(v => !selectableVariables?.includes(v) || v.Code === visualizationSettings.multiselectableVariableCode);
        return multiValueVariableCodes;
    }

    return (
        <FormControl fullWidth>
            <VariableListWrapper direction="row">
                <VariableList
                    title={t("chartSettings.rowVariables")}
                    variables={getMultiValueVariables(visualizationSettings.rowVariableCodes)}
                    selectedVariableCode={selected}
                    selectedChangedHandler={(newSel) => setSelected(newSel)}
                />
                <IconWrapper>
                    <IconButton aria-label={t('tableSettings.up')} onClick={() => upHandler()}>
                        <ArrowUpwardIcon />
                    </IconButton>
                    <IconButton aria-label={t('tableSettings.down')} onClick={() => downHandler()}>
                        <ArrowDownwardIcon />
                    </IconButton>
                    <IconButton aria-label={t('tableSettings.swap')} onClick={() => swapHandler()}>
                        <SwapHorizIcon />
                    </IconButton>
                </IconWrapper>
                <VariableList
                    title={t("chartSettings.columnVariables")}
                    variables={getMultiValueVariables(visualizationSettings.columnVariableCodes)}
                    selectedVariableCode={selected}
                    selectedChangedHandler={(newSel) => setSelected(newSel)}
                />
            </VariableListWrapper>
        </FormControl>
    );
}

export default TablePivotSettings;
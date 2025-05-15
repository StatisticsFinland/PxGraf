import { Stack, IconButton, FormControl } from '@mui/material';
import React, { useState } from 'react';
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import SwapHorizIcon from '@mui/icons-material/SwapHoriz';
import { useTranslation } from 'react-i18next';
import DimensionList from '../UtilityComponents/DimensionList';
import { IVisualizationSettingsProps } from '../VisualizationSettingsControl';
import { IDimension } from 'types/cubeMeta';
import styled from 'styled-components';
import { Query } from "types/query";
import { EditorContext } from '../../../contexts/editorContext';

const DimensionListWrapper = styled(Stack)`
    padding: 8px;
    align-items: center;
    min-width: 375px;
`;

const IconWrapper = styled(Stack)`
    min-width: 75px;
`;

interface ITableSettingsProps extends IVisualizationSettingsProps {
    dimensions: IDimension[]
    selectableDimensions?: IDimension[]
    query?: Query
}

export const TablePivotSettings: React.FC<ITableSettingsProps> = ({ visualizationSettings, dimensions, selectableDimensions, query }) => {
    const { t } = useTranslation();
    const [selected, setSelected] = useState("");
    const { setVisualizationSettingsUserInput } = React.useContext(EditorContext);

    const upHandler = () => {
        if (selected == null) return;
        if (visualizationSettings.columnVariableCodes.includes(selected) && visualizationSettings.columnVariableCodes[0] !== selected) {
            const dimsCopy = [...visualizationSettings.columnVariableCodes];
            const i = dimsCopy.indexOf(selected);
            dimsCopy[i] = dimsCopy[i - 1];
            dimsCopy[i - 1] = selected;
            setVisualizationSettingsUserInput({ ...visualizationSettings, columnVariableCodes: dimsCopy });
        }
        else if (visualizationSettings.rowVariableCodes.includes(selected) && visualizationSettings.rowVariableCodes[0] !== selected) {
            const dimsCopy = [...visualizationSettings.rowVariableCodes];
            const i = dimsCopy.indexOf(selected);
            dimsCopy[i] = dimsCopy[i - 1];
            dimsCopy[i - 1] = selected;
            setVisualizationSettingsUserInput({ ...visualizationSettings, rowVariableCodes: dimsCopy });
        }
    }

    const downHandler = () => {
        if (selected == null) return;
        if (visualizationSettings.columnVariableCodes.includes(selected) && visualizationSettings.columnVariableCodes[visualizationSettings.columnVariableCodes.length - 1] !== selected) {
            const dimsCopy = [...visualizationSettings.columnVariableCodes];
            const i = dimsCopy.indexOf(selected);
            dimsCopy[i] = dimsCopy[i + 1];
            dimsCopy[i + 1] = selected;
            setVisualizationSettingsUserInput({ ...visualizationSettings, columnVariableCodes: dimsCopy });
        }
        else if (visualizationSettings.rowVariableCodes.includes(selected) && visualizationSettings.rowVariableCodes[visualizationSettings.rowVariableCodes.length - 1] !== selected) {
            const dimsCopy = [...visualizationSettings.rowVariableCodes];
            const i = dimsCopy.indexOf(selected);
            dimsCopy[i] = dimsCopy[i + 1];
            dimsCopy[i + 1] = selected;
            setVisualizationSettingsUserInput({ ...visualizationSettings, rowVariableCodes: dimsCopy });
        }
    }

    const swapHandler = () => {
        const rowCopy = [...visualizationSettings.rowVariableCodes];
        const columnCopy = [...visualizationSettings.columnVariableCodes];
        const rowIndex = rowCopy.indexOf(selected);
        const columnIndex = columnCopy.indexOf(selected);
        if (rowIndex > -1) {
            rowCopy.splice(rowIndex, 1);
            setVisualizationSettingsUserInput({ ...visualizationSettings, rowVariableCodes: rowCopy, columnVariableCodes: [...columnCopy, selected] });
        } else if (columnIndex > -1) {
            columnCopy.splice(columnIndex, 1);
            setVisualizationSettingsUserInput({ ...visualizationSettings, rowVariableCodes: [...rowCopy, selected], columnVariableCodes: columnCopy });
        }
    }

    const getMultivalueDimensions = (codes): IDimension[] => {
        //Filter multi value dimensions and dimensions that are defined with either "from" or "all" value filters. Exclude selectables - except for multivalueseletable dimension
        const multiValueDimensionCodes = codes
            .map(c => dimensions.find(v => v.code === c))
            .filter(v => dimensions.filter(v => query[v.code].valueFilter.type === 'from' || query[v.code].valueFilter.type === 'all')?.includes(v) || v.values.length > 1)
            .filter(v => !selectableDimensions?.includes(v) || v.Code === visualizationSettings.multiselectableVariableCode);
        return multiValueDimensionCodes;
    }

    return (
        <FormControl fullWidth>
            <DimensionListWrapper direction="row">
                <DimensionList
                    title={t("chartSettings.rowVariables")}
                    dimensions={getMultivalueDimensions(visualizationSettings.rowVariableCodes)}
                    selectedDimensionCode={selected}
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
                <DimensionList
                    title={t("chartSettings.columnVariables")}
                    dimensions={getMultivalueDimensions(visualizationSettings.columnVariableCodes)}
                    selectedDimensionCode={selected}
                    selectedChangedHandler={(newSel) => setSelected(newSel)}
                />
            </DimensionListWrapper>
        </FormControl>
    );
}

export default TablePivotSettings;
import DimensionSelectionList from 'components/VariableSelection/DimensionSelectionList';
import React from 'react';
import { Box, Button } from '@mui/material';
import EditNoteIcon from '@mui/icons-material/EditNote';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';

interface EditorFilterSectionProps {
    dimensions: IDimension[],
    resolvedDimensionCodes: { [key: string]: string[] }
    queries: Query
    width?: number
    maxWidthPercentage?: number
    onEditMetadata: () => void
}

const SelectorWrapper = styled(Box)<{width: number, $maxWidthPercentage: number}>`
  max-width: ${props => props.$maxWidthPercentage}%;
  flex: 0 0 ${props => props.width}px;
  width: ${props => props.width}px;
  height: 100%;
    display: flex;
    flex-direction: column;
    min-height: 0;
  background-color: white;
  border-right: thin solid rgba(0, 0, 0, 0.12);
`;

const SelectionScroller = styled(Box)`
    flex: 1;
    min-height: 0;
    overflow-y: auto;
`;

const ActionFooter = styled(Box)`
    flex: 0 0 auto;
    height: 57px;
    box-sizing: border-box;
    display: flex;
    align-items: center;
    padding: 8px 16px;
    border-top: 1px solid var(--border-light);
    background-color: var(--surface-white);
`;

/**
 * Component for the filter section in the editor. Contains @see {@link DimensionSelectionList} for each dimension for filtering values and defining selectable dimensionss.
 * @param {IDimension[]} dimensions Dimensions available for the table.
 * @param {{[key:string]: string[]}} resolvedDimensionCodes Codes for the resolved dimension values.
 * @param {Query} queries Object that contains dimension queries
 * @param {number} width Width of the dimension filter section
 * @param {number} maxWidthPercentage Maximum width of the dimension filter section on the whole window defined in percentages
 */
export const EditorFilterSection: React.FC<EditorFilterSectionProps> = ({ dimensions, resolvedDimensionCodes, queries, width, maxWidthPercentage, onEditMetadata }) => {
    const { t } = useTranslation();
    const hasSelectedValues = Object.values(resolvedDimensionCodes ?? {}).some(valueCodes => valueCodes.length > 0);

    return (
        <SelectorWrapper width={width} $maxWidthPercentage={maxWidthPercentage}>
            <SelectionScroller>
                <DimensionSelectionList
                    dimensions={dimensions}
                    resolvedDimensionCodes={resolvedDimensionCodes}
                    query={queries}
                />
            </SelectionScroller>
            <ActionFooter>
                <Button
                    fullWidth
                    variant="outlined"
                    size="small"
                    startIcon={<EditNoteIcon />}
                    disabled={!hasSelectedValues}
                    onClick={onEditMetadata}
                >
                    {t('editMetadata.dialogTitle')}
                </Button>
            </ActionFooter>
        </SelectorWrapper>
    )
}

export default EditorFilterSection;
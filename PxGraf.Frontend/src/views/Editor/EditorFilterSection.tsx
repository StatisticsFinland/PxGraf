import DimensionSelectionList from 'components/VariableSelection/DimensionSelectionList';
import React from 'react';
import { Box, Button } from '@mui/material';
import EditNoteIcon from '@mui/icons-material/EditNote';
import { useTranslation } from 'react-i18next';
import { alpha, styled } from '@mui/material/styles';
import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';

interface EditorFilterSectionProps {
    dimensions: IDimension[],
    resolvedDimensionCodes: { [key: string]: string[] }
    queries: Query
    width: number
    maxWidthPercentage: number
    onEditMetadata: () => void
}

const SelectorWrapper = styled(Box, {
    shouldForwardProp: prop => prop !== 'width' && prop !== 'maxWidthPercentage',
})<{ width: number, maxWidthPercentage: number }>(({ width, maxWidthPercentage, theme }) => ({
    maxWidth: `${maxWidthPercentage}%`,
    flex: `0 0 ${width}px`,
    width,
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
    minHeight: 0,
    backgroundColor: theme.palette.background.paper,
    borderRight: `1px solid ${theme.palette.divider}`,
}));

const SelectionScroller = styled(Box)({ flex: 1, minHeight: 0, overflowY: 'auto' });

const ActionFooter = styled(Box)(({ theme }) => ({
        flex: '0 0 auto',
        height: 57,
        boxSizing: 'border-box',
        display: 'flex',
        alignItems: 'center',
        padding: '8px 16px',
        borderTop: `1px solid ${theme.palette.divider}`,
        backgroundColor: theme.palette.background.paper,
}));

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
        <SelectorWrapper width={width} maxWidthPercentage={maxWidthPercentage}>
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
                    sx={theme => ({
                        backgroundColor: theme.palette.primary.light,
                        '&:hover': { backgroundColor: alpha(theme.palette.primary.main, 0.16) },
                    })}
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
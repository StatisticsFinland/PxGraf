import React from 'react';
import { Typography, Accordion, AccordionSummary, AccordionDetails } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import DimensionSelection from './DimensionSelection';
import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { sortedDimensions } from 'utils/sortingHelpers';

interface DimensionSelectionListProps {
    dimensions: IDimension[],
    resolvedDimensionCodes: { [key: string]: string[] },
    query: Query
}

const TitleWrapper = styled('div')({
    display: 'flex',
    padding: '16px 12px 12px',
    alignItems: 'center',
});

const StyledAccordionDetails = styled(AccordionDetails)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    padding: '8px 12px 16px',
}));

const StyledAccordion = styled(Accordion)(({ theme }) => ({
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    boxShadow: 'none !important',
    margin: '0 12px 8px',
    overflow: 'hidden',
    '&:before': { display: 'none' },
    '&.Mui-expanded': { margin: '0 12px 8px' },
}));

const StyledAccordionSummary = styled(AccordionSummary)(({ theme }) => ({
    minHeight: 48,
    padding: '0 12px',
    backgroundColor: theme.palette.background.default,
    '&.Mui-expanded': { minHeight: 48 },
    '& .MuiAccordionSummary-content, & .MuiAccordionSummary-content.Mui-expanded': {
        margin: '12px 0',
    },
}));

const DimensionTitle = styled(Typography)`
    display: flex;
    align-items: baseline;
    justify-content: space-between;
    gap: 8px;
    width: 100%;
`;

const SelectionCount = styled('span')(({ theme }) => ({
    color: theme.palette.text.secondary,
    ...theme.typography.caption,
    fontWeight: theme.typography.fontWeightMedium,
    whiteSpace: 'nowrap',
}));

/**
 * Component for defining dimension filters and selectable dimensions in @see {@link Editor}.
 * @param {IDimension[]} dimensions Dimensions for the table in question.
 * @param {{ [key: string]: string[] }} resolvedDimensionCodes Resolved dimension value codes.
 * @param {Query} query Dimension queries.
 */
export const DimensionSelectionList: React.FC<DimensionSelectionListProps> = ({ dimensions, resolvedDimensionCodes, query }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const selectedValues = (code: string ) => {
        //Formats the text to show 0 if no values are selected
        return resolvedDimensionCodes?.[code] ? resolvedDimensionCodes[code].length : 0;
    };

    return (
        <>
            <TitleWrapper>
                <Typography variant="body2" color="text.secondary">{t('variableSelect.title')}</Typography>
            </TitleWrapper>
            {sortedDimensions(dimensions).map(dimension => {
                const summaryId = `dimension-${dimension.code}-header`;
                const contentId = `dimension-${dimension.code}-content`;
                return (
                    <StyledAccordion key={dimension.code} defaultExpanded={true} disableGutters elevation={0} square>
                        <StyledAccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls={contentId}
                            id={summaryId}
                        >
                            <DimensionTitle variant="h2">
                                <span>{dimension.name[uiContentLanguage] ?? dimension.code}</span>
                                <SelectionCount>{selectedValues(dimension.code)}/{dimension.values.length}</SelectionCount>
                            </DimensionTitle>
                        </StyledAccordionSummary>
                        <StyledAccordionDetails>
                            <DimensionSelection
                                dimension={dimension}
                                resolvedDimensionValueCodes={resolvedDimensionCodes?.[dimension.code]}
                                query={query}
                            />
                        </StyledAccordionDetails>
                    </StyledAccordion>
                );
            })}
        </>
    );
}

export default DimensionSelectionList;
import React from 'react';
import { Typography, Accordion, AccordionSummary, AccordionDetails } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import DimensionSelection from './DimensionSelection';
import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import { useTheme } from '@mui/material/styles';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { sortedDimensions } from 'utils/sortingHelpers';
import { EditorContext } from '../../contexts/editorContext';

interface DimensionSelectionListProps {
    dimensions: IDimension[],
    resolvedDimensionCodes: { [key: string]: string[] },
    query: Query
}

const TitleWrapper = styled.div`
    display: flex;
    padding-top: 16px;
    padding-left: 16px;
    padding-right: 16px;
    align-items: center;
`;

const StyledAccordionDetails = styled(AccordionDetails)`
    background-color: #f8f8f8;
`;

const StyledAccordion = styled(Accordion)`
    border-top: 1px solid #dcdcdc;
`;

/**
 * Component for defining dimension filters and selectable dimensions in @see {@link Editor}.
 * @param {IDimension[]} dimensions Dimensions for the table in question.
 * @param {{ [key: string]: string[] }} resolvedDimensionCodes Resolved dimension value codes.
 * @param {Query} query Dimension queries.
 */
export const DimensionSelectionList: React.FC<DimensionSelectionListProps> = ({ dimensions, resolvedDimensionCodes, query }) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const { setQuery } = React.useContext(EditorContext);

    const infoContent = (
        <>
            {t("infoText.selectableButton")}
            <br />
            <br />
            {t("infoText.selectableInput")}
        </>
    );

    const selectedValues = (code: string ) => {
        //Formats the text to show 0 if no values are selected
        return resolvedDimensionCodes?.[code] ? resolvedDimensionCodes[code].length : 0;
    };

    return (
        <>
            <TitleWrapper>
                <Typography variant="body2" sx={{ color: theme.palette.text.secondary }}>{t('variableSelect.title')}</Typography>
                <InfoBubble info={infoContent} ariaLabel={t('variableSelect.title')} id="mainContent" />
            </TitleWrapper>
            {sortedDimensions(dimensions).map(dimension => {
                return (
                    <StyledAccordion key={dimension.code} defaultExpanded={true}>
                        <AccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1a-content"
                        >
                            <Typography variant="h2"><b>{dimension.name[uiContentLanguage] ?? dimension.code} {selectedValues(dimension.code)}/{dimension.values.length}</b></Typography>
                        </AccordionSummary>
                        <StyledAccordionDetails>
                            <DimensionSelection
                                dimension={dimension}
                                resolvedDimensionValueCodes={resolvedDimensionCodes?.[dimension.code]}
                                dimensionQuery={query[dimension.code]}
                                onQueryChanged={newQuery => setQuery({ ...query, [dimension.code]: newQuery })}
                            />
                        </StyledAccordionDetails>
                    </StyledAccordion>
                );
            })}
        </>
    );
}

export default DimensionSelectionList;
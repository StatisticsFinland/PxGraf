import React from 'react';
import { Typography, Accordion, AccordionSummary, AccordionDetails } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import VariableSelection from './VariableSelection';
import { IVariable } from 'types/cubeMeta';
import { Query } from 'types/query';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import { useTheme } from '@mui/material/styles';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { sortedVariables } from 'utils/variableSorting';

interface VariableSelectionListProps {
    variables: IVariable[],
    resolvedVariableCodes: { [key: string]: string[] },
    query: Query,
    onQueryChanged: (newQuery: Query) => void
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
 * Component for defining variable filters and selectable variables in @see {@link Editor}.
 * @param {IVariable[]} variables Variables for the table in question.
 * @param {{ [key: string]: string[] }} resolvedVariableCodes Resolved variable value codes.
 * @param {Query} query Variable queries.
 * @param {(newQuery: Query) => void} onQueryChanged Callback function for when a variable query is edited.
 */
export const VariableSelectionList: React.FC<VariableSelectionListProps> = ({ variables, resolvedVariableCodes, query, onQueryChanged }) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

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
        return resolvedVariableCodes?.[code] ? resolvedVariableCodes[code].length : 0;
    };

    return (
        <>
            <TitleWrapper>
                <Typography variant="body2" sx={{ color: theme.palette.text.secondary }}>{t('variableSelect.title')}</Typography>
                <InfoBubble info={infoContent} ariaLabel={t('variableSelect.title')} id="mainContent" />
            </TitleWrapper>
            {sortedVariables(variables).map(variable => {
                return (
                    <StyledAccordion key={variable.code} defaultExpanded={true}>
                        <AccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1a-content"
                        >
                            <Typography variant="h2"><b>{variable.name[uiContentLanguage] ?? variable.code} {selectedValues(variable.code)}/{variable.values.length}</b></Typography>
                        </AccordionSummary>
                        <StyledAccordionDetails>
                            <VariableSelection
                                variable={variable}
                                resolvedVariableValueCodes={resolvedVariableCodes?.[variable.code]}
                                query={query[variable.code]}
                                onQueryChanged={newQuery => onQueryChanged({ ...query, [variable.code]: newQuery })}
                            />
                        </StyledAccordionDetails>
                    </StyledAccordion>
                );
            })}
        </>
    );
}

export default VariableSelectionList;
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Typography, Accordion, AccordionSummary, AccordionDetails, Tabs, Tab } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { TabPanel } from 'components/TabPanel/TabPanel';
import { VariableEditor } from './VariableEditor';
import { HeaderEditor } from './HeaderEditor';
import { a11yProps } from 'utils/componentHelpers';
import styled from 'styled-components';
import { ICubeQuery } from 'types/query';
import { MultiLanguageString } from 'types/multiLanguageString';
import { IVariable } from 'types/cubeMeta';
import { IHeaderResult } from 'api/services/default-header';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

const MetaEditorWrapper = styled(Box)`
  grid-area: 'parameters';
  display: grid;
  gap: 16px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
  padding: 0px;
`;

const StyledAccordion = styled(Accordion)`
  width: 100%;
`;

const TabsWrapper = styled(Box)`
  border-bottom: 1px solid;
  grid-column: span 12;
`;

const TabPanelWrapper = styled(Box)`
  grid-column: span 12;
`;

const MetaEditBarWrapper = styled.div`
  display: flex;
`;

const GridFixer = styled.div`
  grid-column: span 12;
`;

const StyledAccordionDetails = styled(AccordionDetails)`
  background-color: #f8f8f8;
`;

export interface INewEditMetaEditor {
    variableQueries?: {
        [variableCode: string]: string;
    };
    chartHeaderEdit?: {
        [language: string]: string;
    };
}

interface IMetaEditorProps {
    language: string;
    resolvedVariables: IVariable[];
    cubeQuery: ICubeQuery;
    defaultHeaderResponse: IHeaderResult;
    onChange: (newEdit: ICubeQuery) => void;
    isMetaAccordionOpen: boolean;
    onMetaAccordionOpenChange: () => void;
    titleMaxLength?: number;
}


export const MetaEditor: React.FC<IMetaEditorProps> = ({
    language,
    resolvedVariables,
    cubeQuery,
    defaultHeaderResponse,
    onChange,
    isMetaAccordionOpen,
    onMetaAccordionOpenChange,
    titleMaxLength,
}) => {
    const { t } = useTranslation();
    const [variableTab, setVariableTab] = useState(0);
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <MetaEditorWrapper>
            <HeaderEditor
                style={{ width: '100%' }}
                defaultHeaderResponse={defaultHeaderResponse}
                editValue={cubeQuery?.chartHeaderEdit}
                language={language}
                onChange={(title: MultiLanguageString) => onChange({ ...cubeQuery, chartHeaderEdit: title })}
                maxLength={titleMaxLength}
            />
            <GridFixer>

                <MetaEditBarWrapper>

                    <InfoBubble info={t('infoText.metaEdition')} ariaLabel={t("editMetadata.editorTitle")} />
                    <StyledAccordion expanded={isMetaAccordionOpen} onChange={onMetaAccordionOpenChange}>
                        <AccordionSummary
                            style={{ width: '100%' }}
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1a-content"
                            id="panel1a-header"
                        >
                            <Typography>{t("editMetadata.editorTitle")}</Typography>
                        </AccordionSummary>
                        <StyledAccordionDetails>
                            <TabsWrapper sx={{ borderColor: 'divider' }}>
                                <Tabs value={variableTab} onChange={(evt, newTab) => setVariableTab(newTab)}>
                                    {resolvedVariables.map((variable, index) => <Tab label={variable.name[uiContentLanguage] ?? variable.code} {...a11yProps(index)} key={variable.code} />)}
                                </Tabs>
                            </TabsWrapper>
                            <TabPanelWrapper>
                                {resolvedVariables.map((variable, index) => {
                                    return (
                                        <TabPanel selectedValue={variableTab} value={index} key={variable.code}>
                                            <VariableEditor
                                                variable={variable}
                                                language={language}
                                                variableEdits={cubeQuery?.variableQueries[variable.code]}
                                                onChange={newEdit => {
                                                    onChange({
                                                        ...cubeQuery,
                                                        variableQueries: {
                                                            ...cubeQuery?.variableQueries,
                                                            [variable.code]: {
                                                                ...cubeQuery?.variableQueries[variable.code],
                                                                valueEdits: {
                                                                    ...cubeQuery?.variableQueries[variable.code]?.valueEdits,
                                                                    ...newEdit.valueEdits
                                                                }
                                                            }
                                                        }
                                                    })
                                                }}
                                            />
                                        </TabPanel>
                                    );
                                })}
                            </TabPanelWrapper>
                        </StyledAccordionDetails>
                    </StyledAccordion>
                </MetaEditBarWrapper>
            </GridFixer>
        </MetaEditorWrapper>
    );
};

export default MetaEditor;
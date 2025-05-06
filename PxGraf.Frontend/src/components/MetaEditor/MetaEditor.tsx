import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Typography, Accordion, AccordionSummary, AccordionDetails, Tabs, Tab } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { TabPanel } from 'components/TabPanel/TabPanel';
import { DimensionEditor } from './DimensionEditor';
import { HeaderEditor } from './HeaderEditor';
import { a11yProps } from 'utils/componentHelpers';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IEditorContentsResult } from '../../api/services/editor-contents';

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
    dimensionQueries?: {
        [dimensionCode: string]: string;
    };
    chartHeaderEdit?: {
        [language: string]: string;
    };
}

interface IMetaEditorProps {
    language: string;
    resolvedDimensions: IDimension[];
    editorContentsResponse: IEditorContentsResult;
    isMetaAccordionOpen: boolean;
    onMetaAccordionOpenChange: () => void;
    titleMaxLength?: number;
}


export const MetaEditor: React.FC<IMetaEditorProps> = ({
    language,
    resolvedDimensions,
    editorContentsResponse,
    isMetaAccordionOpen,
    onMetaAccordionOpenChange,
    titleMaxLength,
}) => {
    const { t } = useTranslation();
    const [dimensionTab, setDimensionTab] = useState(0);
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <MetaEditorWrapper>
            <HeaderEditor
                style={{ width: '100%' }}
                editorContentResponse={editorContentsResponse}
                language={language}
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
                                <Tabs value={dimensionTab} onChange={(evt, newTab) => setDimensionTab(newTab)}>
                                    {resolvedDimensions.map((dimension, index) => <Tab label={dimension.name[uiContentLanguage] ?? dimension.code} {...a11yProps(index)} key={dimension.code} />)}
                                </Tabs>
                            </TabsWrapper>
                            <TabPanelWrapper>
                                {resolvedDimensions.map((dimension, index) => {
                                    return (
                                        <TabPanel selectedValue={dimensionTab} value={index} key={dimension.code}>
                                            <DimensionEditor
                                                dimension={dimension}
                                                language={language}
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
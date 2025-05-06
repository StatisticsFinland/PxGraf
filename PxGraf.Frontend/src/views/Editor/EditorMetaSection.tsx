import React, { useEffect } from 'react';
import { Box, Tabs, Tab, Alert } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import Typography from '@mui/material/Typography';
import MetaEditor from 'components/MetaEditor/MetaEditor';
import ChartTypeSelector from 'components/ChartTypeSelector/ChartTypeSelector';
import { useTranslation } from 'react-i18next';
import { a11yProps } from 'utils/componentHelpers';
import TabPanel from 'components/TabPanel/TabPanel';
import VisualizationSettingControl from 'components/VisualizationSettingsControls/VisualizationSettingsControl';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { VisualizationType } from 'types/visualizationType';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { Query } from 'types/query';
import ChartTypeRejectionReasons from 'components/ChartTypeRejectionReasons/ChartTypeRejectionReasons';
import CellCount from 'components/CellCount/CellCount';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { getVisualizationOptionsForType } from '../../utils/editorHelpers';

const MetaWrapper = styled(Box)`
  grid-area: 'parameters';
  display: grid;
  gap: 16px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
  padding: 24px;
`;

const TabWrapper = styled(Box)`
  border-bottom: 8px;
  grid-column: span 12;
`;

const StyledSecondaryText = styled(Typography)`
  margin-bottom: -2px;
  margin-top: -2px;
`;

const StyledTab = styled(Tab)`
  padding-top: 0;
  padding-bottom: 0;
`;

const MetaEditorWrapper = styled(Box)`
  grid-column: span 12;
`;

const ResponseWrapper = styled(Box)`
    justify-self: center;
    align-self: center;
    align-items: center;
    margin: 32px;
`;
const GridFixer = styled.div`
    display: grid;
    grid-column: span 12;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 8px;
    padding-bottom: 8px;
`;

const ChartTypeSelectorWrapper = styled.div`
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    align-items: left;
    justify-content: space-between;
    width: 100%;
`;

const ButtonGroupWrapper = styled.div`
    padding-right: 8px;
    display: inline-block;
`;

const FlexContentWrapper = styled.div`
    display: flex;
    align-items: center;
    padding-bottom: 8px;
    padding-left: 8px;
    padding-right: 8px;
`;

interface IEditorMetaSectionProps {
    editorContentsResponse: IEditorContentsResult;
    selectedVisualization: VisualizationType;
    settings: IVisualizationSettings;
    resolvedDimensions: IDimension[];
    dimensionQuery: Query;
    contentLanguages: string[];
}

const TitleWrapper = styled.div`
    display: flex;
    align-items: center;
`;

/**
 * Component for editing meta data information for the visualization. Used in @see {@link Editor} view.
 * In this view the user can change the visualization type and settings and edit the meta data information such as the chart header for the visualization.
 * @param {IHeaderResult} defaultHeaderResponse Default header retrieved for the table.
 * @param {VisualizationType} selectedVisualization Currently selected visualization type.
 * @param {IVisualizationSettings} settings Visualization settings.
 * @param {IDimension[]} resolvedDimensions: Resolved dimension codes.
 * @param {IVisualizationSettingsResult} visualizationRulesResponse Response object for rules based on the selected visualization type.
 * @param {IQueryInfo} queryInfo Information about the query.
 */
export const EditorMetaSection: React.FC<IEditorMetaSectionProps> = ({ editorContentsResponse, selectedVisualization, settings, resolvedDimensions, dimensionQuery, contentLanguages }) => {
    const { language, languageTab, setLanguageTab } = React.useContext(UiLanguageContext);
    const [isMetaAccordionOpen, setIsMetaAccordionOpen] = React.useState(false);

    // If the UI language is changed, content language is updated if applicable
    useEffect(() => {
        if (contentLanguages.includes(language)) {
            setLanguageTab(language);
        }
    }, [language]);

    const { t } = useTranslation();
    const theme = useTheme();

    const handleMetaAccordionOpenChange = () => {
        setIsMetaAccordionOpen(!isMetaAccordionOpen);
    }

    // TODO: tää ehkä pois koska on error ennenkun on asiat kasassa. Ehkä voi previewsectionista myös katsoa?
    if (editorContentsResponse.isError) {
        return (
            <ResponseWrapper>
                <Alert severity="error">{t("error.contentLoad")}</Alert>
            </ResponseWrapper>
        );
    }

    const buttonInfo =(
        <>
            {t('infoText.visualizationSelection')}
            <br />
            <br />
            {t('infoText.otherVisualizations')}
        </>
    );

    return (
        <MetaWrapper>
            <TabWrapper sx={{ borderColor: 'divider' }}>
                <TitleWrapper>
                    <StyledSecondaryText variant="body2" sx={{color: theme.palette.text.secondary}}>{t("editor.contentLanguage")}</StyledSecondaryText>
                    <InfoBubble info={t("infoText.langTab")} ariaLabel={t("editor.contentLanguage")} />
                </TitleWrapper>
                <Tabs value={languageTab} onChange={(evt, newLanguageTab) => setLanguageTab(newLanguageTab)}>
                    {contentLanguages.map(editLanguage =>
                        <StyledTab
                            sx={{minWidth: 'auto'}}
                            label={editLanguage}
                            value={editLanguage}
                            key={editLanguage}
                            aria-label={`${t("editor.contentLanguage")}: ${t("lang.local." + editLanguage)}`}
                            {...a11yProps(editLanguage)}
                        />
                    )}
                </Tabs>
            </TabWrapper>
            <MetaEditorWrapper>
                {contentLanguages.map(editLanguage =>
                    <TabPanel value={editLanguage} selectedValue={languageTab} key={editLanguage}>
                        <MetaEditor
                            language={editLanguage}
                            resolvedDimensions={resolvedDimensions}
                            editorContentsResponse={editorContentsResponse}
                            isMetaAccordionOpen={isMetaAccordionOpen}
                            onMetaAccordionOpenChange={handleMetaAccordionOpenChange}
                            titleMaxLength={editorContentsResponse.data ? editorContentsResponse.data.maximumHeaderLength : undefined}
                        />
                    </TabPanel>
                )}
            </MetaEditorWrapper>
            <GridFixer>
                <ChartTypeSelectorWrapper>
                    <FlexContentWrapper>
                        <InfoBubble info={buttonInfo} ariaLabel={t('tooltip.visualizationType')} />
                        <ButtonGroupWrapper>
                            <ChartTypeSelector
                                possibleTypes={editorContentsResponse.data?.visualizationOptions.map(options => options.type.toString())}
                                selectedType={selectedVisualization}
                            />
                        </ButtonGroupWrapper>
                        {(editorContentsResponse.data?.visualizationRejectionReasons && Object.keys(editorContentsResponse.data?.visualizationRejectionReasons).length > 0) ? <ChartTypeRejectionReasons rejectionReasons={editorContentsResponse.data?.visualizationRejectionReasons} /> : <></>}
                    </FlexContentWrapper>
                    <FlexContentWrapper>
                        {(editorContentsResponse.data?.size && editorContentsResponse.data?.maximumSupportedSize && editorContentsResponse.data?.sizeWarningLimit) ? <CellCount size={editorContentsResponse.data?.size} maximumSize={editorContentsResponse.data?.maximumSupportedSize} warningLimit={editorContentsResponse.data?.sizeWarningLimit} /> : <></>}
                    </FlexContentWrapper>
                </ChartTypeSelectorWrapper>
            </GridFixer>
                {selectedVisualization != null && settings != null && <VisualizationSettingControl
                    selectedVisualization={selectedVisualization}
                    visualizationSettings={settings}
                    visualizationOptions={getVisualizationOptionsForType(editorContentsResponse.data.visualizationOptions, selectedVisualization)}
                    dimensions={resolvedDimensions}
                    dimensionQuery={dimensionQuery}
                />}
        </MetaWrapper>
    );
}

export default EditorMetaSection;
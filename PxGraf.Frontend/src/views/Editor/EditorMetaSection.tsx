import React, { useEffect } from 'react';
import { Box, Tabs, Tab, ToggleButton, ToggleButtonGroup } from '@mui/material';
import MetaEditor from 'components/MetaEditor/MetaEditor';
import ChartTypeSelector from 'components/ChartTypeSelector/ChartTypeSelector';
import { useTranslation } from 'react-i18next';
import { a11yProps } from 'utils/componentHelpers';
import TabPanel from 'components/TabPanel/TabPanel';
import VisualizationSettingControl from 'components/VisualizationSettingsControls/VisualizationSettingsControl';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { VisualizationType } from 'types/visualizationType';
import { Query } from 'types/query';
import ChartTypeRejectionReasons from 'components/ChartTypeRejectionReasons/ChartTypeRejectionReasons';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { EPreviewSize } from 'types/previewSize';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { getVisualizationOptionsForVisualizationType } from '../../utils/editorHelpers';
import { IVisualizationSettings } from '../../types/visualizationSettings';

const MetaWrapper = styled(Box)`
  grid-area: 'parameters';
  display: grid;
  gap: 8px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
  padding: 8px 16px 16px 16px;
`;

const TabWrapper = styled(Box)`
  border-bottom: 8px;
  grid-column: span 12;
`;

const StyledTab = styled(Tab)`
  padding-top: 0;
  padding-bottom: 0;
`;

const MetaEditorWrapper = styled(Box)`
  grid-column: span 12;
`;

const GridFixer = styled.div`
    display: grid;
    grid-column: span 12;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 4px;
    padding-bottom: 4px;
`;

const VisualizationSettingsRow = styled.div`
    display: flex;
    align-items: center;
    grid-column: span 12;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 4px;
    padding-bottom: 4px;
`;

const PreviewSizeControlWrapper = styled.div`
    display: flex;
    align-items: center;
    margin-left: auto;
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
    resolvedDimensions: IDimension[];
    dimensionQuery: Query;
    contentLanguages: string[];
    visualizationSettings: IVisualizationSettings;
    previewSize: EPreviewSize;
    onPreviewSizeChange: (size: EPreviewSize) => void;
}

const TitleWrapper = styled.div`
    display: flex;
    align-items: center;
`;

/**
 * Component for editing meta data information for the visualization. Used in @see {@link Editor} view.
 * In this view the user can change the visualization type and settings and edit the meta data information such as the chart header for the visualization.
 * @param {IEditorMetaSectionProps} editorContentsResponse Editor contents response from the API.
 * @param {VisualizationType} selectedVisualization Currently selected visualization type.
 * @param {IDimension[]} resolvedDimensions: Resolved dimension codes.
 * @param {Query} dimensionQuery: Query object containing the selected values for each dimension.
 * @param {string[]} contentLanguages: List of available content languages.
 * @param {IVisualizationSettings} visualizationSettings: Visualization settings for the selected visualization type.
 * @param {EPreviewSize} previewSize: The current preview size selection.
 * @param {(size: EPreviewSize) => void} onPreviewSizeChange: Callback for when the preview size changes.
 */
export const EditorMetaSection: React.FC<IEditorMetaSectionProps> = ({ editorContentsResponse, selectedVisualization, resolvedDimensions, dimensionQuery, contentLanguages, visualizationSettings, previewSize, onPreviewSizeChange }) => {
    const { language, languageTab, setLanguageTab } = React.useContext(UiLanguageContext);
    const [isMetaAccordionOpen, setIsMetaAccordionOpen] = React.useState(false);

    // If the UI language is changed, content language is updated if applicable
    useEffect(() => {
        if (contentLanguages.includes(language)) {
            setLanguageTab(language);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: only react to language changes, contentLanguages/setLanguageTab are stable
    }, [language]);

    const { t } = useTranslation();

    const handleMetaAccordionOpenChange = () => {
        setIsMetaAccordionOpen(!isMetaAccordionOpen);
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
                    <InfoBubble info={t("infoText.langTab")} ariaLabel={t("editor.contentLanguage")} />
                </TitleWrapper>
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
                    <PreviewSizeControlWrapper>
                        <InfoBubble info={t("infoText.rescaleButtons")} ariaLabel={t('tooltip.visualizationSize')} />
                        <ToggleButtonGroup
                            size="small"
                            color="primary"
                            exclusive
                            value={previewSize}
                            aria-label={t('tooltip.visualizationSize')}
                            onChange={(_, val) => val != null && onPreviewSizeChange(val)}
                        >
                            {Object.entries(EPreviewSize).map(([label, value]) => (
                                <ToggleButton
                                    key={value}
                                    value={value}
                                >
                                    {t(`previewSize.${label.toLowerCase()}`)}
                                </ToggleButton>
                            ))}
                        </ToggleButtonGroup>
                    </PreviewSizeControlWrapper>
                </ChartTypeSelectorWrapper>
            </GridFixer>
                {selectedVisualization != null && <VisualizationSettingsRow><VisualizationSettingControl
                    selectedVisualization={selectedVisualization}
                    dimensions={resolvedDimensions}
                    dimensionQuery={dimensionQuery}
                    visualizationOptions={getVisualizationOptionsForVisualizationType(editorContentsResponse.data.visualizationOptions, selectedVisualization)}
                    visualizationSettings={visualizationSettings}
                /></VisualizationSettingsRow>}
        </MetaWrapper>
    );
}

export default EditorMetaSection;
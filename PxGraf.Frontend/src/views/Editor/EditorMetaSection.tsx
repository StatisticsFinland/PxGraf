import React, { useEffect } from 'react';
import { Box, Tabs, Tab, ToggleButton, ToggleButtonGroup } from '@mui/material';
import MetaEditor from 'components/MetaEditor/MetaEditor';
import ChartTypeSelector from 'components/ChartTypeSelector/ChartTypeSelector';
import { useTranslation } from 'react-i18next';
import { a11yProps } from 'utils/componentHelpers';
import TabPanel from 'components/TabPanel/TabPanel';
import VisualizationSettingControl from 'components/VisualizationSettingsControls/VisualizationSettingsControl';
import { styled } from '@mui/material/styles';
import { IDimension } from 'types/cubeMeta';
import { VisualizationType } from 'types/visualizationType';
import { Query } from 'types/query';
import ChartTypeRejectionReasons from 'components/ChartTypeRejectionReasons/ChartTypeRejectionReasons';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { EPreviewSize } from 'types/previewSize';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { getVisualizationOptionsForVisualizationType, getVisualizationSettingVisibility } from '../../utils/editorHelpers';
import { IVisualizationSettings } from '../../types/visualizationSettings';

const MetaWrapper = styled(Box)(({ theme }) => ({
  gridArea: 'parameters',
  display: 'grid',
  rowGap: 8,
  gridTemplateColumns: 'repeat(12, 1fr)',
  padding: '8px 16px 12px',
  backgroundColor: theme.palette.background.paper,
  borderBottom: `1px solid ${theme.palette.divider}`,
}));

const TabWrapper = styled(Box)(({ theme }) => ({
  gridColumn: 'span 12',
  borderBottom: `1px solid ${theme.palette.divider}`,
}));

const StyledTabs = styled(Tabs)({ minHeight: 40 });

const StyledTab = styled(Tab)({ minHeight: 40, paddingTop: 0, paddingBottom: 0 });

const MetaEditorWrapper = styled(Box)({ gridColumn: 'span 12' });

const GridFixer = styled('div')({ display: 'grid' });

const VisualizationControlsPanel = styled('div')(({ theme }) => ({
    gridColumn: 'span 12',
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    backgroundColor: theme.palette.background.default,
    overflow: 'hidden',
}));

const VisualizationSettingsRow = styled('div')(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    minHeight: 48,
    padding: '8px 12px 8px 24px',
    boxSizing: 'border-box',
    borderTop: `1px solid ${theme.palette.divider}`,
    backgroundColor: theme.palette.background.paper,
}));

const PreviewSizeControlWrapper = styled('div')({
    display: 'flex', alignItems: 'center', gap: 4, marginLeft: 'auto',
});

const ChartTypeSelectorWrapper = styled('div')({
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: '12px 24px',
    width: '100%',
    padding: 12,
    boxSizing: 'border-box',
});

const ButtonGroupWrapper = styled('div')({ display: 'inline-block' });

const FlexContentWrapper = styled('div')({ display: 'flex', alignItems: 'center', gap: 4 });

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

const TitleWrapper = styled('div')({ display: 'flex', alignItems: 'center' });

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

    // If the UI language is changed, content language is updated if applicable
    useEffect(() => {
        if (contentLanguages.includes(language)) {
            setLanguageTab(language);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: only react to language changes, contentLanguages/setLanguageTab are stable
    }, [language]);

    const { t } = useTranslation();
    const visualizationOptions = getVisualizationOptionsForVisualizationType(editorContentsResponse.data?.visualizationOptions, selectedVisualization);
    const hasVisibleVisualizationSettings = selectedVisualization != null && getVisualizationSettingVisibility(
        selectedVisualization,
        resolvedDimensions,
        dimensionQuery,
        visualizationOptions,
        visualizationSettings,
    ).hasVisibleSettings;

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
                    <StyledTabs value={languageTab} onChange={(evt, newLanguageTab) => setLanguageTab(newLanguageTab)} aria-label={t("editor.contentLanguage")}>
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
                    </StyledTabs>
                    <InfoBubble info={t("infoText.langTab")} ariaLabel={t("editor.contentLanguage")} />
                </TitleWrapper>
            </TabWrapper>
            <MetaEditorWrapper>
                {contentLanguages.map(editLanguage =>
                    <TabPanel value={editLanguage} selectedValue={languageTab} contentPadding="12px 12px 8px" key={editLanguage}>
                        <MetaEditor
                            language={editLanguage}
                            editorContentsResponse={editorContentsResponse}
                            titleMaxLength={editorContentsResponse.data ? editorContentsResponse.data.maximumHeaderLength : undefined}
                        />
                    </TabPanel>
                )}
            </MetaEditorWrapper>
            <VisualizationControlsPanel>
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
                {hasVisibleVisualizationSettings && <VisualizationSettingsRow data-testid="visualization-settings-row"><VisualizationSettingControl
                    selectedVisualization={selectedVisualization}
                    dimensions={resolvedDimensions}
                    dimensionQuery={dimensionQuery}
                    visualizationOptions={visualizationOptions}
                    visualizationSettings={visualizationSettings}
                /></VisualizationSettingsRow>}
            </VisualizationControlsPanel>
        </MetaWrapper>
    );
}

export default EditorMetaSection;
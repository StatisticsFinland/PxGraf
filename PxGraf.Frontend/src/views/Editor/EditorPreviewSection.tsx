import React from 'react';
import { Box } from '@mui/material';
import { styled } from '@mui/material/styles';
import Preview from 'components/Preview/Preview';
import { EPreviewSize } from 'types/previewSize';
import { Query } from 'types/query';
import { VisualizationType } from 'types/visualizationType';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useTranslation } from 'react-i18next';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { VisualizationLibrary } from 'utils/visualizationLibrary';

const PreviewWrapper = styled(Box)(({ theme }) => ({
    gridArea: 'preview',
    display: 'flex',
    height: '100%',
    position: 'relative',
    padding: '12px 24px',
    boxSizing: 'border-box',
    overflowY: 'auto',
    minHeight: 0,
    backgroundColor: theme.palette.background.default,
}));

const PreviewCenterer = styled('div')({
    display: 'flex',
    alignItems: 'stretch',
    width: '100%',
    height: '100%',
    minHeight: 0,
    '& > *': {
        flexShrink: 0,
    },
});

const GuideTextWrapper = styled('div')({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
});

interface IEditorPreviewSectionProps {
    path: string[];
    query: Query;
    selectedVisualization: VisualizationType;
    visualizationSettings: IVisualizationSettings;
    editorContents: IEditorContentsResult;
    previewSize: EPreviewSize;
    visualizationLibrary: VisualizationLibrary;
}

/**
 * Component for previewing the visualization. Used in the @see {@link Editor} component. The preview is rendered in the @see {@link Preview} component.
 * @param {string[]} path Path to the current table in the Px file system.
 * @param {Query} query Current query settings.
 * @param {VisualizationType} selectedVisualization Selected visualization type.
 * @param {IVisualizationSettings} visualizationSettings Selected visualization settings.
 * @param {IEditorContentsResult} editorContents Contents of the editor. This includes the visualization options and rejection reasons.
 * @param {EPreviewSize} previewSize The current preview size for the chart.
 * @param {VisualizationLibrary} visualizationLibrary Library used to render the preview.
 */
export const EditorPreviewSection: React.FC<IEditorPreviewSectionProps> = ({ path, query, selectedVisualization, visualizationSettings, editorContents, previewSize, visualizationLibrary }) => {
    const { t } = useTranslation();

    if (!editorContents.data?.visualizationOptions?.length) {
        if (editorContents.data && Object.keys(editorContents.data.visualizationRejectionReasons).length > 0) {
            // Selected filters cannot produce a visualization
            return (
                <GuideTextWrapper>
                    <span>{t("previewGuide.impossibleToVisualize")}</span>
                </GuideTextWrapper>
            );
        }
        // Not enough filters
        return (
            <GuideTextWrapper>
                <span>{t("previewGuide.tooSmallQuery")}</span>
            </GuideTextWrapper>
        );
    }
    return (
        <PreviewWrapper>
            <PreviewCenterer>
                <Preview
                    path={path}
                    query={query}
                    selectedVisualization={selectedVisualization}
                    visualizationSettings={visualizationSettings}
                    previewSize={previewSize}
                    visualizationLibrary={visualizationLibrary}
                />
            </PreviewCenterer>
        </PreviewWrapper>
    );
}

export default EditorPreviewSection;
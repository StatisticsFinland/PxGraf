import React from 'react';
import { Box } from '@mui/material';
import styled from 'styled-components';
import Preview from 'components/Preview/Preview';
import { IQueryInfo, Query } from 'types/query';
import { VisualizationType } from 'types/visualizationType';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { useTranslation } from 'react-i18next';

const PreviewWrapper = styled(Box)`
    grid-area: 'preview';
    display: block;
    position: relative;
    grid-template-rows: auto 1fr;
    padding: 16px;
    gap: 16px;
`;

const GuideTextWrapper = styled.div`
    display: flex;
    align-items: center;
    justify-content: center;
`;

interface IEditorPreviewSectionProps {
    path: string[];
    query: Query;
    selectedVisualization: VisualizationType;
    visualizationSettings: IVisualizationSettings;
    queryInfo: IQueryInfo;

}

/**
 * Component for previewing the visualization. Used in the @see {@link Editor} component. The preview is rendered in the @see {@link Preview} component.
 * @param {string[]} path Path to the current table in the Px file system.
 * @param {Query} query Current query settings.
 * @param {VisualizationType} visualizationType Selected visualization type.
 * @param {IVisualizationSettings} visualizationSettings Selected visualization settings.
 * @param {IQueryInfo} queryInfo Information about the query.
 */
export const EditorPreviewSection: React.FC<IEditorPreviewSectionProps> = ({path, query, selectedVisualization, visualizationSettings, queryInfo}) => {
    const { t } = useTranslation();

    if (queryInfo?.validVisualizations?.length === 0) {
        if (Object.keys(queryInfo?.visualizationRejectionReasons).length > 0) {
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
            <Preview
                path={path}
                query={query}
                selectedVisualization={selectedVisualization}
                visualizationSettings={visualizationSettings}
            />
        </PreviewWrapper>
    );
}

export default EditorPreviewSection;
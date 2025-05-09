import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { ContentDimensionEditor } from './ContentDimensionEditor';
import { BasicDimensionEditor } from './BasicDimensionEditor';
import styled from 'styled-components';
import { IDimension, EDimensionType } from 'types/cubeMeta';

const ContentWrapper = styled(Stack)`
  padding: 0;
`;

const StyledEm = styled.em`
  color: #ccc;
`;

interface IDimensionEditorProps {
    dimension: IDimension;
    language: string;
}

export const DimensionEditor: React.FC<IDimensionEditorProps> = ({ dimension, language }) => {
    const { t } = useTranslation();

    if (dimension.values?.length === 0) {
        return (
            <ContentWrapper spacing={2}>
                <StyledEm>{t("editMetadata.noVariableValuesSelected")}</StyledEm>
            </ContentWrapper>
        );
    }
    else if (dimension.type === EDimensionType.Content) {
        return <ContentDimensionEditor
            dimension={dimension}
            language={language}
        />
    }
    else {
        return <BasicDimensionEditor
            dimension={dimension}
            language={language}
        />
    }
}

export default DimensionEditor;
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { ContentDimensionEditor } from './ContentDimensionEditor';
import { BasicDimensionEditor } from './BasicDimensionEditor';
import styled from 'styled-components';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { IDimensionEditions } from 'types/query';

const ContentWrapper = styled(Stack)`
  padding: 0;
`;

const StyledEm = styled.em`
  color: #ccc;
`;

interface IDimensionEditorProps {
    dimension: IDimension;
    language: string;
    dimensionEdits: IDimensionEditions;
    onChange: (newEdit: IDimensionEditions) => void;
}

export const DimensionEditor: React.FC<IDimensionEditorProps> = ({ dimension, language, dimensionEdits, onChange }) => {
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
            dimensionEdits={dimensionEdits}
            onChange={onChange}
        />
    }
    else {
        return <BasicDimensionEditor
            dimension={dimension}
            language={language}
            dimensionEdits={dimensionEdits}
            onChange={onChange}
        />
    }
}

export default DimensionEditor;
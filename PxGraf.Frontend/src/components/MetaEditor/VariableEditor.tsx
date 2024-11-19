import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { ContentVariableEditor } from './ContentVariableEditor';
import { BasicVariableEditor } from './BasicVariableEditor';
import styled from 'styled-components';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';

const ContentWrapper = styled(Stack)`
  padding: 0;
`;

const StyledEm = styled.em`
  color: #ccc;
`;

interface IVariableEditorProps {
    variable: IDimension;
    language: string;
    variableEdits: IVariableEditions;
    onChange: (newEdit: IVariableEditions) => void;
}

export const VariableEditor: React.FC<IVariableEditorProps> = ({ variable, language, variableEdits, onChange }) => {
    const { t } = useTranslation();

    if (variable.values?.length === 0) {
        return (
            <ContentWrapper spacing={2}>
                <StyledEm>{t("editMetadata.noVariableValuesSelected")}</StyledEm>
            </ContentWrapper>
        );
    }
    else if (variable.type === EDimensionType.Content) {
        return <ContentVariableEditor
            variable={variable}
            language={language}
            variableEdits={variableEdits}
            onChange={onChange}
        />
    }
    else {
        return <BasicVariableEditor
            variable={variable}
            language={language}
            variableEdits={variableEdits}
            onChange={onChange}
        />
    }
}

export default VariableEditor;
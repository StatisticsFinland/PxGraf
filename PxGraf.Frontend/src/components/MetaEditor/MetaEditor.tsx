import React from 'react';
import { Box } from '@mui/material';
import { HeaderEditor } from './HeaderEditor';
import styled from 'styled-components';
import { IEditorContentsResult } from '../../api/services/editor-contents';

const MetaEditorWrapper = styled(Box)`
  grid-area: 'parameters';
  display: grid;
  gap: 16px;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
  padding: 0px;
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
    editorContentsResponse: IEditorContentsResult;
    titleMaxLength?: number;
}


export const MetaEditor: React.FC<IMetaEditorProps> = ({
    language,
    editorContentsResponse,
    titleMaxLength,
}) => {
    return (
        <MetaEditorWrapper>
            <HeaderEditor
                style={{ width: '100%' }}
                editorContentResponse={editorContentsResponse}
                language={language}
                maxLength={titleMaxLength}
            />
        </MetaEditorWrapper>
    );
};

export default MetaEditor;
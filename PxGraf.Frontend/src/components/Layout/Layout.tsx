import React, { ReactNode } from 'react';
import { useLocation } from 'react-router-dom';
import { Divider } from '@mui/material';
import styled from 'styled-components';
import Header from 'components/Header/Header';
import { EditorProvider } from 'contexts/editorContext';
import Editor from 'views/Editor/Editor';

const LayoutWrapper = styled.div`
    display: grid;
    grid-template-rows: auto 1fr;
    height: 100%;
    overflow: hidden;
`;

const HeaderArea = styled.div`
    z-index: 10;
    background-color: white;
`;

const ContentArea = styled.div`
    overflow-y: auto;
    min-height: 0;
`;

const EditorContentArea = styled.div`
    overflow: hidden;
    min-height: 0;
`;

export const PageLayout: React.FC<{ element: ReactNode }> = ({ element }) => (
    <LayoutWrapper>
        <HeaderArea><Header /><Divider /></HeaderArea>
        <ContentArea>{element}</ContentArea>
    </LayoutWrapper>
);

export const EditorRoute: React.FC = () => {
    const location = useLocation();
    const state = location.state as { resetEditor?: boolean } | null;
    const editorKey = state?.resetEditor ? location.key : location.pathname;

    return (
        <LayoutWrapper>
            <HeaderArea><Header /><Divider /></HeaderArea>
            <EditorContentArea>
                <EditorProvider key={editorKey}>
                    <Editor />
                </EditorProvider>
            </EditorContentArea>
        </LayoutWrapper>
    );
};

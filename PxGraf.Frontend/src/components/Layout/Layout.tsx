import React, { ReactNode } from 'react';
import { useLocation } from 'react-router-dom';
import { Divider } from '@mui/material';
import { styled } from '@mui/material/styles';
import Header from 'components/Header/Header';
import { EditorProvider } from 'contexts/editorContext';
import Editor from 'views/Editor/Editor';

const LayoutWrapper = styled('div')({
    display: 'grid',
    gridTemplateRows: 'auto 1fr',
    height: '100%',
    overflow: 'hidden',
});

const HeaderArea = styled('div')(({ theme }) => ({
    zIndex: 10,
    backgroundColor: theme.palette.background.paper,
}));

const ContentArea = styled('main')({ overflowY: 'auto', minHeight: 0 });

const EditorContentArea = styled('main')({ overflow: 'hidden', minHeight: 0 });

export const PageLayout: React.FC<{ element: ReactNode }> = ({ element }) => (
    <LayoutWrapper>
        <HeaderArea><Header /><Divider /></HeaderArea>
        <ContentArea id="mainContent" tabIndex={-1}>{element}</ContentArea>
    </LayoutWrapper>
);

export const EditorRoute: React.FC = () => {
    const location = useLocation();
    const state = location.state as { resetEditor?: boolean } | null;
    const editorKey = state?.resetEditor ? location.key : location.pathname;

    return (
        <LayoutWrapper>
            <HeaderArea><Header /><Divider /></HeaderArea>
            <EditorContentArea id="mainContent" tabIndex={-1}>
                <EditorProvider key={editorKey}>
                    <Editor />
                </EditorProvider>
            </EditorContentArea>
        </LayoutWrapper>
    );
};

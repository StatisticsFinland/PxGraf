import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import { PageLayout, EditorRoute } from './Layout';

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl/',
    PublicUrl: 'publicUrl/',
    BasePath: ''
}));

jest.mock('components/Header/Header', () => ({
    __esModule: true,
    default: () => <div data-testid="header" />,
}));

jest.mock('views/Editor/Editor', () => ({
    __esModule: true,
    default: () => <div data-testid="editor" />,
}));

jest.mock('contexts/editorContext', () => ({
    EditorProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}));

describe('PageLayout', () => {
    it('renders the header', () => {
        render(
            <MemoryRouter>
                <PageLayout element={<span />} />
            </MemoryRouter>
        );
        expect(screen.getByTestId('header')).toBeInTheDocument();
    });

    it('renders the provided element', () => {
        render(
            <MemoryRouter>
                <PageLayout element={<div data-testid="page-content" />} />
            </MemoryRouter>
        );
        expect(screen.getByTestId('page-content')).toBeInTheDocument();
    });

    it('renders the main content area with id="mainContent" and tabIndex=-1', () => {
        render(
            <MemoryRouter>
                <PageLayout element={<span />} />
            </MemoryRouter>
        );
        const main = document.getElementById('mainContent');
        expect(main).toBeInTheDocument();
        expect(main).toHaveAttribute('tabindex', '-1');
    });
});

describe('EditorRoute', () => {
    it('renders the header', () => {
        render(
            <MemoryRouter>
                <EditorRoute />
            </MemoryRouter>
        );
        expect(screen.getByTestId('header')).toBeInTheDocument();
    });

    it('renders the editor', () => {
        render(
            <MemoryRouter>
                <EditorRoute />
            </MemoryRouter>
        );
        expect(screen.getByTestId('editor')).toBeInTheDocument();
    });

    it('renders the main content area with id="mainContent" and tabIndex=-1', () => {
        render(
            <MemoryRouter>
                <EditorRoute />
            </MemoryRouter>
        );
        const main = document.getElementById('mainContent');
        expect(main).toBeInTheDocument();
        expect(main).toHaveAttribute('tabindex', '-1');
    });

    it('renders the editor when resetEditor state is true', () => {
        render(
            <MemoryRouter initialEntries={[{ pathname: '/editor/db/table/', state: { resetEditor: true } }]}>
                <EditorRoute />
            </MemoryRouter>
        );
        expect(screen.getByTestId('editor')).toBeInTheDocument();
    });

    it('renders the editor when there is no location state', () => {
        render(
            <MemoryRouter initialEntries={[{ pathname: '/editor/db/table/', state: null }]}>
                <EditorRoute />
            </MemoryRouter>
        );
        expect(screen.getByTestId('editor')).toBeInTheDocument();
    });
});

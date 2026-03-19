import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { ChartTypeSelector } from './ChartTypeSelector';
import '@testing-library/jest-dom';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';

const onTypeSelectedMock = jest.fn((value: string) => value);

const mockTypes = ['foo', 'bar', 'baz'];

const editorContextMock = {
    defaultSelectables: {},
    setDefaultSelectables: jest.fn(),
    cubeQuery: null,
    setCubeQuery: jest.fn(),
    query: {},
    setQuery: jest.fn(),
    saveDialogOpen: false,
    setSaveDialogOpen: jest.fn(),
    selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
    setSelectedVisualizationUserInput: onTypeSelectedMock,
    visualizationSettingsUserInput: {},
    setVisualizationSettingsUserInput: jest.fn(),
    loadedQueryId: '',
    setLoadedQueryId: jest.fn(),
    loadedQueryIsDraft: false,
    setLoadedQueryIsDraft: jest.fn(),
    publicationWebhookEnabled: true,
    setPublicationWebhookEnabled: jest.fn()
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    beforeEach(() => {
        onTypeSelectedMock.mockClear();
    });

    it('should invoke handler if clicked', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />
            </EditorContext.Provider>);
        fireEvent.click(screen.getByText('chartTypes.bar'));
        expect(onTypeSelectedMock).toHaveBeenCalledTimes(1);
    });

    it('renders all possible types as buttons', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />
            </EditorContext.Provider>);
        expect(screen.getByText('chartTypes.foo')).toBeInTheDocument();
        expect(screen.getByText('chartTypes.bar')).toBeInTheDocument();
        expect(screen.getByText('chartTypes.baz')).toBeInTheDocument();
    });

    it('shows "no possible visualizations" when possibleTypes is empty', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={[]} selectedType={'foo'} />
            </EditorContext.Provider>);
        expect(screen.getByText('general.noPossibleVisualizations')).toBeInTheDocument();
    });

    it('shows "no possible visualizations" when possibleTypes is null', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={null} selectedType={'foo'} />
            </EditorContext.Provider>);
        expect(screen.getByText('general.noPossibleVisualizations')).toBeInTheDocument();
    });

    it('selects first type by default when selectedType is not provided', () => {
        const { container } = render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} />
            </EditorContext.Provider>);
        // First button should be selected (bold text)
        const firstButton = screen.getByText('chartTypes.foo');
        expect(firstButton.tagName).toBe('B');
    });

    it('invokes handler with correct value when different types are clicked', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />
            </EditorContext.Provider>);
        fireEvent.click(screen.getByText('chartTypes.baz'));
        expect(onTypeSelectedMock).toHaveBeenCalledWith('baz');
    });

    it('renders toggle button group with correct aria-label', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />
            </EditorContext.Provider>);
        expect(screen.getByRole('group')).toHaveAttribute('aria-label', 'tooltip.visualizationType');
    });
});
import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { ChartTypeSelector } from './ChartTypeSelector';
import '@testing-library/jest-dom';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
}));

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
    it('should invoke handler if clicked', () => {
        render(
            <EditorContext.Provider value={editorContextMock}>
                <ChartTypeSelector possibleTypes={mockTypes} selectedType={'foo'} />
            </EditorContext.Provider>);
        fireEvent.click(screen.getByText('chartTypes.bar'));
        expect(onTypeSelectedMock).toHaveBeenCalledTimes(1);
    });
});
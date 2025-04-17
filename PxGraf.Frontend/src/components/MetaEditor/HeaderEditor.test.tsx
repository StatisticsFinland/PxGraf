import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import HeaderEditor from './HeaderEditor';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';
import { ICubeQuery } from '../../types/query';

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

const mockDefaultResponse = {
    isLoading: false,
    isError: false,
    data: {
        'fi': 'foo'
    }
};

const mockDefaultResponseLoading = {
    isLoading: true,
    isError: false,
    data: {
        'fi': 'foo'
    }
};

const mockDefaultResponseError = {
    isLoading: false,
    isError: true,
    data: {
        'fi': 'foo'
    }
};

const mockLang = 'fi';
const mockFunction = jest.fn();
const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: { 'fi': 'bar' },
    variableQueries: {}
}

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <EditorContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: false,
                setSaveDialogOpen: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn()
            }}>
                <HeaderEditor defaultHeaderResponse={mockDefaultResponse} language={mockLang} style={{}} />
            </EditorContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly when loading', () => {
        const { asFragment } = render(
                <EditorContext.Provider value={{
                    defaultSelectables: {},
                    setDefaultSelectables: jest.fn(),
                    cubeQuery: mockCubeQuery,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                    saveDialogOpen: false,
                    setSaveDialogOpen: jest.fn(),
                    selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                    setSelectedVisualizationUserInput: jest.fn(),
                    visualizationSettingsUserInput: {},
                    setVisualizationSettingsUserInput: jest.fn()
                }}>
                    <HeaderEditor defaultHeaderResponse={mockDefaultResponseLoading} language={mockLang} style={{}} />
                </EditorContext.Provider>

        );
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly when receiving error', () => {
        const { asFragment } = render(
                    <EditorContext.Provider value={{
                        defaultSelectables: {},
                        setDefaultSelectables: jest.fn(),
                        cubeQuery: mockCubeQuery,
                        setCubeQuery: mockFunction,
                        query: {},
                        setQuery: jest.fn(),
                        saveDialogOpen: false,
                        setSaveDialogOpen: jest.fn(),
                        selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                        setSelectedVisualizationUserInput: jest.fn(),
                        visualizationSettingsUserInput: {},
                        setVisualizationSettingsUserInput: jest.fn()
                    }}>
                        <HeaderEditor defaultHeaderResponse={mockDefaultResponseError} language={mockLang} style={{}} />
                    </EditorContext.Provider>

        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <EditorContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: false,
                setSaveDialogOpen: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn()
            }}>
                <HeaderEditor defaultHeaderResponse={mockDefaultResponse} language={mockLang} style={{}} />
            </EditorContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue(mockCubeQuery.chartHeaderEdit['fi']), { target: { value: 'editValue2' } });
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});

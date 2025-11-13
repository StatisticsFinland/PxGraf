import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import BasicDimensionEditor from './BasicDimensionEditor';
import '@testing-library/jest-dom';
import UiLanguageContext from 'contexts/uiLanguageContext';
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

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const mockDimension: IDimension = {
    code: 'foo',
    name: {
        'fi': 'asd',
        'sv': 'asd',
        'en': 'asd'
    },
    type: EDimensionType.Content,
    values: [
        {
            code: 'bar',
            name: {
                'fi': 'fgfgfg',
                'sv': 'fgfgfg',
                'en': 'fgfgfg'
            },
            isVirtual: false
        }
    ]
}

const mockLang = 'fi';
const mockFunction = jest.fn();

const mockDimEdits: ICubeQuery = {
    chartHeaderEdit: {},
    variableQueries: {
        foo: {
            valueEdits: {
                'bar': {
                    nameEdit: {
                        'fi': 'bar',
                        'sv': 'bar',
                        'en': 'bar'
                    }
                }
            }
        }
    }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{
                    defaultSelectables: {},
                    setDefaultSelectables: jest.fn(),
                    cubeQuery: mockDimEdits,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                    saveDialogOpen: false,
                    setSaveDialogOpen: jest.fn(),
                    selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                    setSelectedVisualizationUserInput: jest.fn(),
                    visualizationSettingsUserInput: {},
                    setVisualizationSettingsUserInput: jest.fn(),
                    loadedQueryId: '',
                    setLoadedQueryId: jest.fn(),
                    loadedQueryIsDraft: false,
                    setLoadedQueryIsDraft: jest.fn(),
                    publicationEnabled: true,
                    setPublicationEnabled: jest.fn()
                }}>
                    <BasicDimensionEditor language={mockLang} dimension={mockDimension} />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{
                    defaultSelectables: {},
                    setDefaultSelectables: jest.fn(),
                    cubeQuery: mockDimEdits,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                    saveDialogOpen: false,
                    setSaveDialogOpen: jest.fn(),
                    selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                    setSelectedVisualizationUserInput: jest.fn(),
                    visualizationSettingsUserInput: {},
                    setVisualizationSettingsUserInput: jest.fn(),
                    loadedQueryId: '',
                    setLoadedQueryId: jest.fn(),
                    loadedQueryIsDraft: false,
                    setLoadedQueryIsDraft: jest.fn(),
                    publicationEnabled: true,
                    setPublicationEnabled: jest.fn()
                }}>
                    <BasicDimensionEditor language={mockLang} dimension={mockDimension} />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'editValue2' } });
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});
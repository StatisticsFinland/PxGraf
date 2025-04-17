import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { EMetaPropertyType, IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery, IDimensionEditions } from 'types/query';
import { ContentDimensionEditor } from './ContentDimensionEditor';
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
            isVirtual: false,
            unit: {
                'fi': 'yksikko',
                'sv': 'enhet',
                'en': 'unit'
            },
            precision: 0,
            lastUpdated: '2021-01-01',
            additionalProperties: {
                SOURCE: {
                    type: EMetaPropertyType.MultilanguageText,
                    value: {
                        'fi': 'lahde',
                        'sv': 'kalla',
                        'en': 'source'
                    }
                }
            }
        }
    ]
}
const mockLang = 'fi';
const mockFunction = jest.fn();
const mockDimensionEdits: IDimensionEditions = {
    valueEdits: {
        'bar': {
            nameEdit: {
                'fi': 'bar',
                'sv': 'bar',
                'en': 'bar'
            }
        }
    }
};
const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: {},
    variableQueries: {
        foo: mockDimensionEdits
    }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
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
                    <ContentDimensionEditor language={mockLang} dimension={mockDimension} />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

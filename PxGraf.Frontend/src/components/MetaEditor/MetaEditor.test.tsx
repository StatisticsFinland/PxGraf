import React from 'react';
import { render } from '@testing-library/react';
import { IHeaderResult } from 'api/services/default-header';
import '@testing-library/jest-dom';
import { IDimension, VariableType } from 'types/cubeMeta';
import { ICubeQuery, Query, FilterType } from 'types/query';
import MetaEditor from './MetaEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { EditorContext, EditorProvider } from 'contexts/editorContext';

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

const mockVariables: IDimension[] = [{
        Code:'foo',
        Name: {
            'fi': 'asd',
            'sv': 'asd',
            'en': 'asd'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code:'bar',
                Name: {
                    'fi': 'fgfgfg',
                    'sv': 'fgfgfg',
                    'en': 'fgfgfg'
                },
                Virtual: false,
                Unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                Precision: 0,
                LastUpdated: '2021-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            }
        ]
    },
    {
        Code:'foo2',
        Name: {
            'fi': null,
            'sv': null,
            'en': null
        },
        Type: VariableType.Content,
        Values: [
            {
                Code:'bar2',
                Name: {
                    'fi': null,
                    'sv': null,
                    'en': null,
                },
                Virtual: false,
                Unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                Precision: 0,
                LastUpdated: '2021-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            }
            ]
    }
]
const mockLang = 'fi';
const mockFunction = jest.fn();

const isMetaAccordionOpenMock = true;

const defaultHeaderResponseMock: IHeaderResult = {
    isLoading: false,
    isError: false,
    data: {
        'fi': 'asd',
        'en': 'asd',
        'sv': 'asd'
    }
}

const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: {
        'fi': 'foo',
        'sv': 'foo',
        'en': 'foo'
    },
    variableQueries: {
        'asd123query': {
            valueEdits: {
                'code1': {
                    contentComponent: {
                        sourceEdit: {
                            'fi': '123',
                            'sv': '123',
                            'en': '123'
                        },
                        unitEdit: {
                            'fi': '456',
                            'sv': '456',
                            'en': '456'
                        }
                    }
                }
            }
        }
    }
};

const mockQuery: Query = {
    foo: {
        valueFilter: {
            type: FilterType.Item,
            query: ['bar']
        },
        selectable: true,
        virtualValueDefinitions: null
    }
};

const defaultSelectables = { foo: ['2018'] };
const setDefaultSelectables = jest.fn();
const cubeQuery = null;
const setCubeQuery = jest.fn();
const query = null;
const setQuery = jest.fn();
const saveDialogOpen = false;
const setSaveDialogOpen = jest.fn();
const selectedVisualizationUserInput = null;
const setSelectedVisualizationUserInput = jest.fn();
const visualizationSettingsUserInput = null;
const setVisualizationSettingsUserInput = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{ cubeQuery, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables }}>
                    <EditorProvider>
                        <MetaEditor
                            resolvedVariables={mockVariables}
                            cubeQuery={mockCubeQuery}
                            defaultHeaderResponse={defaultHeaderResponseMock}
                            isMetaAccordionOpen={isMetaAccordionOpenMock}
                            language={mockLang}
                            onChange={mockFunction}
                            onMetaAccordionOpenChange={mockFunction}
                        />
                    </EditorProvider>
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
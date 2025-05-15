import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { EMetaPropertyType, IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import MetaEditor from './MetaEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { EditorContext } from 'contexts/editorContext';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { IEditorContentsResponse } from '../../types/editorContentsResponse';

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

const mockDimensions: IDimension[] = [{
        code:'foo',
        name: {
            'fi': 'asd',
            'sv': 'asd',
            'en': 'asd'
        },
        type: EDimensionType.Content,
        values: [
            {
                code:'bar',
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
    },
    {
        code:'foo2',
        name: {
            'fi': null,
            'sv': null,
            'en': null
        },
        type: EDimensionType.Content,
        values: [
            {
                code:'bar2',
                name: {
                    'fi': null,
                    'sv': null,
                    'en': null,
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
]
const mockLang = 'fi';
const mockFunction = jest.fn();

const isMetaAccordionOpenMock = true;

const data: IEditorContentsResponse = {
    headerText: {
        'fi': 'asd',
        'en': 'asd',
        'sv': 'asd'
    }
} as unknown as IEditorContentsResponse;

const defaultHeaderResponseMock: IEditorContentsResult = {
    isLoading: false,
    isError: false,
    data: data
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

const defaultSelectables = { foo: ['2018'] };
const setDefaultSelectables = jest.fn();
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
                <EditorContext.Provider value={{ cubeQuery: mockCubeQuery, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables }}>
                    <MetaEditor
                        resolvedDimensions={mockDimensions}
                        editorContentsResponse={defaultHeaderResponseMock}
                        isMetaAccordionOpen={isMetaAccordionOpenMock}
                        language={mockLang}
                        onMetaAccordionOpenChange={mockFunction}
                    />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
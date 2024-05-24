import { render } from '@testing-library/react';
import { IHeaderResult } from 'api/services/default-header';
import React from 'react';
import { IVariable, VariableType } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import MetaEditor from './MetaEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';

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

const mockVariables: IVariable[] = [{
        code: 'foo',
        name: {
            'fi': 'asd',
            'sv': 'asd',
            'en': 'asd'
        },
        note: {
            'fi': 'seppo',
            'sv': 'seppo',
            'en': 'seppo'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'bar',
                isSum: false,
                name: {
                    'fi': 'fgfgfg',
                    'sv': 'fgfgfg',
                    'en': 'fgfgfg'
                },
                note: {
                    'fi': 'fghjfgh',
                    'sv': 'fghjfgh',
                    'en': 'fghjfgh'
                }
            }
        ]
    },
    {
        code: 'foo2',
        name: {
            'fi': null,
            'sv': null,
            'en': null
        },
        note: {
            'fi': 'seppo2',
            'sv': 'seppo2',
            'en': 'seppo2'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'bar2',
                isSum: false,
                name: {
                    'fi': null,
                    'sv': null,
                    'en': null,
                },
                note: null
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

const cubeQueryMock: ICubeQuery = {
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

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <MetaEditor resolvedVariables={mockVariables} cubeQuery={cubeQueryMock} defaultHeaderResponse={defaultHeaderResponseMock} isMetaAccordionOpen={isMetaAccordionOpenMock} language={mockLang} onChange={mockFunction} onMetaAccordionOpenChange={mockFunction} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { EMetaPropertyType, IDimension, EDimensionType } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';
import { ContentVariableEditor } from './ContentVariableEditor';
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

const mockVariable: IDimension = {
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

const mockVarEdits: IVariableEditions = {
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

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <ContentVariableEditor language={mockLang} onChange={mockFunction} variable={mockVariable} variableEdits={mockVarEdits} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

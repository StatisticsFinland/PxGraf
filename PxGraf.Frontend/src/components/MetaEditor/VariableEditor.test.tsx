import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { IDimension, VariableType } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';
import VariableEditor from './VariableEditor';
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

const mockVariable: IDimension = {
    Code: 'foo',
    Name: {
        'fi': 'asd',
        'sv': 'asd',
        'en': 'asd'
    },
    Type: VariableType.Content,
    Values: [
        {
            Code: 'bar',
            Name: {
                'fi': 'fgfgfg',
                'sv': 'fgfgfg',
                'en': 'fgfgfg'
            },
            Virtual: true,
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
const mockLang = 'fi';
const mockFunction = jest.fn();

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

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
                <VariableEditor language={mockLang} onChange={mockFunction} variable={mockVariable} variableEdits={mockVarEdits} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

import { render } from '@testing-library/react';
import React from 'react';
import { IVariable, VariableType } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';
import VariableEditor from './VariableEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => { }),
            },
        };
    },
}));

const mockVariable: IVariable = {
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

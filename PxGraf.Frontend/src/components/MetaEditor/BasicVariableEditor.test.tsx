import { fireEvent, render, screen } from '@testing-library/react';
import { IVariable, VariableType } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';
import BasicVariableEditor from './BasicVariableEditor';
import '@testing-library/jest-dom';
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
                <BasicVariableEditor language={mockLang} onChange={mockFunction} variableEdits={mockVarEdits} variable={mockVariable} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <BasicVariableEditor language={mockLang} onChange={mockFunction} variableEdits={mockVarEdits} variable={mockVariable} />
            </UiLanguageContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'editValue2' } });
        expect(mockFunction).toBeCalledTimes(1);
    });
});

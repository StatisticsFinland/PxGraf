import React from 'react';
import '@testing-library/jest-dom';
import { fireEvent, render, screen } from '@testing-library/react';
import { EMetaPropertyType, IDimensionValue } from 'types/cubeMeta';
import { IVariableValueEditions } from 'types/query';
import ContentVariableValueEditor from './ContentVariableValueEditor';
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

const mockVariableValue: IDimensionValue = {
    lastUpdated: '1.1.2000',
    precision: 5,
    additionalProperties:
    {
        SOURCE: {
            type: EMetaPropertyType.MultilanguageText,
            value: {
                'fi': 'lahde',
                'sv': 'kalla',
                'en': 'source'
            }
        }
    },
    unit: {
        'fi': 'qwe',
        'sv': 'qwe',
        'en': 'qwe'
    },
    code: 'foo',
    name: {
        'fi': 'asd',
        'sv': 'asd',
        'en': 'asd'
    },
    isVirtual: false
}
const mockLang = 'fi';
const mockFunction = jest.fn();

const mockValEdits: IVariableValueEditions = {
    contentComponent: {
        sourceEdit: {
            'foo': 'bar'
        }
    }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <ContentVariableValueEditor language={mockLang} onChange={mockFunction} valueEdits={mockValEdits} variableValue={mockVariableValue} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <ContentVariableValueEditor language={mockLang} onChange={mockFunction} valueEdits={mockValEdits} variableValue={mockVariableValue} />
            </UiLanguageContext.Provider >
        );
        fireEvent.change(screen.getByDisplayValue('asd'), { target: { value: 'editValue2' } });
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});

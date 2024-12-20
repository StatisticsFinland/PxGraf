import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { IDimensionEditions } from 'types/query';
import BasicDimensionEditor from './BasicDimensionEditor';
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

const mockDimEdits: IDimensionEditions = {
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
                <BasicDimensionEditor language={mockLang} onChange={mockFunction} dimensionEdits={mockDimEdits} dimension={mockDimension} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <BasicDimensionEditor language={mockLang} onChange={mockFunction} dimensionEdits={mockDimEdits} dimension={mockDimension} />
            </UiLanguageContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'editValue2' } });
        expect(mockFunction).toBeCalledTimes(1);
    });
});

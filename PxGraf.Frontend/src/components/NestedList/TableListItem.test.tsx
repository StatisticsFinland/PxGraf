import React from 'react';
import { render } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import { IDatabaseGroupHeader } from 'api/services/table';
import { TableListItem } from './TableListItem';

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

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

const mockItem: IDatabaseGroupHeader = {
    code: 'dbid1',
    name: { 'fi': 'seppodbid', 'en': 'seppodbiden' },
    languages: ['fi', 'en']
}

const mockDepth = 2;
const mockPath = ['foo', 'bar', 'baz'];

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];

const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();


describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListItem currentPath={mockPath} item={mockItem} depth={mockDepth} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('should show available languages with database level item name', () => {
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListItem currentPath={[]} item={mockItem} depth={0} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment().textContent).toContain('seppodbid (FI, EN)');
    });

    it('should show name by default language if given UI language is not available', () => {
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: "sv", setLanguage, languageTab: "sv", setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListItem currentPath={[]} item={mockItem} depth={0} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment().textContent).toContain('seppodbid (FI, EN)');
    });
});

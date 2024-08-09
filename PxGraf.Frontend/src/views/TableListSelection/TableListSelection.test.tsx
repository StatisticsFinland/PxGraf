import React from 'react';
import { render } from '@testing-library/react';
import { ITableResult } from 'api/services/table';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import TableListSelection from './TableListSelection';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => {}),
            },
        };
    },
}));

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/'
}));

jest.mock('api/services/languages', () => ({
    ...jest.requireActual('api/services/languages'),
    useLanguagesQuery: () => {
        return {
            data: ['fi', 'en', 'sv']
        }
    },
}));

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useParams: () => {
        return {
            '*': 'foo/bar'
        };
    },
}));

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];

const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

const mockTableResult: ITableResult = {
    isLoading: false,
    isError: false,
    data:
    {
        headers: [
            {
                name: { 'fi': 'foo1-fi', 'en': 'foo1-en', 'sv': 'foo1-sv' },
                code: 'id1',
                languages: ['fi', 'en', 'sv']
            },
            {
                name: { 'fi': 'foo2-fi', 'en': 'foo2-en' },
                code: 'id2',
                languages: ['fi', 'en']
            }
        ],
        files: [
            {
                name: { 'fi': 'foo3-fi', 'en': 'foo3-en', 'sv': 'foo3-sv' },
                code: 'id2',
                lastUpdated: '1.1.2000',
                languages: ['fi', 'en', 'sv']
            }
        ],
    }
}

const mockLanguagesResult = ["fi", "en", "sv"];


jest.mock('api/services/languages', () => ({
    ...jest.requireActual('api/services/languages'),
    useLanguagesQuery: () => {
        return mockLanguagesResult;
    },
}));

jest.mock('api/services/table', () => ({
    ...jest.requireActual('api/services/table'),
    useTableQuery: (path: string, lang: string) => {
        return mockTableResult;
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<MemoryRouter><UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}><TableListSelection /></UiLanguageContext.Provider></MemoryRouter>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('should show the contents with primary database language if given UI language is not supported', () => {
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: "fr", setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListSelection />
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment().textContent).toContain('foo1-fi');
    });
});

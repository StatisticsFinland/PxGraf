import React from 'react';
import { render } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import { IDatabaseGroupContents, ITableResult } from '../../api/services/table';
import NestedList from './NestedList';

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

const mockDatabaseContents: IDatabaseGroupContents = {
    headers: [
        {
            code: 'asd',
            name: { 'fi': 'foo-fi', 'en': 'foo-en', 'sv': 'foo-sv' },
            languages: ['fi', 'en', 'sv']
        },
        {
            code: 'id',
            name: { 'fi': 'foo3-fi', 'en': 'foo3-en' },
            languages: ['fi', 'en']
        }
    ],
    files: [
        {
            code: 'asd2',
            lastUpdated: '2021-11-13T14:53:06',
            name: { 'fi': 'foo2-fi', 'sv': 'foo2-sv' },
            languages: ['en', 'sv']
        }
    ]
}

const mockTableQueryResult: ITableResult = {
    isLoading: false,
    isError: false,
    data: mockDatabaseContents
}

jest.mock('api/services/table', () => ({
    ...jest.requireActual('api/services/table'),
    useTableQuery: () => {
        return mockTableQueryResult;
    },
}));

const mockDepth = 3;
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
                    <NestedList depth={mockDepth} path={mockPath} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly while loading data', () => {
        mockTableQueryResult.isLoading = true;
        mockTableQueryResult.data = null;
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NestedList depth={mockDepth} path={mockPath} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly when table query ', () => {
        mockTableQueryResult.isLoading = false;
        mockTableQueryResult.data = null;
        mockTableQueryResult.isError = true;
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NestedList depth={mockDepth} path={mockPath} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows item listed with the first available if given ui language is not supported', () => {
        mockTableQueryResult.isLoading = false;
        mockTableQueryResult.isError = false;
        mockTableQueryResult.data = mockDatabaseContents;
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: 'fr', setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NestedList depth={null} path={mockPath} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment().textContent).toContain('foo-fi');
    });
});
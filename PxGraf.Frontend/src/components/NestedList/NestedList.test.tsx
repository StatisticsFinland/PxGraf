import { render } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
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

jest.mock('envVars', () => {
    return {
        PxGrafUrl: 'test-url.fi/',
    };
})

const mockTableQueryResult = {
    isLoading: false,
    isError: false,
    data: [
        {
            id: 'asd',
            type: 'l',
            updated: '2021-10-13T14:53:06',
            text: { 'fi': 'foo-fi', 'en': 'foo-en', 'sv': 'foo-sv'},
            languages: ['fi', 'en', 'sv']
        },
        {
            id: 'asd2',
            type: 't',
            updated: '2021-11-13T14:53:06',
            text: { 'fi': 'foo2-fi', 'sv': 'foo2-sv' },
            languages: ['en', 'sv']
        },
        {
            id: 'id',
            text: { 'fi': 'foo3-fi', 'en': 'foo3-en' },
            languages: ['fi', 'en']
        }
    ]
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
});

describe('Assertion tests', () => {
    it('shows item listed with the first available if given ui language is not supported', () => {
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
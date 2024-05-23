import { render, screen } from '@testing-library/react';
import { DirectoryInfo } from './DirectoryInfo';
import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockPath = 'asd123';
const mockItem = {
    id: 'id',
    updated: '01.01.2000',
    text: { 'fi': 'foo1-fi', 'en': 'foo1-en', 'sv': 'foo1-sv' },
    languages: ['fi', 'en', 'sv']
};

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => undefined),
            },
        };
    },
}));

jest.mock('api/services/table', () => ({
    ...jest.requireActual('api/services/table'),
    useTableQuery: () => {
        return mockItem;
    },
}));

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        // mock router for testing purposes
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <DirectoryInfo path={mockPath} item={mockItem} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('should contain a specific link with a specific text', () => {
        const expectedHref = '/table-list/a/s/d/1/2/3/id/';
        render(<MemoryRouter><DirectoryInfo path={mockPath} item={mockItem} /></MemoryRouter>);
        expect(screen.getByText(mockItem.text[language])).toBeInTheDocument();
        expect(screen.getByRole('link').getAttribute('href')).toEqual(expectedHref);
    });

    it('displays text in the correct language when the language is included in item.languages', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <DirectoryInfo path={mockPath} item={mockItem} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('foo1-fi')).toBeInTheDocument();
    });

    it('displays text in the default language when the language is not included in item.languages', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: 'fr', setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <DirectoryInfo path={mockPath} item={mockItem} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('foo1-fi')).toBeInTheDocument();
    });
});
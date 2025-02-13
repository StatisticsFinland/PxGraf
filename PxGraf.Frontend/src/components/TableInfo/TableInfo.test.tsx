import React from 'react';
import { render } from '@testing-library/react';
import { TableInfo } from './TableInfo';
import '@testing-library/jest-dom';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { MemoryRouter } from 'react-router-dom';
import { EDatabaseTableError, IDatabaseTable } from 'types/tableListItems';

const mockPath = 'asd123';
const mockItem: IDatabaseTable = {
    fileName: 'id',
    lastUpdated: '2021-10-13T14:53:06',
    name: { 'fi': 'text-fi', 'en': 'text-en', 'sv': 'text-sv'},
    languages: ['fi', 'en', 'sv'],
};

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

const mockTableQueryResult = {
    data: {
        dimensions: [
            {
                code: 'code',
                text: 'text',
                valueTexts: 'valueTexts'
            }
        ]
    }
}

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

jest.mock('api/services/table', () => ({
    ...jest.requireActual('api/services/table'),
    useTableQuery: () => {
        return mockTableQueryResult;
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
        const { asFragment } = render(<MemoryRouter><UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}><TableInfo path={mockPath} item={mockItem} /></UiLanguageContext.Provider></MemoryRouter>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('displays text in the correct language when the language is included in item.languages', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableInfo path={mockPath} item={mockItem} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('text-fi')).toBeInTheDocument();
    });

    it('displays text in the correct language when the language is not included in item.languages', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: 'fr', setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableInfo path={mockPath} item={mockItem} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('text-fi')).toBeInTheDocument();
    });

    it('displays a warning alert when item is missing content dimension', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableInfo path={mockPath} item={{ ...mockItem, error: EDatabaseTableError.contentDimensionMissing }} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('error.contentVariableMissing')).toBeInTheDocument();
    });

    it('displays a warning alert when item is missing time dimension', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableInfo path={mockPath} item={{ ...mockItem, error: EDatabaseTableError.timeDimensionMissing }} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('error.timeVariableMissing')).toBeInTheDocument();
    });

    it('displays a warning alert when item has a contentLoad error', () => {
        const { getByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableInfo path={mockPath} item={{ ...mockItem, error: EDatabaseTableError.contentLoad }} />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        expect(getByText('error.contentLoad')).toBeInTheDocument();
    });
});
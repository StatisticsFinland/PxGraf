import { render } from '@testing-library/react';
import { TableInfo } from './TableInfo';
import '@testing-library/jest-dom';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { MemoryRouter } from 'react-router-dom';

const mockPath = 'asd123';
const mockItem = {
    id: 'id',
    updated: '2021-10-13T14:53:06',
    text: { 'fi': 'text-fi', 'en': 'text-en', 'sv': 'text-sv'},
    languages: ['fi', 'en', 'sv'],
};

const mockTableQueryResult = {
    data: {
        variables: [
            {
                code: 'code',
                text: 'text',
                valueTexts: 'valueTexts'
            }
        ]
    }
}

jest.mock('envVars', () => {
    return {
        PxGrafUrl: 'test-url.fi/',
    };
})

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
    useTableQuery: (currentPath: string, language: string) => {
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
});
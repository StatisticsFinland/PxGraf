import React from 'react';
import { render } from '@testing-library/react';
import { ITableResult } from 'api/services/table';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import TableTreeSelection from './TableTreeSelection';
import { NavigationProvider } from 'contexts/navigationContext';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => ({})),
            },
        };
    },
}));

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
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
    data: {
        headers: [],
        files: [
            {
                name: { 'fi': 'foo1', 'en': 'foo1', 'sv': 'foo1' },
                fileName: 'id1.px',
                lastUpdated: '1.1.2000',
                languages: ['fi', 'en', 'sv']
            },
            {
                name: { 'fi': 'foo2', 'en': 'foo2' },
                fileName: 'id2.px',
                lastUpdated: '1.1.2000',
                languages: ['fi', 'en']
            }
        ],
    }
}

jest.mock('api/services/table', () => ({
    ...jest.requireActual('api/services/table'),
    useTableQuery: () => {
        return mockTableResult;
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NavigationProvider>
                        <TableTreeSelection />
                    </NavigationProvider>
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

import React from 'react';
import { render } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import { TableItem } from './TableItem';
import { IDatabaseTable } from 'api/services/table';

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

const mockItem: IDatabaseTable = {
    code: 'asd',
    lastUpdated: '2021-10-13T14:53:06',
    name: { 'fi': 'seppo', 'sv': 'seppo-sv' },
    languages: ['fi', 'sv']
}

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
                    <TableItem currentPath={mockPath} item={mockItem} depth={mockDepth} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows available languages and last updated with table level item names', async () => {
        const { findByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableItem currentPath={mockPath} item={mockItem} depth={mockDepth}  />
                </UiLanguageContext.Provider>
            </MemoryRouter>);

        const name = await findByText("seppo (FI, SV)");
        const lastUpdate = await findByText("tableSelect.updated: 13.10.2021 klo 14.53.06");
        expect(name).toBeInTheDocument();
        expect(lastUpdate).toBeInTheDocument();
    });

    it('renders MUI alert when error property is true', async () => {
        const mockErrorItem: IDatabaseTable = {
            code: 'error',
            lastUpdated: null,
            name: { 'fi': 'error', 'sv': 'error-sv' },
            languages: ['fi', 'sv'],
            error: true
        };

        const { findByRole, findByText } = render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableItem currentPath={mockPath} item={mockErrorItem} depth={mockDepth}  />
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        const alert = await findByRole('alert');
        const alertHeader = await findByText("error");
        expect(alert).toBeInTheDocument();
        expect(alertHeader).toBeInTheDocument();
    });
});
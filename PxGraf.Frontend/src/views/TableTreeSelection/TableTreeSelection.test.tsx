import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { ITableResult } from 'api/services/table';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import TableTreeSelection from './TableTreeSelection';
import { NavigationProvider } from 'contexts/navigationContext';

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

    it('preserves router history state when opening a folder', () => {
        const previousData = mockTableResult.data;
        mockTableResult.data = {
            headers: [
                { code: 'db1', name: { fi: 'Database One' }, languages: ['fi'] },
                { code: 'db2', name: { fi: 'Database Two' }, languages: ['fi'] },
            ],
            files: [],
        };
        const routerState = { idx: 2, key: 'router-key', usr: { source: 'test' } };
        globalThis.history.replaceState(routerState, '', '/');
        const replaceState = jest.spyOn(globalThis.history, 'replaceState');

        render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NavigationProvider>
                        <TableTreeSelection />
                    </NavigationProvider>
                </UiLanguageContext.Provider>
            </MemoryRouter>
        );

        fireEvent.click(screen.getByRole('button', { name: /Database One/i }));

        expect(replaceState).toHaveBeenCalledWith(routerState, '', '/?tablePath=db1');
        replaceState.mockRestore();
        mockTableResult.data = previousData;
    });
});

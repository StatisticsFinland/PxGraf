import React from 'react';
import { fireEvent, render, screen, waitForElementToBeRemoved } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import { TableListItem } from './TableListItem';
import { IDatabaseGroupHeader } from 'types/tableListItems';
import { NavigationProvider } from 'contexts/navigationContext';

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

jest.mock('./NestedList', () => ({
    __esModule: true,
    default: () => <div>child content</div>,
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
                    <NavigationProvider>
                        <TableListItem currentPath={mockPath} item={mockItem} depth={mockDepth} />
                    </NavigationProvider>
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows available languages separately from the database name', () => {
        render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NavigationProvider>
                        <TableListItem currentPath={[]} item={mockItem} depth={0} />
                    </NavigationProvider>
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(screen.getByText('seppodbid')).toBeInTheDocument();
        expect(screen.getByText('(FI, EN)')).toBeInTheDocument();
    });

    it('should show name by default language if given UI language is not available', () => {
        render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: "sv", setLanguage, languageTab: "sv", setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <NavigationProvider>
                        <TableListItem currentPath={[]} item={mockItem} depth={0} />
                    </NavigationProvider>
                </UiLanguageContext.Provider>
            </MemoryRouter>);
        expect(screen.getByText('seppodbid')).toBeInTheDocument();
        expect(screen.getByText('(FI, EN)')).toBeInTheDocument();
    });

    it('falls back to the first declared language when the UI language name is missing', () => {
        const itemWithMissingName = {
            ...mockItem,
            name: { fi: 'first-language-name' },
        };
        render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language: 'en', setLanguage, languageTab: 'en', setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListItem currentPath={[]} item={itemWithMissingName} depth={0} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);

        expect(screen.getByText('first-language-name')).toBeInTheDocument();
    });

    it('expands and collapses from the full row while reporting only opened paths', async () => {
        const onPathOpen = jest.fn();
        render(
            <MemoryRouter>
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <TableListItem currentPath={mockPath} item={mockItem} depth={mockDepth} onPathOpen={onPathOpen} />
                </UiLanguageContext.Provider>
            </MemoryRouter>);

        const row = screen.getByRole('button', { name: /seppodbid/i });
        expect(row).toHaveAttribute('aria-expanded', 'false');
        expect(screen.queryByText('child content')).not.toBeInTheDocument();

        fireEvent.click(row);

        expect(row).toHaveAttribute('aria-expanded', 'true');
        expect(onPathOpen).toHaveBeenCalledWith(mockPath);
        expect(screen.getByText('child content')).toBeInTheDocument();
        expect(screen.queryByRole('link', { name: /listView/i })).not.toBeInTheDocument();

        fireEvent.click(row);

        expect(row).toHaveAttribute('aria-expanded', 'false');
        expect(onPathOpen).toHaveBeenCalledTimes(1);
        await waitForElementToBeRemoved(() => screen.queryByText('child content'));
    });

});

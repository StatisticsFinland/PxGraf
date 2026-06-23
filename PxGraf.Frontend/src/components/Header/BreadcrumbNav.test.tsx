import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom';
import BreadcrumbNav from './BreadcrumbNav';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl/',
    PublicUrl: 'publicUrl/',
    BasePath: ''
}));

jest.mock('Router', () => ({
    urls: {
        editor: (path: string[]) => `/editor/${path.join('/')}/`
    }
}));

const mockUseQueries = jest.fn();
jest.mock('@tanstack/react-query', () => ({
    ...jest.requireActual('@tanstack/react-query'),
    useQueries: (options: unknown) => mockUseQueries(options),
}));

// Use the real module so that UiLanguageContext retains its React context identity
// and UiLanguageContext.Provider works correctly in test wrappers.
jest.mock('contexts/uiLanguageContext', () => jest.requireActual('contexts/uiLanguageContext'));

const mockSetTablePath = jest.fn();
jest.mock('contexts/navigationContext', () => ({
    useNavigationContext: () => ({ tablePath: null, setTablePath: mockSetTablePath }),
}));

const mockContextValue = {
    language: 'en',
    setLanguage: jest.fn(),
    languageTab: 'en',
    setLanguageTab: jest.fn(),
    availableUiLanguages: ['en'],
    uiContentLanguage: 'en',
    setUiContentLanguage: jest.fn(),
};

const renderBreadcrumb = (tablePath: string[], useQueriesResult: unknown[] = [], initialEntries: string[] = ['/']) => {
    mockUseQueries.mockReturnValue(useQueriesResult);
    return render(
        <UiLanguageContext.Provider value={mockContextValue}>
            <MemoryRouter initialEntries={initialEntries}>
                <BreadcrumbNav tablePath={tablePath} />
            </MemoryRouter>
        </UiLanguageContext.Provider>
    );
};

describe('BreadcrumbNav', () => {
    it('renders raw segment IDs when queries are loading', async () => {
        renderBreadcrumb(['db1', 'stat1', 'table1'], [{ data: null, isLoading: true }, { data: null, isLoading: true }]);
        await waitFor(() => {
            expect(screen.getByText('db1')).toBeInTheDocument();
            expect(screen.getByText('stat1')).toBeInTheDocument();
            expect(screen.getByText('table1')).toBeInTheDocument();
        });
    });

    it('renders translated names when query data is available', async () => {
        const mockResults = [
            {
                data: {
                    headers: [{ code: 'db1', name: { en: 'Database One' }, languages: ['en'] }],
                    files: []
                },
                isLoading: false
            },
            {
                data: {
                    headers: [{ code: 'stat1', name: { en: 'Statistics One' }, languages: ['en'] }],
                    files: []
                },
                isLoading: false
            }
        ];
        renderBreadcrumb(['db1', 'stat1', 'table1'], mockResults);
        await waitFor(() => {
            expect(screen.getByText('Database One')).toBeInTheDocument();
            expect(screen.getByText('Statistics One')).toBeInTheDocument();
            expect(screen.getByText('table1')).toBeInTheDocument();
        });
    });

    it('renders correct link targets on editor route', async () => {
        renderBreadcrumb(['db1', 'stat1', 'table1'], [{ data: null }, { data: null }, { data: null }], ['/editor/db1/stat1/table1/']);
        await waitFor(() => {
            const links = screen.getAllByRole('link');
            expect(links[0]).toHaveAttribute('href', '/?tablePath=db1');
            expect(links[1]).toHaveAttribute('href', '/?tablePath=db1,stat1');
            expect(links[2]).toHaveAttribute('href', '/editor/db1/stat1/table1/');
        });
    });

    it('renders last segment as plain text on non-editor route', async () => {
        renderBreadcrumb(['db1', 'stat1'], [{ data: null }, { data: null }]);
        await waitFor(() => {
            const links = screen.getAllByRole('link');
            expect(links).toHaveLength(1);
            expect(links[0]).toHaveAttribute('href', '/?tablePath=db1');
            // Last segment is plain text, not a link
            expect(screen.getByText('stat1')).toBeInTheDocument();
            expect(screen.getByText('stat1').closest('a')).toBeNull();
        });
    });

    it('calls useQueries with correct parent paths for each segment', () => {
        renderBreadcrumb(['db1', 'stat1', 'table1'], [{ data: null }, { data: null }, { data: null }]);
        expect(mockUseQueries).toHaveBeenCalledWith(
            expect.objectContaining({
                queries: expect.arrayContaining([
                    expect.objectContaining({ queryKey: ['table', []] }),
                    expect.objectContaining({ queryKey: ['table', ['db1']] }),
                    expect.objectContaining({ queryKey: ['table', ['db1', 'stat1']] }),
                ])
            })
        );
    });

    it('uses language from context and falls back to first available language when context language is absent', async () => {
        const mockResults = [
            {
                data: {
                    headers: [{ code: 'db1', name: { en: 'English DB', fi: 'Finnish DB' }, languages: ['en', 'fi'] }],
                    files: []
                },
                isLoading: false
            },
            {
                data: {
                    headers: [{ code: 'stat1', name: { fi: 'Finnish Stat' }, languages: ['fi'] }],
                    files: []
                },
                isLoading: false
            }
        ];
        renderBreadcrumb(['db1', 'stat1', 'table1'], mockResults);
        await waitFor(() => {
            // context language 'en' is in db1's languages — use 'en' name
            expect(screen.getByText('English DB')).toBeInTheDocument();
            // context language 'en' is NOT in stat1's languages — fall back to first language 'fi'
            expect(screen.getByText('Finnish Stat')).toBeInTheDocument();
        });
    });

    it('renders single segment without crashing', async () => {
        renderBreadcrumb(['table1'], []);
        await waitFor(() => {
            expect(screen.getByText('table1')).toBeInTheDocument();
        });
    });

    it('falls back to raw code when query data is unavailable', async () => {
        renderBreadcrumb(['db1', 'stat1', 'table1'], [{ data: null, isError: true }, { data: null, isError: true }]);
        await waitFor(() => {
            expect(screen.getByText('db1')).toBeInTheDocument();
            expect(screen.getByText('stat1')).toBeInTheDocument();
        });
    });

    it('resolves table file names from files array in parent listing', async () => {
        const mockResults = [
            {
                data: {
                    headers: [{ code: 'db1', name: { en: 'Database One' }, languages: ['en'] }],
                    files: []
                },
                isLoading: false
            },
            {
                data: {
                    headers: [],
                    files: [{ fileName: 'table1', name: { en: 'Population by age' }, languages: ['en'], lastUpdated: null }]
                },
                isLoading: false
            }
        ];
        renderBreadcrumb(['db1', 'table1'], mockResults);
        await waitFor(() => {
            expect(screen.getByText('Database One')).toBeInTheDocument();
            expect(screen.getByText('Population by age')).toBeInTheDocument();
        });
    });

    it('uses > as separator', () => {
        renderBreadcrumb(['db1', 'stat1'], [{ data: null }, { data: null }]);
        expect(screen.getByRole('navigation').querySelector('.MuiBreadcrumbs-separator')).toHaveTextContent('>');
    });
});

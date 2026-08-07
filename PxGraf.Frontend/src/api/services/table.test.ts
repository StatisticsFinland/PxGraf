import { renderHook, waitFor } from '@testing-library/react';
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useTableQuery } from './table';

const mockGetAsync = jest.fn();

jest.mock('api/client', () => {
    return jest.fn().mockImplementation(() => ({
        getAsync: mockGetAsync,
    }));
});

jest.mock('envVars', () => ({
    PxGrafUrl: 'http://localhost:3000/',
    BasePath: ''
}));

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    });
    return function TestWrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children);
    };
};

describe('useTableQuery', () => {
    beforeEach(() => {
        mockGetAsync.mockReset();
    });

    it('calls the correct API endpoint from idStack', async () => {
        const mockData = { headers: [], files: [] };
        mockGetAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(() => useTableQuery(['db1', 'folder1']), {
            wrapper: createWrapper(),
        });

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockGetAsync).toHaveBeenCalledWith('creation/data-bases/db1/folder1');
        expect(result.current.data).toEqual(mockData);
        expect(result.current.isError).toBe(false);
    });

    it('handles empty idStack', async () => {
        const mockData = { headers: [], files: [] };
        mockGetAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(() => useTableQuery([]), {
            wrapper: createWrapper(),
        });

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockGetAsync).toHaveBeenCalledWith('creation/data-bases/');
    });

    it('sets isError when fetch fails', async () => {
        mockGetAsync.mockRejectedValueOnce(new Error('Network error'));

        const { result } = renderHook(() => useTableQuery(['db1']), {
            wrapper: createWrapper(),
        });

        await waitFor(() => expect(result.current.isError).toBe(true));
        expect(result.current.data).toBeUndefined();
    });
});

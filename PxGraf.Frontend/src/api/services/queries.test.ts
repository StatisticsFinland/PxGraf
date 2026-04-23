import { renderHook, waitFor } from '@testing-library/react';
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useFetchSavedQuery, fetchSavedQuery } from './queries';

const mockGetAsync = jest.fn();
const mockPostAsync = jest.fn();

jest.mock('api/client', () => {
    return jest.fn().mockImplementation(() => ({
        getAsync: mockGetAsync,
        postAsync: mockPostAsync,
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
    return ({ children }: { children: React.ReactNode }) =>
        React.createElement(QueryClientProvider, { client: queryClient }, children);
};

describe('fetchSavedQuery', () => {
    beforeEach(() => {
        mockGetAsync.mockReset();
    });

    it('calls the correct endpoint with queryId', async () => {
        const mockResponse = {
            id: 'test-id',
            draft: false,
            query: { tableReference: { name: 'table', hierarchy: [] }, variableQueries: {} },
            settings: {}
        };
        mockGetAsync.mockResolvedValueOnce(mockResponse);

        const result = await fetchSavedQuery('test-id');
        expect(mockGetAsync).toHaveBeenCalledWith('sq/test-id');
        expect(result).toEqual(mockResponse);
    });

    it('throws when API call fails', async () => {
        mockGetAsync.mockRejectedValueOnce(new Error('Not found'));
        await expect(fetchSavedQuery('bad-id')).rejects.toThrow('Not found');
    });
});

describe('useFetchSavedQuery', () => {
    beforeEach(() => {
        mockGetAsync.mockReset();
    });

    it('fetches and returns saved query data', async () => {
        const mockResponse = {
            id: 'test-id',
            draft: false,
            query: { tableReference: { name: 'table', hierarchy: [] }, variableQueries: {} },
            settings: {}
        };
        mockGetAsync.mockResolvedValueOnce(mockResponse);

        const { result } = renderHook(() => useFetchSavedQuery('test-id'), {
            wrapper: createWrapper(),
        });

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(result.current.data).toEqual(mockResponse);
        expect(result.current.isError).toBe(false);
        expect(result.current.isSuccess).toBe(true);
    });

    it('sets isError when fetch fails', async () => {
        mockGetAsync.mockRejectedValueOnce(new Error('Network error'));

        const { result } = renderHook(() => useFetchSavedQuery('test-id'), {
            wrapper: createWrapper(),
        });

        await waitFor(() => expect(result.current.isError).toBe(true));
        expect(result.current.isSuccess).toBe(false);
    });
});

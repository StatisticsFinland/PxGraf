import { renderHook, waitFor } from '@testing-library/react';
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useResolveDimensionFiltersQuery } from './filter-dimension';
import { FilterType, Query } from 'types/query';

const mockPostAsync = jest.fn();

jest.mock('api/client', () => {
    return jest.fn().mockImplementation(() => ({
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

const mockIdStack = ['db', 'table.px'];

describe('useResolveDimensionFiltersQuery', () => {
    beforeEach(() => {
        mockPostAsync.mockReset();
    });

    it('transforms query dimension filters and calls the correct endpoint', async () => {
        const mockQuery: Query = {
            dim1: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: null },
            dim2: { valueFilter: { type: FilterType.Item, query: ['a', 'b'] }, selectable: true, virtualValueDefinitions: null }
        };
        const mockData = { dim1: ['val1'], dim2: ['a', 'b'] };
        mockPostAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(
            () => useResolveDimensionFiltersQuery(mockIdStack, mockQuery),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockPostAsync).toHaveBeenCalledWith('creation/filter-dimension', expect.any(String));

        // Verify the request body contains the correct structure
        const requestBody = JSON.parse(mockPostAsync.mock.calls[0][1]);
        expect(requestBody).toHaveProperty('tableReference');
        expect(requestBody.tableReference).toEqual({ name: 'table.px', hierarchy: ['db'] });
        expect(requestBody).toHaveProperty('filters');
        expect(requestBody.filters.dim1).toEqual({ type: FilterType.All });
        expect(requestBody.filters.dim2).toEqual({ type: FilterType.Item, query: ['a', 'b'] });
        expect(result.current.data).toEqual(mockData);
    });

    it('handles null query gracefully', async () => {
        const mockData = {};
        mockPostAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(
            () => useResolveDimensionFiltersQuery(mockIdStack, null),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockPostAsync).toHaveBeenCalled();
    });

    it('handles empty query', async () => {
        const mockData = {};
        mockPostAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(
            () => useResolveDimensionFiltersQuery(mockIdStack, {}),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        const requestBody = JSON.parse(mockPostAsync.mock.calls[0][1]);
        expect(requestBody.filters).toEqual({});
    });

    it('sets isError when fetch fails', async () => {
        const mockQuery: Query = {
            dim1: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: null }
        };
        mockPostAsync.mockRejectedValueOnce(new Error('Network error'));

        const { result } = renderHook(
            () => useResolveDimensionFiltersQuery(mockIdStack, mockQuery),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isError).toBe(true));
    });
});

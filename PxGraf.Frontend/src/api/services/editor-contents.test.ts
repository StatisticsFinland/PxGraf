import { renderHook, waitFor } from '@testing-library/react';
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useEditorContentsQuery } from './editor-contents';
import { FilterType, ICubeQuery, Query } from 'types/query';

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
    return function TestWrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children);
    };
};

const mockQuery: Query = {
    dim1: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: null }
};
const mockCubeQuery: ICubeQuery = { variableQueries: {} };
const mockIdStack = ['db', 'table.px'];

describe('useEditorContentsQuery', () => {
    beforeEach(() => {
        mockPostAsync.mockReset();
    });

    it('calls the correct endpoint with POST and serialized body', async () => {
        const mockData = {
            size: 10, sizeWarningLimit: 100, maximumSupportedSize: 1000,
            headerText: { fi: 'header' }, maximumHeaderLength: 100,
            visualizationOptions: [], visualizationRejectionReasons: {}
        };
        mockPostAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(
            () => useEditorContentsQuery(mockIdStack, mockQuery, mockCubeQuery),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockPostAsync).toHaveBeenCalledWith('creation/editor-contents', expect.any(String));
        expect(result.current.data).toEqual(mockData);
    });

    it('is disabled when query is null', async () => {
        const { result } = renderHook(
            () => useEditorContentsQuery(mockIdStack, null, mockCubeQuery),
            { wrapper: createWrapper() }
        );

        // When disabled, fetchStatus is 'idle' and the API is not called
        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('sets isError when fetch fails', async () => {
        mockPostAsync.mockRejectedValueOnce(new Error('Network error'));

        const { result } = renderHook(
            () => useEditorContentsQuery(mockIdStack, mockQuery, mockCubeQuery),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isError).toBe(true));
    });
});

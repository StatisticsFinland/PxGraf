import { renderHook, waitFor } from '@testing-library/react';
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useVisualizationQuery } from './visualization';
import { FilterType, ICubeQuery, Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';

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
const mockQuery: Query = {
    dim1: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: null }
};
const mockCubeQuery: ICubeQuery = { variableQueries: {} };

describe('useVisualizationQuery', () => {
    beforeEach(() => {
        mockPostAsync.mockReset();
    });

    it('is disabled when query is null', () => {
        const settings: IVisualizationSettings = {};
        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, null, mockCubeQuery, 'fi', 'lineChart', settings),
            { wrapper: createWrapper() }
        );

        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('is disabled when selectedVisualization is null', () => {
        const settings: IVisualizationSettings = {};
        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', null, settings),
            { wrapper: createWrapper() }
        );

        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('is disabled when visualizationSettings is null', () => {
        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', 'lineChart', null),
            { wrapper: createWrapper() }
        );

        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('is disabled when sorting is required but missing', () => {
        const settings: IVisualizationSettings = {};
        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', 'horizontalBarChart', settings),
            { wrapper: createWrapper() }
        );

        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('is disabled when sorting is required for pieChart but missing', () => {
        const settings: IVisualizationSettings = {};
        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', 'pieChart', settings),
            { wrapper: createWrapper() }
        );

        expect(mockPostAsync).not.toHaveBeenCalled();
        expect(result.current.data).toBeUndefined();
    });

    it('calls API when all conditions are met for a chart that does not require sorting', async () => {
        const settings: IVisualizationSettings = {};
        const mockData = { data: [], metaData: [], selectableVariableCodes: [] };
        mockPostAsync.mockResolvedValueOnce(mockData);

        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', 'lineChart', settings),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isLoading).toBe(false));
        expect(mockPostAsync).toHaveBeenCalledWith('creation/visualization', expect.any(String));
    });

    it('sets isError when fetch fails', async () => {
        const settings: IVisualizationSettings = {};
        mockPostAsync.mockRejectedValueOnce(new Error('Network error'));

        const { result } = renderHook(
            () => useVisualizationQuery(mockIdStack, mockQuery, mockCubeQuery, 'fi', 'lineChart', settings),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isError).toBe(true));
    });
});

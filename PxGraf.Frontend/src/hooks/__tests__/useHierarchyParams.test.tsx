import useHierarchyParams from "hooks/useHierarchyParams";
import { renderHook } from "@testing-library/react";
import { Location, useLocation } from "react-router-dom";

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useLocation: jest.fn(),
}));

const createMockLocation = (query: string): Location => ({
    search: query,
    state: null,
    key: null,
    pathname: '/',
    hash: '',
});

describe('useHierarchyParams hook', () => {
    it('should return correct query param values', () => {
        const mockLocation = createMockLocation('?tablePath=testDb,testStat,testTable');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useHierarchyParams());
        const resultValue = renderResult.result.current;

        expect(resultValue).toEqual(['testDb', 'testStat', 'testTable']);
    });

    it('should return empty array if no query params in url', () => {
        const mockLocation = createMockLocation(null);

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useHierarchyParams());
        const resultValue = renderResult.result.current;

        expect(resultValue).toEqual([]);
    });

    it('should not care about extra params', () => {
        const mockLocation = createMockLocation('?randomParam=someValue&tablePath=testDb,testTable');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useHierarchyParams());
        const resultValue = renderResult.result.current;

        expect(resultValue).toEqual(['testDb', 'testTable']);
    });
});

import useQueryParams from "hooks/useQueryParams";
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

describe('useQueryParams hook', () => {
    it('should return correct query param string', () => {
        const mockLocation = createMockLocation('?db=testDb&sub=testSub&table=testTable');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useQueryParams());
        const resultParams = renderResult.result.current;

        expect(resultParams.get('db')).toBe('testDb');
        expect(resultParams.get('sub')).toBe('testSub');
        expect(resultParams.get('table')).toBe('testTable');
    });

    it('should handle URL-encoded param values', () => {
        const mockLocation = createMockLocation('?param=test%20value%20with%20spaces');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useQueryParams());
        const resultParams = renderResult.result.current;

        expect(resultParams.get('param')).toBe('test value with spaces');
    });

    it('should handle repeated params as array with getAll method', () => {
        const mockLocation = createMockLocation('?param1=test1&param2=test2&param1=test3');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useQueryParams());
        const resultParams = renderResult.result.current;

        expect(resultParams.getAll('param1')).toEqual(['test1', 'test3']);
        expect(resultParams.get('param1')).toBe('test1');
        expect(resultParams.get('param2')).toBe('test2');
    });

    it('should handle empty param values', () => {
        const mockLocation = createMockLocation('?param1=&param2=value');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useQueryParams());
        const resultParams = renderResult.result.current;

        expect(resultParams.get('param1')).toBe('');
    });

    it('should handle non-existent params', () => {
        const mockLocation = createMockLocation('?param=value');

        (useLocation as jest.Mock).mockReturnValueOnce(mockLocation);

        const renderResult = renderHook(() => useQueryParams());
        const resultParams = renderResult.result.current;

        expect(resultParams.get('doesNotExist')).toBeNull();
    });
});

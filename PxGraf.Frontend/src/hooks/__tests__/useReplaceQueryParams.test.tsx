import { renderHook } from '@testing-library/react';
import useReplaceQueryParams from 'hooks/useReplaceQueryParams';
import * as useHierarchyParams from 'hooks/useHierarchyParams';
import * as navigationContext from 'contexts/navigationContext'

const mockNavigate = jest.fn();

jest.mock('react-router-dom', () => {
    return {
        __esModule: true,
        ...jest.requireActual('react-router-dom'),
        useNavigate: jest.fn(() => mockNavigate),
        useLocation: jest.fn(() => ({ pathname: '/', search: '', hash: '', state: null, key: 'default' })),
    }
});

const useParams = jest.spyOn(useHierarchyParams, 'default');
const useNavigationContext = jest.spyOn(navigationContext, 'useNavigationContext');

describe('useReplaceQueryParams hook', () => {
    beforeEach(() => {
        mockNavigate.mockClear();
    });

    it('should replace query params if context does not match current params', () => {
        // First render: mount guard skips navigate
        useParams.mockReturnValue(['testDb', 'testStat', 'testTable']);
        useNavigationContext.mockReturnValue({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });

        const { rerender } = renderHook(() => useReplaceQueryParams('/'));
        expect(mockNavigate).not.toHaveBeenCalled();

        // Second render: context changed (simulates user folder click)
        useNavigationContext.mockReturnValue({
            tablePath: ['testDb2', 'testStat2', 'testTable2'],
            setTablePath: jest.fn(),
        });
        rerender();

        expect(mockNavigate).toHaveBeenCalledWith({
            pathname: '/',
            search: '?tablePath=testDb2,testStat2,testTable2',
        }, { replace: true});
    });

    it('should replace query params if current params are empty but context is not', () => {
        // First render: mount guard skips navigate
        useParams.mockReturnValue([null, null, null]);
        useNavigationContext.mockReturnValue({
            tablePath: null,
            setTablePath: jest.fn(),
        });

        const { rerender } = renderHook(() => useReplaceQueryParams('/my-route'));
        expect(mockNavigate).not.toHaveBeenCalled();

        // Second render: context updated (simulates user interaction)
        useNavigationContext.mockReturnValue({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });
        rerender();

        expect(mockNavigate).toHaveBeenCalledWith({
            pathname: '/my-route',
            search: '?tablePath=testDb,testStat,testTable',
        }, { replace: true});
    });

    it('should not call navigate if all current params match context', () => {
        useParams.mockReturnValueOnce(['testDb', 'testStat', 'testTable']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should not call navigate when context tablePath is empty', () => {
        useParams.mockReturnValueOnce(['testDb', 'testStat']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: [],
            setTablePath: jest.fn(),
        });

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should not call navigate when context tablePath is null', () => {
        useParams.mockReturnValueOnce(['testDb']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: null,
            setTablePath: jest.fn(),
        });

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).not.toHaveBeenCalled();
    });
});

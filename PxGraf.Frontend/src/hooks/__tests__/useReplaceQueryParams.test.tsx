import { renderHook } from '@testing-library/react';
import useReplaceQueryParams from 'hooks/useReplaceQueryParams';
import * as useHierarchyParams from 'hooks/useHierarchyParams';
import * as navigationContext from 'contexts/navigationContext'
import { useNavigate } from 'react-router-dom';

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

    it('should replace query params and reload if context not matching current params', () => {
        useParams.mockReturnValueOnce(['testDb', 'testStat', 'testTable']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb2', 'testStat2', 'testTable2'],
            setTablePath: jest.fn(),
        });

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).toHaveBeenCalledWith({
            pathname: '/',
            search: '?tablePath=testDb2,testStat2,testTable2',
        }, { replace: true});
    });

    it('should replace query params and reload if current params are empty but context is not', () => {
        useParams.mockReturnValueOnce([null, null, null]);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });

        renderHook(() => useReplaceQueryParams('/my-route'));

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

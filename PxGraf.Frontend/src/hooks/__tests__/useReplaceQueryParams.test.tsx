import { renderHook } from '@testing-library/react';
import useReplaceQueryParams from 'hooks/useReplaceQueryParams';
import * as useHierarchyParams from 'hooks/useHierarchyParams';
import * as navigationContext from 'contexts/navigationContext'
import * as Router from 'react-router-dom';

jest.mock('react-router-dom', () => {
    return {
        __esModule: true,
        ...jest.requireActual('react-router-dom'),
    }
});

const useParams = jest.spyOn(useHierarchyParams, 'default');
const useNavigationContext = jest.spyOn(navigationContext, 'useNavigationContext');
const useNavigate = jest.spyOn(Router, 'useNavigate');

describe('useReplaceQueryParams hook', () => {
    it('should replace query params and reload if context not matching current params', () => {
        useParams.mockReturnValueOnce(['testDb', 'testStat', 'testTable']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb2', 'testStat2', 'testTable2'],
            setTablePath: jest.fn(),
        });
        const mockNavigate = jest.fn();
        useNavigate.mockReturnValueOnce(mockNavigate);

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).toHaveBeenNthCalledWith(1, {
            pathname: '/',
            search: '?tablePath=testDb2,testStat2,testTable2',
        }, { replace: true});

        expect(mockNavigate).toHaveBeenNthCalledWith(2, 0);
    });

    it('should replace query params and reload if current params are empty but context is not', () => {
        useParams.mockReturnValueOnce([null, null, null]);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });
        const mockNavigate = jest.fn();
        useNavigate.mockReturnValueOnce(mockNavigate);

        renderHook(() => useReplaceQueryParams('/my-route'));

        expect(mockNavigate).toHaveBeenNthCalledWith(1, {
            pathname: '/my-route',
            search: '?tablePath=testDb,testStat,testTable',
        }, { replace: true});

        expect(mockNavigate).toHaveBeenNthCalledWith(2, 0);
    });

    it('should not call navigate if all current params match context', () => {
        useParams.mockReturnValueOnce(['testDb', 'testStat', 'testTable']);
        useNavigationContext.mockReturnValueOnce({
            tablePath: ['testDb', 'testStat', 'testTable'],
            setTablePath: jest.fn(),
        });
        const mockNavigate = jest.fn();
        useNavigate.mockReturnValueOnce(mockNavigate);

        renderHook(() => useReplaceQueryParams('/'));

        expect(mockNavigate).not.toHaveBeenCalled();
    });
});

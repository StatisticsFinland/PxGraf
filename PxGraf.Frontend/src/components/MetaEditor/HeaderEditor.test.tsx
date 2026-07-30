import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import HeaderEditor from './HeaderEditor';
import { QueryContext } from '../../contexts/queryContext';
import { ICubeQuery } from '../../types/query';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { IEditorContentsResponse } from '../../types/editorContentsResponse';

const data: IEditorContentsResponse = {
    headerText:
    {
        'fi': 'foo',
        'sv': 'baz'
    }
} as unknown as IEditorContentsResponse;

const mockDefaultResponse: IEditorContentsResult = {
    isLoading: false,
    isError: false,
    data: data
};

const mockLang = 'fi';
const mockFunction = jest.fn();
const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: { 'fi': 'bar' },
    variableQueries: {}
}

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <QueryContext.Provider value={{
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
            }}>
                <HeaderEditor editorContentResponse={mockDefaultResponse} language={mockLang} style={{}} />
            </QueryContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    beforeEach(() => {
        jest.useFakeTimers();
        mockFunction.mockClear();
    });

    afterEach(() => {
        jest.useRealTimers();
    });

    it('debounces changes before updating the query', () => {
        render(
            <QueryContext.Provider value={{
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
            }}>
                <HeaderEditor editorContentResponse={mockDefaultResponse} language={mockLang} style={{}} />
            </QueryContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue(mockCubeQuery.chartHeaderEdit['fi']), { target: { value: 'editValue2' } });

        expect(mockFunction).not.toHaveBeenCalled();
        jest.advanceTimersByTime(1000);

        expect(mockFunction).toHaveBeenCalledTimes(1);
    });

    it('flushes a pending change when unmounted', () => {
        const { unmount } = render(
            <QueryContext.Provider value={{
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
            }}>
                <HeaderEditor editorContentResponse={mockDefaultResponse} language={mockLang} style={{}} />
            </QueryContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue(mockCubeQuery.chartHeaderEdit['fi']), { target: { value: 'editValue2' } });

        unmount();

        expect(mockFunction).toHaveBeenCalledTimes(1);
    });

    it('accumulates pending changes across languages', () => {
        const { rerender } = render(
            <QueryContext.Provider value={{
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
            }}>
                <HeaderEditor editorContentResponse={mockDefaultResponse} language="fi" style={{}} />
            </QueryContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'Finnish edit' } });

        rerender(
            <QueryContext.Provider value={{
                cubeQuery: mockCubeQuery,
                setCubeQuery: mockFunction,
                query: {},
                setQuery: jest.fn(),
            }}>
                <HeaderEditor editorContentResponse={mockDefaultResponse} language="sv" style={{}} />
            </QueryContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('baz'), { target: { value: 'Swedish edit' } });
        jest.advanceTimersByTime(1000);

        expect(mockFunction).toHaveBeenCalledTimes(1);
        const updateCubeQuery = mockFunction.mock.calls[0][0];
        expect(updateCubeQuery(mockCubeQuery).chartHeaderEdit).toEqual({
            fi: 'Finnish edit',
            sv: 'Swedish edit',
        });
    });
});

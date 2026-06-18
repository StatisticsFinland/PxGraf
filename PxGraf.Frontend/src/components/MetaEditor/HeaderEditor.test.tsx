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
        'fi': 'foo'
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
    it('Change event should fire when value has changed', () => {
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
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});

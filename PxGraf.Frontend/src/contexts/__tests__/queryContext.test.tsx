import React from 'react';
import { QueryContext, QueryProvider } from 'contexts/queryContext';
import { act, render, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { FilterType, ICubeQuery } from 'types/query';

const TestComponent = () => {
    const { cubeQuery, setCubeQuery, query, setQuery } = React.useContext(QueryContext);

    return (
        <>
            <div data-testid="cubeQuery">{JSON.stringify(cubeQuery)}</div>
            <div data-testid="query">{query ? JSON.stringify(query) : 'null'}</div>
            <button
                data-testid="setCubeQuery"
                onClick={() => setCubeQuery({ chartHeaderEdit: { fi: 'Testi' }, variableQueries: { dim1: { valueEdits: {} } } })}
            />
            <button
                data-testid="setQuery"
                onClick={() => setQuery({ dim1: { valueFilter: { type: FilterType.Item, query: ['val1'] }, selectable: false, virtualValueDefinitions: null } })}
            />
        </>
    );
};

describe('QueryContext', () => {
    beforeEach(() => {
        jest.useFakeTimers();
    });

    afterEach(() => {
        jest.useRealTimers();
    });

    it('should have default values initially', () => {
        const { getByTestId } = render(
            <QueryProvider><TestComponent /></QueryProvider>
        );
        expect(getByTestId('cubeQuery')).toHaveTextContent('{"variableQueries":{}}');
        expect(getByTestId('query')).toHaveTextContent('null');
    });

    it('should update query immediately', () => {
        const { getByTestId } = render(
            <QueryProvider><TestComponent /></QueryProvider>
        );

        act(() => {
            getByTestId('setQuery').click();
        });

        expect(getByTestId('query')).toHaveTextContent('"dim1"');
        expect(getByTestId('query')).toHaveTextContent('"item"');
    });

    it('should debounce cubeQuery updates', async () => {
        const { getByTestId } = render(
            <QueryProvider><TestComponent /></QueryProvider>
        );

        act(() => {
            getByTestId('setCubeQuery').click();
        });

        // Should still show the initial value before debounce fires
        expect(getByTestId('cubeQuery')).toHaveTextContent('{"variableQueries":{}}');

        // Advance timers past the 1000ms debounce
        act(() => {
            jest.advanceTimersByTime(1000);
        });

        await waitFor(() => {
            expect(getByTestId('cubeQuery')).toHaveTextContent('"dim1"');
            expect(getByTestId('cubeQuery')).toHaveTextContent('"Testi"');
        });
    });

    it('should only apply the last cubeQuery when called rapidly', async () => {
        let setCubeQueryRef: (q: ICubeQuery) => void;

        const CapturingComponent = () => {
            const { cubeQuery, setCubeQuery } = React.useContext(QueryContext);
            React.useLayoutEffect(() => {
                setCubeQueryRef = setCubeQuery;
            }, [setCubeQuery]);
            return <div data-testid="cubeQuery">{JSON.stringify(cubeQuery)}</div>;
        };

        const { getByTestId } = render(
            <QueryProvider><CapturingComponent /></QueryProvider>
        );

        // Fire multiple updates within the debounce window
        act(() => {
            setCubeQueryRef({ variableQueries: { first: { valueEdits: {} } } });
        });
        act(() => {
            setCubeQueryRef({ variableQueries: { second: { valueEdits: {} } } });
        });
        act(() => {
            setCubeQueryRef({ variableQueries: { third: { valueEdits: {} } } });
        });

        act(() => {
            jest.advanceTimersByTime(1000);
        });

        await waitFor(() => {
            expect(getByTestId('cubeQuery')).toHaveTextContent('"third"');
            expect(getByTestId('cubeQuery')).not.toHaveTextContent('"first"');
            expect(getByTestId('cubeQuery')).not.toHaveTextContent('"second"');
        });
    });
});

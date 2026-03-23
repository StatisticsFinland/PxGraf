import { debounce } from 'lodash';
import * as React from 'react';
import { ICubeQuery, Query } from 'types/query';

interface IQueryContext {
    cubeQuery: ICubeQuery;
    setCubeQuery: (newQuery: ICubeQuery) => void;
    query: Query | null;
    setQuery: React.Dispatch<React.SetStateAction<Query | null>>;
}

export const QueryContext = React.createContext<IQueryContext>({
    cubeQuery: { variableQueries: {} },
    setCubeQuery: () => { /* no base implementation */ },
    query: null,
    setQuery: () => { /* no base implementation */ },
});

export const QueryProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [cubeQuery, setCubeQueryState] = React.useState<ICubeQuery>({ variableQueries: {} });
    const [query, setQuery] = React.useState<Query | null>(null);

    const debouncedCubeQuery = React.useMemo(() => {
        return debounce((newQuery: ICubeQuery) => {
            setCubeQueryState(newQuery);
        }, 1000);
    }, []);

    const setCubeQuery = React.useCallback((newQuery: ICubeQuery) => {
        debouncedCubeQuery(newQuery);
    }, [debouncedCubeQuery]);

    React.useEffect(() => {
        return () => {
            debouncedCubeQuery.cancel();
        };
    }, [debouncedCubeQuery]);

    const contextValue = React.useMemo(() => ({
        cubeQuery, setCubeQuery,
        query, setQuery,
    }), [cubeQuery, setCubeQuery, query, setQuery]);

    return (
        <QueryContext.Provider value={contextValue}>
            {children}
        </QueryContext.Provider>
    );
};

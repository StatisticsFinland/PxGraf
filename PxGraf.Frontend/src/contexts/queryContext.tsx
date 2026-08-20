import * as React from 'react';
import { ICubeQuery, Query } from 'types/query';

interface IQueryContext {
    cubeQuery: ICubeQuery;
    setCubeQuery: React.Dispatch<React.SetStateAction<ICubeQuery>>;
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

    const contextValue = React.useMemo(() => ({
        cubeQuery, setCubeQuery: setCubeQueryState,
        query, setQuery,
    }), [cubeQuery, query]);

    return (
        <QueryContext.Provider value={contextValue}>
            {children}
        </QueryContext.Provider>
    );
};

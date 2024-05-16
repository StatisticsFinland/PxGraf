import * as React from "react";

interface INavigationContextData {
    tablePath: string[] | null,
}

interface INavigationContextMethods {
    setTablePath: React.Dispatch<React.SetStateAction<string[] | null>>,
}

type TNavigationContextValue = INavigationContextData & INavigationContextMethods;

interface INavigationProviderProps {
    children: React.ReactNode;
}

const NavigationContext = React.createContext<(TNavigationContextValue)>(null);

export const NavigationProvider: React.FC<INavigationProviderProps> = ({ children }) => {
    const [tablePath, setTablePath] = React.useState<string[] | null>(null);

    const contextValue: TNavigationContextValue = React.useMemo(() => ({
        tablePath,
        setTablePath,
    }), [tablePath]);

    return <NavigationContext.Provider value={contextValue}>{children}</NavigationContext.Provider>
}

export const useNavigationContext = (): TNavigationContextValue => {
    return React.useContext(NavigationContext);
}

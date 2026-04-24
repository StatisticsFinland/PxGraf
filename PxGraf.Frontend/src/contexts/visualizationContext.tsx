import * as React from 'react';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';

interface IVisualizationContext {
    selectedVisualizationUserInput: VisualizationType | null;
    setSelectedVisualizationUserInput: React.Dispatch<React.SetStateAction<VisualizationType | null>>;
    visualizationSettingsUserInput: IVisualizationSettings | null;
    setVisualizationSettingsUserInput: React.Dispatch<React.SetStateAction<IVisualizationSettings | null>>;
    defaultSelectables: { [key: string]: string[] } | null;
    setDefaultSelectables: React.Dispatch<React.SetStateAction<{ [key: string]: string[] } | null>>;
}

export const VisualizationContext = React.createContext<IVisualizationContext>({
    selectedVisualizationUserInput: null,
    setSelectedVisualizationUserInput: () => { /* no base implementation */ },
    visualizationSettingsUserInput: null,
    setVisualizationSettingsUserInput: () => { /* no base implementation */ },
    defaultSelectables: null,
    setDefaultSelectables: () => { /* no base implementation */ },
});

export const VisualizationProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [selectedVisualizationUserInput, setSelectedVisualizationUserInput] = React.useState<VisualizationType | null>(null);
    const [visualizationSettingsUserInput, setVisualizationSettingsUserInput] = React.useState<IVisualizationSettings | null>(null);
    const [defaultSelectables, setDefaultSelectables] = React.useState<{ [key: string]: string[] } | null>(null);

    const contextValue = React.useMemo(() => ({
        selectedVisualizationUserInput, setSelectedVisualizationUserInput,
        visualizationSettingsUserInput, setVisualizationSettingsUserInput,
        defaultSelectables, setDefaultSelectables,
    }), [selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables]);

    return (
        <VisualizationContext.Provider value={contextValue}>
            {children}
        </VisualizationContext.Provider>
    );
};

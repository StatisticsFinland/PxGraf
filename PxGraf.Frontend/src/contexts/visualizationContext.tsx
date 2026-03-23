import * as React from 'react';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';

interface IVisualizationContext {
    selectedVisualizationUserInput: VisualizationType;
    setSelectedVisualizationUserInput: React.Dispatch<React.SetStateAction<VisualizationType>>;
    visualizationSettingsUserInput: IVisualizationSettings;
    setVisualizationSettingsUserInput: React.Dispatch<React.SetStateAction<IVisualizationSettings>>;
    defaultSelectables: { [key: string]: string[] };
    setDefaultSelectables: React.Dispatch<React.SetStateAction<{ [key: string]: string[] }>>;
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
    const [selectedVisualizationUserInput, setSelectedVisualizationUserInput] = React.useState(null);
    const [visualizationSettingsUserInput, setVisualizationSettingsUserInput] = React.useState(null);
    const [defaultSelectables, setDefaultSelectables] = React.useState(null);

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

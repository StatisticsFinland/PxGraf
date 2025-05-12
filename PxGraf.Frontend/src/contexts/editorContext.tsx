/* istanbul ignore file */

import { debounce } from 'lodash';
import * as React from 'react';
import { ICubeQuery, Query } from 'types/query';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';

/**
 * Interface for editor context properties
 * @property {ICubeQuery} cubeQuery - Query object that contains the header and dimension queries
 * @property {React.Dispatch<ICubeQuery>} setCubeQuery - Function to set the cube query
 * @property {Query} query - Query object that contains the dimension queries
 * @property {React.Dispatch<Query>} setQuery - Function to set the query
 * @property {boolean} saveDialogOpen - Flag to indicate if the save dialog is open
 * @property {React.Dispatch<boolean>} setSaveDialogOpen - Function to set the save dialog open flag
 * @property {VisualizationType} selectedVisualizationUserInput - The selected visualization type
 * @property {React.Dispatch<VisualizationType>} setSelectedVisualizationUserInput - Function to set the selected visualization type
 * @property {IVisualizationSettings} visualizationSettingsUserInput - The visualization settings
 * @property {React.Dispatch<IVisualizationSettings>} setVisualizationSettingsUserInput - Function to set the visualization settings
 * @property {{ [key: string]: string[] }} defaultSelectables - The default selectables for each dimension
 * @property {React.Dispatch<{ [key: string]: string[] }>} setDefaultSelectables - Function to set the default selectables
 */
interface IEditorContext {
    cubeQuery: ICubeQuery;
    setCubeQuery: React.Dispatch<ICubeQuery>;
    query: Query;
    setQuery: React.Dispatch<Query>;
    saveDialogOpen: boolean;
    setSaveDialogOpen: React.Dispatch<boolean>;
    selectedVisualizationUserInput: VisualizationType;
    setSelectedVisualizationUserInput: React.Dispatch<VisualizationType>;
    visualizationSettingsUserInput: IVisualizationSettings;
    setVisualizationSettingsUserInput: React.Dispatch<IVisualizationSettings>;
    defaultSelectables: { [key: string]: string[] };
    setDefaultSelectables: React.Dispatch<{ [key: string]: string[] }>
}

/**
 * Context required for the editor
 */
export const EditorContext = React.createContext<IEditorContext>({
    cubeQuery: null,
    setCubeQuery: () => { /* no base implementation */ },
    query: null,
    setQuery: () => { /* no base implementation */ },
    saveDialogOpen: false,
    setSaveDialogOpen: () => { /* no base implementation */ },
    selectedVisualizationUserInput: null,
    setSelectedVisualizationUserInput: () => { /* no base implementation */ },
    visualizationSettingsUserInput: null,
    setVisualizationSettingsUserInput: () => { /* no base implementation */ },
    defaultSelectables: null,
    setDefaultSelectables: () => { /* no base implementation */ }
});

/**
 * Provider for the editor context
 * @param children - The child components
 */
export const EditorProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [cubeQuery, setCubeQueryState] = React.useState({ variableQueries: {} });
    const [query, setQuery] = React.useState(null);
    const [saveDialogOpen, setSaveDialogOpen] = React.useState(false);
    const [selectedVisualizationUserInput, setSelectedVisualizationUserInput] = React.useState(null);
    const [visualizationSettingsUserInput, setVisualizationSettingsUserInput] = React.useState(null);
    const [defaultSelectables, setDefaultSelectables] = React.useState(null);

    const debouncedSetCubeQuery = React.useMemo(() => {
        return debounce((newQuery: ICubeQuery) => {
            setCubeQueryState(newQuery);
        }, 1000);
    }, []);

    const setCubeQuery = React.useCallback((newQuery: ICubeQuery) => {
        debouncedSetCubeQuery(newQuery);
    }, [debouncedSetCubeQuery]);

    React.useEffect(() => {
        return () => {
            debouncedSetCubeQuery.cancel();
        };
    }, [debouncedSetCubeQuery]);

    const contextValue = React.useMemo(() => {
        return {
            cubeQuery, setCubeQuery,
            query, setQuery,
            saveDialogOpen, setSaveDialogOpen,
            selectedVisualizationUserInput, setSelectedVisualizationUserInput,
            visualizationSettingsUserInput, setVisualizationSettingsUserInput,
            defaultSelectables, setDefaultSelectables
        }
    }, [
        cubeQuery, query, saveDialogOpen, selectedVisualizationUserInput,
        visualizationSettingsUserInput, defaultSelectables
    ]);

    return (
        <EditorContext.Provider value={contextValue}>
            {children}
        </EditorContext.Provider>
    );
}
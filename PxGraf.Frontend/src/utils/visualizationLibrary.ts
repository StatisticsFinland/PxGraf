export type VisualizationLibrary = 'jsonstatgraphs' | 'pxvisualizer';

export const DEFAULT_VISUALIZATION_LIBRARY: VisualizationLibrary = 'jsonstatgraphs';
const storageKey = 'pxgraf.visualizationLibrary';

export const isVisualizationLibrary = (value: unknown): value is VisualizationLibrary =>
    value === 'jsonstatgraphs' || value === 'pxvisualizer';

export const getVisualizationLibraryPreference = (): VisualizationLibrary => {
    try {
        const value = window.localStorage.getItem(storageKey);
        return isVisualizationLibrary(value) ? value : DEFAULT_VISUALIZATION_LIBRARY;
    } catch {
        return DEFAULT_VISUALIZATION_LIBRARY;
    }
};

export const setVisualizationLibraryPreference = (library: VisualizationLibrary): void => {
    try {
        window.localStorage.setItem(storageKey, library);
    } catch {
        // Storage may be unavailable in restricted browser contexts.
    }
};
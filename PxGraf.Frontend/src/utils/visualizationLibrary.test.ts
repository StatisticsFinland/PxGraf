import {
    DEFAULT_VISUALIZATION_LIBRARY,
    getVisualizationLibraryPreference,
    setVisualizationLibraryPreference,
} from './visualizationLibrary';

describe('visualization library preference', () => {
    beforeEach(() => window.localStorage.clear());

    it('defaults to JSON-stat when no preference is stored', () => {
        expect(getVisualizationLibraryPreference()).toBe(DEFAULT_VISUALIZATION_LIBRARY);
    });

    it('persists and restores the selected library', () => {
        setVisualizationLibraryPreference('pxvisualizer');

        expect(getVisualizationLibraryPreference()).toBe('pxvisualizer');
    });

    it('falls back to JSON-stat for an invalid stored value', () => {
        window.localStorage.setItem('pxgraf.visualizationLibrary', 'unknown');

        expect(getVisualizationLibraryPreference()).toBe(DEFAULT_VISUALIZATION_LIBRARY);
    });
});
/* istanbul ignore file */

import ApiClient from "api/client";
import { IQueryVisualizationResponse } from "@statisticsfinland/pxvisualizer";
import { useQuery } from "react-query";
import { ICubeQuery, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";

import { buildCubeQuery, defaultQueryOptions, useDebounceState } from "utils/ApiHelpers";

/**
 * Interface for a visualization result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IQueryVisualizationResponse} data - The visualization result represented as PxVisualizer @see {@link IQueryVisualizationResponse}.
 */
export interface IVisualizationResult {
    isLoading: boolean;
    isError: boolean;
    data: IQueryVisualizationResponse;
}

const fetchVisualization = async (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    language: string,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings
): Promise<IQueryVisualizationResponse> => {

    const client = new ApiClient();

    const requestBody = JSON.stringify({
        query: buildCubeQuery(query, metaEdits, idStack),
        language: language,
        visualizationSettings: {
            ...visualizationSettings,
            selectedVisualization: selectedVisualization,
        }
    });

    const url = 'creation/visualization';
    return await client.postAsync(url, requestBody);
}

export const useVisualizationQuery = (
    idStack: string[],
    query: Query,
    cubeQuery: ICubeQuery,
    language: string,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings
): IVisualizationResult => {
    [idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings] = useDebounceState(1000, idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings);
    const queryKey = ['chart', ...idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings];

    const checkSettingsValidity = (selectedVisualization: string, settings: IVisualizationSettings) => {
        if (settings == null || selectedVisualization == null) return false;
        const requireSorting = ["horizontalBarChart", "groupHorizontalBarChart", "stackedHorizontalBarChart", "percentHorizontalBarChart", "pieChart"];
        if (requireSorting.includes(selectedVisualization) && settings.sorting == null) return false;
    }

    return useQuery(
        queryKey,
        () => fetchVisualization(idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings),
        {
            ...defaultQueryOptions,
            enabled:
                query != null &&
                checkSettingsValidity(selectedVisualization, visualizationSettings),
            keepPreviousData: true,
        }
    );
}
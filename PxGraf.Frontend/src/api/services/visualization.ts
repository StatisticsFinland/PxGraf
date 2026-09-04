/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "@tanstack/react-query";
import { ICubeQuery, Query } from "types/query";
import { IJsonStatDataset } from "types/jsonStatChart";
import { IVisualizationSettings } from "types/visualizationSettings";

import { buildCubeQuery, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for a visualization result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IJsonStatDataset} data - The visualization result represented as a JSON-stat 2.0 dataset.
 */
export interface IVisualizationResult {
    isLoading: boolean;
    isFetching: boolean;
    isError: boolean;
    data: IJsonStatDataset;
}

const fetchVisualization = async (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    language: string,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings
): Promise<IJsonStatDataset> => {

    const client = new ApiClient();

    const requestBody = JSON.stringify({
        query: buildCubeQuery(query, metaEdits, idStack),
        language: language,
        visualizationSettings: {
            ...visualizationSettings,
            selectedVisualization: selectedVisualization,
        }
    });

    const url = `creation/jsonstat?lang=${encodeURIComponent(language)}`;
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
    const queryKey = ['chart', ...idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings];

    const checkSettingsValidity = (selectedVisualization: string, settings: IVisualizationSettings) => {
        if (settings == null || selectedVisualization == null) return false;
        const requireSorting = ["horizontalBarChart", "groupHorizontalBarChart", "stackedHorizontalBarChart", "percentHorizontalBarChart", "pieChart"];
        if (requireSorting.includes(selectedVisualization) && settings.sorting == null) return false;
        return true;
    }

    return useQuery({
        queryKey,
        queryFn: () => fetchVisualization(idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings),
        ...defaultQueryOptions,
        enabled:
            query != null &&
            checkSettingsValidity(selectedVisualization, visualizationSettings),
        placeholderData: (previousData) => previousData,
    });
}
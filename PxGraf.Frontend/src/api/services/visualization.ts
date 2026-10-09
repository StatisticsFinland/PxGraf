/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "@tanstack/react-query";
import { ICubeQuery, Query } from "types/query";
import { IJsonStatDataset } from "types/jsonStatChart";
import { IVisualizationSettings } from "types/visualizationSettings";
import { IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { DEFAULT_VISUALIZATION_LIBRARY, VisualizationLibrary } from 'utils/visualizationLibrary';

import { buildCubeQuery, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for a visualization result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IJsonStatDataset | IQueryVisualizationResponse} data - Visualization data returned by the selected chart library endpoint.
 */
export interface IVisualizationResult {
    isLoading: boolean;
    isFetching: boolean;
    isError: boolean;
    data: IJsonStatDataset | IQueryVisualizationResponse;
}

export type VisualizationData = IJsonStatDataset | IQueryVisualizationResponse;

const fetchVisualization = async (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    language: string,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    visualizationLibrary: VisualizationLibrary
): Promise<VisualizationData> => {

    const client = new ApiClient();

    const requestBody = JSON.stringify({
        query: buildCubeQuery(query, metaEdits, idStack),
        language: language,
        visualizationSettings: {
            ...visualizationSettings,
            selectedVisualization: selectedVisualization,
        }
    });

    const url = visualizationLibrary === 'jsonstatgraphs'
        ? `creation/jsonstat?lang=${encodeURIComponent(language)}`
        : 'creation/visualization';
    return await client.postAsync(url, requestBody);
}

export const useVisualizationQuery = (
    idStack: string[],
    query: Query,
    cubeQuery: ICubeQuery,
    language: string,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    visualizationLibrary: VisualizationLibrary = DEFAULT_VISUALIZATION_LIBRARY
): IVisualizationResult => {
    const queryKey = ['chart', visualizationLibrary, ...idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings];

    const checkSettingsValidity = (selectedVisualization: string, settings: IVisualizationSettings) => {
        if (settings == null || selectedVisualization == null) return false;
        const requireSorting = ["horizontalBarChart", "groupHorizontalBarChart", "stackedHorizontalBarChart", "percentHorizontalBarChart", "pieChart"];
        if (requireSorting.includes(selectedVisualization) && settings.sorting == null) return false;
        return true;
    }

    return useQuery({
        queryKey,
        queryFn: () => fetchVisualization(idStack, query, cubeQuery, language, selectedVisualization, visualizationSettings, visualizationLibrary),
        ...defaultQueryOptions,
        placeholderData: (previousData, previousQuery) => previousQuery?.queryKey[1] === visualizationLibrary ? previousData : undefined,
        enabled:
            query != null &&
            checkSettingsValidity(selectedVisualization, visualizationSettings),
    });
}
/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { ICubeQuery, Query } from "types/query";
import { buildCubeQuery, defaultQueryOptions, useDebounceState } from "utils/ApiHelpers";

/**
 * Interface for valid visualizations result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {string[]} data - The valid visualizations listed by name.
 */
export interface IValidVisualizationsResult {
    isLoading: boolean;
    isError: boolean;
    data: string[];
}

const fetchValidVisualizations = async (idStack: string[], query: Query, metaEdits: ICubeQuery): Promise<string[]> => {

    const client = new ApiClient();
    const url = 'creation/valid-visualizations';
    const requestBody = JSON.stringify(buildCubeQuery(query, metaEdits, idStack));
    return await client.postAsync(url, requestBody);
}

export const useValidVisualizationsQuery = (idStack: string[], query: Query, cubeQuery: ICubeQuery): IValidVisualizationsResult => {
    //DEPRECATED, USE QUERY-INFO
    [idStack, query, cubeQuery] = useDebounceState(1000, idStack, query, cubeQuery);
    const queryKey = ['visualizations', ...idStack, query, cubeQuery];

    return useQuery(
        queryKey,
        () => fetchValidVisualizations(idStack, query, cubeQuery),
        {
            ...defaultQueryOptions,
            enabled: query != null,
            keepPreviousData: true,
        }
    );
}
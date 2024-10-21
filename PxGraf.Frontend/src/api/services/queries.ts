/* istanbul ignore file */

import ApiClient from "api/client";
import { useMutation, useQuery } from "react-query";
import { ICubeQuery, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import { buildCubeQuery, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Response object for saving a query.
 * @property {string} id - The id of the saved query.
 */
export interface ISaveQueryResponse {
    id: string;
}

/**
 * Response object for fetching a saved query.
 * @property query - The query object consisting of the table reference and variable queries @see {@link Query}.
 * @property {IVisualizationSettings} settings - The visualization settings for the query.
 */
export interface IFetchSavedQueryResponse {
    query: {
        tableReference: {
            name: string;
            hierarchy: string[];
        };
        variableQueries: Query;
    } & ICubeQuery;
    settings: IVisualizationSettings;
}

/**
 * Interface for save query result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {boolean} isSuccess - Flag to indicate if the request was successful.
 * @property {ISaveQueryResponse} data - The save query response data.
 * @property {function} mutate - Function to mutate the state of the query.
 */
export interface ISaveQueryResult {
    isLoading: boolean;
    isError: boolean;
    isSuccess: boolean;
    data: ISaveQueryResponse;
    mutate: (property: boolean) => void;
}

/**
 * Interface for fetch saved query result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {boolean} isSuccess - Flag to indicate if the request was successful.
 * @property {IFetchSavedQueryResponse} data - The fetch saved query response data.
 */
export interface IFetchSavedQueryResult {
    isLoading: boolean;
    isError: boolean;
    isSuccess: boolean;
    data: IFetchSavedQueryResponse;
}

const sendSaveRequest = async (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    archive: boolean | void
): Promise<ISaveQueryResponse> => {

    const client = new ApiClient();

    const requestBody = JSON.stringify({
        query: buildCubeQuery(query, metaEdits, idStack),
        settings: {
            ...visualizationSettings,
            selectedVisualization: selectedVisualization,
        }
    });
    const url = archive ? 'sq/archive' : 'sq/save';
    return await client.postAsync(url, requestBody);
}

export const fetchSavedQuery = async (queryId: string): Promise<IFetchSavedQueryResponse> => {
    const client = new ApiClient();
    const url = 'sq/' + queryId;
    const response = await client.getAsync(url);

    // Check for VariableQueries and assign to variableQueries
    if (!response.query?.variableQueries && response.query?.VariableQueries) {
        response.query.variableQueries = response.query.VariableQueries;
    }

    return response as IFetchSavedQueryResponse;
};

export const useFetchSavedQuery = (queryId: string): IFetchSavedQueryResult => {
    return useQuery(
        [queryId],
        () => fetchSavedQuery(queryId),
        defaultQueryOptions
    );
};

export const useSaveMutation = (idStack: string[], query: Query, metaEdits: ICubeQuery, selectedVisualization: string, visualizationSettings: IVisualizationSettings): ISaveQueryResult => {
    return useMutation(
        (archive) => sendSaveRequest(idStack, query, metaEdits, selectedVisualization, visualizationSettings, archive),
        { retry: false }
    );
}
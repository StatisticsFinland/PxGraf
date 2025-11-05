/* istanbul ignore file */
import ApiClient from "api/client";
import { useMutation, useQuery, UseMutationResult } from "react-query";
import { ICubeQuery, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import { buildCubeQuery, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Response object for saving a query.
 * @property {string} id - The id of the saved query.
 * @property {EQueryPublicationStatus} publicationStatus - The publication status of the saved query.
 */
export interface ISaveQueryResponse {
    id: string;
    publicationStatus: EQueryPublicationStatus;
}

/**
 * Response object for fetching a saved query.
 * @property query - The query object consisting of the table reference and dimension queries @see {@link Query}.
 * @property {IVisualizationSettings} settings - The visualization settings for the query.
 * @property {string} id - The id of the retreived query.
 * @property {boolean} draft - Indicates if the query is a draft and can be overwritten.
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
    id: string;
    draft: boolean;
}

/**
 * Interface for save query mutation payload
 * @property {boolean} archive - Flag to indicate if the query should be archived.
 * @property {boolean} isDraft - Flag to indicate if the query is a draft and can be overwritten.
 */
export interface ISaveQueryMutationParams {
    archive: boolean;
    isDraft: boolean;
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
    mutate: (params: ISaveQueryMutationParams) => void;
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

/**
 * Enum for query publication status
 * @property {string} Unpublished - The query is unpublished.
 * @property {string} Success - The query was published successfully.
 * @property {string} Failed - The query publication failed.
 */
export enum EQueryPublicationStatus {
    Unpublished,
    Success,
    Failed
}

const sendSaveRequest = async (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    params: ISaveQueryMutationParams,
    id: string
): Promise<ISaveQueryResponse> => {

    const client = new ApiClient();

    const requestBody = JSON.stringify({
        query: buildCubeQuery(query, metaEdits, idStack),
        settings: {
            ...visualizationSettings,
            selectedVisualization: selectedVisualization,
        },
        id: id,
        draft: params.isDraft
    });
    const url = params.archive ? 'sq/archive' : 'sq/save';
    return await client.postAsync(url, requestBody);
}

export const fetchSavedQuery = async (queryId: string): Promise<IFetchSavedQueryResponse> => {
    const client = new ApiClient();
    const url = 'sq/' + queryId;
    const response = await client.getAsync(url);

    return response as IFetchSavedQueryResponse;
};

export const useFetchSavedQuery = (queryId: string): IFetchSavedQueryResult => {
    return useQuery(
        [queryId],
        () => fetchSavedQuery(queryId),
        defaultQueryOptions
    );
};

export const useSaveMutation = (
    idStack: string[],
    query: Query,
    metaEdits: ICubeQuery,
    selectedVisualization: string,
    visualizationSettings: IVisualizationSettings,
    id: string
): UseMutationResult<ISaveQueryResponse, unknown, ISaveQueryMutationParams> => {
    return useMutation(
        (params: ISaveQueryMutationParams) => sendSaveRequest(
            idStack,
            query,
            metaEdits,
            selectedVisualization,
            visualizationSettings,
            params,
            id
        ),
        { retry: false }
    );
}
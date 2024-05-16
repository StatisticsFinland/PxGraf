/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { ICubeQuery, IQueryInfo, Query } from "types/query";
import { buildCubeQuery, defaultQueryOptions, useDebounceState } from "utils/ApiHelpers";

/**
 * Interface for query info result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IQueryInfo} data - The query info data reprented by @see {@link IQueryInfo}.
 */
export interface IQueryInfoResult {
    isLoading: boolean;
    isError: boolean;
    data: IQueryInfo;
}

const fetchQueryInfo = async (idStack: string[], query: Query, metaEdits: ICubeQuery): Promise<IQueryInfo> => {

    const client = new ApiClient();
    const url = 'creation/query-info';
    const requestBody = JSON.stringify(buildCubeQuery(query, metaEdits, idStack));
    return await client.postAsync(url, requestBody);
}

export const useQueryInfoQuery = (idStack: string[], query: Query, cubeQuery: ICubeQuery): IQueryInfoResult => {
    [idStack, query, cubeQuery] = useDebounceState(1000, idStack, query, cubeQuery);
    const queryKey = ['query-info', ...idStack, query, cubeQuery];

    return useQuery(
        queryKey,
        () => fetchQueryInfo(idStack, query, cubeQuery),
        {
            ...defaultQueryOptions,
            enabled: query != null,
            keepPreviousData: true,
        }
    );
}
/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { ICubeMeta } from "types/cubeMeta";
import { defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for cube metadata result. Cube is a multi-dimensional result of Px file data filtered by a query.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {ICubeMeta} data - The cube meta data as an @see {@link ICubeMeta} object.
 */
export interface ICubeMetaResult {
    isLoading: boolean;
    isError: boolean;
    data: ICubeMeta;
}

const fetchCubeMeta = async(idStack: string[]): Promise<ICubeMeta> => {
    const client = new ApiClient();
    const url = `creation/cube-meta/${idStack.join('/')}`;

    return await client.getAsync(url);
}

export const useCubeMetaQuery = (idStack: string[]): ICubeMetaResult => {
    return useQuery(
        ['cube-meta', idStack],
        () => fetchCubeMeta(idStack),
        defaultQueryOptions
    );
}
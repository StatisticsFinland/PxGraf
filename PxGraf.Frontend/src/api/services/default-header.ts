/* istanbul ignore file */

import ApiClient from "api/client";
import { merge } from "lodash";
import { useQuery } from "react-query";
import { MultiLanguageString } from "types/multiLanguageString";
import { Query } from "types/query";
import { buildTableReference, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for the chart's header result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred while retrieving the header.
 * @property {MultiLanguageString} data - The header data represented as a multi-language string.
 */
export interface IHeaderResult {
    isLoading: boolean;
    isError: boolean;
    data: MultiLanguageString;
}

const fetchDefaultHeader = async(idStack: string[], query: Query): Promise<MultiLanguageString> => {
    const client = new ApiClient();
    const requestBody = JSON.stringify(
        merge(
            { tableReference: buildTableReference(idStack) },
            { variableQueries: query }
        )
    );
  
    const url = 'creation/default-header';
    return await client.postAsync(url, requestBody);
}

export const useDefaultHeaderQuery = (idStack: string[], query: Query): IHeaderResult => {
    return useQuery(
        ['default-header', ...idStack, query],
        () => fetchDefaultHeader(idStack, query),
        {
            ...defaultQueryOptions,
            enabled: query != null,
        }
    );
}
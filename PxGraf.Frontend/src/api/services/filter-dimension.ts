/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { IValueFilter, Query } from "types/query";
import { buildTableReference, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for dimension filter result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {{ [key: string]: string[] }} data - The dimension filter data represented by a dictionary. Key is the dimension code and value is a list of strings that represent value codes for that dimension.
 */
export interface IFilterDimensionResult {
    isLoading: boolean;
    isError: boolean;
    data: {[key: string]: string[]}
}

const fetchResolveDimensionFilter = async (idStack: string[], dimensionFilters: { [key: string]: IValueFilter }): Promise<{ [key: string]: string[] }> => {
    const client = new ApiClient();
    const requestBody = JSON.stringify(
        {
            tableReference: buildTableReference(idStack),
            filters: dimensionFilters,
        }
    );

    const url = 'creation/filter-dimension'
    return await client.postAsync(url, requestBody);
}

export const useResolveDimensionFiltersQuery = (idStack: string[], query: Query): IFilterDimensionResult => {
    const dimQueries = Object.entries(query ?? {});
    const dimFilters =  Object.fromEntries(dimQueries.map(([dimensionCode, dimensionQuery]) => {
        return [dimensionCode, dimensionQuery.valueFilter]
    }));

    return useQuery(
        ['filter-dimension', ...idStack, query],
        () => fetchResolveDimensionFilter(idStack, dimFilters),
        defaultQueryOptions
    );
}
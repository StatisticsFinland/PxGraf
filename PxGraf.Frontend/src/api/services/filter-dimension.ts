/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { IValueFilter, Query } from "types/query";
import { buildTableReference, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for variable filter result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {{ [key: string]: string[] }} data - The variable filter data represented by a dictionary. Key is the variable code and value is a list of strings that represent value codes for that variable.
 */
export interface IFilterVariableResult {
    isLoading: boolean;
    isError: boolean;
    data: {[key: string]: string[]}
}

const fetchResolveVariableFilter = async (idStack: string[], variableFilters: { [key: string]: IValueFilter }): Promise<{ [key: string]: string[] }> => {
    const client = new ApiClient();
    const requestBody = JSON.stringify(
        {
            tableReference: buildTableReference(idStack),
            filters: variableFilters,
        }
    );

    const url = 'creation/filter-dimension'
    return await client.postAsync(url, requestBody);
}

export const useResolveVariableFiltersQuery = (idStack: string[], query: Query): IFilterVariableResult => {
    const varQueries = Object.entries(query ?? {});
    const varFilters =  Object.fromEntries(varQueries.map(([variableCode, variableQuery]) => {
        return [variableCode, variableQuery.valueFilter]
    }));

    return useQuery(
        ['filter-dimension', ...idStack, query],
        () => fetchResolveVariableFilter(idStack, varFilters),
        defaultQueryOptions
    );
}
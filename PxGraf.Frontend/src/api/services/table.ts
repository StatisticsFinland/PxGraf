/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "@tanstack/react-query";
import { defaultQueryOptions } from "utils/ApiHelpers";
import { IDatabaseGroupContents } from "types/tableListItems";

/**
 * Interface for a table result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IDatabaseGroupContents[]} data - The table list response data.
 */
export interface ITableResult {
    isLoading: boolean;
    isError: boolean;
    data: IDatabaseGroupContents;
}

export const fetchTable = async (idStack: string[]): Promise<IDatabaseGroupContents> => {
    const client = new ApiClient();
    const path = idStack.join("/");
    const url = "creation/data-bases/" + path;

    return await client.getAsync(url);
};

export const tableQueryKey = (idStack: string[]) => ["table", idStack] as const;

export const tableQueryOptions = (idStack: string[]) => ({
    queryKey: tableQueryKey(idStack),
    queryFn: () => fetchTable(idStack),
    ...defaultQueryOptions,
});

export const useTableQuery = (idStack: string[]): ITableResult => {
    const result = useQuery(tableQueryOptions(idStack));

    return result;
};
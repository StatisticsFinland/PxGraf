/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { defaultQueryOptions } from "utils/ApiHelpers";
import { MultiLanguageString } from "../../types/multiLanguageString";

/**
 * Interface for a table list response.
 * @property {string} id - The id of the table.
 * @property {string} type - The type of the table where 't' is a table and 'l' is a list item.
 * @property {string} updated - The last updated date of the table.
 * @property {MultiLanguageString} text - The description of the table as a multi-language string.
 * @property {string[]} languages - Available languages of the table.
 */
export interface IDatabaseGroupContents {
    headers: IDatabaseGroupHeader[];
    files: IDatabaseTable[];
}

export interface IDatabaseGroupHeader {
    code: string;
    name: MultiLanguageString;
    languages: string[];
}

export interface IDatabaseTable extends IDatabaseGroupHeader {
    lastUpdated: string;
}

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

const fetchTable = async (idStack: string[]): Promise<IDatabaseGroupContents> => {
    const client = new ApiClient();
    const path = idStack.join("/");
    const url = "creation/data-bases/" + path;

    return await client.getAsync(url);
};

export const useTableQuery = (idStack: string[]): ITableResult => {
    const result = useQuery(
        ["table", idStack],
        () => fetchTable(idStack),
        defaultQueryOptions);

    return result;
};
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
export interface ITableListResponse {
    id: string;
    type?: 't' | 'l';
    updated?: string;
    text: MultiLanguageString;
    languages?: string[];
}

/**
 * Interface for a table result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {ITableListResponse[]} data - The table list response data.
 */
export interface ITableResult {
    isLoading: boolean;
    isError: boolean;
    data: ITableListResponse[];
}

const fetchTable = async (idStack: string[], lang: string): Promise<ITableListResponse[]> => {
    const client = new ApiClient();
    const path = idStack.join("/");
    const url = "creation/data-bases/" + path;
    const getParams = {
        'lang': lang
    }

    return await client.getAsync(url, getParams);
};

export const sortTableData = (data: ITableListResponse[], primaryLanguage: string, databaseLanguages: string[]): ITableListResponse[] => {
    return [...data].sort((a, b) => {
        const textA = a.text[primaryLanguage] || a.text[databaseLanguages[0]];
        const textB = b.text[primaryLanguage] || b.text[databaseLanguages[0]];

        return textA.localeCompare(textB, primaryLanguage);
    });
};

export const useTableQuery = (idStack: string[], lang: string): ITableResult => {
    // If trying to fetch a database without a language, return empty
    const fetchFunction = lang ?
        () => fetchTable(idStack, lang) :
        () => Promise.resolve([]);

    return useQuery(
        ['table', ...idStack, lang],
        fetchFunction,
        defaultQueryOptions,
    );
};
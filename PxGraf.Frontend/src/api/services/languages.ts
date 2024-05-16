/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { defaultQueryOptions } from "utils/ApiHelpers";

const fetchLanguages = async (): Promise<string[]> => {
    const client = new ApiClient();
    const url = "creation/languages/";

    return await client.getAsync(url);
};

/**
 * Parses a list of languages into a string for display.
 * @param {string[]} languages List of languages.
 * @returns All listed languages in one string, in upper case.
 */
export const parseLanguageString = (languages: string[]): string => {
    return `(${languages.join(", ").toUpperCase()})`;
}

/**
 * Chooses a listing language for the given listing item, trying to prefer the selected uiLanguage.
 * @param {string} id Id of the database or table.
 * @param {string[]} languages Languages supported by the listing item.
 * @param {string} uiLanguage Currently selected UI language.
 * @returns UI language, if it is supported by the listing item's languages. Otherwise the default language of the item.
 */
export const getDatabaseListingLanguage = (id: string, languages: string[], uiLanguage: string): string => {
    return languages.includes(uiLanguage) ? uiLanguage : languages[0];
}

export const useLanguagesQuery = (): string[] => {
    const queryResult = useQuery(
        ['languages'],
        () => fetchLanguages(),
        defaultQueryOptions
    );
    return queryResult.data;
};
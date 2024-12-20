import { IDatabaseGroupHeader, IDatabaseTable } from '../api/services/table';
import { IDimension, IDimensionValue, EDimensionType } from "types/cubeMeta";
import { getAdditionalPropertyValue } from './metadataUtils';
import { eliminationKey } from './keywordConstants';

/**
 * Function for sorting databases based on the primary language.
 * @param {IDatabaseGroupHeader[]} data Databases or group headers to be sorted.
 * @param {string} primaryLanguage Primary language for sorting.
 * @returns Sorted databases or sub group headers.
 */
export const sortDatabaseGroups = (data: IDatabaseGroupHeader[], primaryLanguage: string): IDatabaseGroupHeader[] => {
    return sortDatabaseItems(data, primaryLanguage);
};

/**
 * Function for sorting tables based on the primary language.
 * @param {IDatabaseTable[]} data Tables to be sorted.
 * @param {string} primaryLanguage Primary language for sorting.
 * @returns Sorted tables.
 */
export const sortDatabaseTables = (data: IDatabaseTable[], primaryLanguage: string): IDatabaseTable[] => {
    return sortDatabaseItems(data, primaryLanguage);
}

const sortDatabaseItems = <T extends IDatabaseGroupHeader>(data: T[], primaryLanguage: string): T[] => {
    return [...data].sort((a, b) => {
        const textA = a.name[primaryLanguage] || a.name[a.languages[0]];
        const textB = b.name[primaryLanguage] || b.name[a.languages[0]];

        return textA.localeCompare(textB, primaryLanguage);
    });
};

/**
 * Function for sorting dimensions for the dimension selection list based on their type.
 * @param {IDimension[]} dimensions  Dimensions to be sorted.
 * @returns A sorted list of dimensions.
 */
export const sortedDimensions = (dimensions: IDimension[]): IDimension[] => {
    //Create a new array for sorted dimensions and store dimensions based on their type
    const sortedDimensions = [];
    let contentDimension: IDimension;
    const timeDimensions = [];
    const otherDimensions = [];
    const eliminationDimensions = [];
    const singleValueDimensions = [];
    dimensions.forEach((dimension: IDimension) => {
        if (dimension.type === EDimensionType.Content) {
            contentDimension = dimension;
        }
        else if (dimension.type === EDimensionType.Time) {
            timeDimensions.push(dimension);
        }
        else if (dimension.values.filter(vv => getValueIsSumValue(vv, dimension)).length > 0) {
            eliminationDimensions.push(dimension);
        }
        else if (dimension.values.length === 1) {
            singleValueDimensions.push(dimension);
        }
        else {
            otherDimensions.push(dimension);
        }
    });

    //Populate sortedDimensions array with dimensions in the correct order
    if (contentDimension) {
        sortedDimensions.push(contentDimension);
    }
    sortedDimensions.push(...timeDimensions);
    sortedDimensions.push(...otherDimensions);
    sortedDimensions.push(...eliminationDimensions);
    sortedDimensions.push(...singleValueDimensions);

    return sortedDimensions;
}

function getValueIsSumValue(value: IDimensionValue, dimension: IDimension): boolean {
    const eliminationCode: string = getAdditionalPropertyValue(eliminationKey, dimension.additionalProperties) as string;
    if (eliminationCode) return eliminationCode === value.code;
    else return false;
}
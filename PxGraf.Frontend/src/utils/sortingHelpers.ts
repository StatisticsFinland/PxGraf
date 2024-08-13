import { IDatabaseGroupHeader, IDatabaseTable } from '../api/services/table';
import { IVariable } from "types/cubeMeta";

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
 * Function for sorting variables for the variable selection list based on their type.
 * @param {IVariable[]} variables  Variables to be sorted.
 * @returns A sorted list of variables.
 */
export const sortedVariables = (variables: IVariable[]) => {

    //Create a new array for sorted variables and store variables based on their type
    const sortedVariables = [];
    let contentVariable: IVariable;
    const timeVariables = [];
    const otherVariables = [];
    const eliminationVariables = [];
    const singleValueVariables = [];
    variables.forEach((variable: IVariable) => {
        if (variable.type == 'C') {
            contentVariable = variable;
        }
        else if (variable.type == 'T') {
            timeVariables.push(variable);
        }
        else if (variable.values.filter((vv: { isSum: boolean; }) => vv.isSum).length > 0) {
            eliminationVariables.push(variable);
        }
        else if (variable.values.length === 1) {
            singleValueVariables.push(variable);
        }
        else {
            otherVariables.push(variable);
        }
    });

    //Populate sortedVariables array with variables in the correct order
    if (contentVariable) {
        sortedVariables.push(contentVariable);
    }
    sortedVariables.push(...timeVariables);
    sortedVariables.push(...otherVariables);
    sortedVariables.push(...eliminationVariables);
    sortedVariables.push(...singleValueVariables);

    return sortedVariables;
}
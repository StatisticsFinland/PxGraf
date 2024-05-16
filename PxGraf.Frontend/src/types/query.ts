/* istanbul ignore file */

/* Query - variable queries*/

/**
 * Type for a query that contains @see {@link IVariableQuery} objects for each variable.
 * @property {{ [key: string]: IVariableQuery }} - Queries for each variable.
 */
export type Query = { [key: string]: IVariableQuery };

/**
 * Enum that defines different variable value filter types.
 */
export enum FilterType {
    Item = 'item',
    All = 'all',
    Top = 'top',
    From = 'from'
}

/**
 * Interface for variable query properties.
 * @property {IValueFilter} valueFilter - Reference to value filter.
 * @property {boolean} selectable - Selected selectable variable.
 * @property {any} virtualValueDefinitions - Virtual value definitions.
  */
export interface IVariableQuery {
    valueFilter: IValueFilter
    selectable: boolean
    virtualValueDefinitions
}

/**
 * Interface for value filter properties.
 * @property {FilterType} type - Selected filter type.
 * @property {string[] | string | number} query - The query for the filter.
 */
export interface IValueFilter {
    type: FilterType;
    query?: string[] | string | number
}

/**
 * Type for string supporting multiple languages.
 */
type MultiLanguageString = { [key: string]: string };

/* CubeQuery - meta data editions*/

/**
 * Interface for cube query properties.
 * @property {MultiLanguageString} chartHeaderEdit - The chart header edition.
 * @property {{ [key: string]: IVariableEditions }} variableQueries - The variable edition queries.
 */
export interface ICubeQuery {
    chartHeaderEdit?: MultiLanguageString;
    variableQueries: { [key: string]: IVariableEditions };
}

/**
 * Interface for variable editions.
 * @property {{ [key: string]: IVariableValueEditions }} valueEdits - The value editions.
 */
export interface IVariableEditions {
    valueEdits: {
        [valueCode: string]: IVariableValueEditions;
    };
}

/**
 * Interface for variable value editions.
 * @property {MultiLanguageString} nameEdit - The name edition.
 * @property {IContentComponentEdition} contentComponent - The content component edition containing source and unit.
  */
export interface IVariableValueEditions {
    nameEdit? : MultiLanguageString;
    contentComponent? : IContentComponentEdition;
}

/**
 * Interface for content component edition.
 * @property {MultiLanguageString} sourceEdit - Edition of the source.
 * @property {MultiLanguageString} unitEdit - Edition of the unit.
 */
interface IContentComponentEdition {
    sourceEdit? : MultiLanguageString;
    unitEdit? : MultiLanguageString;
}

/**
 * Interface that stores information about the query.
 * @property {number} size - The size of the query.
 * @property {number} sizeWarningLimit - The size warning limit of the query.
 * @property {number} maximumSupportedSize - The maximum supported size of the query.
 * @property {string[]} validVisualizations - Names of valid visualization types for of the query.
 * @property {{[key: string]: MultiLanguageString}} visualizationRejectionReasons - Reasons for why certain visualization types are rejected for the query.
 * @property {number} maximumHeaderLength - The maximum header length of the query.
 */
export interface IQueryInfo {
    size: number;
    sizeWarningLimit: number;
    maximumSupportedSize: number;
    validVisualizations: string[];
    visualizationRejectionReasons: {[key: string]: MultiLanguageString};
    maximumHeaderLength: number;
}
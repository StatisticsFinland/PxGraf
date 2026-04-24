/* istanbul ignore file */
import { MultiLanguageString } from "types/multiLanguageString";
/* Query - dimension queries*/

/**
 * Type for a query that contains @see {@link IDimensionQuery} objects for each dimension.
 * @property {{ [key: string]: IDimensionQuery }} - Queries for each dimension.
 */
export type Query = { [key: string]: IDimensionQuery };

/**
 * Enum that defines different dimension value filter types.
 */
export enum FilterType {
    Item = 'item',
    All = 'all',
    Top = 'top',
    From = 'from'
}

/**
 * Interface for dimension query properties.
 * @property {IValueFilter} valueFilter - Reference to value filter.
 * @property {boolean} selectable - Selected selectable dimension.
 * @property {any} virtualValueDefinitions - Virtual value definitions.
  */
export interface IDimensionQuery {
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

/* CubeQuery - meta data editions*/

/**
 * Interface for cube query properties.
 * @property {MultiLanguageString} chartHeaderEdit - The chart header edition.
 * @property {{ [key: string]: IDimensionEditions }} variableQueries - The IDimensionEditions edition queries.
 */
export interface ICubeQuery {
    chartHeaderEdit?: MultiLanguageString;
    variableQueries: { [key: string]: IDimensionEditions };
}

/**
 * Interface for IDimensionEditions editions.
 * @property {{ [key: string]: IDimensionValueEditions }} valueEdits - The value editions.
 */
export interface IDimensionEditions {
    valueEdits: {
        [valueCode: string]: IDimensionValueEditions;
    };
}

/**
 * Interface for dimension value editions.
 * @property {MultiLanguageString} nameEdit - The name edition.
 * @property {IContentComponentEdition} contentComponent - The content component edition containing source and unit.
  */
export interface IDimensionValueEditions {
    nameEdit?: MultiLanguageString;
    contentComponent? : IContentComponentEdition;
}

/**
 * Interface for content component edition.
 * @property {MultiLanguageString} sourceEdit - Edition of the source.
 * @property {MultiLanguageString} unitEdit - Edition of the unit.
 */
interface IContentComponentEdition {
    sourceEdit?: MultiLanguageString;
    unitEdit?: MultiLanguageString;
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
    visualizationRejectionReasons: { [key: string]: MultiLanguageString };
    maximumHeaderLength: number;
}
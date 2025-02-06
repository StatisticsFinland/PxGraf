/* istanbul ignore file */

import { MultiLanguageString } from "./multiLanguageString";

/**
 * Interface for a section in the hierarchy of the table list.
 * @property {IDatabaseGroupHeader[]} headers - The headers in the section.
 * @property {IDatabaseTable[]} files - The tables in the section.
 */
export interface IDatabaseGroupContents {
    headers: IDatabaseGroupHeader[];
    files: IDatabaseTable[];
}

/**
 * Interface for items shown on the table listing that can be sorted by the translated name.
 * @property {MultiLanguageString} name - Localized names in all available languages.
 * @property {string[]} languages - Available languages of the item. 
 */
export interface ISortableTableListItem {
    name: MultiLanguageString;
    languages: string[];
}

/***
 * Interface for a table group header
 * @property {string} code - The code of the table group.
 * @property {MultiLanguageString} name - The name of the table group as a multi-language string.
 */
export interface IDatabaseGroupHeader extends ISortableTableListItem {
    code: string;
}

/***
 * Enum for database table errors
 */
export enum EDatabaseTableError {
    contentLoad = 'ContentLoad',
    contentDimensionMissing = 'ContentDimensionMissing',
    timeDimensionMissing = 'TimeDimensionMissing',
}

/***
 * Interface for a px table
 * @property {fileName} fileName - Name of the px table file.
 * @property {string} lastUpdated - The last updated date of the table.
 * @property {EDatabaseTableError} error - Enum to indicate if there's an error with the table.
 */
export interface IDatabaseTable extends ISortableTableListItem {
    fileName: string;
    lastUpdated: string | null;
    error?: EDatabaseTableError;
}

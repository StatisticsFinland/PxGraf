/**
 * Interface for the result of table meta data validation
 * @property {boolean} tableHasContentVariable - True if the table has a content variable
 * @property {boolean} tableHasTimeVariable - True if the table has a time variable
 * @property {boolean} allVariablesContainValues - True if all variables contain values
 */
export interface ITableMetaValidationResult {
    tableHasContentVariable: boolean,
    tableHasTimeVariable: boolean,
    allVariablesContainValues: boolean,
}
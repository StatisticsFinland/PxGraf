/**
 * Interface for the result of table meta data validation
 * @property {boolean} tableHasContentDimension - True if the table has a content dimension
 * @property {boolean} tableHasTimeDimension - True if the table has a time dimension
 * @property {boolean} allDimensionsContainValues - True if all dimensions contain values
 */
export interface ITableMetaValidationResult {
    tableHasContentDimension: boolean,
    tableHasTimeDimension: boolean,
    allDimensionsContainValues: boolean,
}
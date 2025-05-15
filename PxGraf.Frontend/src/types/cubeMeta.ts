/* istanbul ignore file */
import { TTimeVariableInterval } from "@statisticsfinland/pxvisualizer";
import { MultiLanguageString } from "./multiLanguageString";

/**
 * Interface for cube meta properties. A cube is a multi-dimensional result of Px file data filtered by a query.
 * @property {string} defaultLanguage - The default language of the cube.
 * @property {availableLanguages[]} languages - List of available languages.
 * @property {IDimension[]} dimensions - List of dimensions.
 * @property {{ [key: string]: IMetaProperty }} additionalProperties - Optional dictionary for additional metadata properties
 */
export interface IMatrixMetadata {
    defaultLanguage: string,
    availableLanguages: string[],
    dimensions: IDimension[],
    additionalProperties?: { [key: string]: IMetaProperty }
}

/**
 * Enumeration for metadata property types.
 */
export enum EMetaPropertyType {
    Text = 'Text',
    MultilanguageText = 'MultilanguageText',
    Numeric = 'Numeric',
    Boolean = 'Boolean',
    TextArray = 'TextArray',
    MultilanguageTextArray = 'MultilanguageTextArray'
}

/**
 * Interface for a metadata property of a cube, dimension or value.
 * @property {MetaPropertyType} Type - The type of the property.
 * @property {MultiLanguageString | string | boolean | number | MultiLanguageString[] | string[]} Value - The value of the property.
 */
export interface IMetaProperty {
    type: EMetaPropertyType,
    value: MultiLanguageString | string | boolean | number | MultiLanguageString[] | string[]
}

/**
 * Interface for dimension properties. Each dimension defines a dimension in a cube.
 * @property {string} Code - The code of the dimension.
 * @property {MultiLanguageString} Name - The multi language name of the dimension.
 * @property {{ [key: string]: IMetaProperty }} additionalProperties - Optional dictionary for additional metadata properties
 * @property {IDimensionValue[]} Values - List of values associated with this dimension.
 * @property {VariableType} Type - The type of the dimension.
 * @property {Interval} Interval - The interval of time dimension if applicable.
 */
export interface IDimension {
    code: string,
    name: MultiLanguageString,
    additionalProperties?: { [key: string]: IMetaProperty },
    values: IDimensionValue[] | IContentDimensionValue[],
    type: EDimensionType,
    interval?: TTimeVariableInterval 
}

/**
 * Interface for dimension value properties.
 * @property {string} code - The code of the value.
 * @property {MultiLanguageString} name - The multi language name of the value.
 * @property {{ [key: string]: IMetaProperty }} additionalProperties - Optional dictionary for additional metadata properties
 */
export interface IDimensionValue {
    code: string,
    name: MultiLanguageString,
    isVirtual: boolean
    additionalProperties?: { [key: string]: IMetaProperty }
}

/**
 * Interface for content dimension value properties.
 * @property {string} code - The code of the value.
 * @property {MultiLanguageString} name - The multi language name of the value.
 * @property {MultiLanguageString} unit - The unit of the value.
 * @property {string} lastUpdated - The last updated date of the value.
 * @property {number} precision - How many decimal places the value has.
 */
export interface IContentDimensionValue extends IDimensionValue {
    unit?: MultiLanguageString,
    lastUpdated?: string,
    precision?: number
}

/**
 * Enumeration for dimension types.
 */
export enum EDimensionType {
    Time = 'Time',
    Content = 'Content',
    Geographical = 'Geographical',
    Ordinal = 'Ordinal',
    Nominal = 'Nominal',
    Other = 'Other',
    Unknown = 'Unknown'
}
/* istanbul ignore file */
import { TTimeVariableInterval } from "@statisticsfinland/pxvisualizer";
import { MultiLanguageString } from "./multiLanguageString";

/**
 * Interface for cube meta properties. A cube is a multi-dimensional result of Px file data filtered by a query.
 * @property {string[]} languages - List of available languages.
 * @property {MultiLanguageString} header - The multi language header of the cube.
 * @property {MultiLanguageString} note - The note of the cube.
 * @property {IDimension[]} variables - List of dimensions.
 */
export interface IMatrixMetadata {
    DefaultLanguage: string,
    AvailableLanguages: string[],
    Dimensions: IDimension[],
    AdditionalProperties?: { [key: string]: IMetaProperty }
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
    Type: EMetaPropertyType,
    Value: MultiLanguageString | string | boolean | number | MultiLanguageString[] | string[]
}

/**
 * Interface for dimension properties. Each variable defines a dimension in a cube.
 * @property {string} Code - The code of the variable.
 * @property {MultiLanguageString} Name - The multi language name of the variable.
 * @property {{ [key: string]: IMetaProperty }} additionalProperties - Optional dictionary for additional metadata properties
 * @property {IDimensionValue[]} Values - List of values associated with this variable.
 * @property {VariableType} Type - The type of the variable.
 * @property {Interval} Interval - The interval of time variable if applicable.
 */
export interface IDimension {
    Code: string,
    Name: MultiLanguageString,
    AdditionalProperties?: { [key: string]: IMetaProperty },
    Values: IDimensionValue[]
    Type: EDimensionType,
    Interval?: TTimeVariableInterval 
}

/**
 * Interface for dimension value properties.
 * @property {string} code - The code of the value.
 * @property {MultiLanguageString} name - The multi language name of the value.
 * @property {{ [key: string]: IMetaProperty }} additionalProperties - Optional dictionary for additional metadata properties
 */
export interface IDimensionValue {
    Code: string,
    Name: MultiLanguageString,
    IsVirtual: boolean
    AdditionalProperties?: { [key: string]: IMetaProperty },
    Unit?: MultiLanguageString,
    LastUpdated?: string,
    Precision?: number,
}

/**
 * Enumeration for variable types.
 */
export enum EDimensionType {
    Time = 'Time',
    Content = 'Content',
    Geographical = 'Geographical',
    Ordinal = 'Ordinal',
    Nominal = 'Nominal',
    Other = 'Other',
    Unknow = 'Unknown'
}
/* istanbul ignore file */
import { TTimeVariableInterval } from "@statisticsfinland/pxvisualizer";
import { MultiLanguageString } from "./multiLanguageString";

/**
 * Interface for cube meta properties. A cube is a multi-dimensional result of Px file data filtered by a query.
 * @property {string[]} languages - List of available languages.
 * @property {MultiLanguageString} header - The multi language header of the cube.
 * @property {MultiLanguageString} note - The note of the cube.
 * @property {IVariable[]} variables - List of variables.
 */
export interface IMatrixMetadata {
    DefaultLanguage: string,
    AvailableLanguages: string[],
    Dimensions: IDimension[],
    AdditionalProperties?: { [key: string]: IMetaProperty }
}

/**
 * Interface for a metadata property of a cube, dimension or value.
 * @property {string} KeyWord - The keyword of the property.
 * @property {boolean} CanGetStringValue - Flag to indicate if the property can be represented as a string.
 * @property {boolean} CanGetMultilanguageValue - Flag to indicate if the property can be represented as a multi language string.
 * @property {MultiLanguageString | string} Entries - The multi language or string value of the property.
 */
export interface IMetaProperty {
    KeyWord: string,
    CanGetStringValue: boolean,
    CanGetMultilanguageValue: boolean,
    Entries: MultiLanguageString | string
}

/**
 * Interface for dimnension properties. Each variable defines a dimension in a cube.
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
    Type: VariableType,
    Interval?: TTimeVariableInterval 
}

/**
 * Interface for variable properties.Each variable defines a dimension in a cube.
 * @property { string } code - The code of the variable.
 * @property { MultiLanguageString } name - The multi language name of the variable.
 * @property { MultiLanguageString } note - The multi language note of the variable.
 * @property { VariableType } type - The type of the variable.
 * @property { IVariableValue[] } values - List of values associated with this variable.
 */
export interface IVariable {
    code: string,
    name: MultiLanguageString,
    note: MultiLanguageString,
    type: VariableType,
    values: IVariableValue[]
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
    Virtual: boolean
    AdditionalProperties?: { [key: string]: IMetaProperty },
    Unit?: MultiLanguageString,
    LastUpdated?: string,
    Precision?: number,
}

/**
 * Interface for variable value properties.
 * @property { string } code - The code of the value.
 * @property { MultiLanguageString } name - The multi language name of the value.
 * @property { MultiLanguageString } note - The multi language note of the value.
 * @property { boolean } isSum - Flag to indicate if the value is an elimination value.
 * @property { IContentComponent } contentComponent - The content variable component of the value.
 * @property { IVirtualComponent[] } virtualComponents - List of virtual components.
 */
export interface IVariableValue {
    code: string,
    name: MultiLanguageString,
    note: MultiLanguageString,
    isSum: boolean,
    contentComponent?: IContentComponent
    virtualComponents?: IVirtualComponent[]
}

/**
 * Interface for value content variable value properties.
 * @property {MultiLanguageString} unit - The unit of the value.
 * @property {MultiLanguageString} source - The source of the value.
 * @property {number} numberOfDecimals - The precision, number of decimals of the value.
 * @property {string} lastUpdated - The last updated date of the value.
 */
export interface IContentComponent {
    unit: MultiLanguageString,
    source: MultiLanguageString,
    numberOfDecimals: number,
    lastUpdated: string
}

/**
 * Interface for virtual component properties.
 * @property {string} operator - The operator of the virtual component.
 * @property {string[]} operandCoded - List of coded operands.
 */
export interface IVirtualComponent {
    operator: string,
    operandCoded: string[]
}

/**
 * Enumeration for variable types.
 */
export enum VariableType {
    Unknown = 'N',
    Time = 'T',
    Ordinal = 'P',
    Geographical = 'G',
    Content = 'C',
    OtherClassificatory = 'F',
}

export function getVariablesFromMatrixMetadata(meta: IMatrixMetadata): IVariable[] {
    return meta.Dimensions.map(d => {
        return {
            code: d.Code,
            name: d.Name,
            note: d.Name,
            type: d.Type,
            values: d.Values.map(v => convertDimensionValueToVariableValue(v, meta))
        }
    })
}

export function convertDimensionValueToVariableValue(value: IDimensionValue, meta: IMatrixMetadata): IVariableValue {
    const dimension: IDimension = meta.Dimensions.find(d => d.Values.find(v => v.Code === value.Code));
    const dimensionSource: MultiLanguageString = getAdditionalProperty("SOURCE", dimension.AdditionalProperties) as MultiLanguageString;
    const tableSource: MultiLanguageString = getAdditionalProperty("SOURCE", meta.AdditionalProperties) as MultiLanguageString;
    const contentComponent = (): IContentComponent | null => {
        if (dimension.Type === VariableType.Content) {
            return {
                unit: value.Unit,
                source: findSourceForValue(value, dimensionSource, tableSource),
                numberOfDecimals: value.Precision,
                lastUpdated: value.LastUpdated
            }
        }
        else return null;
    }
    return {
        code: value.Code,
        name: value.Name,
        note: value.Name,
        isSum: getValueIsSumValue(value, dimension),
        contentComponent: contentComponent()
    }
}

function findSourceForValue(value: IDimensionValue, dimensionSource: MultiLanguageString, tableSource: MultiLanguageString): MultiLanguageString {
    const valueSource = getAdditionalProperty("SOURCE", value.AdditionalProperties) as MultiLanguageString;
    if (valueSource) return valueSource;
    else if (dimensionSource) return dimensionSource;
    else return tableSource;
}

export function getValueIsSumValue(value: IDimensionValue, dimension: IDimension): boolean {
    return getAdditionalProperty("ELIMINATION", dimension.AdditionalProperties) === value.Code;
}

function getAdditionalProperty(key: string, additionalProperties: { [key: string]: IMetaProperty }, forceToString = false): string | MultiLanguageString {
    if (additionalProperties[key]) {
        if (additionalProperties[key].CanGetStringValue) {
            return additionalProperties[key].Entries[0];
        }
        else if (additionalProperties[key].CanGetMultilanguageValue) {
            if (forceToString) return additionalProperties[key].Entries[0];
            else return additionalProperties[key].Entries;
        }
    }
    return null;
}

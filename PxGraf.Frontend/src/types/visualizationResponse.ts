/* istanbul ignore file */
import { EDimensionType } from "./cubeMeta";
import { MultiLanguageString } from "./multiLanguageString";

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
    type: EDimensionType,
    values: IVariableValue[]
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
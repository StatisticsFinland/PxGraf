/* istanbul ignore file */

import { MultiLanguageString } from "./multiLanguageString"

/**
 * Interface for visualization rules.
 * @property allowManualPivot - Flag to indicate if manual pivot is allowed.
 * @property sortingOptions - List of sorting options.
 * @property multiselectVariableAllowed - Flag to indicate if multiselect variable is allowed.
 * @property visualizationTypeSpecificRules - Object for visualization type specific rules.
 */
export interface IVisualizationRules {
    allowManualPivot: boolean,
    sortingOptions: ISortingOption[],
    multiselectVariableAllowed: boolean
    visualizationTypeSpecificRules?: ITypeSpecificVisualizationRules
}

/**
 * Interface for different sorting options
 * @property code - Code for the sorting option
 * @property description - Description for the sorting option as a multi language string
 */
export interface ISortingOption {
    code: string,
    description: MultiLanguageString
}

/**
 * Interface for visualization type specific rules.
 * @property allowShowingDataPoints - Flag to indicate if showing data points is allowed.
 * @property allowCuttingYAxis - Flag to indicate if cutting the Y-axis is allowed.
 * @property allowMatchXLabelsToEnd - Flag to indicate if matching X labels to the end is allowed.
 * @property allowSetMarkerScale - Flag to indicate if setting marker scale is allowed.
 */
export interface ITypeSpecificVisualizationRules {
    allowShowingDataPoints: boolean,
    allowCuttingYAxis: boolean,
    allowMatchXLabelsToEnd: boolean,
    allowSetMarkerScale: boolean
}
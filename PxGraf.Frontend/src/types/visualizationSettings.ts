/* istanbul ignore file */

import {VisualizationType} from './visualizationType';

/**
 * Interface for visualization settings
 * @property {string[]} defaultSelectableVariableCodes - Dimension codes for default selectable dimension values.
 * @property {boolean} pivotRequested - Flag to indicate if the user has requested manual pivoting.
 * @property {boolean} cutYAxis - Flag to indicate if the y-axis should be cut based on the values.
 * @property {string} multiselectableVariableCode - The dimension code for the multiselectable dimension if any.
 * @property {string[]} rowVariableCodes - Dimension codes for the row dimensions.
 * @property {string[]} columnVariableCodes - Dimension codes for the column dimensions.
 * @property {string} sorting - Selected sorting order.
 * @property {boolean} matchXLabelsToEnd - Flag to indicate if the x-axis labels should be matched to the end.
 * @property {number} markerSize - The size of scatter plot markers.
 * @property {VisualizationType} selectedVisualization - The selected visualization type.
 * @property {boolean} showDataPoints - Flag to indicate if data points should be shown in the visualization.
 */
export interface IVisualizationSettings {
    defaultSelectableVariableCodes?: { [key: string]: string[] };
    pivotRequested?: boolean;
    cutYAxis?: boolean;
    multiselectableVariableCode?: string;
    rowVariableCodes?: string[];
    columnVariableCodes?: string[];
    sorting?: string;
    matchXLabelsToEnd?: boolean;
    markerSize?: number;
    selectedVisualization?: VisualizationType;
    showDataPoints?: boolean;
}
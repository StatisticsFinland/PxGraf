import { MultiLanguageString } from "./multiLanguageString";
import { VisualizationType } from "./visualizationType";

/**
 * Interface for dimension sorting options.
 * @property {string} code - The code of the sorting option.
 * @property {MultiLanguageString} description - Longer, user facing description of the sorting option.
 */
export interface ISortingOption {
    code: string;
    description: MultiLanguageString;
}

/**
 * Interface for sorting option collection. Contains default and pivoted sorting options if available.
 * @property {ISortingOption[]} default - Default sorting options.
 * @property {ISortingOption[]} pivoted - Pivoted sorting options.
 */
interface ISortingOptionCollection {
    default: ISortingOption[];
    pivoted: ISortingOption[];
}

/**
 * Interface for visualization options for a specific visualization type.
 * @property {VisualizationType} type - The type of visualization.
 * @property {boolean} [allowShowingDataPoints] - Flag to indicate if showing data points is allowed.
 * @property {boolean} [allowCuttingYAxis] - Flag to indicate if cutting the Y-axis is allowed.
 * @property {boolean} [allowMatchXLabelsToEnd] - Flag to indicate if matching X labels to the end is allowed.
 * @property {boolean} [allowSetMarkerScale] - Flag to indicate if setting marker scale is allowed.
 * @property {boolean} allowManualPivot - Flag to indicate if manual pivoting is allowed.
 * @property {boolean} allowMultiselect - Flag to indicate if multivalue selectable is allowed.
 * @property {ISortingOptionCollection} sortingOptions - Sorting options for the visualization containing default and pivoted options.
 */
export interface IVisualizationOptions {
    type: VisualizationType;
    allowShowingDataPoints?: boolean;
    allowCuttingYAxis?: boolean;
    allowMatchXLabelsToEnd?: boolean;
    allowSetMarkerScale?: boolean;
    allowManualPivot: boolean;
    allowMultiselect: boolean;
    sortingOptions: ISortingOptionCollection;
}

/**
 * Interface for the response from the editor contents API used to populate the settings for adjusting the visualization in the editor.
 * @property {number} size - The size of the response based on the amount of values selected from all dimensions.
 * @property {number} sizeWarningLimit - The size warning limit for the response.
 * @property {number} maximumSupportedSize - The maximum supported size for the response.
 * @property {MultiLanguageString} headerText - Default header text for the visualization as a @ {@link MultiLanguageString} object.
 * @property {number} maximumHeaderLength - The maximum length of the header text.
 * @property {IVisualizationOptions[]} visualizationOptions - Visualization options for the valid visualization types as an array of @ {@link IVisualizationOptions} objects.
 * @property {object} visualizationRejectionReasons - Object containing rejection reasons for each visualization type as a @ {@link MultiLanguageString} object.
 */
export interface IEditorContentsResponse {
    size: number;
    sizeWarningLimit: number;
    maximumSupportedSize: number;
    headerText: MultiLanguageString;
    maximumHeaderLength: number;
    visualizationOptions: IVisualizationOptions[];
    visualizationRejectionReasons: {
        [visualizationType: string]: MultiLanguageString;
    }
}
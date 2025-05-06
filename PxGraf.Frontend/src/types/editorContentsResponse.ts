import { MultiLanguageString } from "./multiLanguageString";
import { VisualizationType } from "./visualizationType";

export interface ISortingOption {
    code: string;
    description: MultiLanguageString;
}

interface ISortingOptionCollection {
    default: ISortingOption[];
    pivoted: ISortingOption[];
}

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
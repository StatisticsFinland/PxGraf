import { IDimension } from "types/cubeMeta";
import { Query } from "types/query";
import { ISortingOption } from "types/visualizationRules";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";

export function getValidatedSettings(currentSettings: IVisualizationSettings, selectedVisualization: VisualizationType, sortingOptions: ISortingOption[], variables: IDimension[], query: Query): IVisualizationSettings {
    switch (selectedVisualization) {
        case VisualizationType.Table:
            return getTableValidatedSettings(currentSettings, variables, query);
        case VisualizationType.LineChart:
            return getLineChartValidatedSettings(currentSettings, query);
        case VisualizationType.PieChart:
            return getSortingValidatedSettings(currentSettings, sortingOptions);
        case VisualizationType.VerticalBarChart:
            return getBasicVerticalBarChartValidatedSettings(currentSettings);
        case VisualizationType.GroupVerticalBarChart:
            return getPivotAndLabelsValidatedSettings(currentSettings);
        case VisualizationType.StackedVerticalBarChart:
            return getPivotAndLabelsValidatedSettings(currentSettings);
        case VisualizationType.PercentVerticalBarChart:
            return getPivotAndLabelsValidatedSettings(currentSettings);
        case VisualizationType.HorizontalBarChart:
            return getSortingValidatedSettings(currentSettings, sortingOptions);
        case VisualizationType.GroupHorizontalBarChart:
            return getSortingAndPivotValidatedSettings(currentSettings, sortingOptions);
        case VisualizationType.StackedHorizontalBarChart:
            return getSortingAndPivotValidatedSettings(currentSettings, sortingOptions);
        case VisualizationType.PercentHorizontalBarChart:
            return getSortingAndPivotValidatedSettings(currentSettings, sortingOptions);
        case VisualizationType.ScatterPlot:
            return getScatterPlotValidatedSettings(currentSettings);
        case VisualizationType.PyramidChart:
            return getPyramidChartValidatedSettings(currentSettings);
        default:
            return {};
    }
}

function getSortingValidatedSettings(currentSettings: IVisualizationSettings, sortingOptions: ISortingOption[]) {
    return {
        showDataPoints: currentSettings?.showDataPoints ?? false,
        sorting: getValidatedSortingValue(currentSettings?.sorting, sortingOptions)
    };
}

function getBasicVerticalBarChartValidatedSettings(currentSettings: IVisualizationSettings) {
    return {
        matchXLabelsToEnd: currentSettings?.matchXLabelsToEnd ?? false,
        showDataPoints: currentSettings?.showDataPoints ?? false
    };
}

function getPyramidChartValidatedSettings(currentSettings: IVisualizationSettings) {
    return {
        showDataPoints: currentSettings?.showDataPoints ?? false
    };
}

function getSortingAndPivotValidatedSettings(currentSettings: IVisualizationSettings, sortingOptions: ISortingOption[]) {
    return {
        sorting: getValidatedSortingValue(currentSettings?.sorting, sortingOptions),
        pivotRequested: currentSettings?.pivotRequested ?? false,
        showDataPoints: currentSettings?.showDataPoints ?? false
    };
}

function getPivotAndLabelsValidatedSettings(currentSettings: IVisualizationSettings) {
    return {
        pivotRequested: currentSettings?.pivotRequested ?? false,
        matchXLabelsToEnd: currentSettings?.matchXLabelsToEnd ?? false,
        showDataPoints: currentSettings?.showDataPoints ?? false
    };
}

function getLineChartValidatedSettings(currentSettings: IVisualizationSettings, query: Query) {
    let multiselectableVariableCode = null;
    if (
        currentSettings?.multiselectableVariableCode != null &&
        query[currentSettings?.multiselectableVariableCode]?.selectable
    ) {
        multiselectableVariableCode = currentSettings?.multiselectableVariableCode;
    }

    return {
        cutYAxis: currentSettings?.cutYAxis ?? false,
        multiselectableVariableCode: multiselectableVariableCode,
        showDataPoints: currentSettings?.showDataPoints ?? false
    };
}

function getScatterPlotValidatedSettings(currentSettings: IVisualizationSettings) {
    return {
        cutYAxis: currentSettings?.cutYAxis ?? false,
        markerSize: currentSettings?.markerSize ?? 100,
    };
}

function getTableValidatedSettings(currentSettings: IVisualizationSettings, variables: IDimension[], query: Query) {
    //Get multivalue variables that are not selectable, excluding multiselectable and sort them by the amount of values
    const sortedMultivalueVariableCodes: string[] = variables.filter(v => v.Values.length > 1)
        .filter(v => !query[v.Code]?.selectable && currentSettings?.multiselectableVariableCode != v.Code)
        .sort((v1, v2) => v1.Values.length - v2.Values.length).map(v => v.Code);
 
    const currentRowVariableCodes = currentSettings?.rowVariableCodes ?? [];
    const currentColumnVariableCodes = currentSettings?.columnVariableCodes ?? [];

    //Drop missing columns
    const rowVariableCodes =
        currentRowVariableCodes.filter(code => sortedMultivalueVariableCodes.includes(code));

    const columnVariableCodes =
        currentColumnVariableCodes.filter(code => sortedMultivalueVariableCodes.includes(code));

    //Append rows and columns based on the amount of values. Variables with more values are assigned to rows and variables with less are assigned to columns.
    const newVariableCodes = sortedMultivalueVariableCodes.filter(code => !rowVariableCodes.includes(code) && !columnVariableCodes.includes(code));
    //Split the stack of variable codes to half. If odd number, the modulo will be assigned to rows
    const columnVariablesCount = Math.floor(newVariableCodes.length / 2);
    //Variables within columns and rows are sorted from outer to inner based on the amount of given values
    const newColumnVariableCodes = newVariableCodes.splice(0, columnVariablesCount);
    const newRowVariableCodes = newVariableCodes;
    //Add remaining (single value and selectable) variables to rows
    newRowVariableCodes.push(...variables.filter(v => !sortedMultivalueVariableCodes.includes(v.Code)).map(v => v.Code));

    return {
        rowVariableCodes: rowVariableCodes.concat(newRowVariableCodes),
        columnVariableCodes: columnVariableCodes.concat(newColumnVariableCodes),
    };
}

function getValidatedSortingValue(sortingCode: string, sortingOptions: ISortingOption[]) {
    if (sortingOptions.findIndex(so => so.code === sortingCode) < 0) {
        return sortingOptions[0].code;
    }
    else {
        return sortingCode;
    }
}
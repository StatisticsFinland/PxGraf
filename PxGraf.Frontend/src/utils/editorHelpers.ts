import { EDimensionType, EMetaPropertyType, IContentDimensionValue, IDimension, IDimensionValue } from 'types/cubeMeta';
import { FilterType, ICubeQuery, IDimensionQuery, IValueFilter, Query, VirtualValueOperator } from 'types/query';
import { getDefaultFilter } from './dimensionSelectionHelpers';
import { EDatabaseTableError } from '../types/tableListItems';
import { IVisualizationOptions } from '../types/editorContentsResponse';
import { VisualizationType } from '../types/visualizationType';
import { sourceKey, eliminationKey } from './keywordConstants';
import { getAdditionalPropertyValue } from './metadataUtils';
import { generatePlaceholderContentEdits, generatePlaceholderNames, getOperatorType } from './virtualValueHelpers';
import { IVisualizationSettings } from '../types/visualizationSettings';

/**
 * Determines the default value filter for a dimension when a table is opened without a saved query.
 * Time dimensions default to "all". Other dimensions default to their elimination value if one is
 * defined and valid, otherwise the first available value. Dimensions without values default to an
 * empty item filter.
 */
const getDefaultValueFilter = (variable: IDimension): IValueFilter => {
    if (variable.type === EDimensionType.Time) {
        return getDefaultFilter(FilterType.All);
    }
    const values = variable.values ?? [];
    if (values.length === 0) {
        return getDefaultFilter(FilterType.Item);
    }
    const eliminationCode = getAdditionalPropertyValue(eliminationKey, variable.additionalProperties);
    const validEliminationCode = typeof eliminationCode === 'string' && values.some(v => v.code === eliminationCode)
        ? eliminationCode
        : null;
    const defaultCode = validEliminationCode ?? values[0].code;
    return { type: FilterType.Item, query: [defaultCode] };
}

export const getDefaultQueries = (variables: IDimension[]) => {
    const queries: { [key: string]: IDimensionQuery } = {};
    for (const variable of variables) {
      queries[variable.code] = {
        valueFilter: getDefaultValueFilter(variable),
        selectable: false,
        virtualValueDefinitions: []
      }
    }
    return queries;
}

export const resolveDimensions = (dimensions: IDimension[], resolvedDimensionCodes: { [key: string]: string[] }) => {
    return dimensions.map(v => {
        const resolvedCodes = resolvedDimensionCodes?.[v.code] ?? [];
        const realValues = v.values.filter(val => resolvedCodes.includes(val.code));
        const realCodes = new Set(realValues.map(val => val.code));
        const virtualCodes = resolvedCodes.filter(code => !realCodes.has(code));
        const virtualValues: IDimensionValue[] = virtualCodes.map(code => ({
            code,
            name: {},
            isVirtual: true,
        }));
        return { code: v.code, name: v.name, type: v.type, values: [...realValues, ...virtualValues] } as IDimension;
    });
}

export const enrichDimensionsWithVirtualValues = (
    dimensions: IDimension[],
    query: { [key: string]: IDimensionQuery } | null,
    availableLanguages: string[],
    translateForLang: (lang: string, operator: VirtualValueOperator) => string,
    cubeQuery: ICubeQuery | null = null,
    restrictToResolved: boolean = false,
): IDimension[] => {
    if (!query) return dimensions;
    return dimensions.map(dim => {
        const defs = query[dim.code]?.virtualValueDefinitions ?? [];
        const preExistingVirtualCodes = restrictToResolved
            ? new Set(dim.values.filter(v => v.isVirtual).map(v => v.code))
            : null;
        const filteredDefs = preExistingVirtualCodes
            ? defs.filter(def => preExistingVirtualCodes.has(def.code))
            : defs;
        if (filteredDefs.length === 0 && preExistingVirtualCodes === null) return dim;
        const virtualValues: IDimensionValue[] = filteredDefs.map(def => {
            const opType = getOperatorType(def);
            const seqNum = filteredDefs.slice(0, filteredDefs.indexOf(def)).filter(d => getOperatorType(d) === opType).length + 1;
            const placeholderName = generatePlaceholderNames(availableLanguages, (lang) => translateForLang(lang, opType), seqNum);
            const base: IDimensionValue = {
                code: def.code,
                name: cubeQuery?.variableQueries?.[dim.code]?.valueEdits?.[def.code]?.nameEdit ?? placeholderName,
                isVirtual: true,
            };
            if (dim.type === EDimensionType.Content) {
                const { unitEdit, sourceEdit } = generatePlaceholderContentEdits(availableLanguages, (lang) => translateForLang(lang, opType));
                const contentValue: IContentDimensionValue = {
                    ...base,
                    unit: unitEdit,
                    ...(sourceEdit && {
                        additionalProperties: {
                            [sourceKey]: {
                                type: EMetaPropertyType.MultilanguageText,
                                value: sourceEdit,
                            },
                        },
                    }),
                };
                return contentValue;
            }
            return base;
        });
        return { ...dim, values: [...dim.values.filter(v => !v.isVirtual), ...virtualValues] };
    });
};

export const getVirtualValueDefinitionsSignature = (
    dimensions: IDimension[],
    query: { [key: string]: IDimensionQuery } | null,
): string => JSON.stringify(dimensions.map(dimension => ({
    code: dimension.code,
    definitions: query?.[dimension.code]?.virtualValueDefinitions ?? [],
})));

export const getErrorText = (error: EDatabaseTableError, t: (key: string) => string) => {
    switch (error) {
        case EDatabaseTableError.contentDimensionMissing:
            return t("error.contentVariableMissing");
        case EDatabaseTableError.timeDimensionMissing:
            return t("error.timeVariableMissing");
        default:
            return t("error.contentLoad");
    }
}

export const getVisualizationOptionsForVisualizationType = (options: IVisualizationOptions[], type: VisualizationType): IVisualizationOptions | undefined => {
    return options?.find(option => option.type === type);
}

interface IVisualizationSettingVisibility {
    sortingOptions: IVisualizationOptions['sortingOptions']['default'];
    selectableDimensionsExcludingContent: IDimension[];
    showTableSettings: boolean;
    showSortingOptions: boolean;
    showMarkerScaler: boolean;
    showMultiselectableSelector: boolean;
    showYAxisCutting: boolean;
    showPivot: boolean;
    showDataPoints: boolean;
    hasVisibleSettings: boolean;
}

export const getVisualizationSettingVisibility = (
    selectedVisualization: VisualizationType,
    dimensions: IDimension[],
    dimensionQuery: Query,
    visualizationOptions: IVisualizationOptions,
    visualizationSettings: IVisualizationSettings,
): IVisualizationSettingVisibility => {
    const sortingOptions = visualizationOptions?.allowManualPivot && visualizationSettings.pivotRequested
        ? visualizationOptions?.sortingOptions.pivoted
        : visualizationOptions?.sortingOptions.default;
    const selectableDimensions = dimensions.filter(dimension => dimensionQuery[dimension.code].selectable);
    const selectableDimensionsExcludingContent = selectableDimensions.filter(dimension => dimension.type !== EDimensionType.Content);
    const showTableSettings = selectedVisualization === VisualizationType.Table;
    const showSortingOptions = sortingOptions?.length > 0;
    const showMarkerScaler = visualizationOptions?.allowSetMarkerScale;
    const showMultiselectableSelector = visualizationOptions?.allowMultiselect && selectableDimensionsExcludingContent.length > 0;
    const showYAxisCutting = visualizationOptions?.allowCuttingYAxis;
    const showPivot = visualizationOptions?.allowManualPivot;
    const showDataPoints = visualizationOptions?.allowShowingDataPoints;

    return {
        sortingOptions,
        selectableDimensionsExcludingContent,
        showTableSettings,
        showSortingOptions,
        showMarkerScaler,
        showMultiselectableSelector,
        showYAxisCutting,
        showPivot,
        showDataPoints,
        hasVisibleSettings: showTableSettings || showSortingOptions || showMarkerScaler || showMultiselectableSelector || showYAxisCutting || showPivot || showDataPoints,
    };
};
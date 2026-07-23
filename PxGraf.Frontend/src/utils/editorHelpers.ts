import { EDimensionType, EMetaPropertyType, IContentDimensionValue, IDimension, IDimensionValue } from 'types/cubeMeta';
import { FilterType, ICubeQuery, IDimensionQuery, VirtualValueOperator } from 'types/query';
import { getDefaultFilter } from './dimensionSelectionHelpers';
import { EDatabaseTableError } from '../types/tableListItems';
import { IVisualizationOptions } from '../types/editorContentsResponse';
import { VisualizationType } from '../types/visualizationType';
import { sourceKey } from './keywordConstants';
import { generatePlaceholderContentEdits, generatePlaceholderNames, getOperatorType } from './virtualValueHelpers';

export const getDefaultQueries = (variables: IDimension[]) => {
    const queries: { [key: string]: IDimensionQuery } = {};
    for (const variable of variables) {
      queries[variable.code] = {
        valueFilter: getDefaultFilter(FilterType.Item),
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
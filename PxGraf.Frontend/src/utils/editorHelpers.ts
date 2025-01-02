import { IDimension } from 'types/cubeMeta';
import { FilterType, IDimensionQuery } from 'types/query';
import { getDefaultFilter } from './dimensionSelectionHelpers';

export const getDefaultQueries = (variables: IDimension[]) => {
    const queries: { [key: string]: IDimensionQuery } = {};
    for (const variable of variables) {
      queries[variable.code] = {
        valueFilter: getDefaultFilter(FilterType.Item),
        selectable: false,
        virtualValueDefinitions: null
      }
    }
    return queries;
}

export const resolveDimensions = (dimensions: IDimension[], resolvedDimensionCodes: { [key: string]: string[] }) => {
    return dimensions.map(v => { return { code: v.code, name: v.name, type: v.type, values: v.values.filter(val => resolvedDimensionCodes?.[v.code]?.includes(val.code)) } as IDimension });
}
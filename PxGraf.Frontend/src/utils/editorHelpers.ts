import { IDimension } from 'types/cubeMeta';
import { FilterType, IVariableQuery } from 'types/query';
import { getDefaultFilter } from './variableSelectionHelpers';

export const getDefaultQueries = (variables: IDimension[]) => {
    const queries: { [key: string]: IVariableQuery } = {};
    for (const variable of variables) {
      queries[variable.code] = {
        valueFilter: getDefaultFilter(FilterType.Item),
        selectable: false,
        virtualValueDefinitions: null
      }
    }
    return queries;
}

export const resolveVariables = (variables: IDimension[], resolvedValueCodes: { [key: string]: string[] }) => {
    return variables.map(v => { return { code: v.code, name: v.name, type: v.type, values: v.values.filter(val => resolvedValueCodes?.[v.code]?.includes(val.code)) } as IDimension });
}
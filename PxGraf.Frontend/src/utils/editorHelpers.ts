import { IDimension } from 'types/cubeMeta';
import { FilterType, IVariableQuery } from 'types/query';
import { getDefaultFilter } from './variableSelectionHelpers';

export const getDefaultQueries = (variables: IDimension[]) => {
    const queries: { [key: string]: IVariableQuery } = {};
    for (const variable of variables) {
      queries[variable.Code] = {
        valueFilter: getDefaultFilter(FilterType.Item),
        selectable: false,
        virtualValueDefinitions: null
      }
    }
    return queries;
}

export const resolveVariables = (variables: IDimension[], resolvedValueCodes: { [key: string]: string[] }) => {
    return variables.map(v => { return { Code: v.Code, Name: v.Name, Type: v.Type, Values: v.Values.filter(val => resolvedValueCodes?.[v.Code]?.includes(val.Code)) } as IDimension });
}
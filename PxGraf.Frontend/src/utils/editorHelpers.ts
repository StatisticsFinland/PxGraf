import { ICubeMeta, IVariable } from 'types/cubeMeta';
import { FilterType, IVariableQuery } from 'types/query';
import { getDefaultFilter } from './variableSelectionHelpers';

export const getDefaultQueries = (variables: IVariable[]) => {
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

export const resolveVariables = (variables: IVariable[], resolvedValueCodes: { [key: string]: string[] }) => {
    return variables.map(v => { return { code: v.code, name: v.name, type: v.type, values: v.values.filter(val => resolvedValueCodes?.[v.code]?.includes(val.code)) } as IVariable });
}

export const getContentLanguages = (meta: ICubeMeta) => {
    if (!meta) return [];
    return meta.variables.reduce((acc: string[], val: IVariable) => {
        if (acc.length === 0) return Object.keys(val.name);
        else if (Object.keys(val.name).every(s => acc.includes(s))) return acc;
        else throw new Error("Metadata has inconsistent languages defined for the variables");
    }, []);
}
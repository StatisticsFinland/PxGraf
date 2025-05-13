import { IDimension } from 'types/cubeMeta';
import { FilterType, IDimensionQuery } from 'types/query';
import { getDefaultFilter } from './dimensionSelectionHelpers';
import { useTranslation } from 'react-i18next';
import { EDatabaseTableError } from '../types/tableListItems';
import { IVisualizationOptions } from '../types/editorContentsResponse';
import { VisualizationType } from '../types/visualizationType';

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

export const getErrorText = (error: EDatabaseTableError) => {
    const { t } = useTranslation();
    switch (error) {
        case EDatabaseTableError.contentDimensionMissing:
            return t("error.contentVariableMissing");
        case EDatabaseTableError.timeDimensionMissing:
            return t("error.timeVariableMissing");
        default:
            return t("error.contentLoad");
    }
}

export const getVisualizationOptionsForType = (options: IVisualizationOptions[], type: VisualizationType): IVisualizationOptions | undefined => {
    return options.find(option => option.type === type);
}
import { IFetchSavedQueryResponse } from "api/services/queries"
import { merge } from "lodash"
import { ICubeQuery, IDimensionEditions, IDimensionQuery, Query } from "types/query"
import { PxGrafUrl } from "envVars"

export const buildCubeQuery = (query: Query, metaEdits: ICubeQuery, idStack: string[]) => {
    return merge(
      { tableReference: buildTableReference(idStack) },
      { variableQueries: query },
      metaEdits
    )
}

export const extractQuery = (completeQueryObject: IFetchSavedQueryResponse): Query => {
  const dimensionQueriesObject: {[key: string]: IDimensionQuery} = {};
  for (const [key, value] of Object.entries(completeQueryObject.query.variableQueries)) {
    dimensionQueriesObject[key] = {
      valueFilter: value.valueFilter,
      selectable: value.selectable,
      virtualValueDefinitions: value.virtualValueDefinitions
    }
  }
  return dimensionQueriesObject;
}

export const extractCubeQuery = (completeQueryObject: IFetchSavedQueryResponse): ICubeQuery => {
  const variableEditionsObject: {[key: string]: IDimensionEditions} = {};
  for (const [key, value] of Object.entries(completeQueryObject.query.variableQueries)) {
    variableEditionsObject[key] = {
      valueEdits: value.valueEdits
    }
  }
  const cubeQueryObject: ICubeQuery = {
    chartHeaderEdit: completeQueryObject.query.chartHeaderEdit,
    variableQueries: variableEditionsObject
  };
  return cubeQueryObject;
}

export const buildTableReference = (idStack: string[]) => {
    return {
      name: idStack[idStack.length - 1],
      hierarchy: idStack.slice(0, -1),
    }
}

export const pxGrafUrl = (path: string) => {
    return PxGrafUrl + path;
}

export const defaultQueryOptions = {
    retry: false,
    staleTime: 60 * 1000, //1min
};

export const parseLanguageString = (languages: string[]): string => {
    return `(${languages.join(", ").toUpperCase()})`;
}
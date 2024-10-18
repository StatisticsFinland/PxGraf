import { IFetchSavedQueryResponse } from "api/services/queries"
import { merge } from "lodash"
import { useEffect, useMemo, useState } from "react"
import { ICubeQuery, IVariableEditions, IVariableQuery, Query } from "types/query"
import { PxGrafUrl } from "envVars"

export const buildCubeQuery = (query: Query, metaEdits: ICubeQuery, idStack: string[]) => {
    return merge(
      { tableReference: buildTableReference(idStack) },
      { variableQueries: query },
      metaEdits
    )
}

export const extractQuery = (completeQueryObject: IFetchSavedQueryResponse): Query => {
  const variableQueriesObject: {[key: string]: IVariableQuery} = {};
  for (const [key, value] of Object.entries(completeQueryObject.query.variableQueries)) {
    variableQueriesObject[key] = {
      valueFilter: value.valueFilter,
      selectable: value.selectable,
      virtualValueDefinitions: value.virtualValueDefinitions
    }
  }
  return variableQueriesObject;
}

export const extractCubeQuery = (completeQueryObject: IFetchSavedQueryResponse): ICubeQuery => {
  const variableEditionsObject: {[key: string]: IVariableEditions} = {};
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

/* istanbul ignore next */
export const useDebounceState = (delay: number, ...params) => {

  const cachedParams = useMemo(() => params, params);
  const [debouncedParams, setDebouncedParams] = useState(cachedParams);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedParams(cachedParams);
    }, delay ?? 500);
    return () => {
      clearTimeout(timer);
    }
  }, [cachedParams, delay]);

  return debouncedParams;
}
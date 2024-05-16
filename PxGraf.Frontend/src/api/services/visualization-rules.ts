/* istanbul ignore file */

import ApiClient from "api/client";
import { merge } from "lodash";
import { useQuery } from "react-query";
import { Query } from "types/query";
import { IVisualizationRules } from "types/visualizationRules";
import { buildTableReference, defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for the visualization settings result
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred while retrieving the visualization settings.
 * @property {IVisualizationRules} data - The visualization settings data.
 */
export interface IVisualizationSettingsResult {
    isLoading: boolean;
    isError: boolean;
    data: IVisualizationRules;
}

const fetchVisualizationSettings = async (
    idStack: string[],
    query: Query,
    selectedVisualization: string,
    pivotRequested: boolean
): Promise<IVisualizationRules> => {

    const client = new ApiClient();

    const cubeQuery = merge(
        { tableReference: buildTableReference(idStack) },
        { variableQueries: query }
    );

    const requestBody = JSON.stringify(
        merge(
            { selectedVisualization: selectedVisualization },
            { pivotRequested: pivotRequested },
            { query: cubeQuery }
        )
    );

    const url = 'creation/visualization-rules';
    return await client.postAsync(url, requestBody);
}

export const useVisualizationOptionsQuery = (idStack: string[], query: Query, selectedVisualization: string, pivotRequested: boolean): IVisualizationSettingsResult => {
    return useQuery(
        ['visualization-rules', ...idStack, query, selectedVisualization, pivotRequested],
        () => fetchVisualizationSettings(idStack, query, selectedVisualization, pivotRequested),
        {
            ...defaultQueryOptions,
            enabled: query != null && selectedVisualization != null,
        }
    );
}
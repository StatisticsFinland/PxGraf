/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { IEditorContentsResponse } from "types/editorContentsResponse";
import { buildCubeQuery, defaultQueryOptions, useDebounceState } from "utils/ApiHelpers";
import { ICubeQuery, Query } from "types/query";

/**
 * Interface for editor contents result. Editor contents are used by the editor to populate the settings for adjusting the visualization.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {IEditorContentsResponse} data - The editor contents as an @see {@link IEditorContentsResponse} object.
 */ 
export interface IEditorContentsResult {
    isLoading: boolean;
    isError: boolean;
    data: IEditorContentsResponse;
}

const fetchEditorContents = async (idStack: string[], query: Query, metaEdits: ICubeQuery): Promise<IEditorContentsResponse> => {
    const client = new ApiClient();
    const url = 'creation/editor-contents';
    const requestBody = JSON.stringify(buildCubeQuery(query, metaEdits, idStack));
    return await client.postAsync(url, requestBody);
}

export const useEditorContentsQuery = (idStack: string[], query: Query, cubeQuery: ICubeQuery): IEditorContentsResult => {
    [idStack, query, cubeQuery] = useDebounceState(1000, idStack, query, cubeQuery);
    const queryKey = ['editor-contents', ...idStack, query, cubeQuery];
    return useQuery(
        queryKey,
        () => fetchEditorContents(idStack, query, cubeQuery),
        {
            ...defaultQueryOptions,
            enabled: query != null,
            keepPreviousData: true,
        }
    );
}
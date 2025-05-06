import ApiClient from "api/client";
import { useQuery } from "react-query";
import { IEditorContentsResponse } from "types/editorContentsResponse";
import { buildCubeQuery, defaultQueryOptions, useDebounceState } from "utils/ApiHelpers";
import { ICubeQuery, Query } from "types/query";

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
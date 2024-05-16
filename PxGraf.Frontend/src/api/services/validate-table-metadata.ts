/* istanbul ignore file */

import ApiClient from "api/client";
import { useQuery } from "react-query";
import { ITableMetaValidationResult } from "types/tableValidation";
import { defaultQueryOptions } from "utils/ApiHelpers";

/**
 * Interface for a table metadata validation result.
 * @property {boolean} isLoading - Flag to indicate if the data is still loading.
 * @property {boolean} isError - Flag to indicate if an error occurred during loading.
 * @property {ITableMetaValidationResult} data - The table metadata validation result.
 */
export interface IValidateTableMetaDataResult {
    isLoading: boolean;
    isError: boolean;
    data: ITableMetaValidationResult;
}

const fetchValidateTableMetadata = async (idStack: string[]): Promise<ITableMetaValidationResult> => {
    const client = new ApiClient();
    const url = `creation/validate-table-metadata/${idStack.join('/')}`;

    return await client.getAsync(url);
}

export const useValidateTableMetadataQuery = (idStack: string[]): IValidateTableMetaDataResult => {
    return useQuery(
        ['validate-table-metadata', idStack],
        () => fetchValidateTableMetadata(idStack),
        defaultQueryOptions
    );
}
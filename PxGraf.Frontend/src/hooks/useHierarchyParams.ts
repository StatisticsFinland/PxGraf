import { useMemo } from "react";
import useQueryParams from "./useQueryParams";

const useHierarchyParams = (): string[] => {
    const query = useQueryParams();

    const returnVal: (string | null)[] = useMemo(() => query.get('tablePath')?.split(',') || [], [query]);

    return returnVal;
}

export default useHierarchyParams;
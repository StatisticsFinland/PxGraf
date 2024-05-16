import { useNavigationContext } from "contexts/navigationContext";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useHierarchyParams from "./useHierarchyParams";

const useReplaceQueryParams = (path: string): void => {
    const navigate = useNavigate();
    const { tablePath } = useNavigationContext();
    const tablePathParams = useHierarchyParams();

    const shouldReplace: boolean = tablePath?.length && tablePath.join(',') !== tablePathParams?.join(',');

    useEffect(() => {
        if(shouldReplace) {
            navigate({pathname: path, search: `?tablePath=${tablePath.join(',')}`}, { replace: true });
            navigate(0);
        }
    }, [navigate, path, location, shouldReplace]);
}

export default useReplaceQueryParams;

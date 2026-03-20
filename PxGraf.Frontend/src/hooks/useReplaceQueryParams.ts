import { useNavigationContext } from "contexts/navigationContext";
import { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import useHierarchyParams from "./useHierarchyParams";

const useReplaceQueryParams = (path: string): void => {
    const navigate = useNavigate();
    const routerLocation = useLocation();
    const { tablePath } = useNavigationContext();
    const tablePathParams = useHierarchyParams();

    const shouldReplace: boolean = tablePath?.length && tablePath.join(',') !== tablePathParams?.join(',');

    useEffect(() => {
        if(shouldReplace) {
            navigate({pathname: path, search: `?tablePath=${tablePath.join(',')}`}, { replace: true });
        }
    }, [navigate, path, routerLocation, shouldReplace]);
}

export default useReplaceQueryParams;

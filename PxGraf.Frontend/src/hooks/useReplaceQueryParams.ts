import { useNavigationContext } from "contexts/navigationContext";
import { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import useHierarchyParams from "./useHierarchyParams";

const useReplaceQueryParams = (path: string): void => {
    const navigate = useNavigate();
    const routerLocation = useLocation();
    const { tablePath } = useNavigationContext();
    const tablePathParams = useHierarchyParams();

    const tablePathKey = tablePath?.join(',') ?? '';
    const tablePathParamsKey = tablePathParams?.join(',') ?? '';
    const shouldReplace = Boolean(tablePath?.length) && tablePathKey !== tablePathParamsKey;

    useEffect(() => {
        if(shouldReplace) {
            navigate({pathname: path, search: `?tablePath=${tablePathKey}`}, { replace: true });
        }
    }, [navigate, path, routerLocation, shouldReplace, tablePathKey, tablePathParamsKey]);
}

export default useReplaceQueryParams;

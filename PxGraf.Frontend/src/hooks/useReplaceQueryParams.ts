import { useNavigationContext } from "contexts/navigationContext";
import { useEffect, useRef, useTransition } from "react";
import { useNavigate } from "react-router-dom";
import useHierarchyParams from "./useHierarchyParams";

const useReplaceQueryParams = (path: string): void => {
    const navigate = useNavigate();
    const { tablePath } = useNavigationContext();
    const tablePathParams = useHierarchyParams();
    const [, startTransition] = useTransition();
    const isMountRef = useRef(true);

    const tablePathKey = tablePath?.join(',') ?? '';
    const tablePathParamsKey = tablePathParams?.join(',') ?? '';
    const shouldReplace = Boolean(tablePath?.length) && tablePathKey !== tablePathParamsKey;

    useEffect(() => {
        // Skip on mount: when navigating to this route, the URL is authoritative.
        // Context may still hold a stale value from the previous route (e.g. editor path
        // including table ID). The sync effect in the view will correct context.
        if (isMountRef.current) {
            isMountRef.current = false;
            return;
        }
        if(shouldReplace) {
            // Use startTransition so the URL update doesn't block rendering
            // of the tree list items that are currently expanding.
            startTransition(() => {
                navigate({pathname: path, search: `?tablePath=${tablePathKey}`}, { replace: true });
            });
        }
    }, [navigate, path, shouldReplace, tablePathKey, tablePathParamsKey]);
}

export default useReplaceQueryParams;

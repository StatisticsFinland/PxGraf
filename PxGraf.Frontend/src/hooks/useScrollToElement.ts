import { useEffect } from "react";

const useScrollToElement = (id?: string, offset = 50) => {
    useEffect(() => {
        if(!id) return;
        const element = document.getElementById(id);

        if(element) {
            const rect = element.getBoundingClientRect();
            window.scrollTo(rect.left + window.scrollX, rect.top + window.scrollY - offset);

            const focusableElement: HTMLElement = element.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
            focusableElement?.focus();
        }
     
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: offset is a stable default parameter, only re-run when id changes
    }, [id]);
};

export default useScrollToElement;

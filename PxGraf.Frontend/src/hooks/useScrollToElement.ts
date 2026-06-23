import { useEffect } from "react";

const getScrollParent = (element: HTMLElement): HTMLElement | null => {
    let parent = element.parentElement;
    while (parent) {
        const overflow = getComputedStyle(parent).overflowY;
        if (overflow === 'auto' || overflow === 'scroll') {
            return parent;
        }
        parent = parent.parentElement;
    }
    return null;
};

const useScrollToElement = (id?: string, offset = 70) => {
    useEffect(() => {
        if(!id) return;
        const element = document.getElementById(id);

        if(element) {
            const scrollParent = getScrollParent(element);
            if (scrollParent) {
                const elementRect = element.getBoundingClientRect();
                const parentRect = scrollParent.getBoundingClientRect();
                scrollParent.scrollTo({
                    top: scrollParent.scrollTop + (elementRect.top - parentRect.top) - offset,
                    behavior: 'smooth',
                });
            } else {
                const rect = element.getBoundingClientRect();
                window.scrollTo(rect.left + window.scrollX, rect.top + window.scrollY - offset);
            }

            const focusableElement: HTMLElement = element.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
            focusableElement?.focus();
        }
     
    }, [id, offset]);
};

export default useScrollToElement;

import { useEffect } from "react";

const useScrollToElement = (id?: string, offset = 50) => {
    useEffect(() => {
        if(!id) return;
        const element = document.getElementById(id);
        const elementX = element?.getBoundingClientRect().left + window.scrollX;
        const elementY = element?.getBoundingClientRect().top + window.scrollY;

        if(element) {
            window.scrollTo(elementX, elementY - offset);

            const focusableElement: HTMLElement = element.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
            focusableElement?.focus();
        }
     
    }, [id]);
};

export default useScrollToElement;

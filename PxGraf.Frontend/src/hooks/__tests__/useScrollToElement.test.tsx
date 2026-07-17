import { renderHook } from "@testing-library/react";
import useScrollToElement from "hooks/useScrollToElement";

describe('useScrollToElement hook', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        window.scrollTo = jest.fn();
    });

    afterEach(() => {
        document.body.innerHTML = '';
        jest.restoreAllMocks();
    });

    it('should scroll to given element and focus on first focusable element', () => {
        const targetId = 'test-element-id';
        const targetTop = 500;
        const targetLeft = 10;
        const offset = 50;

        const dummy = document.createElement('div');
        const button = document.createElement('button');
        dummy.id = targetId;
        button.focus = jest.fn();
        dummy.append(button);
        dummy.getBoundingClientRect = jest.fn(() => ({top: targetTop, left: targetLeft} as DOMRect));
        jest.spyOn(document, 'getElementById').mockReturnValue(dummy);

        renderHook(() => useScrollToElement(targetId, offset));

        expect(window.scrollTo).toHaveBeenCalledWith(targetLeft, targetTop - offset);
        expect(button.focus).toHaveBeenCalled();
    });

    it('should scroll to given element but not focus on anything if no focusable elements inside it', () => {
        const targetId = 'test-element-id';
        const targetTop = 500;
        const targetLeft = 10;
        const offset = 50;

        const dummy = document.createElement('div');
        const span = document.createElement('span');
        dummy.id = targetId;
        span.focus = jest.fn();
        dummy.append(span);
        dummy.getBoundingClientRect = jest.fn(() => ({top: targetTop, left: targetLeft} as DOMRect));
        jest.spyOn(document, 'getElementById').mockReturnValue(dummy);

        renderHook(() => useScrollToElement(targetId, offset));

        expect(window.scrollTo).toHaveBeenCalledWith(targetLeft, targetTop - offset);
        expect(span.focus).not.toHaveBeenCalled();
    });

    it('should scroll if given element is not found', () => {
        const targetId = 'test-element-id';
        const offset = 50;

        jest.spyOn(document, 'getElementById').mockReturnValue(null);

        renderHook(() => useScrollToElement(targetId, offset));

        expect(window.scrollTo).not.toHaveBeenCalled();
    });

    it('should not scroll if no id given', () => {
        const targetTop = 500;
        const targetLeft = 10;

        const dummy = document.createElement('div');
        const button = document.createElement('button');
        dummy.id = 'test-element-id';
        button.focus = jest.fn();
        dummy.append(button);
        dummy.getBoundingClientRect = jest.fn(() => ({top: targetTop, left: targetLeft} as DOMRect));
        jest.spyOn(document, 'getElementById').mockReturnValue(dummy);

        renderHook(() => useScrollToElement());

        expect(window.scrollTo).not.toHaveBeenCalled();
        expect(button.focus).not.toHaveBeenCalled();
    });

    it('should use default offset of 70 when offset is not specified', () => {
        const targetId = 'default-offset';
        const targetTop = 200;
        const targetLeft = 0;

        const dummy = document.createElement('div');
        dummy.id = targetId;
        dummy.getBoundingClientRect = jest.fn(() => ({top: targetTop, left: targetLeft} as DOMRect));
        jest.spyOn(document, 'getElementById').mockReturnValue(dummy);

        renderHook(() => useScrollToElement(targetId));

        expect(window.scrollTo).toHaveBeenCalledWith(targetLeft, targetTop - 70);
    });

    it('should not scroll when id is empty string', () => {
        jest.spyOn(document, 'getElementById').mockReturnValue(null);

        renderHook(() => useScrollToElement(''));

        expect(window.scrollTo).not.toHaveBeenCalled();
    });

    describe('scroll-parent branch', () => {
        let scrollParent: HTMLDivElement;
        let target: HTMLDivElement;

        beforeEach(() => {
            scrollParent = document.createElement('div');
            target = document.createElement('div');
            scrollParent.appendChild(target);
            document.body.appendChild(scrollParent);

            jest.spyOn(window, 'getComputedStyle').mockImplementation((el) => {
                if (el === scrollParent) return { overflowY: 'scroll' } as CSSStyleDeclaration;
                return { overflowY: 'visible' } as CSSStyleDeclaration;
            });
        });

        it('calls scrollParent.scrollTo with correct offset math and not window.scrollTo', () => {
            const targetId = 'scroll-parent-target';
            const offset = 50;
            target.id = targetId;

            scrollParent.scrollTo = jest.fn();
            Object.defineProperty(scrollParent, 'scrollTop', { value: 100, configurable: true });
            target.getBoundingClientRect = jest.fn(() => ({ top: 300 } as DOMRect));
            scrollParent.getBoundingClientRect = jest.fn(() => ({ top: 200 } as DOMRect));
            jest.spyOn(document, 'getElementById').mockReturnValue(target);

            renderHook(() => useScrollToElement(targetId, offset));

            // scrollTop + (elementTop - parentTop) - offset = 100 + (300 - 200) - 50 = 150
            expect(scrollParent.scrollTo).toHaveBeenCalledWith({ top: 150, behavior: 'smooth' });
            expect(window.scrollTo).not.toHaveBeenCalled();
        });

        it('still focuses the first focusable element when a scroll parent is found', () => {
            const targetId = 'scroll-parent-focus-target';
            target.id = targetId;

            const button = document.createElement('button');
            button.focus = jest.fn();
            target.appendChild(button);

            scrollParent.scrollTo = jest.fn();
            Object.defineProperty(scrollParent, 'scrollTop', { value: 0, configurable: true });
            target.getBoundingClientRect = jest.fn(() => ({ top: 100 } as DOMRect));
            scrollParent.getBoundingClientRect = jest.fn(() => ({ top: 0 } as DOMRect));
            jest.spyOn(document, 'getElementById').mockReturnValue(target);

            renderHook(() => useScrollToElement(targetId, 0));

            expect(scrollParent.scrollTo).toHaveBeenCalled();
            expect(button.focus).toHaveBeenCalled();
        });

        it('walks up multiple ancestor levels to find the scrollable parent', () => {
            const targetId = 'deep-scroll-parent-target';
            const intermediate = document.createElement('div');
            scrollParent.innerHTML = '';
            scrollParent.appendChild(intermediate);
            intermediate.appendChild(target);
            target.id = targetId;

            scrollParent.scrollTo = jest.fn();
            Object.defineProperty(scrollParent, 'scrollTop', { value: 0, configurable: true });
            target.getBoundingClientRect = jest.fn(() => ({ top: 200 } as DOMRect));
            scrollParent.getBoundingClientRect = jest.fn(() => ({ top: 100 } as DOMRect));
            jest.spyOn(document, 'getElementById').mockReturnValue(target);

            renderHook(() => useScrollToElement(targetId, 0));

            expect(scrollParent.scrollTo).toHaveBeenCalledWith({ top: 100, behavior: 'smooth' });
        });

        it('recognises overflowY "auto" as a scrollable parent', () => {
            const targetId = 'auto-overflow-target';
            target.id = targetId;

            jest.spyOn(window, 'getComputedStyle').mockImplementation((el) => {
                if (el === scrollParent) return { overflowY: 'auto' } as CSSStyleDeclaration;
                return { overflowY: 'visible' } as CSSStyleDeclaration;
            });

            scrollParent.scrollTo = jest.fn();
            Object.defineProperty(scrollParent, 'scrollTop', { value: 0, configurable: true });
            target.getBoundingClientRect = jest.fn(() => ({ top: 50 } as DOMRect));
            scrollParent.getBoundingClientRect = jest.fn(() => ({ top: 0 } as DOMRect));
            jest.spyOn(document, 'getElementById').mockReturnValue(target);

            renderHook(() => useScrollToElement(targetId, 0));

            expect(scrollParent.scrollTo).toHaveBeenCalledWith({ top: 50, behavior: 'smooth' });
        });
    });
});

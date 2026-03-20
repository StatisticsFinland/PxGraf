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

    it('should use default offset of 50 when offset is not specified', () => {
        const targetId = 'default-offset';
        const targetTop = 200;
        const targetLeft = 0;

        const dummy = document.createElement('div');
        dummy.id = targetId;
        dummy.getBoundingClientRect = jest.fn(() => ({top: targetTop, left: targetLeft} as DOMRect));
        jest.spyOn(document, 'getElementById').mockReturnValue(dummy);

        renderHook(() => useScrollToElement(targetId));

        expect(window.scrollTo).toHaveBeenCalledWith(targetLeft, targetTop - 50);
    });

    it('should not scroll when id is empty string', () => {
        jest.spyOn(document, 'getElementById').mockReturnValue(null);

        renderHook(() => useScrollToElement(''));

        expect(window.scrollTo).not.toHaveBeenCalled();
    });
});

const emotionClassPattern = /^css-[a-zA-Z0-9-]+$/;

const isDomContainer = (value: unknown): value is Element | DocumentFragment =>
    value instanceof Element || value instanceof DocumentFragment;

const hasEmotionClass = (element: Element): boolean =>
    (element.getAttribute('class') ?? '').split(/\s+/)
        .some(className => emotionClassPattern.test(className));

const getElements = (container: Element | DocumentFragment): Element[] => {
    const descendants = Array.from(container.querySelectorAll('[class]'));
    return container instanceof Element ? [container, ...descendants] : descendants;
};

const removeEmotionClasses = (container: Element | DocumentFragment): void => {
    getElements(container).forEach(element => {
        const stableClasses = (element.getAttribute('class') ?? '').split(/\s+/)
            .filter(className => !emotionClassPattern.test(className));

        if (stableClasses.length === 0) {
            element.removeAttribute('class');
        } else {
            element.setAttribute('class', stableClasses.join(' '));
        }
    });
};

const muiSnapshotSerializer: jest.SnapshotSerializerPlugin = {
    test(value: unknown): boolean {
        return isDomContainer(value) && getElements(value).some(hasEmotionClass);
    },
    serialize(value, config, indentation, depth, refs, printer): string {
        const stableValue = (value as Element | DocumentFragment).cloneNode(true) as Element | DocumentFragment;
        removeEmotionClasses(stableValue);
        return printer(stableValue, config, indentation, depth, refs);
    },
};

export default muiSnapshotSerializer;
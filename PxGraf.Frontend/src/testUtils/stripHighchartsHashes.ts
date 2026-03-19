import { prettyDOM } from '@testing-library/dom';

const stripHighchartsHashes = (html: string): string => {
    const parser = new DOMParser();
    const doc = parser.parseFromString(html, 'text/html');

    const elementsWithId = doc.querySelectorAll('[id^="highcharts-"]');
    elementsWithId.forEach((element) => {
        const currentId = element.getAttribute('id');
        const newId = currentId.replace(/highcharts-.*/, 'highcharts');
        element.setAttribute('id', newId);
    });

    const elementsWithClipPath = doc.querySelectorAll('[clip-path*="highcharts-"]');
    elementsWithClipPath.forEach((element) => {
        const currentClipPath = element.getAttribute('clip-path');
        const newClipPath = currentClipPath.replace(/#highcharts-\S*\)/, '#highcharts)');
        element.setAttribute('clip-path', newClipPath);
    });

    return doc.body.innerHTML;
};

const serializer = {
    test(value: unknown) {
        return value instanceof HTMLElement;
    },
    serialize(value: HTMLElement) {
        const modifiedHTML = stripHighchartsHashes(value.outerHTML);
        const parser = new DOMParser();
        const doc = parser.parseFromString(modifiedHTML, 'text/html');
        const modifiedContainer = doc.body.firstChild as HTMLElement;

        return prettyDOM(modifiedContainer, undefined, { highlight: false }) || "";
    },
};

export default serializer;


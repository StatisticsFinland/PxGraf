import { JSDOM } from 'jsdom';
import { prettyDOM } from '@testing-library/dom';

const stripHighchartsHashes = (html: string): any => {
    const dom = new JSDOM(html);
    const { document } = dom.window;

    const elementsWithId = document.querySelectorAll('[id^="highcharts-"]');
    elementsWithId.forEach((element) => {
        const currentId = element.getAttribute('id');
        const newId = currentId.replace(/highcharts-.*/, 'highcharts');
        element.setAttribute('id', newId);
    });

    const elementsWithClipPath = document.querySelectorAll('[clip-path*="highcharts-"]');
    elementsWithClipPath.forEach((element) => {
        const currentClipPath = element.getAttribute('clip-path');
        const newClipPath = currentClipPath.replace(/#highcharts-\S*\)/, '#highcharts)');
        element.setAttribute('clip-path', newClipPath);
    });

    return dom.serialize();
};

const serializer = {
    test(value: unknown) {
        return value instanceof HTMLElement;
    },
    serialize(value: HTMLElement) {
        const modifiedHTML = stripHighchartsHashes(value.outerHTML);
        const modifiedDOM = new JSDOM(modifiedHTML);
        const modifiedContainer = modifiedDOM.window.document.body.firstChild as HTMLElement;

        return prettyDOM(modifiedContainer, undefined, { highlight: false }) || "";
    },
};

export default serializer;


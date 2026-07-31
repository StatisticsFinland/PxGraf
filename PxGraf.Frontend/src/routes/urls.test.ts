import { urls } from './urls';

describe('urls', () => {
    describe('editor', () => {
        it('builds the editor path from a single-segment path', () => {
            expect(urls.editor(['table'])).toBe('/editor/table/');
        });

        it('builds the editor path from a multi-segment path', () => {
            expect(urls.editor(['a', 'b', 'table'])).toBe('/editor/a/b/table/');
        });
    });

    describe('tableTree', () => {
        it('is the root path', () => {
            expect(urls.tableTree).toBe('/');
        });
    });
});

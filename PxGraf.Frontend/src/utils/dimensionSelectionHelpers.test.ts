import { FilterType } from "types/query";
import { getDefaultFilter, queryTypeLabels } from "./dimensionSelectionHelpers";

describe('getDefaultFilter tests', () => {
    it('Should return the correct object on All', () => {
        const result = getDefaultFilter(FilterType.All);
        expect(result).toEqual({ type: FilterType.All });
    });

    it('Should return the correct object on Item', () => {
        const result = getDefaultFilter(FilterType.Item);
        expect(result).toEqual({ type: FilterType.Item, query: [] });
    });

    it('Should return the correct object on From', () => {
        const result = getDefaultFilter(FilterType.From);
        expect(result).toEqual({ type: FilterType.From, query: null });
    });

    it('Should return the correct object on Top', () => {
        const result = getDefaultFilter(FilterType.Top);
        expect(result).toEqual({ type: FilterType.Top, query: 1 });
    });
});

describe('queryTypeLabels tests', () => {
    it('Should have a label for every FilterType', () => {
        expect(queryTypeLabels[FilterType.All]).toBe('variableSelect.allFilter');
        expect(queryTypeLabels[FilterType.From]).toBe('variableSelect.fromFilter');
        expect(queryTypeLabels[FilterType.Top]).toBe('variableSelect.topFilter');
        expect(queryTypeLabels[FilterType.Item]).toBe('variableSelect.itemFilter');
    });

    it('Should have exactly four entries', () => {
        expect(Object.keys(queryTypeLabels)).toHaveLength(4);
    });
});
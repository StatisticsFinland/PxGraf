import { FilterType } from "types/query";
import { getDefaultFilter } from "./dimensionSelectionHelpers";

describe('getDefaultFilter tests', () => {
    it('Should return the correct object on All', () => {
        const result = getDefaultFilter(FilterType.All);
        expect(result).toBeTruthy();
        expect(result).toEqual({ type: FilterType.All });
    });

    it('Should return the correct object on Item', () => {
        const result = getDefaultFilter(FilterType.Item);
        expect(result).toBeTruthy();
        expect(result).toEqual({ type: FilterType.Item, query: [] });
    });

    it('Should return the correct object on From', () => {
        const result = getDefaultFilter(FilterType.From);
        expect(result).toBeTruthy();
        expect(result).toEqual({ type: FilterType.From, query: null });
    });

    it('Should return the correct object on Top', () => {
        const result = getDefaultFilter(FilterType.Top);
        expect(result).toBeTruthy();
        expect(result).toEqual({ type: FilterType.Top, query: 1 });
    });
});
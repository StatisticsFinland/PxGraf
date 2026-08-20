import { FilterType, IDimensionQuery, IVirtualValueDefinition, ISumDefinition } from '../types/query';
import {
    generateVirtualValueCode,
    validateNoCycles,
    generatePlaceholderNames,
    generatePlaceholderContentEdits,
    removeDeletedVirtualCodesFromItemFilter,
} from './virtualValueHelpers';

const makeVirtual = (code: string, operandCodes: string[] = []): ISumDefinition => ({
    type: 'sum',
    code,
    operandCodes,
});

describe('generateVirtualValueCode', () => {
    it('returns sum_1 for an empty list with sum operator', () => {
        expect(generateVirtualValueCode([], 'sum')).toBe('sum_1');
    });

    it('returns sum_2 when sum_1 already exists', () => {
        expect(generateVirtualValueCode([makeVirtual('sum_1')], 'sum')).toBe('sum_2');
    });

    it('returns sum_4 when sum_1 and sum_3 exist (next after max)', () => {
        expect(generateVirtualValueCode([makeVirtual('sum_1'), makeVirtual('sum_3')], 'sum')).toBe('sum_4');
    });

    it('returns division_1 when only sum codes exist (independent counter per operator)', () => {
        expect(generateVirtualValueCode([makeVirtual('sum_1'), makeVirtual('sum_2')], 'division')).toBe('division_1');
    });

    it('returns subtraction_1 when only non-matching codes exist', () => {
        expect(generateVirtualValueCode([makeVirtual('A'), makeVirtual('B')], 'subtraction')).toBe('subtraction_1');
    });
});

describe('validateNoCycles', () => {
    it('returns true for empty definitions', () => {
        expect(validateNoCycles([], [])).toBe(true);
    });

    it('returns true for a single definition depending on a real value', () => {
        const defs: IVirtualValueDefinition[] = [makeVirtual('virtual_1', ['real_A'])];
        expect(validateNoCycles(defs, ['real_A'])).toBe(true);
    });

    it('returns true for a valid chain: A depends on B (virtual), B depends on real', () => {
        const defs: IVirtualValueDefinition[] = [
            makeVirtual('virtual_A', ['virtual_B']),
            makeVirtual('virtual_B', ['real_X']),
        ];
        expect(validateNoCycles(defs, ['real_X'])).toBe(true);
    });

    it('returns false when a definition references itself (self-cycle)', () => {
        const defs: IVirtualValueDefinition[] = [makeVirtual('virtual_A', ['virtual_A'])];
        expect(validateNoCycles(defs, [])).toBe(false);
    });

    it('returns false for a mutual cycle: A depends on B, B depends on A', () => {
        const defs: IVirtualValueDefinition[] = [
            makeVirtual('virtual_A', ['virtual_B']),
            makeVirtual('virtual_B', ['virtual_A']),
        ];
        expect(validateNoCycles(defs, [])).toBe(false);
    });
});

describe('generatePlaceholderNames', () => {
    const translateForLang = (lang: string) => `placeholder_${lang}`;

    it('maps each language to the correct placeholder for sequentialNumber 1', () => {
        expect(generatePlaceholderNames(['en', 'fi'], translateForLang, 1)).toEqual({
            en: 'placeholder_en 1',
            fi: 'placeholder_fi 1',
        });
    });

    it('maps single language to the correct placeholder for sequentialNumber 3', () => {
        expect(generatePlaceholderNames(['en'], translateForLang, 3)).toEqual({
            en: 'placeholder_en 3',
        });
    });

    it('returns an empty object for an empty language list', () => {
        expect(generatePlaceholderNames([], translateForLang, 1)).toEqual({});
    });
});

describe('generatePlaceholderContentEdits', () => {
    const translateForLang = (lang: string) => `placeholder_${lang}`;

    it('maps each language to the placeholder text for both unitEdit and sourceEdit', () => {
        expect(generatePlaceholderContentEdits(['en', 'fi'], translateForLang)).toEqual({
            unitEdit: { en: 'placeholder_en', fi: 'placeholder_fi' },
            sourceEdit: { en: 'placeholder_en', fi: 'placeholder_fi' },
        });
    });

    it('works for a single language', () => {
        expect(generatePlaceholderContentEdits(['sv'], translateForLang)).toEqual({
            unitEdit: { sv: 'placeholder_sv' },
            sourceEdit: { sv: 'placeholder_sv' },
        });
    });

    it('returns empty objects for an empty language list', () => {
        expect(generatePlaceholderContentEdits([], translateForLang)).toEqual({
            unitEdit: {},
            sourceEdit: {},
        });
    });
});

const makeItemQuery = (queryCodes: string[], defs: IVirtualValueDefinition[]): IDimensionQuery => ({
    valueFilter: { type: FilterType.Item, query: queryCodes },
    selectable: false,
    virtualValueDefinitions: defs,
});

describe('removeDeletedVirtualCodesFromItemFilter', () => {
    it('removes a deleted virtual code from FilterType.Item query', () => {
        const previous: IVirtualValueDefinition[] = [makeVirtual('virtual_1')];
        const next = makeItemQuery(['2020', 'virtual_1'], []);
        const result = removeDeletedVirtualCodesFromItemFilter(previous, next);
        expect(result.valueFilter.query).toEqual(['2020']);
    });

    it('removes multiple deleted virtual codes from FilterType.Item query', () => {
        const previous: IVirtualValueDefinition[] = [
            makeVirtual('virtual_1'),
            makeVirtual('virtual_2'),
        ];
        const next = makeItemQuery(['virtual_1', '2020', 'virtual_2'], []);
        const result = removeDeletedVirtualCodesFromItemFilter(previous, next);
        expect(result.valueFilter.query).toEqual(['2020']);
    });

    it('preserves real value codes in FilterType.Item query', () => {
        const previous: IVirtualValueDefinition[] = [makeVirtual('virtual_1')];
        const next = makeItemQuery(['2018', '2020'], []);
        const result = removeDeletedVirtualCodesFromItemFilter(previous, next);
        expect(result.valueFilter.query).toEqual(['2018', '2020']);
    });

    it('returns unchanged for non-Item filter types', () => {
        const previous: IVirtualValueDefinition[] = [makeVirtual('virtual_1')];
        const next: IDimensionQuery = {
            valueFilter: { type: FilterType.All },
            selectable: false,
            virtualValueDefinitions: [],
        };
        const result = removeDeletedVirtualCodesFromItemFilter(previous, next);
        expect(result).toBe(next);
    });

    it('returns unchanged when no virtual definitions were deleted', () => {
        const def = makeVirtual('virtual_1');
        const next = makeItemQuery(['2020', 'virtual_1'], [def]);
        const result = removeDeletedVirtualCodesFromItemFilter([def], next);
        expect(result).toBe(next);
    });

    it('returns unchanged when valueFilter.query is undefined', () => {
        const previous: IVirtualValueDefinition[] = [makeVirtual('virtual_1')];
        const next: IDimensionQuery = {
            valueFilter: { type: FilterType.Item },
            selectable: false,
            virtualValueDefinitions: [],
        };
        const result = removeDeletedVirtualCodesFromItemFilter(previous, next);
        expect(result).toBe(next);
    });
});

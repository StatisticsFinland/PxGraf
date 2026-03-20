import { IFetchSavedQueryResponse } from "api/services/queries";
import { merge } from "lodash";
import { FilterType, ICubeQuery, Query } from "types/query";
import { buildCubeQuery, buildTableReference, defaultQueryOptions, extractCubeQuery, extractQuery, parseLanguageString, pxGrafUrl } from "./ApiHelpers";

const mockIdStack: string[] = ['foo', 'bar', 'baz'];
const mockMetaEdits: ICubeQuery = {
    variableQueries: {
        'foo': {
            valueEdits: {
                'bar': {
                    nameEdit: {
                        'fi': 'seppo',
                        'sv': 'jorma',
                        'en': 'pasi'
                    }
                }
            }
        }
    }
};

const mockQuery: Query = {
    'foo': {
        selectable: false,
        valueFilter: {
            type: FilterType.All
        },
        virtualValueDefinitions: ['foo', 'bar', 'baz']
    }
}

const mockBackendOldQueryRespons: IFetchSavedQueryResponse = {
    id: 'mock-id',
    draft: false,
    query: merge(
        { tableReference: buildTableReference(mockIdStack) },
        { variableQueries: mockQuery },
        mockMetaEdits
    ),
    settings: {}
}

describe('buildCubeQuery tests', () => {
    it('Should build an object', () => {
        const result = buildCubeQuery(mockQuery, mockMetaEdits, mockIdStack);
        expect(result).toHaveProperty('tableReference', { name: 'baz', hierarchy: ['foo', 'bar'] });
        expect(result).toHaveProperty('variableQueries.foo.selectable', false);
        expect(result).toHaveProperty('variableQueries.foo.valueEdits');
    });
});

describe('buildTableReference tests', () => {
    it('Should build an object', () => {
        const result = buildTableReference(mockIdStack);
        expect(result).toEqual({ name: 'baz', hierarchy: [ 'foo', 'bar' ] });
    });
});

jest.mock('envVars', () => ({
    PxGrafUrl: 'mockedUrl.fi/',
    BasePath: ''
}));

describe('pxGrafUrl tests', () => {
    it('Should build a correct format string', () => {
        const result: string = pxGrafUrl('foobar');
        expect(result).toEqual('mockedUrl.fi/foobar');
    });
});

describe('extractQuery tests', () => {
    it('Should extract a Query object', () => {
        const result = extractQuery(mockBackendOldQueryRespons);
        expect(result).toEqual(mockQuery);
    });
});

describe('extractCubeQuery tests', () => {
    it('Should extract a CubeQuery object', () => {
        const result = extractCubeQuery(mockBackendOldQueryRespons);
        expect(result).toEqual(mockMetaEdits);
    });
});

describe('defaultQueryOptions tests', () => {
    it('Should have retry set to false', () => {
        expect(defaultQueryOptions.retry).toBe(false);
    });

    it('Should have staleTime set to 60000ms', () => {
        expect(defaultQueryOptions.staleTime).toBe(60000);
    });
});

describe('parseLanguageString tests', () => {
    it('Should format a single language correctly', () => {
        expect(parseLanguageString(['fi'])).toBe('(FI)');
    });

    it('Should format multiple languages with comma separation', () => {
        expect(parseLanguageString(['fi', 'sv', 'en'])).toBe('(FI, SV, EN)');
    });

    it('Should return empty parens for an empty array', () => {
        expect(parseLanguageString([])).toBe('()');
    });
});

describe('buildTableReference edge cases', () => {
    it('Should handle a single-item idStack', () => {
        const result = buildTableReference(['onlyItem']);
        expect(result).toEqual({ name: 'onlyItem', hierarchy: [] });
    });
});

describe('buildCubeQuery edge cases', () => {
    it('Should handle an empty query object', () => {
        const emptyQuery: Query = {};
        const result = buildCubeQuery(emptyQuery, mockMetaEdits, mockIdStack);
        expect(result).toHaveProperty('tableReference');
        expect(result).toHaveProperty('variableQueries');
    });

    it('Should handle empty metaEdits', () => {
        const emptyEdits: ICubeQuery = { variableQueries: {} };
        const result = buildCubeQuery(mockQuery, emptyEdits, mockIdStack);
        expect(result).toHaveProperty('tableReference', { name: 'baz', hierarchy: ['foo', 'bar'] });
        expect(result).toHaveProperty('variableQueries.foo.selectable', false);
    });
});
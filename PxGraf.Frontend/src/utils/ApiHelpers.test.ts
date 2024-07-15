import { IFetchSavedQueryResponse } from "api/services/queries";
import { merge } from "lodash";
import { FilterType, ICubeQuery, Query } from "types/query";
import { buildCubeQuery, buildTableReference, extractCubeQuery, extractQuery, pxGrafUrl } from "./ApiHelpers";

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
        expect(result).toBeTruthy();
    });
});

describe('buildTableReference tests', () => {
    it('Should build an object', () => {
        const result = buildTableReference(mockIdStack);
        expect(result).toBeTruthy();
        expect(result).toEqual({ name: 'baz', hierarchy: [ 'foo', 'bar' ] });
    });
});

describe('pxGrafUrl tests', () => {
    const env = process.env

    beforeEach(() => {
        jest.resetModules()
        process.env = { ...env }
    })

    afterEach(() => {
        process.env = env
    })
    it('Should build a correct format string', () => {
        import.meta.env.VITE_PXGRAF_URL = 'seppo.fi/';
        const result = pxGrafUrl('foobar');
        expect(result).toBeTruthy();
        expect(result).toEqual('seppo.fi/foobar');
    });
});

describe('buildTableReference tests', () => {
    it('Should build an object', () => {
        const result = buildTableReference(mockIdStack);
        expect(result).toBeTruthy();
        expect(result).toEqual({ name: 'baz', hierarchy: [ 'foo', 'bar' ] });
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
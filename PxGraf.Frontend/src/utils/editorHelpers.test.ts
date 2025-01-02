import { IDimension, EDimensionType } from "types/cubeMeta";
import { getDefaultQueries, resolveDimensions } from "./editorHelpers";

const mockDimensions: IDimension[] = [
    {
        code: "foo",
        name: {
            'fi': 'nimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'foo',
                name: {
                    'fi': 'nimi'
                },
                isVirtual: false
            }
        ]
    }
];

describe('getDefaultQueries tests', () => {
    it('Should return the correct object', () => {
        const expected = {
            foo: {
                valueFilter: { type: 'item', query: [] },
                selectable: false,
                virtualValueDefinitions: null
            }
        }
        const result = getDefaultQueries(mockDimensions);
        expect(result).toBeTruthy();
        expect(result).toEqual(expected);
    });
});

describe('resolveVariables tests', () => {
    it('Should return the correct object', () => {
        const expected: IDimension[] = [{ code: 'foo', name: { fi: 'nimi' }, type: EDimensionType.Content, values: [] }];
        const result = resolveDimensions(mockDimensions, {'foo': ['bar', 'baz']});
        expect(result).toBeTruthy();
        expect(result).toEqual(expected);
    });
});
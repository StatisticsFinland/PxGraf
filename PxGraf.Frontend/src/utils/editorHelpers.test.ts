import { IDimension, EDimensionType } from "types/cubeMeta";
import { getDefaultQueries, resolveVariables } from "./editorHelpers";

const mockVariables: IDimension[] = [
    {
        Code: "foo",
        Name: {
            'fi': 'nimi'
        },
        Type: EDimensionType.Content,
        Values: [
            {
                Code: 'foo',
                Name: {
                    'fi': 'nimi'
                },
                IsVirtual: false
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
        const result = getDefaultQueries(mockVariables);
        expect(result).toBeTruthy();
        expect(result).toEqual(expected);
    });
});

describe('resolveVariables tests', () => {
    it('Should return the correct object', () => {
        const expected = [{ Code: 'foo', Name: { fi: 'nimi' }, Type: 'Content', Values: [] }];
        const result = resolveVariables(mockVariables, {'foo': ['bar', 'baz']});
        expect(result).toBeTruthy();
        expect(result).toEqual(expected);
    });
});
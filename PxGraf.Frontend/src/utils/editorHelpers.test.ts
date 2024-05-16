import { IVariable, VariableType } from "types/cubeMeta";
import { getDefaultQueries, resolveVariables } from "./editorHelpers";

const mockVariables: IVariable[] = [
    {
        code: 'foo',
        name: {
            'fi': 'nimi'
        },
        note: {
            'fi': 'nootti'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'foo',
                isSum: false,
                name: {
                    'fi': 'nimi'
                },
                note: {
                    'fi': 'nootti'
                }
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
        const expected = [ { code: 'foo', name: { fi: 'nimi' }, type: 'C', values: [] } ];
        const result = resolveVariables(mockVariables, {'foo': ['bar', 'baz']});
        expect(result).toBeTruthy();
        expect(result).toEqual(expected);
    });
});
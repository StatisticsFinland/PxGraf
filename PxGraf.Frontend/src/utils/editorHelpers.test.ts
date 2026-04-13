import { IDimension, EDimensionType } from "types/cubeMeta";
import { getDefaultQueries, getErrorText, getVisualizationOptionsForVisualizationType, resolveDimensions } from "./editorHelpers";
import { IVisualizationOptions } from "../types/editorContentsResponse";
import { VisualizationType } from "../types/visualizationType";
import { EDatabaseTableError } from "../types/tableListItems";

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

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

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
        expect(result).toEqual(expected);
    });
});

describe('resolveVariables tests', () => {
    it('Should return the correct object', () => {
        const expected: IDimension[] = [{ code: 'foo', name: { fi: 'nimi' }, type: EDimensionType.Content, values: [] }];
        const result = resolveDimensions(mockDimensions, {'foo': ['bar', 'baz']});
        expect(result).toEqual(expected);
    });
});

describe('getVisualizationOptionsForType tests', () => {
    it('Should return the correct object', () => {
        const mockVisualizationOptions: IVisualizationOptions[] = [
            {
                type: VisualizationType.LineChart,
                allowManualPivot: false,
                allowMultiselect: true,
                sortingOptions: {
                    default: null,
                    pivoted: null
                }
            },
            {
                type: VisualizationType.Table,
                allowManualPivot: true,
                allowMultiselect: false,
                sortingOptions: {
                    default: null,
                    pivoted: null
                }
            }
        ];
        const result = getVisualizationOptionsForVisualizationType(mockVisualizationOptions, VisualizationType.LineChart);
        expect(result).toEqual(mockVisualizationOptions[0]);
    });

    it('Should return undefined if type not found', () => {
        const mockVisualizationOptions: IVisualizationOptions[] = [];
        const result = getVisualizationOptionsForVisualizationType(mockVisualizationOptions, VisualizationType.LineChart);
        expect(result).toEqual(undefined);
    });

    it('Should return undefined when options is null', () => {
        const result = getVisualizationOptionsForVisualizationType(null, VisualizationType.LineChart);
        expect(result).toBeUndefined();
    });

    it('Should return undefined when options is undefined', () => {
        const result = getVisualizationOptionsForVisualizationType(undefined, VisualizationType.LineChart);
        expect(result).toBeUndefined();
    });
});

describe('getDefaultQueries edge cases', () => {
    it('Should return an empty object for an empty dimensions array', () => {
        const result = getDefaultQueries([]);
        expect(result).toEqual({});
    });
});

describe('resolveDimensions edge cases', () => {
    it('Should return dimensions with empty values when resolvedDimensionCodes is null', () => {
        const result = resolveDimensions(mockDimensions, null);
        expect(result).toHaveLength(1);
        expect(result[0].values).toEqual([]);
    });

    it('Should return dimensions with empty values when resolvedDimensionCodes is undefined', () => {
        const result = resolveDimensions(mockDimensions, undefined);
        expect(result).toHaveLength(1);
        expect(result[0].values).toEqual([]);
    });

    it('Should return all values when resolvedDimensionCodes includes all value codes', () => {
        const result = resolveDimensions(mockDimensions, { 'foo': ['foo'] });
        expect(result[0].values).toHaveLength(1);
        expect(result[0].values[0].code).toBe('foo');
    });

    it('Should handle empty dimensions array', () => {
        const result = resolveDimensions([], { 'foo': ['bar'] });
        expect(result).toEqual([]);
    });
});

describe('getErrorText tests', () => {
    const mockT = (key: string) => key;

    it('Should return contentVariableMissing for contentDimensionMissing error', () => {
        const result = getErrorText(EDatabaseTableError.contentDimensionMissing, mockT);
        expect(result).toBe('error.contentVariableMissing');
    });

    it('Should return timeVariableMissing for timeDimensionMissing error', () => {
        const result = getErrorText(EDatabaseTableError.timeDimensionMissing, mockT);
        expect(result).toBe('error.timeVariableMissing');
    });

    it('Should return contentLoad for default/unknown error', () => {
        const result = getErrorText(EDatabaseTableError.contentLoad, mockT);
        expect(result).toBe('error.contentLoad');
    });
});
import { IDimension, EDimensionType, IContentDimensionValue, EMetaPropertyType } from "types/cubeMeta";
import { getDefaultQueries, getErrorText, getVisualizationOptionsForVisualizationType, resolveDimensions, enrichDimensionsWithVirtualValues, getVirtualValueDefinitionsSignature } from "./editorHelpers";
import { IVisualizationOptions } from "../types/editorContentsResponse";
import { VisualizationType } from "../types/visualizationType";
import { EDatabaseTableError } from "../types/tableListItems";
import { FilterType, ICubeQuery, IDimensionQuery, ISumDefinition } from "types/query";

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
                virtualValueDefinitions: []
            }
        }
        const result = getDefaultQueries(mockDimensions);
        expect(result).toEqual(expected);
    });
});

describe('resolveVariables tests', () => {
    it('Should return the correct object', () => {
        const expected: IDimension[] = [{ code: 'foo', name: { fi: 'nimi' }, type: EDimensionType.Content, values: [
            { code: 'bar', name: {}, isVirtual: true },
            { code: 'baz', name: {}, isVirtual: true },
        ] }];
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

describe('enrichDimensionsWithVirtualValues tests', () => {
    const translateForLang = (lang: string, operator: string) => `${operator}_placeholder_${lang}`;

    it('Should return dimensions unchanged when query is null', () => {
        const result = enrichDimensionsWithVirtualValues(mockDimensions, null, [], translateForLang);
        expect(result).toEqual(mockDimensions);
    });

    it('Should return dimensions unchanged when no virtual definitions', () => {
        const query: { [key: string]: IDimensionQuery } = { foo: { valueFilter: { type: FilterType.Item, query: [] }, selectable: false, virtualValueDefinitions: [] } };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, [], translateForLang);
        expect(result).toEqual(mockDimensions);
    });

    it('Should append virtual values with placeholder names', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi'], translateForLang);
        expect(result[0].values).toHaveLength(2);
        expect(result[0].values[1].name).toEqual({ fi: 'sum_placeholder_fi 1' });
        expect(result[0].values[1].code).toBe('virtual_1');
        expect(result[0].values[1].isVirtual).toBe(true);
    });

    it('Should generate placeholder name even without any valueEdits', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi'], translateForLang);
        expect(result[0].values).toHaveLength(2);
        expect(result[0].values[1].name).toEqual({ fi: 'sum_placeholder_fi 1' });
    });

    it('Should handle empty dimensions array', () => {
        const query: { [key: string]: IDimensionQuery } = {};
        const result = enrichDimensionsWithVirtualValues([], query, [], translateForLang);
        expect(result).toEqual([]);
    });

    it('Should populate unit and additionalProperties for content dimension virtual values', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi'], translateForLang);
        const virtualValue = result[0].values[1] as IContentDimensionValue;
        expect(virtualValue.unit).toEqual({ fi: 'sum_placeholder_fi' });
        expect(virtualValue.additionalProperties?.['SOURCE']).toEqual({
            type: EMetaPropertyType.MultilanguageText,
            value: { fi: 'sum_placeholder_fi' },
        });
    });

    it('Should generate placeholder names with correct sequence numbers for multiple virtual values', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [
                    { type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition,
                    { type: 'sum', code: 'virtual_2', operandCodes: ['foo'] } as ISumDefinition,
                ]
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi'], translateForLang);
        expect(result[0].values).toHaveLength(3);
        expect(result[0].values[1].name).toEqual({ fi: 'sum_placeholder_fi 1' });
        expect(result[0].values[2].name).toEqual({ fi: 'sum_placeholder_fi 2' });
    });

    it('Should replace existing virtual stubs with enriched ones (no duplicates)', () => {
        const dimensionsWithStub: IDimension[] = [{
            code: 'foo',
            name: { fi: 'nimi' },
            type: EDimensionType.Content,
            values: [
                { code: 'foo', name: { fi: 'nimi' }, isVirtual: false },
                { code: 'virtual_1', name: {}, isVirtual: true }, // pre-existing empty stub
            ]
        }];
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const result = enrichDimensionsWithVirtualValues(dimensionsWithStub, query, ['fi'], translateForLang);
        // Should have exactly 2 values (1 real + 1 enriched virtual), not 3
        expect(result[0].values).toHaveLength(2);
        expect(result[0].values[1].code).toBe('virtual_1');
        expect(result[0].values[1].name).toEqual({ fi: 'sum_placeholder_fi 1' });
    });

    it('Should generate placeholder names for all provided languages', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi', 'en'], translateForLang);
        expect(result[0].values[1].name).toEqual({ fi: 'sum_placeholder_fi 1', en: 'sum_placeholder_en 1' });
    });

    it('Should use nameEdit from cubeQuery when provided', () => {
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition]
            }
        };
        const cubeQuery: ICubeQuery = {
            variableQueries: {
                foo: {
                    valueEdits: {
                        virtual_1: { nameEdit: { fi: 'Muokattu nimi' } }
                    }
                }
            }
        };
        const result = enrichDimensionsWithVirtualValues(mockDimensions, query, ['fi'], translateForLang, cubeQuery);
        expect(result[0].values[1].name).toEqual({ fi: 'Muokattu nimi' });
    });

    it('Should only enrich resolved virtual stubs when restrictToResolved is true', () => {
        const dimensionsWithOneStub: IDimension[] = [{
            code: 'foo',
            name: { fi: 'nimi' },
            type: EDimensionType.Nominal,
            values: [
                { code: 'foo', name: { fi: 'nimi' }, isVirtual: false },
                { code: 'virtual_1', name: {}, isVirtual: true }, // only virtual_1 is resolved
            ]
        }];
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [
                    { type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition,
                    { type: 'sum', code: 'virtual_2', operandCodes: ['foo'] } as ISumDefinition, // not resolved
                ]
            }
        };
        const result = enrichDimensionsWithVirtualValues(dimensionsWithOneStub, query, ['fi'], translateForLang, null, true);
        expect(result[0].values).toHaveLength(2); // 1 real + 1 resolved virtual (not virtual_2)
        expect(result[0].values.map(v => v.code)).toEqual(['foo', 'virtual_1']);
    });

    it('Should remove unresolved virtual stubs when restrictToResolved is true and no stubs are resolved', () => {
        const dimensionsWithNoStubs: IDimension[] = [{
            code: 'foo',
            name: { fi: 'nimi' },
            type: EDimensionType.Nominal,
            values: [
                { code: 'foo', name: { fi: 'nimi' }, isVirtual: false },
            ]
        }];
        const query: { [key: string]: IDimensionQuery } = {
            foo: {
                valueFilter: { type: FilterType.Item, query: [] },
                selectable: false,
                virtualValueDefinitions: [
                    { type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition,
                ]
            }
        };
        const result = enrichDimensionsWithVirtualValues(dimensionsWithNoStubs, query, ['fi'], translateForLang, null, true);
        expect(result[0].values).toHaveLength(1); // only the real value
        expect(result[0].values[0].code).toBe('foo');
    });
});

describe('getVirtualValueDefinitionsSignature tests', () => {
    const virtualValueDefinitions = [{ type: 'sum', code: 'virtual_1', operandCodes: ['foo'] } as ISumDefinition];

    it('stays unchanged when only dimension filters change', () => {
        const initialQuery: { [key: string]: IDimensionQuery } = {
            foo: { valueFilter: { type: FilterType.Item, query: ['value_1'] }, selectable: false, virtualValueDefinitions }
        };
        const changedQuery: { [key: string]: IDimensionQuery } = {
            foo: { valueFilter: { type: FilterType.Item, query: ['value_2'] }, selectable: false, virtualValueDefinitions }
        };

        expect(getVirtualValueDefinitionsSignature(mockDimensions, changedQuery))
            .toBe(getVirtualValueDefinitionsSignature(mockDimensions, initialQuery));
    });

    it('changes when virtual value definitions change', () => {
        const initialQuery: { [key: string]: IDimensionQuery } = {
            foo: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions: [] }
        };
        const changedQuery: { [key: string]: IDimensionQuery } = {
            foo: { valueFilter: { type: FilterType.All }, selectable: false, virtualValueDefinitions }
        };

        expect(getVirtualValueDefinitionsSignature(mockDimensions, changedQuery))
            .not.toBe(getVirtualValueDefinitionsSignature(mockDimensions, initialQuery));
    });
});

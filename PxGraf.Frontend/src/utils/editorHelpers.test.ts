import { IDimension, EDimensionType } from "types/cubeMeta";
import { getDefaultQueries, getVisualizationOptionsForType, resolveDimensions } from "./editorHelpers";
import { IVisualizationOptions } from "../types/editorContentsResponse";
import { VisualizationType } from "../types/visualizationType";

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
        const result = getVisualizationOptionsForType(mockVisualizationOptions, VisualizationType.LineChart);
        expect(result).toBeTruthy();
        expect(result).toEqual(mockVisualizationOptions[0]);
    });

    it('Should return undefined if type not found', () => {
        const mockVisualizationOptions: IVisualizationOptions[] = [];
        const result = getVisualizationOptionsForType(mockVisualizationOptions, VisualizationType.LineChart);
        expect(result).toEqual(undefined);
    });
});
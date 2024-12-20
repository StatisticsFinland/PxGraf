import { IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import { ISortingOption } from "types/visualizationRules";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import { getValidatedSettings } from "./ChartSettingHelpers";

const mockVisualizationSettings: IVisualizationSettings = {
    columnVariableCodes: ['foo', 'bar', 'baz'],
}

const mockDefaultTableVisualizationSettings: IVisualizationSettings = {
    rowVariableCodes: [],
    columnVariableCodes: []
}

const mockSortingOptions: ISortingOption[] = [
    {
        code: 'foo',
        description: {
            'fi': 'selite'
        }
    },
    {
        code: 'bar',
        description: {
            'fi': 'selite2'
        }
    },
    {
        code: 'baz',
        description: {
            'fi': 'selite3'
        }
    },
];

const mockZeroDimensions: IDimension[] = [];

const mockDimensions: IDimension[] = [
    {
        code: 'foo',
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

const mockTableTwoDimensions: IDimension[] = [
    {
        code: 'foo',
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
            },
            {
                code: 'bar',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'bar',
        name: {
            'fi': 'toinennimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'bar',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            },
            {
                code: 'lorem',
                name: {
                    'fi': 'kolmasnimi'
                },
                isVirtual: false
            },
            {
                code: 'ipsum',
                name: {
                    'fi': 'neljasnimi'
                },
                isVirtual: false
            }
        ]
    }
];

const mockTableThreeDimensions: IDimension[] = [
    {
        code: 'foo',
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
            },
            {
                code: 'bar',
                name: {
                    'fi': 'jokunimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'bar',
        name: {
            'fi': 'toinennimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'bar',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            },
            {
                code: 'lorem',
                name: {
                    'fi': 'kolmasnimi'
                },
                isVirtual: false
            },
            {
                code: 'ipsum',
                name: {
                    'fi': 'neljasnimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'baz',
        name: {
            'fi': 'kolmasnimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'foobar',
                name: {
                    'fi': 'nimi'
                },
                isVirtual: false
            },
            {
                code: 'loremipsum',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            }
        ]
    }
];

const mockTableFourDimensions: IDimension[] = [
    {
        code: 'foo',
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
            },
            {
                code: 'bar',
                name: {
                    'fi': 'jokunimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'bar',
        name: {
            'fi': 'toinennimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'bar',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            },
            {
                code: 'lorem',
                name: {
                    'fi': 'kolmasnimi'
                },
                isVirtual: false
            },
            {
                code: 'ipsum',
                name: {
                    'fi': 'neljasnimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'baz',
        name: {
            'fi': 'kolmasnimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'foobar',
                name: {
                    'fi': 'nimi'
                },
                isVirtual: false
            },
            {
                code: 'loremipsum',
                name: {
                    'fi': 'toinennimi'
                },
                isVirtual: false
            }
        ]
    },
    {
        code: 'foobar',
        name: {
            'fi': 'neljasnimi'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'barfoo',
                name: {
                    'fi': 'nimi'
                },
                isVirtual: false
            }
        ]
    }
];

const mockQuery: Query = {
    'foo': {
        selectable: false,
        valueFilter: {
            type: FilterType.All
        },
        virtualValueDefinitions: ['foo', 'bar', 'baz']
    }
}

const mockQueryWithSelectable: Query = {
    'foo': {
        selectable: true,
        valueFilter: {
            type: FilterType.All
        },
        virtualValueDefinitions: []
    }
}

describe('getValidatedSettings tests', () => {
    it('Should return the correct object on Table', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: [ 'foo' ], columnVariableCodes: [  ] });
    });

    it('Should not throw an exception with zero variables', () => {
        function testTableWithZeroVariables() {
            getValidatedSettings(mockVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockZeroDimensions, mockQuery);
        }
        expect(testTableWithZeroVariables).not.toThrow(TypeError);
    });

    it('Should return the correct default pivot on Table with two variables', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableTwoDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['bar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with three variables', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableThreeDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['baz', 'bar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with four variables and with a single value variable', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableFourDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['baz', 'bar', 'foobar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with four variables, with a single value variable and a selectable variable', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableFourDimensions, mockQueryWithSelectable);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['bar', 'foo', 'foobar'], columnVariableCodes: ['baz'] });
    });

    it('Should return the correct object on LineChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.LineChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ cutYAxis: false, multiselectableVariableCode: null, showDataPoints: false });
    });
    
    it('Should return the correct object on PieChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PieChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ showDataPoints: false, sorting: 'foo' });
    });

    it('Should return the correct object on VerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.VerticalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on GroupVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.GroupVerticalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on StackedVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.StackedVerticalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on PercentVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PercentVerticalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on HorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.HorizontalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', showDataPoints: false });
    });

    it('Should return the correct object on GroupHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.GroupHorizontalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on StackedHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.StackedHorizontalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on PercentHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PercentHorizontalBarChart, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on ScatterPlot', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.ScatterPlot, mockSortingOptions, mockDimensions, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ cutYAxis: false, markerSize: 100 });
    });
});
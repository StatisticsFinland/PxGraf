import { IDimension, VariableType } from "types/cubeMeta";
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

const mockZeroVariables: IDimension[] = [];

const mockVariables: IDimension[] = [
    {
        Code: 'foo',
        Name: {
            'fi': 'nimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foo',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            }
        ]
    }
];

const mockTableTwoVariables: IDimension[] = [
    {
        Code: 'foo',
        Name: {
            'fi': 'nimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foo',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            },
            {
                Code: 'bar',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'bar',
        Name: {
            'fi': 'toinennimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'bar',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            },
            {
                Code: 'lorem',
                Name: {
                    'fi': 'kolmasnimi'
                },
                Virtual: false
            },
            {
                Code: 'ipsum',
                Name: {
                    'fi': 'neljasnimi'
                },
                Virtual: false
            }
        ]
    }
];

const mockTableThreeVariables: IDimension[] = [
    {
        Code: 'foo',
        Name: {
            'fi': 'nimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foo',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            },
            {
                Code: 'bar',
                Name: {
                    'fi': 'jokunimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'bar',
        Name: {
            'fi': 'toinennimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'bar',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            },
            {
                Code: 'lorem',
                Name: {
                    'fi': 'kolmasnimi'
                },
                Virtual: false
            },
            {
                Code: 'ipsum',
                Name: {
                    'fi': 'neljasnimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'baz',
        Name: {
            'fi': 'kolmasnimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foobar',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            },
            {
                Code: 'loremipsum',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            }
        ]
    }
];

const mockTableFourVariables: IDimension[] = [
    {
        Code: 'foo',
        Name: {
            'fi': 'nimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foo',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            },
            {
                Code: 'bar',
                Name: {
                    'fi': 'jokunimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'bar',
        Name: {
            'fi': 'toinennimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'bar',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            },
            {
                Code: 'lorem',
                Name: {
                    'fi': 'kolmasnimi'
                },
                Virtual: false
            },
            {
                Code: 'ipsum',
                Name: {
                    'fi': 'neljasnimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'baz',
        Name: {
            'fi': 'kolmasnimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'foobar',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
            },
            {
                Code: 'loremipsum',
                Name: {
                    'fi': 'toinennimi'
                },
                Virtual: false
            }
        ]
    },
    {
        Code: 'foobar',
        Name: {
            'fi': 'neljasnimi'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'barfoo',
                Name: {
                    'fi': 'nimi'
                },
                Virtual: false
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
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: [ 'foo' ], columnVariableCodes: [  ] });
    });

    it('Should not throw an exception with zero variables', () => {
        function testTableWithZeroVariables() {
            getValidatedSettings(mockVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockZeroVariables, mockQuery);
        }
        expect(testTableWithZeroVariables).not.toThrow(TypeError);
    });

    it('Should return the correct default pivot on Table with two variables', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableTwoVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['bar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with three variables', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableThreeVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['baz', 'bar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with four variables and with a single value variable', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableFourVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['baz', 'bar', 'foobar'], columnVariableCodes: ['foo'] });
    });

    it('Should return the correct default pivot on Table with four variables, with a single value variable and a selectable variable', () => {
        const result = getValidatedSettings(mockDefaultTableVisualizationSettings, VisualizationType.Table, mockSortingOptions, mockTableFourVariables, mockQueryWithSelectable);
        expect(result).toBeTruthy();
        expect(result).toEqual({ rowVariableCodes: ['bar', 'foo', 'foobar'], columnVariableCodes: ['baz'] });
    });

    it('Should return the correct object on LineChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.LineChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ cutYAxis: false, multiselectableVariableCode: null, showDataPoints: false });
    });
    
    it('Should return the correct object on PieChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PieChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ showDataPoints: false, sorting: 'foo' });
    });

    it('Should return the correct object on VerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.VerticalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on GroupVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.GroupVerticalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on StackedVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.StackedVerticalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on PercentVerticalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PercentVerticalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ pivotRequested: false, matchXLabelsToEnd: false, showDataPoints: false });
    });

    it('Should return the correct object on HorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.HorizontalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', showDataPoints: false });
    });

    it('Should return the correct object on GroupHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.GroupHorizontalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on StackedHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.StackedHorizontalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on PercentHorizontalBarChart', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.PercentHorizontalBarChart, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ sorting: 'foo', pivotRequested: false, showDataPoints: false });
    });

    it('Should return the correct object on ScatterPlot', () => {
        const result = getValidatedSettings(mockVisualizationSettings, VisualizationType.ScatterPlot, mockSortingOptions, mockVariables, mockQuery);
        expect(result).toBeTruthy();
        expect(result).toEqual({ cutYAxis: false, markerSize: 100 });
    });
});
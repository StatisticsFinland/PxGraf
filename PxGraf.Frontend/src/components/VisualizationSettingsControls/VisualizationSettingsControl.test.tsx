import React from 'react';
import '@testing-library/jest-dom';
import { queryByLabelText, render, fireEvent } from '@testing-library/react';
import { IDimension, EDimensionType } from "types/cubeMeta";
import VisualizationSettingControl from "./VisualizationSettingsControl";
import { FilterType, Query } from "types/query";

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
}));

const mockTypeSpecificVisualizationRules = {
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true
}

const mockVisualizationRules = {
    allowManualPivot: false,
    sortingOptions: null,
    multiselectVariableAllowed: false,
    visualizationTypeSpecificRules: mockTypeSpecificVisualizationRules
};

const mockVisualizationSettings = {
    defaultSelectableVariableCodes: null,
    pivotRequested: null,
    cutYAxis: null,
    multiselectableVariableCode: null,
    rowVariableCodes: ["foobar1", "foobar3"],
    columnVariableCodes: ["foobar2", "foobar4"],
    sorting: null,
    matchXLabelsToEnd: false,
    markerSize: null,
    showDataPoints: false
}

const mockVariables: IDimension[]  = [
    {
        code: "foobar1",
        name: { fi: "foo1", sv: "bar1", en: "foobar1" },
        type: EDimensionType.Other,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                isVirtual: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                isVirtual: false
            }
        ],
    },
    {
        code: "foobar2",
        name: { fi: "foo2", sv: "bar2", en: "foobar2" },
        type: EDimensionType.Other,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                isVirtual: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                isVirtual: false
            }
        ],
    },
    {
        code: "foobar3",
        name: { fi: "foo3", sv: "bar3", en: "foobar3" },
        type: EDimensionType.Other,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                isVirtual: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                isVirtual: false
            }
        ],
    },
    {
        code: "foobar4",
        name: { fi: "foo4", sv: "bar4", en: "foobar4" },
        type: EDimensionType.Other,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                isVirtual: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                isVirtual: false
            }
        ],
    }
]

const mockSettingsChangedHandler = jest.fn();

const mockVariableQuery: Query = {
    'foobar4': {
        selectable: false,
        valueFilter: {
            type: FilterType.Item,
            query: 'barfoo1'
        },
        virtualValueDefinitions: [
            'asd',
            '123'
        ]
    },
    'foobar3': {
        selectable: false,
        valueFilter: {
            type: FilterType.Item,
            query: 'barfoo1'
        },
        virtualValueDefinitions: [
            'asd',
            '123'
        ]
    },
    'foobar2': {
        selectable: false,
        valueFilter: {
            type: FilterType.Item,
            query: 'barfoo1'
        },
        virtualValueDefinitions: [
            'asd',
            '123'
        ]
    },
    'foobar1': {
        selectable: false,
        valueFilter: {
            type: FilterType.Item,
            query: 'barfoo1'
        },
        virtualValueDefinitions: [
            'asd',
            '123'
        ]
    }
}

describe('Rendering test', () => {
    it('renders table correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="Table"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders linechart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="LineChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders piechart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="PieChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders verticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="VerticalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders groupverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="GroupVerticalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders stackedverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="StackedVerticalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders percentverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="PercentVerticalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders horizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="HorizontalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders grouphorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="GroupHorizontalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders stackedhorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="StackedHorizontalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders percenthorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="PercentHorizontalBarChart"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders scatterplot correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization="ScatterPlot"
            variableQuery={mockVariableQuery}
            variables={mockVariables}
            visualizationRules={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
            settingsChangedHandler={mockSettingsChangedHandler}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows the proper components based on visualizationRules', () => {
        const modifiedVisualizationRules = {
            ...mockVisualizationRules,
            visualizationTypeSpecificRules: {
                ...mockVisualizationRules.visualizationTypeSpecificRules,
                allowShowingDataPoints: false,
                allowSetMarkerScale: false
            }
        };

        const { getByLabelText } = render(
            <VisualizationSettingControl
                selectedVisualization="Table"
                variableQuery={mockVariableQuery}
                variables={mockVariables}
                visualizationRules={modifiedVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
            />
        );
        expect(getByLabelText('chartSettings.cutYAxis')).toBeInTheDocument();
        expect(getByLabelText('chartSettings.matchXLabelsToEnd')).toBeInTheDocument();
        expect(queryByLabelText(document.body, 'chartSettings.markerScale')).not.toBeInTheDocument();
        expect(queryByLabelText(document.body, 'visualizationSettings.showDataPoints')).not.toBeInTheDocument();
        expect(queryByLabelText(document.body, 'chartSettings.multiSelectVariable')).not.toBeInTheDocument();
    });

    it('updates values properly when user changes switches', () => {
        const { getByLabelText } = render(
            <VisualizationSettingControl
                selectedVisualization="Table"
                variableQuery={mockVariableQuery}
                variables={mockVariables}
                visualizationRules={mockVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
            />
        );

        fireEvent.click(getByLabelText('visualizationSettings.showDataPoints'));
        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({
            ...mockVisualizationSettings,
            showDataPoints: !mockVisualizationSettings.showDataPoints
        });
    });
});
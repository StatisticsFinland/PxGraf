import React from 'react';
import '@testing-library/jest-dom';
import { queryByLabelText, render, fireEvent } from '@testing-library/react';
import { IDimension, EDimensionType } from "types/cubeMeta";
import VisualizationSettingControl from "./VisualizationSettingsControl";
import { FilterType, Query } from "types/query";
import { IVisualizationSettings } from '../../types/visualizationSettings';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';
import { IVisualizationOptions } from '../../types/editorContentsResponse';

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

const mockVisualizationRules: IVisualizationOptions = {
    allowManualPivot: false,
    sortingOptions: {
        default: [],
        pivoted: []
    },
    allowMultiselect: false,
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true,
    type: VisualizationType.VerticalBarChart
};

const mockVisualizationSettings: IVisualizationSettings = {
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

const mockDimensions: IDimension[]  = [
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

const mockDimensionQuery: Query = {
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
            selectedVisualization={VisualizationType.Table}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders linechart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.LineChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders piechart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.PieChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders verticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.VerticalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders groupverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.GroupVerticalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders stackedverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.StackedVerticalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders percentverticalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.PercentVerticalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders horizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.HorizontalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders grouphorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.HorizontalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders stackedhorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.StackedHorizontalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders percenthorizontalbarchart correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.PercentHorizontalBarChart}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders scatterplot correctly', () => {
        const { asFragment } = render(<VisualizationSettingControl
            selectedVisualization={VisualizationType.ScatterPlot}
            dimensionQuery={mockDimensionQuery}
            dimensions={mockDimensions}
            visualizationOptions={mockVisualizationRules}
            visualizationSettings={mockVisualizationSettings}
        />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows the proper components based on visualizationRules', () => {
        const modifiedVisualizationRules = {
            ...mockVisualizationRules,
            allowShowingDataPoints: false,
            allowSetMarkerScale: false
        };

        const { getByLabelText } = render(
            <VisualizationSettingControl
                selectedVisualization={VisualizationType.Table}
                dimensionQuery={mockDimensionQuery}
                dimensions={mockDimensions}
                visualizationOptions={modifiedVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
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
            <EditorContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: false,
                setSaveDialogOpen: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: mockSettingsChangedHandler,
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: jest.fn(),
                publicationEnabled: true,
                setPublicationEnabled: jest.fn()
            }}>
                <VisualizationSettingControl
                    selectedVisualization={VisualizationType.Table}
                    dimensionQuery={mockDimensionQuery}
                    dimensions={mockDimensions}
                    visualizationOptions={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                />
            </EditorContext.Provider>
        );

        fireEvent.click(getByLabelText('visualizationSettings.showDataPoints'));
        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({
            ...mockVisualizationSettings,
            showDataPoints: !mockVisualizationSettings.showDataPoints
        });
    });
});
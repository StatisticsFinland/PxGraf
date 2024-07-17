import React from 'react';
import { render } from '@testing-library/react';
import { IMenuProps, SelectableVariableMenus, ISelectionState } from './SelectableVariableMenus';
import '@testing-library/jest-dom';
import { IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { EVariableType } from '@statisticsfinland/pxvisualizer';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { getSelections } from './SelectableVariableMenus';
import { ISelectabilityInfo } from '../Preview/Preview';
import { VariableType } from 'types/cubeMeta'; 

const mockSetSelections = jest.fn((selections: IMenuProps | ((prevState: IMenuProps) => IMenuProps)) => {
    return;
});

const mockSelectables: ISelectabilityInfo[] = [
    {
        variable: {
            code: "foobar1",
            name: { fi: "foo1", sv: "bar1", en: "foobar1" },
            note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
            type: VariableType.OtherClassificatory,
            values: [{
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false,
                contentComponent: null,
            }, {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false,
                contentComponent: null,
            }],
        },
        multiselectable: false,
    },
    {
        variable: {
            code: "foobar2",
            name: { fi: "foo2", sv: "bar2", en: "foobar2" },
            note: { fi: "föö2", sv: "bär2", en: "fööbär2" },
            type: VariableType.OtherClassificatory,
            values: [{
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false,
                contentComponent: null,
            }, {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false,
                contentComponent: null,
            }],
        },
        multiselectable: false,
    }
]

const mockVisualizationSettings: IVisualizationSettings = {
    defaultSelectableVariableCodes: { "foobar1": ["barfoo2"] },
    pivotRequested: false,
    cutYAxis: false,
    multiselectableVariableCode: "someCode",
    rowVariableCodes: ["rowVar1", "rowVar2"],
    columnVariableCodes: ["colVar1", "colVar2"],
    sorting: "someSorting",
    matchXLabelsToEnd: false,
    markerSize: 10,
    selectedVisualization: VisualizationType.HorizontalBarChart,
};

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const mockData: IQueryVisualizationResponse = {
    data: [],
    missingDataInfo: {},
    dataNotes: [],
    rowVariableCodes: [],
    columnVariableCodes: [],
    header: { fi: "foo", sv: "bar", en: "foobar" },
    selectableVariableCodes: ["foobar1", "foobar2"],
    visualizationSettings: null,
    metaData: [{
        code: "foobar1",
        name: { fi: "foo1", sv: "bar1", en: "foobar1" },
        note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
        type: EVariableType.OtherClassificatory,
        values: [{
            code: "barfoo1",
            name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
            note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
            isSum: false,
            contentComponent: null,
        }, {
            code: "barfoo2",
            name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
            note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
            isSum: false,
            contentComponent: null,
        }],
    }, {
        code: "foobar2",
        name: { fi: "foo2", sv: "bar2", en: "foobar2" },
        note: { fi: "föö2", sv: "bär2", en: "fööbär2" },
        type: EVariableType.OtherClassificatory,
        values: [{
            code: "barfoo1",
            name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
            note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
            isSum: false,
            contentComponent: null,
        }, {
            code: "barfoo2",
            name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
            note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
            isSum: false,
            contentComponent: null,
        }],
    }],
    tableReference: { hierarchy: ["foo", "bar"], name:  "foobar_table" }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <SelectableVariableMenus
                data={mockData}
                setSelections={mockSetSelections}
                visualizationSettings={mockVisualizationSettings}
                selectedVisualization='foobar' />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('applies default selectable variable value if selections have not been manually changed', () => {
        const initialSelectionState = {
            activeSelections: {},
            visualization: 'HorizontalBarChart',
            multiselect: null,
            manuallyChanged: {},
        };

        const selections = getSelections(
            mockSelectables,
            'HorizontalBarChart',
            mockVisualizationSettings,
            initialSelectionState,
            jest.fn()
        )

        expect(selections['foobar1']).toEqual(mockVisualizationSettings.defaultSelectableVariableCodes['foobar1']);
    });

    it('applies first available variable value if default value has not been set', () => {
        const initialSelectionState = {
            activeSelections: {},
            visualization: 'HorizontalBarChart',
            multiselect: null,
            manuallyChanged: {},
        };

        const selections = getSelections(
            mockSelectables,
            'HorizontalBarChart',
            mockVisualizationSettings,
            initialSelectionState,
            jest.fn()
        )

        const foobar2Var = mockData.metaData[1];

        expect(selections[foobar2Var.code]).toEqual([foobar2Var.values[0].code]);
    });

    it('retains manually changed selections over default selectable variable values', () => {
        const initialSelectionState: ISelectionState = {
            activeSelections: {
                'foobar1': ['barfoo1'],
                'foobar2': ['barfoo2'],
            },
            visualization: 'HorizontalBarChart',
            multiselect: "someCode",
            manuallyChanged: {
                'foobar1': true,
                'foobar2': true,
            },
        };

        const selections = getSelections(
            mockSelectables,
            "HorizontalBarChart",
            mockVisualizationSettings,
            initialSelectionState,
            jest.fn(),
        )

        expect(selections['foobar1']).toEqual(['barfoo1']);
        expect(selections['foobar2']).toEqual(['barfoo2']);
    });
});

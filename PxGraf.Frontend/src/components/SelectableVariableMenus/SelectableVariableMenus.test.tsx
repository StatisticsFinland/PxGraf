import { render } from '@testing-library/react';
import { IMenuProps, SelectableVariableMenus } from './SelectableVariableMenus';
import React from 'react';
import { IQueryVisualizationResponse } from '@statisticsfinland/pxvisualizer';
import { EVariableType } from '@statisticsfinland/pxvisualizer';
import { IVisualizationSettings } from 'types/visualizationSettings';
import { VisualizationType } from 'types/visualizationType';
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockSetSelections = jest.fn((selections: IMenuProps | ((prevState: IMenuProps) => IMenuProps)) => {
    return;
});

const mockVisualizationSettings: IVisualizationSettings = {
    defaultSelectableVariableCodes: { "key": ["value"] },
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
import { render } from "@testing-library/react";
import { IHeaderResult } from "api/services/default-header";
import { IVisualizationSettingsResult } from "api/services/visualization-rules";
import React from "react";
import { IVariable, VariableType } from "types/cubeMeta";
import { FilterType, IQueryInfo, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import EditorMetaSection from "./EditorMetaSection";
import { ITypeSpecificVisualizationRules } from "types/visualizationRules";
import { UiLanguageContext } from "contexts/uiLanguageContext";

const headerResultMock: IHeaderResult = {
    isError: false,
    isLoading: false,
    data: {
        'fi': 'foo'
    }
}

const mockVariables: IVariable[] = [{
    code: 'foo',
    name: {
        'fi': 'asd',
        'sv': 'asd',
        'en': 'asd'
    },
    note: {
        'fi': 'seppo',
        'sv': 'seppo',
        'en': 'seppo'
    },
    type: VariableType.Content,
    values: [
        {
            code: 'bar',
            isSum: false,
            name: {
                'fi': 'fgfgfg',
                'sv': 'fgfgfg',
                'en': 'fgfgfg'
            },
            note: {
                'fi': 'fghjfgh',
                'sv': 'fghjfgh',
                'en': 'fghjfgh'
            }
        }
    ]
}]

const mockVisualizationSettings: IVisualizationSettings = {
    columnVariableCodes: ['foo', 'bar', 'baz'],
    cutYAxis: false,
    defaultSelectableVariableCodes: {
        'foo': ['bar', 'baz']
    },
    markerSize: 1,
    matchXLabelsToEnd: false,
    multiselectableVariableCode: 'foo',
    pivotRequested: false,
    rowVariableCodes: ['foo', 'bar', 'baz']
}

const mockQuery: Query = {
    'foo': {
        valueFilter: {
            type: FilterType.Top,
            query: 4
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    'bar': {
        valueFilter: {
            type: FilterType.Item,
            query: [
                "kulutus_t"
            ]
        },
        selectable: false,
        virtualValueDefinitions: null
    }
};

const mockTypeSpecificVisualizationRules: ITypeSpecificVisualizationRules = {
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true
}

const visualizationRulesResponseMock: IVisualizationSettingsResult = {
    isLoading: false,
    isError: false,
    data: {
        allowManualPivot: false,
        multiselectVariableAllowed: false,
        sortingOptions: [
            {
                code: 'foo',
                description: {
                    'fi': 'bar'
                }
            }
        ],
        visualizationTypeSpecificRules: mockTypeSpecificVisualizationRules
    }
}

const mockQueryInfo: IQueryInfo = {
    maximumHeaderLength: 120,
    maximumSupportedSize: 200,
    size: 5,
    sizeWarningLimit: 175,
    validVisualizations: ['lineChart', 'foo', 'bar', 'baz'],
    visualizationRejectionReasons: {'pieChart': {
        'fi': 'huono kaavio'
    }
}}

const selectedVisualizationMock: VisualizationType = VisualizationType.LineChart;

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => undefined),
            },
        };
    },
}));

const language = "fi";
const setLanguage = jest.fn();
const languageTab = "fi";
const setLanguageTab = jest.fn();
const availableUiLanguages = ["fi", "sv", "en"];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorMetaSection
                    defaultHeaderResponse={headerResultMock}
                    resolvedVariables={mockVariables}
                    selectedVisualization={selectedVisualizationMock}
                    settings={mockVisualizationSettings}
                    variableQuery={mockQuery}
                    visualizationRulesResponse={visualizationRulesResponseMock}
                    queryInfo={mockQueryInfo}
                    contentLanguages={["fi", "sv", "en"]}
                />
                </UiLanguageContext.Provider>
            );
        expect(asFragment()).toMatchSnapshot();
    });
});
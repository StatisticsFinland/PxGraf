
import EditorMetaSection from "./EditorMetaSection";
import { render } from "@testing-library/react";
import { IHeaderResult } from "api/services/default-header";
import { IVisualizationSettingsResult } from "api/services/visualization-rules";
import { IVariable, VariableType } from "types/cubeMeta";
import { FilterType, IQueryInfo, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import { ITypeSpecificVisualizationRules } from "types/visualizationRules";
import { UiLanguageContext } from "contexts/uiLanguageContext";

const headerResultMock: IHeaderResult = {
    isError: false,
    isLoading: false,
    data: {
        'fi': 'foo'
    }
}

const mockVariables: IVariable[] = [
    {
        code: 'foo',
        name: {
            'fi': 'foofi',
            'sv': 'foosv',
            'en': 'fooen'
        },
        note: {
            'fi': 'foonotefi',
            'sv': 'foonotesv',
            'en': 'foonoteen'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'fooval1',
                isSum: false,
                name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                note: {
                    'fi': 'fghjfgh1',
                    'sv': 'fghjfgh1',
                    'en': 'fghjfgh1'
                }
            },
            {
                code: 'fooval2',
                isSum: false,
                name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                note: {
                    'fi': 'fghjfgh2',
                    'sv': 'fghjfgh2',
                    'en': 'fghjfgh2'
                }
            }
        ]
    },
    {
        code: 'bar',
        name: {
            'fi': 'barfi',
            'sv': 'barsv',
            'en': 'baren'
        },
        note: {
            'fi': 'barnotefi',
            'sv': 'barnotesv',
            'en': 'barnoteen'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'barval1',
                isSum: false,
                name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                note: {
                    'fi': 'fghjfgh1',
                    'sv': 'fghjfgh1',
                    'en': 'fghjfgh1'
                }
            },
            {
                code: 'barval2',
                isSum: false,
                name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                note: {
                    'fi': 'fghjfgh2',
                    'sv': 'fghjfgh2',
                    'en': 'fghjfgh2'
                }
            }
        ]
    }
]

const mockVisualizationSettings: IVisualizationSettings = {
    columnVariableCodes: ['foo'],
    rowVariableCodes: ['bar'],
    cutYAxis: false,
    defaultSelectableVariableCodes: {
        'foo': ['fooval1']
    },
    markerSize: 1,
    matchXLabelsToEnd: false,
    multiselectableVariableCode: 'foo',
    pivotRequested: false,
    sorting: 'DESCENDING'
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
                code: 'DESCENDING',
                description: {
                    'fi': 'aaa',
                    'en': 'bbb',
                    'sv': 'ccc'
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
    validVisualizations: ['HorizontalBarChart'],
    visualizationRejectionReasons: {'pieChart': {
        'fi': 'huono kaavio'
    }
}}

const selectedVisualizationMock: VisualizationType = VisualizationType.HorizontalBarChart

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
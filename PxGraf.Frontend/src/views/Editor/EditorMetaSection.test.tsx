import React from 'react';
import EditorMetaSection from "./EditorMetaSection";
import { render } from "@testing-library/react";
import { IHeaderResult } from "api/services/default-header";
import { IVisualizationSettingsResult } from "api/services/visualization-rules";
import { IDimension, VariableType } from "types/cubeMeta";
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

const mockVariables: IDimension[] = [
    {
        Code: 'foo',
        Name: {
            'fi': 'foofi',
            'sv': 'foosv',
            'en': 'fooen'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'fooval1',
                Name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                Virtual: false,
                Unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                Precision: 0,
                LastUpdated: '2021-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            },
            {
                Code: 'fooval2',
                Name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                Virtual: false,
                Unit: {
                    'fi': 'prosenttia',
                    'sv': 'procent',
                    'en': 'percent'
                },
                Precision: 1,
                LastUpdated: '2022-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            }
        ]
    },
    {
        Code: 'bar',
        Name: {
            'fi': 'barfi',
            'sv': 'barsv',
            'en': 'baren'
        },
        Type: VariableType.Content,
        Values: [
            {
                Code: 'barval1',
                Name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                Virtual: false,
                Unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                Precision: 0,
                LastUpdated: '2021-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            },
            {
                Code: 'barval2',
                Name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                Virtual: false,
                Unit: {
                    'fi': 'prosenttia',
                    'sv': 'procent',
                    'en': 'percent'
                },
                Precision: 1,
                LastUpdated: '2022-01-01',
                AdditionalProperties: {
                    SOURCE: {
                        KeyWord: 'SOURCE',
                        CanGetStringValue: false,
                        CanGetMultilanguageValue: true,
                        Entries: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
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
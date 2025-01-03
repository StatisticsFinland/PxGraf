import React from 'react';
import EditorMetaSection from "./EditorMetaSection";
import { render } from "@testing-library/react";
import { IHeaderResult } from "api/services/default-header";
import { IVisualizationSettingsResult } from "api/services/visualization-rules";
import { EMetaPropertyType, IDimension, EDimensionType } from "types/cubeMeta";
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

const mockDimensions: IDimension[] = [
    {
        code: 'foo',
        name: {
            'fi': 'foofi',
            'sv': 'foosv',
            'en': 'fooen'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'fooval1',
                name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                isVirtual: false,
                unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                precision: 0,
                lastUpdated: '2021-01-01',
                additionalProperties: {
                    SOURCE: {
                        type: EMetaPropertyType.MultilanguageText,
                        value: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            },
            {
                code: 'fooval2',
                name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                isVirtual: false,
                unit: {
                    'fi': 'prosenttia',
                    'sv': 'procent',
                    'en': 'percent'
                },
                precision: 1,
                lastUpdated: '2022-01-01',
                additionalProperties: {
                    SOURCE: {
                        type: EMetaPropertyType.MultilanguageText,
                        value: {
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
        code: 'bar',
        name: {
            'fi': 'barfi',
            'sv': 'barsv',
            'en': 'baren'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'barval1',
                name: {
                    'fi': 'fgfgfg1',
                    'sv': 'fgfgfg1',
                    'en': 'fgfgfg1'
                },
                isVirtual: false,
                unit: {
                    'fi': 'yksikko',
                    'sv': 'enhet',
                    'en': 'unit'
                },
                precision: 0,
                lastUpdated: '2021-01-01',
                additionalProperties: {
                    SOURCE: {
                        type: EMetaPropertyType.MultilanguageText,
                        value: {
                            'fi': 'lahde',
                            'sv': 'kalla',
                            'en': 'source'
                        }
                    }
                }
            },
            {
                code: 'barval2',
                name: {
                    'fi': 'fgfgfg2',
                    'sv': 'fgfgfg2',
                    'en': 'fgfgfg2'
                },
                isVirtual: false,
                unit: {
                    'fi': 'prosenttia',
                    'sv': 'procent',
                    'en': 'percent'
                },
                precision: 1,
                lastUpdated: '2022-01-01',
                additionalProperties: {
                    SOURCE: {
                        type: EMetaPropertyType.MultilanguageText,
                        value: {
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
        multiselectDimensionAllowed: false,
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
                    resolvedDimensions={mockDimensions}
                    selectedVisualization={selectedVisualizationMock}
                    settings={mockVisualizationSettings}
                    dimensionQuery={mockQuery}
                    visualizationRulesResponse={visualizationRulesResponseMock}
                    queryInfo={mockQueryInfo}
                    contentLanguages={["fi", "sv", "en"]}
                />
                </UiLanguageContext.Provider>
            );
        expect(asFragment()).toMatchSnapshot();
    });
});
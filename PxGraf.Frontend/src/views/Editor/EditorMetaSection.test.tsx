import React from 'react';
import EditorMetaSection from "./EditorMetaSection";
import { render } from "@testing-library/react";
import { EMetaPropertyType, IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import { VisualizationType } from "types/visualizationType";
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { IEditorContentsResponse, IVisualizationOptions } from '../../types/editorContentsResponse';

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

const visualizationOptions: IVisualizationOptions = {
    allowManualPivot: false,
    allowMultiselect: false,
    sortingOptions: {
        default: [
            {
                code: 'DESCENDING',
                description: {
                    'fi': 'aaa',
                    'en': 'bbb',
                    'sv': 'ccc'
                }
            }
        ],
        pivoted: []
    },
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true,
    type: VisualizationType.HorizontalBarChart,
};

const editorContents: IEditorContentsResponse = {
    headerText: {
        'fi': 'foo'
    },
    maximumHeaderLength: 120,
    maximumSupportedSize: 200,
    size: 5,
    sizeWarningLimit: 175,
    visualizationRejectionReasons: {
        'pieChart': {
            'fi': 'huono kaavio'
        }
    },
    visualizationOptions: [visualizationOptions]
};

const editorContentsResult: IEditorContentsResult = {
    isError: false,
    isLoading: false,
    data: editorContents,
}

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
                    editorContentsResponse={editorContentsResult}
                    resolvedDimensions={mockDimensions}
                    selectedVisualization={selectedVisualizationMock}
                    dimensionQuery={mockQuery}
                    contentLanguages={["fi", "sv", "en"]}
                />
                </UiLanguageContext.Provider>
            );
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly in case of an error', () => {
        const errorEditorContentsResult: IEditorContentsResult = {
            isError: true,
            isLoading: false,
            data: null,
        };
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorMetaSection
                    editorContentsResponse={errorEditorContentsResult}
                    resolvedDimensions={mockDimensions}
                    selectedVisualization={selectedVisualizationMock}
                    dimensionQuery={mockQuery}
                    contentLanguages={["fi", "sv", "en"]}
                />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
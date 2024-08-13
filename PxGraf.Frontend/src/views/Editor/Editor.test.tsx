import { render, screen } from "@testing-library/react";
import { ICubeMetaResult } from "api/services/cube-meta";
import { IHeaderResult } from "api/services/default-header";
import { IFilterVariableResult } from "api/services/filter-dimension";
import { IVisualizationSettingsResult } from "api/services/visualization-rules";
import { ISaveQueryResult } from "api/services/queries";
import React from "react";
import { VariableType } from "types/cubeMeta";
import Editor from "./Editor";
import { IQueryInfoResult } from "api/services/query-info";
import { HashRouter } from "react-router-dom";
import { IVisualizationResult } from "api/services/visualization";
import { EVariableType, ETimeVariableInterval, EVisualizationType } from "@statisticsfinland/pxvisualizer";
import { ITypeSpecificVisualizationRules } from "types/visualizationRules";
import serializer from "../../testUtils/stripHighchartsHashes";
import { NavigationProvider } from "contexts/navigationContext";
import { IValidateTableMetaDataResult } from "api/services/validate-table-metadata";
import { QueryClient, QueryClientProvider } from 'react-query';
import { UiLanguageContext } from "contexts/uiLanguageContext";

const queryClient = new QueryClient();

const language = "fi";
const setLanguage = jest.fn();
const languageTab = "fi";
const setLanguageTab = jest.fn();
const availableUiLanguages = ["fi", "sv", "en"];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useParams: () => {
        return {
            '*': 'foo/bar'
        };
    },
}));

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => ({})),
            },
        };
    },
}));

jest.mock('api/services/cube-meta', () => ({
    ...jest.requireActual('api/services/cube-meta'),
    useCubeMetaQuery: () => {
        return mockCubeMetaResult;
    }
}));

jest.mock('api/services/default-header', () => ({
    ...jest.requireActual('api/services/default-header'),
    useDefaultHeaderQuery: () => {
        return mockHeaderResult;
    }
}));

jest.mock('api/services/query-info', () => ({
    ...jest.requireActual('api/services/query-info'),
    useQueryInfoQuery: () => {
        return mockQueryInfoResult;
    }
}));

jest.mock('api/services/filter-dimension', () => ({
    ...jest.requireActual('api/services/filter-dimension'),
    useResolveVariableFiltersQuery: () => {
        return mockFilterVariableResult;
    }
}));

jest.mock('api/services/visualization-rules', () => ({
    ...jest.requireActual('api/services/visualization-rules'),
    useVisualizationOptionsQuery: () => {
        return mockVisualizationSettingsResult;
    }
}));

jest.mock('api/services/queries', () => ({
    ...jest.requireActual('api/services/queries'),
    useSaveMutation: () => {
        return mockSaveQueryResult;
    }
}));

jest.mock('api/services/visualization', () => ({
    ...jest.requireActual('api/services/visualization'),
    useVisualizationQuery: () => {
        return mockVisualizationResult;
    }
}));

jest.mock('@statisticsfinland/pxvisualizer', () => {
    const lib = jest.requireActual("@statisticsfinland/pxvisualizer");
    return {
        ...lib,
        Chart: (...args: any[]) => {
            return (
                <pre data-testid={'Chart'}>
                   args={JSON.stringify(args)}
                </pre>
            );
        }
    }
});

const mockVisualizationResult: IVisualizationResult = {
    isLoading: false,
    isError: false,
    data: {
        data: [ 1, 2, 3, 4 ],
        missingDataInfo: {},
        dataNotes: [],
        rowVariableCodes: ["foobar2"],
        columnVariableCodes: ["foobar1"],
        header: { fi: "foo", sv: "bar", en: "foobar" },
        selectableVariableCodes: ["foobar1"],
        visualizationSettings:
        {
            timeVariableIntervals: ETimeVariableInterval.Irregular,
            visualizationType: EVisualizationType.HorizontalBarChart,
            sorting: 'DESCENDING'
        },
        metaData: [{
            code: "foobar1",
            name: { fi: "foo1", sv: "bar1", en: "foobar1" },
            note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
            type: EVariableType.Content,
            values: [{
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false,
                contentComponent: {
                    unit: { fi: "unitfi1", sv: "unitsv1", en: "uniten1" },
                    source: { fi: "sourcefi1", sv: "sourcesv1", en: "sourceen1" },
                    numberOfDecimals: 0,
                    lastUpdated: "2021-01-01T00:00:00Z"
                }
            }, {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false,
                contentComponent: {
                    unit: { fi: "unitfi2", sv: "unitsv2", en: "uniten2" },
                    source: { fi: "sourcefi2", sv: "sourcesv2", en: "sourceen2" },
                    numberOfDecimals: 0,
                    lastUpdated: "2021-01-01T00:00:00Z"
                }
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
        tableReference: { hierarchy: ["foo", "bar"], name:  "foobar_table" },
    },
}

const mockSaveQueryResult: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: {
        id: 'id'
    },
    mutate: jest.fn()
}

const mockTypeSpecificVisualizationRules: ITypeSpecificVisualizationRules = {
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true
}

const mockVisualizationSettingsResult: IVisualizationSettingsResult = {
    isLoading: false,
    isError: false,
    data: {
        allowManualPivot: false,
        multiselectVariableAllowed: false,
        sortingOptions: [
            {
                code: 'DESCENDING',
                description: {
                    'fi': 'Laskeva'
                }
            }
        ],
        visualizationTypeSpecificRules: mockTypeSpecificVisualizationRules
    }
}

const mockFilterVariableResult: IFilterVariableResult = {
    isLoading: false,
    isError: false,
    data: {
        'foo': ['bar', 'baz']
    }
}

const mockQueryInfoResult: IQueryInfoResult = {
    isLoading: false,
    isError: false,
    data: {
        maximumHeaderLength: 120,
        maximumSupportedSize: 100,
        size: 5,
        sizeWarningLimit: 75,
        validVisualizations: ['HorizontalBarChart', 'VerticalBarChart'],
        visualizationRejectionReasons: {
            'pieChart': {
                'fi': 'huono kaavio'
            }
        }
    }
}

const mockHeaderResult: IHeaderResult = {
    isLoading: false,
    isError: false,
    data: {
        'fi': 'header'
    }
}

const mockCubeMetaResult: ICubeMetaResult = {
    isLoading: false,
    isError: false,
    data: {
        header: {
            'fi': 'header'
        },
        languages: ['fi'],
        note: {
            'fi': 'note'
        },
        variables: [
            {
                code: 'code',
                name: {
                    'fi': 'variableName'
                },
                note: {
                    'fi': 'variableNote'
                },
                type: VariableType.Content,
                values: [
                    {
                        code: 'variableValueCode',
                        isSum: false,
                        name: {
                            'fi': 'variableValueName'
                        },
                        note: {
                            'fi': 'variableValueNote'
                        }
                    }
                ]
            }
        ]
    }
}

const mockTableValidationResult: IValidateTableMetaDataResult = {
    isLoading: false,
    isError: false,
    data: {
        tableHasContentVariable: true,
        tableHasTimeVariable: true,
        allVariablesContainValues: true
    }
}

const mockInvalidTableValidationResult: IValidateTableMetaDataResult = {
    isLoading: false,
    isError: false,
    data: {
        tableHasContentVariable: false,
        tableHasTimeVariable: false,
        allVariablesContainValues: false
    }
}

let mockResult: IValidateTableMetaDataResult;

jest.mock('api/services/validate-table-metadata', () => ({
    useValidateTableMetadataQuery: () => mockResult,
}));

describe('Rendering test', () => {
    beforeAll(() => {
        expect.addSnapshotSerializer(serializer);
        mockResult = mockTableValidationResult;
    });

    it('renders correctly', () => {
        const { asFragment } = render(
            <QueryClientProvider client={queryClient}>
                <NavigationProvider>
                    <HashRouter>
                        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                            <Editor />
                        </UiLanguageContext.Provider>
                    </HashRouter>
                </NavigationProvider>
            </QueryClientProvider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('renders errorContainer with correct message when tableValidityResponse is invalid', () => {
        mockResult = mockInvalidTableValidationResult;
        render(
            <QueryClientProvider client={queryClient}>
                <NavigationProvider>
                    <HashRouter>
                        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                            <Editor />
                        </UiLanguageContext.Provider>
                    </HashRouter>
                </NavigationProvider>
            </QueryClientProvider>
        );

        expect(screen.getByText('error.contentVariableMissing error.timeVariableMissing error.variablesMissingValues')).toBeInTheDocument();
    });
});
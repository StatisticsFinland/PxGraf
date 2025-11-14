import React from 'react';
import { render, screen } from "@testing-library/react";
import { ICubeMetaResult } from "api/services/cube-meta";
import { IFilterDimensionResult } from "api/services/filter-dimension";
import { ISaveQueryResult } from "api/services/queries";
import { EDimensionType } from "types/cubeMeta";
import Editor from "./Editor";
import { HashRouter } from "react-router-dom";
import { IVisualizationResult } from "api/services/visualization";
import { EVariableType, ETimeVariableInterval, EVisualizationType } from "@statisticsfinland/pxvisualizer";
import serializer from "../../testUtils/stripHighchartsHashes";
import { NavigationProvider } from "contexts/navigationContext";
import { IValidateTableMetaDataResult } from "api/services/validate-table-metadata";
import { QueryClient, QueryClientProvider } from 'react-query';
import { UiLanguageContext } from "contexts/uiLanguageContext";
import '@testing-library/jest-dom';
import { IVisualizationOptions } from '../../types/editorContentsResponse';
import { VisualizationType } from '../../types/visualizationType';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { EditorContext } from '../../contexts/editorContext';

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

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
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

jest.mock('api/services/filter-dimension', () => ({
    ...jest.requireActual('api/services/filter-dimension'),
    useResolveDimensionFiltersQuery: () => {
        return mockFilterDimensionResult;
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

jest.mock('api/services/editor-contents', () => ({
    ...jest.requireActual('api/services/editor-contents'),
    useEditorContentsQuery: () => {
        return mockEditorContentsResult;
    }
}));

const mockVisualizationResult: IVisualizationResult = {
    isLoading: false,
    isError: false,
    data: {
        data: [1, 2, 3, 4],
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
        tableReference: { hierarchy: ["foo", "bar"], name: "foobar_table" },
    },
}

const mockSaveQueryResult: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: {
        id: 'id',
        publicationStatus: 0
    },
    mutate: jest.fn()
}

const mockVisualizationSettingsResult: IVisualizationOptions = {
    type: VisualizationType.HorizontalBarChart,
    allowManualPivot: false,
    allowMultiselect: false,
    sortingOptions: {
        default: [
            {
                code: 'DESCENDING',
                description: {
                    'fi': 'Laskeva'
                }
            }
        ],
        pivoted: []
    },
    allowShowingDataPoints: true,
    allowCuttingYAxis: true,
    allowMatchXLabelsToEnd: true,
    allowSetMarkerScale: true
}

const mockFilterDimensionResult: IFilterDimensionResult = {
    isLoading: false,
    isError: false,
    data: {
        'foo': ['bar', 'baz']
    }
}

const mockEditorContentsResult: IEditorContentsResult = {
    isLoading: false,
    isError: false,
    data: {
        maximumHeaderLength: 120,
        maximumSupportedSize: 100,
        size: 5,
        sizeWarningLimit: 75,
        visualizationRejectionReasons: {
            'pieChart': {
                'fi': 'huono kaavio'
            }
        },
        visualizationOptions: [mockVisualizationSettingsResult],
        headerText: {
            'fi': 'header'
        },
        publicationEnabled: true
    }
}

const mockCubeMetaResult: ICubeMetaResult = {
    isLoading: false,
    isError: false,
    data: {
        defaultLanguage: 'fi',
        availableLanguages: ['fi'],
        additionalProperties: {},
        dimensions: [
            {
                code: 'code',
                name: {
                    'fi': 'variableName'
                },
                type: EDimensionType.Content,
                values: [
                    {
                        code: 'variableValueCode',
                        name: {
                            'fi': 'variableValueName'
                        },
                        isVirtual: false,
                        additionalProperties: {},
                    }
                ],
                additionalProperties: {}
            }
        ]
    }
}

const mockTableValidationResult: IValidateTableMetaDataResult = {
    isLoading: false,
    isError: false,
    data: {
        tableHasContentDimension: true,
        tableHasTimeDimension: true,
        allDimensionsContainValues: true
    }
}

const mockInvalidTableValidationResult: IValidateTableMetaDataResult = {
    isLoading: false,
    isError: false,
    data: {
        tableHasContentDimension: false,
        tableHasTimeDimension: false,
        allDimensionsContainValues: false
    }
}

const errorTableValidationResult: IValidateTableMetaDataResult = {
    isLoading: false,
    isError: true,
    data: null
}

const mockLocation = {
    state: {
        result: {
            id: 'test-query-id',
            draft: true,
            settings: {
                selectedVisualization: EVisualizationType.HorizontalBarChart,
                defaultSelectableVariableCodes: { foobar1: ['barfoo1', 'barfoo2'] }
            },
            query: {
                variableQueries: {
                    foobar1: {
                        valueFilter: ['barfoo1', 'barfoo2'],
                        selectable: true,
                        virtualValueDefinitions: [],
                        valueEdits: {}
                    },
                    foobar2: {
                        valueFilter: ['barfoo1', 'barfoo2'],
                        selectable: true,
                        virtualValueDefinitions: [],
                        valueEdits: {}
                    }
                },
                chartHeaderEdit: {
                    fi: 'test header',
                    sv: 'test header sv',
                    en: 'test header en'
                }
            }
        }
    }
};

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useParams: () => {
        return {
            '*': 'foo/bar'
        };
    },
    useLocation: () => mockLocation
}));

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

    it('renders correctly when cubeMetaResponse is loading', () => {
        mockCubeMetaResult.data = null;
        mockCubeMetaResult.isLoading = true;
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
        mockCubeMetaResult.isError = true;
        mockCubeMetaResult.isLoading = false;
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

    it('renders errorContainer with generic message when tableValidityResponse is error', () => {
        mockResult = errorTableValidationResult;
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

        expect(screen.getByText('error.contentLoad')).toBeInTheDocument();
    });

    it('loads query data from location state', () => {
        mockResult = mockTableValidationResult;
        mockCubeMetaResult.isLoading = false;
        mockCubeMetaResult.isError = false;
        mockCubeMetaResult.data = {
            defaultLanguage: 'fi',
            availableLanguages: ['fi'],
            additionalProperties: {},
            dimensions: [
                {
                    code: 'code',
                    name: {
                        'fi': 'variableName'
                    },
                    type: EDimensionType.Content,
                    values: [
                        {
                            code: 'variableValueCode',
                            name: {
                                'fi': 'variableValueName'
                            },
                            isVirtual: false,
                            additionalProperties: {},
                        }
                    ],
                    additionalProperties: {}
                }
            ]
        };

        const setSelectedVisualizationUserInput = jest.fn();
        const setLoadedQueryId = jest.fn();
        const setLoadedQueryIsDraft = jest.fn();
        const originalUseContext = React.useContext;

        const mockEditorContext = {
            cubeQuery: { variableQueries: {} },
            setCubeQuery: jest.fn(),
            query: null,
            setQuery: jest.fn(),
            saveDialogOpen: false,
            setSaveDialogOpen: jest.fn(),
            selectedVisualizationUserInput: null,
            setSelectedVisualizationUserInput,
            visualizationSettingsUserInput: null,
            setVisualizationSettingsUserInput: jest.fn(),
            defaultSelectables: null,
            setDefaultSelectables: jest.fn(),
            loadedQueryId: '',
            setLoadedQueryId,
            loadedQueryIsDraft: false,
            setLoadedQueryIsDraft,
            publicationEnabled: true,
            setPublicationEnabled: jest.fn()
        };

        // Mock useContext to return the mockEditorContext for EditorContext
        jest.spyOn(React, 'useContext').mockImplementation(context => {
            if (context === EditorContext) {
                return mockEditorContext;
            }
            // Return the original context for other contexts
            return originalUseContext(context);
        });

        render(
            <QueryClientProvider client={queryClient}>
                <NavigationProvider>
                    <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                        <Editor />
                    </UiLanguageContext.Provider>
                </NavigationProvider>
            </QueryClientProvider>
        );

        expect(setLoadedQueryId).toHaveBeenCalledWith('test-query-id');
        expect(setLoadedQueryIsDraft).toHaveBeenCalledWith(true);
        expect(setSelectedVisualizationUserInput).toHaveBeenCalledWith(EVisualizationType.HorizontalBarChart);

        jest.restoreAllMocks();
    });

    it('calls setPublicationEnabled when editorContentsResponse has publicationEnabled property', () => {
        mockResult = mockTableValidationResult;
        mockCubeMetaResult.isLoading = false;
        mockCubeMetaResult.isError = false;
        mockCubeMetaResult.data = {
            defaultLanguage: 'fi',
            availableLanguages: ['fi'],
            additionalProperties: {},
            dimensions: [{
                code: 'code',
                name: { 'fi': 'variableName' },
                type: EDimensionType.Content,
                values: [{
                    code: 'variableValueCode',
                    name: { 'fi': 'variableValueName' },
                    isVirtual: false,
                    additionalProperties: {}
                }],
                additionalProperties: {}
            }]
        };

        const setPublicationEnabled = jest.fn();
        const originalUseContext = React.useContext;

        const mockEditorContextWithPublication = {
            cubeQuery: { variableQueries: {} },
            setCubeQuery: jest.fn(),
            query: null,
            setQuery: jest.fn(),
            saveDialogOpen: false,
            setSaveDialogOpen: jest.fn(),
            selectedVisualizationUserInput: null,
            setSelectedVisualizationUserInput: jest.fn(),
            visualizationSettingsUserInput: null,
            setVisualizationSettingsUserInput: jest.fn(),
            defaultSelectables: null,
            setDefaultSelectables: jest.fn(),
            loadedQueryId: '',
            setLoadedQueryId: jest.fn(),
            loadedQueryIsDraft: false,
            setLoadedQueryIsDraft: jest.fn(),
            publicationEnabled: true,
            setPublicationEnabled
        };

        // Mock useContext to return our mock for EditorContext
        jest.spyOn(React, 'useContext').mockImplementation(context => {
            if (context === EditorContext) {
                return mockEditorContextWithPublication;
            }
            return originalUseContext(context);
        });

        render(
            <QueryClientProvider client={queryClient}>
                <NavigationProvider>
                    <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                        <Editor />
                    </UiLanguageContext.Provider>
                </NavigationProvider>
            </QueryClientProvider>
        );

        // Verify that setPublicationEnabled was called with true (from the mocked API response)
        expect(setPublicationEnabled).toHaveBeenCalledWith(true);

        jest.restoreAllMocks();
    });
});
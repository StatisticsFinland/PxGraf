import { render } from "@testing-library/react";
import React from 'react';
import { FilterType, ICubeQuery, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import Preview, { ISelectabilityInfo, getSelectables, getResolvedSelections } from "./Preview";
import { EVariableType, EVisualizationType, ETimeVariableInterval, IQueryVisualizationResponse } from "@statisticsfinland/pxvisualizer";
import { IVisualizationResult } from "api/services/visualization";
import serializer from "../../testUtils/stripHighchartsHashes";
import UiLanguageContext from "../../contexts/uiLanguageContext";
import { EditorContext } from "../../contexts/editorContext";
import { ISelectableSelections } from "../SelectableVariableMenus/SelectableDimensionMenus";
import { EDimensionType } from "../../types/cubeMeta";

function mockComponentMocker(name) {
    return function MockComponent(...args) {
        return (
            <pre data-testid={name}>
               args={JSON.stringify(args)}
            </pre>
        );
    };
}

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

jest.mock('@statisticsfinland/pxvisualizer', () => {
    const lib = jest.requireActual("@statisticsfinland/pxvisualizer");
    return {
        ...lib,
        Chart: mockComponentMocker('Chart')
    }
});

const mockVisualizationQueryResult: IVisualizationResult = {
    isLoading: false,
    isError: false,
    data: {
        data: [3107, 2383, 1555, 1839],
        missingDataInfo: {},
        dataNotes: {},
        metaData: [
            {
                code: "Tiedot",
                name: {
                    fi: "Tiedot",
                    sv: "Uppgifter",
                    en: "Information",
                },
                note: null,
                type: EVariableType.Content,
                values: [
                    {
                        code: "kulutus_t",
                        name: {
                            fi: "kulutus_t-fi",
                            sv: "kulutus_t-sv",
                            en: "kulutus_t-en",
                        },
                        note: null,
                        isSum: false,
                        contentComponent: {
                            unit: {
                                fi: "1000 t",
                                sv: "1000 t",
                                en: "1000 t",
                            },
                            source: {
                                fi: "PxGraf-fi",
                                sv: "PxGraf-sv",
                                en: "PxGraf-en",
                            },
                            numberOfDecimals: 0,
                            lastUpdated: "2023-02-01T06:00:00Z",
                        },
                    },
                ],
            },
            {
                code: "Vuosi",
                name: {
                    fi: "Vuosi",
                    sv: "År",
                    en: "Year",
                },
                note: null,
                type: EVariableType.Time,
                values: [
                    {
                        code: "2018",
                        name: {
                            fi: "2018",
                            sv: "2018",
                            en: "2018",
                        },
                        note: null,
                        isSum: false,
                        contentComponent: null,
                    },
                    {
                        code: "2019",
                        name: {
                            fi: "2019",
                            sv: "2019",
                            en: "2019",
                        },
                        note: null,
                        isSum: false,
                        contentComponent: null,
                    },
                    {
                        code: "2020",
                        name: {
                            fi: "2020",
                            sv: "2020",
                            en: "2020",
                        },
                        note: null,
                        isSum: false,
                        contentComponent: null,
                    },
                    {
                        code: "2021",
                        name: {
                            fi: "2021",
                            sv: "2021",
                            en: "2021",
                        },
                        note: null,
                        isSum: false,
                        contentComponent: null,
                    },
                ],
            },
        ],
        selectableVariableCodes: [],
        rowVariableCodes: [],
        columnVariableCodes: ["Vuosi"],
        header: {
            fi: "kulutus_t-fi 2018-2021",
            sv: "kulutus_t-sv 2018-2021",
            en: "kulutus_t-en 2018-2021",
        },
        visualizationSettings: {
            visualizationType: EVisualizationType.VerticalBarChart,
            timeVariableIntervals: ETimeVariableInterval.Year,
            defaultSelectableVariableCodes: null,
        },
        tableReference: { hierarchy: ["foo", "bar"], name:  "table" }
    }
};
const mockPath = [
    "foo",
    "bar",
    "table.px"
];
const mockQuery: Query = {
    Vuosi: {
        valueFilter: {
            type: FilterType.Top,
            query: 4
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    Tiedot: {
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
const mockLanguage = 'fi';
const mockCubeQueryTextEdits: ICubeQuery = {
    chartHeaderEdit: null,
    variableQueries: {}
};

const mockSelectedVisualization = 'table';
const mockVisualizationSettings: IVisualizationSettings = {
    rowVariableCodes: [],
    columnVariableCodes: [
        "Vuosi",
        "Tiedot"
    ]
};

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

jest.mock('api/services/visualization', () => ({
    ...jest.requireActual('api/services/visualization'),
    useVisualizationQuery: () => {
        return mockVisualizationQueryResult;
    },
}));

const defaultSelectables = { foo: ['2018'] };
const setDefaultSelectables = jest.fn();
const setCubeQuery = jest.fn();
const query = null;
const setQuery = jest.fn();
const saveDialogOpen = false;
const setSaveDialogOpen = jest.fn();
const selectedVisualizationUserInput = null;
const setSelectedVisualizationUserInput = jest.fn();
const visualizationSettingsUserInput = null;
const setVisualizationSettingsUserInput = jest.fn();

describe('Rendering test', () => {
    beforeAll(() => {
        expect.addSnapshotSerializer(serializer);
    });

    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{
                language: mockLanguage,
                setLanguage: jest.fn(),
                languageTab: mockLanguage,
                setLanguageTab: jest.fn(),
                uiContentLanguage: mockLanguage,
                setUiContentLanguage: jest.fn(),
                availableUiLanguages: ['fi', 'en', 'sv'],
            }}>
                <EditorContext.Provider value={{ cubeQuery: mockCubeQueryTextEdits, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables }}>
                    <Preview
                        path={mockPath}
                        query={mockQuery}
                        selectedVisualization={mockSelectedVisualization}
                        visualizationSettings={mockVisualizationSettings}
                    />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>);

        expect(asFragment()).toMatchSnapshot();
    });
});

const mockSelectables: ISelectabilityInfo[] = [
    {
        dimension: {
            code: 'foobar1',
            name: { fi: 'foobar1.fi', sv: 'foobar1.sv', en: 'foobar1.en' },
            note: null,
            type: EDimensionType.Content,
            values: [
                {
                    code: 'barfoo1',
                    name: { fi: 'barfoo1.fi', sv: 'barfoo1.sv', en: 'barfoo1.en' },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: 'barfoo2',
                    name: { fi: 'barfoo2.fi', sv: 'barfoo2.sv', en: 'barfoo2.en' },
                    note: null,
                    isSum: false,
                    contentComponent: null
                }
            ]
        },
        multiselectable: false
    }
]

describe('Assertion tests', () => {
    it('getResolvedSelections applies default selectable variable value if selections have not been manually changed', () => {
        const mockSelections: ISelectableSelections = {
        };
        const defaultSelectables: ISelectableSelections = {};
        const result = getResolvedSelections(mockSelectables, mockSelections, defaultSelectables);
        expect(result).toEqual({
            foobar1: ['barfoo1']
        });
    });

    it('getResolvedSelections applies first available variable value if default value has not been set', () => {
        const mockSelections: ISelectableSelections = {};
        const defaultSelectables: ISelectableSelections = {
            foobar1: ['barfoo2']
        };
        const result = getResolvedSelections(mockSelectables, mockSelections, defaultSelectables);
        expect(result).toEqual({
            foobar1: ['barfoo2']
        });
    });

    it('getResolvedSelections retains manually changed selections over default selectable variable values', () => {
        const mockSelections: ISelectableSelections = {
            foobar1: ['barfoo1', 'barfoo2']
        };
        const defaultSelectables: ISelectableSelections = {
            foobar1: ['barfoo2']
        };
        const result = getResolvedSelections(mockSelectables, mockSelections, defaultSelectables, 'foobar1');
        expect(result).toEqual({
            foobar1: ['barfoo1', 'barfoo2']
        });
    });

    it('getResolvedSelections returns only first selected value if multiple values are selected from non-multiselectable dimension', () => {
        const mockSelections: ISelectableSelections = {
            foobar1: ['barfoo1', 'barfoo2']
        };
        const defaultSelectables: ISelectableSelections = {
            foobar1: ['barfoo2']
        };
        const result = getResolvedSelections(mockSelectables, mockSelections, defaultSelectables);
        expect(result).toEqual({
            foobar1: ['barfoo1']
        });
    });

    it('getSelectables returns correct information about selectable dimensions', () => {
        const mockVisualizationResponseWithSelectables: IQueryVisualizationResponse = {
            ...mockVisualizationQueryResult.data,
            selectableVariableCodes: ['Tiedot', 'Vuosi']
        }
        const mockVisualizationSettingsWithMultiselect: IVisualizationSettings = {
            ...mockVisualizationSettings,
            multiselectableVariableCode: 'Vuosi'
        };
        
        const result = getSelectables(mockVisualizationResponseWithSelectables, mockVisualizationSettingsWithMultiselect);
        expect(result.length).toBe(2);
        expect(result[0].dimension.code).toBe('Tiedot');
        expect(result[1].dimension.code).toBe('Vuosi');
        expect(result[0].multiselectable).toBe(false);
        expect(result[1].multiselectable).toBe(true);
    });
});
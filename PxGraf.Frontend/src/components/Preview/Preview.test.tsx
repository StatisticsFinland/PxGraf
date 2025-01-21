import { render } from "@testing-library/react";
import React from 'react';
import { FilterType, ICubeQuery, IDimensionEditions, IDimensionQuery, Query } from "types/query";
import { IVisualizationSettings } from "types/visualizationSettings";
import Preview from "./Preview";
import { EVariableType, EVisualizationType, ETimeVariableInterval } from "@statisticsfinland/pxvisualizer";
import { IVisualizationResult } from "api/services/visualization";

import serializer from "../../testUtils/stripHighchartsHashes";

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

describe('Rendering test', () => {
    beforeAll(() => {
        expect.addSnapshotSerializer(serializer);
    });

    it('renders correctly', () => {
        const { asFragment } = render(
            <Preview
                path={mockPath}
                query={mockQuery}
                language={mockLanguage}
                cubeQueryTextEdits={mockCubeQueryTextEdits}
                selectedVisualization={mockSelectedVisualization}
                visualizationSettings={mockVisualizationSettings}
            />);

        expect(asFragment()).toMatchSnapshot();
    });
});
import React from 'react';
import { render } from "@testing-library/react";
import { EMetaPropertyType, IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import DimensionSelectionList from "./DimensionSelectionList";
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockDimensions: IDimension[] =
    [
        {
            code: "FoobarManual",
            name: {
                fi: "FoobarManualFi",
                sv: "FoobarManualSv",
                en: "FoobarManualEn"
            },
            type: EDimensionType.Other,
            values: [
                {
                    code: "eka",
                    name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    isVirtual: false
                },
                {
                    code: "toka",
                    name: {
                        fi: "tokaFi",
                        sv: "tokaSv",
                        en: "tokaEn"
                    },
                    isVirtual: false
                },
                {
                    code: "peruna",
                    name: {
                        fi: "perunaFi",
                        sv: "perunaSv",
                        en: "perunaEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "Vuosi",
            name: {
                fi: "Vuosi",
                sv: "År",
                en: "Year"
            },
            type: EDimensionType.Time,
            values: [
                {
                    code: "2018",
                    name: {
                        fi: "2018",
                        sv: "2018",
                        en: "2018"
                    },
                    isVirtual: false
                },
                {
                    code: "2019",
                    name: {
                        fi: "2019",
                        sv: "2019",
                        en: "2019"
                    },
                    isVirtual: false
                },
                {
                    code: "2020",
                    name: {
                        fi: "2020",
                        sv: "2020",
                        en: "2020"
                    },
                    isVirtual: false
                },
                {
                    code: "2021",
                    name: {
                        fi: "2021*",
                        sv: "2021*",
                        en: "2021*"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarFrom",
            name: {
                fi: "FoobarFromFi",
                sv: "FoobarFromSv",
                en: "FoobarFromEn"
            },
            type: EDimensionType.Other,
            values: [
                {
                    code: "aaa",
                    name: {
                        fi: "aaaFi",
                        sv: "aaaSv",
                        en: "aaaEn"
                    },
                    isVirtual: false
                },
                {
                    code: "bbb",
                    name: {
                        fi: "bbbFi",
                        sv: "bbbSv",
                        en: "bbbEn"
                    },
                    isVirtual: false
                },
                {
                    code: "ccc",
                    name: {
                        fi: "cccFi",
                        sv: "cccSv",
                        en: "cccEn"
                    },
                    isVirtual: false
                },
                {
                    code: "ddd",
                    name: {
                        fi: "dddFi",
                        sv: "dddSv",
                        en: "dddEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarAll",
            name: {
                fi: "FoobarAllFi",
                sv: "FoobarAllSv",
                en: "FoobarAllEn"
            },
            type: EDimensionType.Other,
            values: [
                {
                    code: "xxx",
                    name: {
                        fi: "xxxFi",
                        sv: "xxxSv",
                        en: "xxxEn"
                    },
                    isVirtual: false
                },
                {
                    code: "yyy",
                    name: {
                        fi: "yyyFi",
                        sv: "yyySv",
                        en: "yyyEn"
                    },
                    isVirtual: false
                },
                {
                    code: "zzz",
                    name: {
                        fi: "zzzFi",
                        sv: "zzzSv",
                        en: "zzzEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarContent",
            name: {
                fi: "AnalyysimuuttujaFi",
                sv: "ContentVariableSv",
                en: "ContentVariableEn"
            },
            type: EDimensionType.Content,
            values: [
                {
                    code: "eka",
                    name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarElimination",
            name: {
                fi: "EliminointiMuuttujaFi",
                sv: "EliminationVariableEn",
                en: "EliminationVariableEn"
            },
            type: EDimensionType.Other,
            additionalProperties: {
                ELIMINATION: {
                    type: EMetaPropertyType.MultilanguageText,
                    value: {
                        fi: "sum",
                        sv: "sum",
                        en: "sum"
                    }
                }
            },
            values: [
                {
                    code: "sum",
                    name: {
                        fi: "sumFi",
                        sv: "sumSv",
                        en: "sumEn"
                    },
                    isVirtual: false
                },
                {
                    code: "val",
                    name: {
                        fi: "valFi",
                        sv: "valSv",
                        en: "valEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarSingle",
            name: {
                fi: "YksikkömuuttujaFi",
                sv: "SingleValueVariableSv",
                en: "SingleValueVariableEn"
            },
            type: EDimensionType.Other,
            values: [
                {
                    code: "single",
                    name: {
                        fi: "singleFi",
                        sv: "singleSv",
                        en: "singleEn"
                    },
                    isVirtual: false
                }
            ]
        },
        {
            code: "FoobarMissingValueName",
            name: {
                fi: "NimetönArvoMuuttujaFi",
                sv: "NamelessValueVariableSv",
                en: "NamelessValueVariableEn"
            },
            type: EDimensionType.Other,
            values: [
                {
                    code: "missingName",
                    name: {
                        fi: null,
                        sv: null,
                        en: null
                    },
                    isVirtual: false
                }
            ]
        }
    ]


const mockQuery: Query = {
    FoobarManual: {
        valueFilter: {
            type: FilterType.Item,
            query: ["eka", "toka"]
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    Vuosi: {
        valueFilter: {
            type: FilterType.Top,
            query: 4
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarFrom: {
        valueFilter: {
            type: FilterType.From,
            query: "bbb"
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarAll: {
        valueFilter: {
            type: FilterType.All
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarContent: {
        valueFilter: {
            type: FilterType.Item,
            query: ["eka"]
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarElimination: {
        valueFilter: {
            type: FilterType.Item,
            query: ["sum"]
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarSingle: {
        valueFilter: {
            type: FilterType.Item,
            query: ["single"]
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    FoobarMissingValueName: {
        valueFilter: {
            type: FilterType.Item,
            query: ["missingName"]
        },
        selectable: false,
        virtualValueDefinitions: null
    }
};

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

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

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <DimensionSelectionList
                    dimensions={mockDimensions}
                    resolvedDimensionCodes={{
                        FoobarManual: ["eka", "toka"],
                        Vuosi: ["2018", "2019", "2020", "2021*"],
                        FoobarFrom: ["bbb", "ccc", "ddd"],
                        FoobarAll: ["xxx", "yyy", "zzz"],
                        FoobarContent: ["eka"],
                        FoobarElimination: ["sum"],
                        FoobarSingle: ["single"],
                        FoobarMissingValueName: ["missingName"]
                    }}
                    query={mockQuery}
                ></DimensionSelectionList>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
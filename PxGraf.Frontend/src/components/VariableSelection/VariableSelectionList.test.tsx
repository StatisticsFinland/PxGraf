import React from 'react';
import { render } from "@testing-library/react";
import { EMetaPropertyType, IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import VariableSelectionList from "./VariableSelectionList";
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockVariables: IDimension[] =
    [
        {
            Code: "FoobarManual",
            Name: {
                fi: "FoobarManualFi",
                sv: "FoobarManualSv",
                en: "FoobarManualEn"
            },
            Type: EDimensionType.Other,
            Values: [
                {
                    Code: "eka",
                    Name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "toka",
                    Name: {
                        fi: "tokaFi",
                        sv: "tokaSv",
                        en: "tokaEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "peruna",
                    Name: {
                        fi: "perunaFi",
                        sv: "perunaSv",
                        en: "perunaEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "Vuosi",
            Name: {
                fi: "Vuosi",
                sv: "År",
                en: "Year"
            },
            Type: EDimensionType.Time,
            Values: [
                {
                    Code: "2018",
                    Name: {
                        fi: "2018",
                        sv: "2018",
                        en: "2018"
                    },
                    IsVirtual: false
                },
                {
                    Code: "2019",
                    Name: {
                        fi: "2019",
                        sv: "2019",
                        en: "2019"
                    },
                    IsVirtual: false
                },
                {
                    Code: "2020",
                    Name: {
                        fi: "2020",
                        sv: "2020",
                        en: "2020"
                    },
                    IsVirtual: false
                },
                {
                    Code: "2021",
                    Name: {
                        fi: "2021*",
                        sv: "2021*",
                        en: "2021*"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarFrom",
            Name: {
                fi: "FoobarFromFi",
                sv: "FoobarFromSv",
                en: "FoobarFromEn"
            },
            Type: EDimensionType.Other,
            Values: [
                {
                    Code: "aaa",
                    Name: {
                        fi: "aaaFi",
                        sv: "aaaSv",
                        en: "aaaEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "bbb",
                    Name: {
                        fi: "bbbFi",
                        sv: "bbbSv",
                        en: "bbbEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "ccc",
                    Name: {
                        fi: "cccFi",
                        sv: "cccSv",
                        en: "cccEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "ddd",
                    Name: {
                        fi: "dddFi",
                        sv: "dddSv",
                        en: "dddEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarAll",
            Name: {
                fi: "FoobarAllFi",
                sv: "FoobarAllSv",
                en: "FoobarAllEn"
            },
            Type: EDimensionType.Other,
            Values: [
                {
                    Code: "xxx",
                    Name: {
                        fi: "xxxFi",
                        sv: "xxxSv",
                        en: "xxxEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "yyy",
                    Name: {
                        fi: "yyyFi",
                        sv: "yyySv",
                        en: "yyyEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "zzz",
                    Name: {
                        fi: "zzzFi",
                        sv: "zzzSv",
                        en: "zzzEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarContent",
            Name: {
                fi: "AnalyysimuuttujaFi",
                sv: "ContentVariableSv",
                en: "ContentVariableEn"
            },
            Type: EDimensionType.Content,
            Values: [
                {
                    Code: "eka",
                    Name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarElimination",
            Name: {
                fi: "EliminointiMuuttujaFi",
                sv: "EliminationVariableEn",
                en: "EliminationVariableEn"
            },
            Type: EDimensionType.Other,
            AdditionalProperties: {
                ELIMINATION: {
                    Type: EMetaPropertyType.MultilanguageText,
                    Value: {
                        fi: "sum",
                        sv: "sum",
                        en: "sum"
                    }
                }
            },
            Values: [
                {
                    Code: "sum",
                    Name: {
                        fi: "sumFi",
                        sv: "sumSv",
                        en: "sumEn"
                    },
                    IsVirtual: false
                },
                {
                    Code: "val",
                    Name: {
                        fi: "valFi",
                        sv: "valSv",
                        en: "valEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarSingle",
            Name: {
                fi: "YksikkömuuttujaFi",
                sv: "SingleValueVariableSv",
                en: "SingleValueVariableEn"
            },
            Type: EDimensionType.Other,
            Values: [
                {
                    Code: "single",
                    Name: {
                        fi: "singleFi",
                        sv: "singleSv",
                        en: "singleEn"
                    },
                    IsVirtual: false
                }
            ]
        },
        {
            Code: "FoobarMissingValueName",
            Name: {
                fi: "NimetönArvoMuuttujaFi",
                sv: "NamelessValueVariableSv",
                en: "NamelessValueVariableEn"
            },
            Type: EDimensionType.Other,
            Values: [
                {
                    Code: "missingName",
                    Name: {
                        fi: null,
                        sv: null,
                        en: null
                    },
                    IsVirtual: false
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
                <VariableSelectionList
                    variables={mockVariables}
                    resolvedVariableCodes={{
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
                    onQueryChanged={(newValues) => undefined}
                ></VariableSelectionList>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
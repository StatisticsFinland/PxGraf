import React from 'react';
import { render } from "@testing-library/react";
import { IDimension, VariableType } from "types/cubeMeta";
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
            Type: VariableType.OtherClassificatory,
            Values: [
                {
                    Code: "eka",
                    Name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    Virtual: false
                },
                {
                    Code: "toka",
                    Name: {
                        fi: "tokaFi",
                        sv: "tokaSv",
                        en: "tokaEn"
                    },
                    Virtual: false
                },
                {
                    Code: "peruna",
                    Name: {
                        fi: "perunaFi",
                        sv: "perunaSv",
                        en: "perunaEn"
                    },
                    Virtual: false
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
            Type: VariableType.Time,
            Values: [
                {
                    Code: "2018",
                    Name: {
                        fi: "2018",
                        sv: "2018",
                        en: "2018"
                    },
                    Virtual: false
                },
                {
                    Code: "2019",
                    Name: {
                        fi: "2019",
                        sv: "2019",
                        en: "2019"
                    },
                    Virtual: false
                },
                {
                    Code: "2020",
                    Name: {
                        fi: "2020",
                        sv: "2020",
                        en: "2020"
                    },
                    Virtual: false
                },
                {
                    Code: "2021",
                    Name: {
                        fi: "2021*",
                        sv: "2021*",
                        en: "2021*"
                    },
                    Virtual: false
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
            Type: VariableType.OtherClassificatory,
            Values: [
                {
                    Code: "aaa",
                    Name: {
                        fi: "aaaFi",
                        sv: "aaaSv",
                        en: "aaaEn"
                    },
                    Virtual: false
                },
                {
                    Code: "bbb",
                    Name: {
                        fi: "bbbFi",
                        sv: "bbbSv",
                        en: "bbbEn"
                    },
                    Virtual: false
                },
                {
                    Code: "ccc",
                    Name: {
                        fi: "cccFi",
                        sv: "cccSv",
                        en: "cccEn"
                    },
                    Virtual: false
                },
                {
                    Code: "ddd",
                    Name: {
                        fi: "dddFi",
                        sv: "dddSv",
                        en: "dddEn"
                    },
                    Virtual: false
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
            Type: VariableType.OtherClassificatory,
            Values: [
                {
                    Code: "xxx",
                    Name: {
                        fi: "xxxFi",
                        sv: "xxxSv",
                        en: "xxxEn"
                    },
                    Virtual: false
                },
                {
                    Code: "yyy",
                    Name: {
                        fi: "yyyFi",
                        sv: "yyySv",
                        en: "yyyEn"
                    },
                    Virtual: false
                },
                {
                    Code: "zzz",
                    Name: {
                        fi: "zzzFi",
                        sv: "zzzSv",
                        en: "zzzEn"
                    },
                    Virtual: false
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
            Type: VariableType.Content,
            Values: [
                {
                    Code: "eka",
                    Name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    Virtual: false
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
            Type: VariableType.OtherClassificatory,
            AdditionalProperties: {
                ELIMINATION: {
                    KeyWord: "ELIMINATION",
                    CanGetStringValue: false,
                    CanGetMultilanguageValue: true,
                    Entries: {
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
                    Virtual: false
                },
                {
                    Code: "val",
                    Name: {
                        fi: "valFi",
                        sv: "valSv",
                        en: "valEn"
                    },
                    Virtual: false
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
            Type: VariableType.OtherClassificatory,
            Values: [
                {
                    Code: "single",
                    Name: {
                        fi: "singleFi",
                        sv: "singleSv",
                        en: "singleEn"
                    },
                    Virtual: false
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
            Type: VariableType.OtherClassificatory,
            Values: [
                {
                    Code: "missingName",
                    Name: {
                        fi: null,
                        sv: null,
                        en: null
                    },
                    Virtual: false
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
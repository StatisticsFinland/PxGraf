import { getByTestId, render } from "@testing-library/react";
import React from "react";
import { IVariable, VariableType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import VariableSelectionList from "./VariableSelectionList";
import { sortedVariables } from "./VariableSelectionList";
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockVariables: IVariable[] =
    [
        {
            code: "FoobarManual",
            name: {
                fi: "FoobarManualFi",
                sv: "FoobarManualSv",
                en: "FoobarManualEn"
            },
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "eka",
                    name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "toka",
                    name: {
                        fi: "tokaFi",
                        sv: "tokaSv",
                        en: "tokaEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "peruna",
                    name: {
                        fi: "perunaFi",
                        sv: "perunaSv",
                        en: "perunaEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.Time,
            note: null,
            values: [
                {
                    code: "2018",
                    name: {
                        fi: "2018",
                        sv: "2018",
                        en: "2018"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "2019",
                    name: {
                        fi: "2019",
                        sv: "2019",
                        en: "2019"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "2020",
                    name: {
                        fi: "2020",
                        sv: "2020",
                        en: "2020"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "2021",
                    name: {
                        fi: "2021*",
                        sv: "2021*",
                        en: "2021*"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "aaa",
                    name: {
                        fi: "aaaFi",
                        sv: "aaaSv",
                        en: "aaaEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "bbb",
                    name: {
                        fi: "bbbFi",
                        sv: "bbbSv",
                        en: "bbbEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "ccc",
                    name: {
                        fi: "cccFi",
                        sv: "cccSv",
                        en: "cccEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "ddd",
                    name: {
                        fi: "dddFi",
                        sv: "dddSv",
                        en: "dddEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "xxx",
                    name: {
                        fi: "xxxFi",
                        sv: "xxxSv",
                        en: "xxxEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "yyy",
                    name: {
                        fi: "yyyFi",
                        sv: "yyySv",
                        en: "yyyEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
                },
                {
                    code: "zzz",
                    name: {
                        fi: "zzzFi",
                        sv: "zzzSv",
                        en: "zzzEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.Content,
            note: null,
            values: [
                {
                    code: "eka",
                    name: {
                        fi: "ekaFi",
                        sv: "ekaSv",
                        en: "ekaEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "sum",
                    name: {
                        fi: "sumFi",
                        sv: "sumSv",
                        en: "sumEn"
                    },
                    note: null,
                    isSum: true,
                    contentComponent: null
                },
                {
                    code: "val",
                    name: {
                        fi: "valFi",
                        sv: "valSv",
                        en: "valEn"
                    },
                    note: null,
                    isSum: true,
                    contentComponent: null
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
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "single",
                    name: {
                        fi: "singleFi",
                        sv: "singleSv",
                        en: "singleEn"
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
            type: VariableType.OtherClassificatory,
            note: null,
            values: [
                {
                    code: "missingName",
                    name: {
                        fi: null,
                        sv: null,
                        en: null
                    },
                    note: null,
                    isSum: false,
                    contentComponent: null
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
                changeLanguage: () => new Promise(() => { }),
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
                    onQueryChanged={(newValues) => { }}
                ></VariableSelectionList>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('sorts variables in correct order', () => {
        const result: IVariable[] = sortedVariables(mockVariables);
        expect(result[0].code).toBe("FoobarContent");
        expect(result[1].code).toBe("Vuosi");
        expect(result[2].code).toBe("FoobarManual");
        expect(result[3].code).toBe("FoobarFrom");
        expect(result[4].code).toBe("FoobarAll");
        expect(result[5].code).toBe("FoobarElimination");
        expect(result[6].code).toBe("FoobarSingle");
    });
});
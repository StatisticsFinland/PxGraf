import { IDatabaseTable } from '../api/services/table';
import { sortDatabaseGroups, sortedVariables } from './sortingHelpers';
import { IDimension, VariableType } from "types/cubeMeta";

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
        AdditionalProperties:
        {
            ELIMINATION: {
                KeyWord: "ELIMINATION",
                CanGetMultilanguageValue: false,
                CanGetStringValue: true,
                Entries: "sum"
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

const mockTableData: IDatabaseTable[] = [
    {
        code: '0',
        lastUpdated: '2024-6-10',
        name: { en: 'Foo-en', fi: 'Foo-fi' },
        languages: ['en', 'fi'],
    },
    {
        code: '1',
        lastUpdated: '2021-6-10',
        name: { en: 'Bar-en', fi: 'Bar-fi' },
        languages: ['en', 'fi'],
    },
    {
        code: '2',
        lastUpdated: '2021-6-10',
        name: { fi: 'Baz-fi' },
        languages: ['fi'],
    },
];

const mockPrimaryLanguage = 'en';

describe('Assertion tests', () => {
    it('sorts variables in correct order', () => {
        const result: IDimension[] = sortedVariables(mockVariables);
        expect(result[0].Code).toBe("FoobarContent");
        expect(result[1].Code).toBe("Vuosi");
        expect(result[2].Code).toBe("FoobarManual");
        expect(result[3].Code).toBe("FoobarFrom");
        expect(result[4].Code).toBe("FoobarAll");
        expect(result[5].Code).toBe("FoobarElimination");
        expect(result[6].Code).toBe("FoobarSingle");
    });

    it('sorts table data by primary or first available language', () => {
        const sortedData = sortDatabaseGroups(mockTableData, mockPrimaryLanguage);
        const expected = ["1", "2", "0"];
        const result = sortedData.map((item) => item.code);
        expect(result).toEqual(expected);
    });
});
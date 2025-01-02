import { IDatabaseTable } from '../api/services/table';
import { sortDatabaseGroups, sortedDimensions } from './sortingHelpers';
import { EMetaPropertyType, IDimension, EDimensionType } from "types/cubeMeta";

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
        additionalProperties:
        {
            ELIMINATION: {
                type: EMetaPropertyType.Text,
                value: "sum"
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
        const result: IDimension[] = sortedDimensions(mockDimensions);
        expect(result[0].code).toBe("FoobarContent");
        expect(result[1].code).toBe("Vuosi");
        expect(result[2].code).toBe("FoobarManual");
        expect(result[3].code).toBe("FoobarFrom");
        expect(result[4].code).toBe("FoobarAll");
        expect(result[5].code).toBe("FoobarElimination");
        expect(result[6].code).toBe("FoobarSingle");
    });

    it('sorts table data by primary or first available language', () => {
        const sortedData = sortDatabaseGroups(mockTableData, mockPrimaryLanguage);
        const expected = ["1", "2", "0"];
        const result = sortedData.map((item) => item.code);
        expect(result).toEqual(expected);
    });
});
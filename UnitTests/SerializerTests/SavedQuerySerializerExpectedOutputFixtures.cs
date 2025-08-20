namespace UnitTests.SerializerTests
{
    internal static class SavedQuerySerializerExpectedOutputFixtures
    {
        public static string V1_1_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
                ""settings"": {
                    ""visualizationType"": ""GroupVerticalBarChart"",
                    ""cutYAxis"": false,
                    ""matchXLabelsToEnd"": false,
                    ""xLabelInterval"": 1,
                    ""layout"": {
                        ""rowVariableCodes"": [""Kuukausi""],
                        ""columnVariableCodes"": [""Ilmoittava lentoasema""]
                    },
                    ""showDataPoints"": false
                },
                ""draft"": false,
                ""version"": ""1.2"",
                ""query"": {
                    ""tableReference"": {
                        ""name"": ""table.px"",
                        ""hierarchy"": [""foo"", ""bar""]
                    },
                    ""chartHeaderEdit"": null,
                    ""variableQueries"": {
                        ""Kuukausi"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""2022M12"", ""2022M11"", ""2019M02""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Ilmoittava lentoasema"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""EFJO"", ""EFKT""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Lennon tyyppi"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""1""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Saapuneet/lähteneet"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""1""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Toinen lentoasema"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""EFHK""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Tiedot"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""matkustajat""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        }
                    }
                },
                ""creationTime"": ""2023-05-16T12:04:12Z"",
                ""archived"": false
            }";

        public static string V1_0_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
                ""settings"": {
                    ""visualizationType"": ""GroupVerticalBarChart"",
                    ""cutYAxis"": false,
                    ""matchXLabelsToEnd"": false,
                    ""xLabelInterval"": 1,
                    ""layout"": null,
                    ""showDataPoints"": false
                },
                ""draft"": false,
                ""version"": ""1.2"",
                ""query"": {
                    ""tableReference"": {
                        ""name"": ""table.px"",
                        ""hierarchy"": [""foo"", ""bar""]
                    },
                    ""chartHeaderEdit"": null,
                    ""variableQueries"": {
                        ""Vuosi"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-2"", ""value-1"", ""value-0""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Syntymävaltio"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-0"", ""value-1""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Adoptiotyyppi"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-0""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Ikä"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-0""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Sukupuoli"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-0""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""Tiedot"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""value-0""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        }
                    }
                },
                ""creationTime"": ""2023-05-16T14:13:33Z"",
                ""archived"": false
            }";

        public static string V1_0_TABLETEST1_SER_DESER_EXPECTED_OUTPUT = @"{
                ""settings"": {
                    ""visualizationType"": ""Table"",
                    ""cutYAxis"": false,
                    ""defaultSelectableVariableCodes"": {},
                    ""layout"": {
                        ""rowVariableCodes"": [
                            ""Kuukausi"",
                            ""Ilmoittava lentoasema"",
                            ""Lennon tyyppi"",
                            ""Saapuneet/lähteneet""
                        ],
                        ""columnVariableCodes"": [
                            ""Toinen lentoasema"",
                            ""Tiedot""
                        ]
                    },
                    ""showDataPoints"": false
                },
                ""draft"": false,
                ""version"": ""1.2"",
                ""query"": {
                    ""tableReference"": {
                        ""name"": ""table.px"",
                        ""hierarchy"": [
                            ""foo"",
                            ""bar""
                        ]
                    },
                    ""chartHeaderEdit"": {
                        ""fi"": ""v10 sq header-fi"",
                        ""sv"": ""v10 sq header-en"",
                        ""en"": ""v10 sq header-sv""
                    },
                    ""variableQueries"": {
                        ""Kuukausi"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""all""
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": true
                        },
                        ""Ilmoittava lentoasema"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS"",
                                    ""EFHK"",
                                    ""EFET"",
                                    ""EFIV"",
                                    ""EFJO""
                                ]
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": false
                        },
                        ""Lennon tyyppi"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": false
                        },
                        ""Saapuneet/lähteneet"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": false
                        },
                        ""Toinen lentoasema"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": false
                        },
                        ""Tiedot"": {
                            ""nameEdit"": {},
                            ""valueEdits"": {
                                ""matkustajat"": {
                                    ""nameEdit"": null,
                                    ""contentComponent"": {
                                        ""unitEdit"": {},
                                        ""sourceEdit"": {
                                            ""fi"": ""PxGraf-fi"",
                                            ""sv"": ""PxGraf-sv"",
                                            ""en"": ""PxGraf-en""
                                        }
                                    }
                                },
                                ""rahti"": {
                                    ""nameEdit"": null,
                                    ""contentComponent"": {
                                        ""unitEdit"": {},
                                        ""sourceEdit"": {
                                            ""fi"": ""PxGraf-fi"",
                                            ""sv"": ""PxGraf-sv"",
                                            ""en"": ""PxGraf-en""
                                        }
                                    }
                                }
                            },
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""matkustajat"",
                                    ""rahti""
                                ]
                            },
                            ""virtualValueDefinitions"": [],
                            ""selectable"": false
                        }
                    }
                },
                ""creationTime"": ""2022-04-19T06:12:40Z"",
                ""archived"": false
            }";

        public static string V1_1_TEST_SAVEDQUERY4_SER_DESER_EXPECTED_OUTPUT = @"{
                ""settings"": {
                    ""visualizationType"": ""LineChart"",
                    ""cutYAxis"": true,
                    ""layout"": {
                        ""rowVariableCodes"": [""variable-0""],
                        ""columnVariableCodes"": [""variable-1""]
                    },
                    ""showDataPoints"": false
                },
                ""draft"": false,
                ""version"": ""1.2"",
                ""query"": {
                    ""tableReference"": {
                        ""name"": ""table.px"",
                        ""hierarchy"": [""foo"", ""bar""]
                    },
                    ""chartHeaderEdit"": null,
                    ""variableQueries"": {
                        ""variable-0"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""all""
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""variable-1"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""item"",
                                ""query"": [""2021"", ""2012"", ""2007""]
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""variable-2"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""from"",
                                ""query"": ""foo""
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        },
                        ""variable-3"": {
                            ""nameEdit"": null,
                            ""valueEdits"": {},
                            ""valueFilter"": {
                                ""type"": ""top"",
                                ""query"": 3
                            },
                            ""virtualValueDefinitions"": null,
                            ""selectable"": false
                        }
                    }
                },
                ""creationTime"": ""2023-05-16T12:31:13Z"",
                ""archived"": false
            }";
    }
}

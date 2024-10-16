﻿namespace UnitTests.SerializerTests
{
    internal static class SavedQuerySerializerExpectedOutputFixtures
    {
        public static string V1_1_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
                    ""Settings"": {
                        ""VisualizationType"": ""GroupVerticalBarChart"",
                        ""CutYAxis"": false,
                        ""MatchXLabelsToEnd"": false,
                        ""XLabelInterval"": 1,
                        ""Layout"": {
                            ""RowVariableCodes"": [""Kuukausi""],
                            ""ColumnVariableCodes"": [""Ilmoittava lentoasema""]
                        },
                        ""ShowDataPoints"": false
                    },
                    ""Version"": ""1.1"",
                    ""Query"": {
                        ""TableReference"": {
                            ""Name"": ""table.px"",
                            ""Hierarchy"": [""foo"", ""bar""]
                        },
                        ""ChartHeaderEdit"": null,
                        ""VariableQueries"": {
                            ""Kuukausi"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""2022M12"", ""2022M11"", ""2019M02""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Ilmoittava lentoasema"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""EFJO"", ""EFKT""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Lennon tyyppi"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""1""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Saapuneet/lähteneet"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""1""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Toinen lentoasema"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""EFHK""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Tiedot"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""matkustajat""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            }
                        }
                    },
                    ""CreationTime"": ""2023-05-16T15:04:12+03:00"",
                    ""Archived"": false
                }";

        public static string V1_0_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
                    ""Settings"": {
                        ""VisualizationType"": ""GroupVerticalBarChart"",
                        ""CutYAxis"": false,
                        ""MatchXLabelsToEnd"": false,
                        ""XLabelInterval"": 1,
                        ""Layout"": null,
                        ""ShowDataPoints"": false
                    },
                    ""Version"": ""1.1"",
                    ""Query"": {
                        ""TableReference"": {
                            ""Name"": ""table.px"",
                            ""Hierarchy"": [""foo"", ""bar""]
                        },
                        ""ChartHeaderEdit"": null,
                        ""VariableQueries"": {
                            ""Vuosi"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-2"", ""value-1"", ""value-0""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Syntymävaltio"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-0"", ""value-1""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Adoptiotyyppi"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-0""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Ikä"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-0""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Sukupuoli"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-0""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            },
                            ""Tiedot"": {
                                ""NameEdit"": null,
                                ""ValueEdits"": {},
                                ""ValueFilter"": {
                                    ""type"": ""item"",
                                    ""query"": [""value-0""]
                                },
                                ""VirtualValueDefinitions"": null,
                                ""Selectable"": false
                            }
                        }
                    },
                    ""CreationTime"": ""2023-05-16T17:13:33+03:00"",
                    ""Archived"": false
                }";

        public static string V1_0_TABLETEST1_SER_DESER_EXPECTED_OUTPUT = @"{
                ""Settings"": {
                    ""VisualizationType"": ""Table"",
                    ""CutYAxis"": false,
                    ""DefaultSelectableVariableCodes"": {},
                    ""Layout"": {
                        ""RowVariableCodes"": [
                            ""Kuukausi"",
                            ""Ilmoittava lentoasema"",
                            ""Lennon tyyppi"",
                            ""Saapuneet/lähteneet""
                        ],
                        ""ColumnVariableCodes"": [
                            ""Toinen lentoasema"",
                            ""Tiedot""
                        ]
                    },
                    ""ShowDataPoints"": false
                },
                ""Version"": ""1.1"",
                ""Query"": {
                    ""TableReference"": {
                        ""Name"": ""table.px"",
                        ""Hierarchy"": [
                            ""foo"",
                            ""bar""
                        ]
                    },
                    ""ChartHeaderEdit"": {
                        ""fi"": ""v10 sq header-fi"",
                        ""sv"": ""v10 sq header-en"",
                        ""en"": ""v10 sq header-sv""
                    },
                    ""VariableQueries"": {
                        ""Kuukausi"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {},
                            ""ValueFilter"": {
                                ""type"": ""all""
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": true
                        },
                        ""Ilmoittava lentoasema"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {},
                            ""ValueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS"",
                                    ""EFHK"",
                                    ""EFET"",
                                    ""EFIV"",
                                    ""EFJO""
                                ]
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": false
                        },
                        ""Lennon tyyppi"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {},
                            ""ValueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": false
                        },
                        ""Saapuneet/lähteneet"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {},
                            ""ValueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": false
                        },
                        ""Toinen lentoasema"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {},
                            ""ValueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""SSS""
                                ]
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": false
                        },
                        ""Tiedot"": {
                            ""NameEdit"": {},
                            ""ValueEdits"": {
                                ""matkustajat"": {
                                    ""NameEdit"": null,
                                    ""ContentComponent"": {
                                        ""UnitEdit"": {},
                                        ""SourceEdit"": {
                                            ""fi"": ""PxGraf-fi"",
                                            ""sv"": ""PxGraf-sv"",
                                            ""en"": ""PxGraf-en""
                                        }
                                    }
                                },
                                ""rahti"": {
                                    ""NameEdit"": null,
                                    ""ContentComponent"": {
                                        ""UnitEdit"": {},
                                        ""SourceEdit"": {
                                            ""fi"": ""PxGraf-fi"",
                                            ""sv"": ""PxGraf-sv"",
                                            ""en"": ""PxGraf-en""
                                        }
                                    }
                                }
                            },
                            ""ValueFilter"": {
                                ""type"": ""item"",
                                ""query"": [
                                    ""matkustajat"",
                                    ""rahti""
                                ]
                            },
                            ""VirtualValueDefinitions"": [],
                            ""Selectable"": false
                        }
                    }
                },
                ""CreationTime"": ""2022-04-19T09:12:40+03:00"",
                ""Archived"": false
            }";
    }
}

namespace UnitTests.SerializerTests
{
    internal static class SavedQuerySerializerExpectedOutputFixtures
    {
        public static string V1_1_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
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
                    ""CreationTime"": ""2023-05-16T15:04:12.0948951+03:00"",
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
                    ""Archived"": false,
                    ""Version"": ""1.1""
                }";

        public static string V1_0_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT = @"{
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
                    ""CreationTime"": ""2023-05-16T17:13:33.2211177+03:00"",
                    ""Settings"": {
                        ""VisualizationType"": ""GroupVerticalBarChart"",
                        ""CutYAxis"": false,
                        ""MatchXLabelsToEnd"": false,
                        ""XLabelInterval"": 1,
                        ""Layout"": null,
                        ""ShowDataPoints"": false
                    },
                    ""Archived"": false,
                    ""Version"": ""1.1""
                }";
    }
}

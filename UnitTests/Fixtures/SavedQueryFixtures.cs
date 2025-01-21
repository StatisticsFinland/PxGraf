namespace UnitTests.Fixtures
{
    internal static class SavedQueryFixtures
    {
        internal const string V1_0_TEST_SAVEDQUERY1 = @"
        {
            ""Query"":{
                ""TableReference"":{
                    ""Name"":""table.px"",
                    ""Hierarchy"":[
                    ""foo"",
                    ""bar""
                    ]
                },
                ""ChartHeaderEdit"":null,
                ""VariableQueries"":{
                    ""Vuosi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-2"",
                            ""value-1"",
                            ""value-0""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    },
                    ""Syntymävaltio"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-0"",
                            ""value-1""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    },
                    ""Adoptiotyyppi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-0""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    },
                    ""Ikä"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-0""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    },
                    ""Sukupuoli"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-0""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    },
                    ""Tiedot"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                        ""type"":""item"",
                        ""query"":[
                            ""value-0""
                        ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                    }
                }
            },
            ""CreationTime"":""2023-05-16T17:13:33.2211177+03:00"",
            ""Archived"":false,
            ""Settings"":{
                ""PivotRequested"":true,
                ""MatchXLabelsToEnd"":false,
                ""XLabelInterval"":1,
                ""SelectedVisualization"":""GroupVerticalBarChart"",
                ""DefaultSelectableVariableCodes"":null
            }
        }";

        internal const string V1_0_TEST_SAVEDQUERY1_CAMEL_CASE_PROPERTY_NAMES = @"
        {
            ""query"": {
                ""tableReference"": {
                    ""name"": ""table.px"",
                    ""hierarchy"": [
                        ""foo"",
                        ""bar""
                    ]
                },
                ""chartHeaderEdit"": null,
                ""variableQueries"": {
                    ""Vuosi"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-2"",
                                ""value-1"",
                                ""value-0""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Syntymävaltio"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-0"",
                                ""value-1""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Adoptiotyyppi"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-0""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Ikä"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-0""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Sukupuoli"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-0""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Tiedot"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""value-0""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    }
                }
            },
            ""creationTime"": ""2023-05-16T17:13:33.2211177+03:00"",
            ""archived"": false,
            ""settings"": {
                ""pivotRequested"": true,
                ""matchXLabelsToEnd"": false,
                ""xLabelInterval"": 1,
                ""selectedVisualization"": ""GroupVerticalBarChart"",
                ""defaultSelectableVariableCodes"": null
            }
        }";

        internal const string V1_1_TEST_SAVEDQUERY1 = @"
        {
           ""Query"":{
              ""TableReference"":{
                 ""Name"":""table.px"",
                 ""Hierarchy"":[
                    ""foo"",
                    ""bar""
                 ]
              },
              ""ChartHeaderEdit"":null,
              ""VariableQueries"":{
                 ""Kuukausi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""2022M12"",
                          ""2022M11"",
                          ""2019M02""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Ilmoittava lentoasema"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""EFJO"",
                          ""EFKT""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Lennon tyyppi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""1""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Saapuneet/lähteneet"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""1""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Toinen lentoasema"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""EFHK""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Tiedot"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""matkustajat""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 }
              }
           },
           ""CreationTime"":""2023-05-16T15:04:12.0948951+03:00"",
           ""Archived"":false,
           ""Settings"":{
              ""MatchXLabelsToEnd"":false,
              ""XLabelInterval"":1,
              ""Layout"":{
                 ""RowVariableCodes"":[
                    ""Kuukausi""
                 ],
                 ""ColumnVariableCodes"":[
                    ""Ilmoittava lentoasema""
                 ]
              },
              ""VisualizationType"":""GroupVerticalBarChart"",
              ""DefaultSelectableVariableCodes"":null
           },
           ""Version"":""1.1""
        }";

        internal const string V1_1_TEST_SAVEDQUERY1_CAMEL_CASE_PROPERTY_NAMES = @"
        {
            ""query"": {
                ""tableReference"": {
                    ""name"": ""table.px"",
                    ""hierarchy"": [
                        ""foo"",
                        ""bar""
                    ]
                },
                ""chartHeaderEdit"": null,
                ""variableQueries"": {
                    ""Kuukausi"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""2022M12"",
                                ""2022M11"",
                                ""2019M02""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Ilmoittava lentoasema"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""EFJO"",
                                ""EFKT""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Lennon tyyppi"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""1""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Saapuneet/lähteneet"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""1""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Toinen lentoasema"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""EFHK""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    },
                    ""Tiedot"": {
                        ""nameEdit"": null,
                        ""valueEdits"": {

                        },
                        ""valueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""matkustajat""
                            ]
                        },
                        ""virtualValueDefinitions"": null,
                        ""selectable"": false
                    }
                }
            },
            ""creationTime"": ""2023-05-16T15:04:12.0948951+03:00"",
            ""archived"": false,
            ""settings"": {
                ""matchXLabelsToEnd"": false,
                ""xLabelInterval"": 1,
                ""layout"": {
                    ""rowVariableCodes"": [
                        ""Kuukausi""
                    ],
                    ""columnVariableCodes"": [
                        ""Ilmoittava lentoasema""
                    ]
                },
                ""visualizationType"": ""GroupVerticalBarChart"",
                ""defaultSelectableVariableCodes"": null
            },
            ""version"": ""1.1""
        }";

        internal const string V1_1_TEST_SAVEDQUERY2 = @"
        {
            ""Query"":{
                  ""TableReference"":{
                     ""Name"":""table.px"",
                     ""Hierarchy"":[
                        ""foo"",
                        ""bar""
                     ]
                  },
                  ""ChartHeaderEdit"":null,
                  ""VariableQueries"":{
                     ""Vuosi"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""2021"",
                              ""2020""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     },
                     ""Syntymävaltio"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""246"",
                              ""100""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     },
                     ""Adoptiotyyppi"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""1""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     },
                     ""Ikä"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""SSS""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     },
                     ""Sukupuoli"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""SSS""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     },
                     ""Tiedot"":{
                        ""NameEdit"":null,
                        ""ValueEdits"":{
               
                        },
                        ""ValueFilter"":{
                           ""type"":""item"",
                           ""query"":[
                              ""vm61""
                           ]
                        },
                        ""VirtualValueDefinitions"":null,
                        ""Selectable"":false
                     }
                  }
               },
               ""CreationTime"":""2023-05-16T15:24:21.4885826+03:00"",
               ""Archived"":false,
               ""Settings"":{
                  ""Sorting"":""2020"",
                  ""Layout"":{
                     ""RowVariableCodes"":[
                        ""Vuosi""
                     ],
                     ""ColumnVariableCodes"":[
                        ""Syntymävaltio""
                     ]
                  },
                  ""VisualizationType"":""GroupHorizontalBarChart"",
                  ""DefaultSelectableVariableCodes"":null
               },
               ""Version"":""1.1""
        }";

        internal const string V1_1_TEST_SAVEDQUERY3 = @"
        {
            ""Query"":{
              ""TableReference"":{
                 ""Name"":""table.px"",
                 ""Hierarchy"":[
                    ""foo"",
                    ""bar""
                 ]
              },
              ""ChartHeaderEdit"":null,
              ""VariableQueries"":{
                 ""Joukkoviestimet"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""all"",
                       ""query"":[
                          ""jvie030"",
                          ""jvie070"",
                          ""jvie060""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Vuosi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""2021"",
                          ""2012"",
                          ""2007""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Tiedot"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""bar""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 }
              }
           },
           ""CreationTime"":""2023-05-16T15:31:13.0853655+03:00"",
           ""Archived"":false,
           ""Settings"":{
              ""CutYAxis"":true,
              ""MultiselectableVariableCode"":null,
              ""Layout"":{
                 ""RowVariableCodes"":[
                    ""Joukkoviestimet""
                 ],
                 ""ColumnVariableCodes"":[
                    ""Vuosi""
                 ]
              },
              ""VisualizationType"":""LineChart"",
              ""DefaultSelectableVariableCodes"":null
           },
           ""Version"":""1.1""
        }";


        internal const string V11_TEST_TABLE_SAVEDQUERY = @"
        {
            ""Query"":{
              ""TableReference"":{
                 ""Name"":""table.px"",
                 ""Hierarchy"":[
                    ""foo"",
                    ""bar""
                 ]
              },
              ""ChartHeaderEdit"":null,
              ""VariableQueries"":{
                 ""Joukkoviestimet"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""jvie030"",
                          ""jvie070"",
                          ""jvie060""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Vuosi"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""2021"",
                          ""2012"",
                          ""2007""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""Tiedot"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""bar""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 }
              }
           },
           ""CreationTime"":""2023-05-16T15:31:13.0853655+03:00"",
           ""Archived"":false,
           ""Settings"":{
              ""Layout"":{
                 ""RowVariableCodes"":[
                    ""Joukkoviestimet""
                 ],
                 ""ColumnVariableCodes"":[
                    ""Vuosi""
                 ]
              },
              ""VisualizationType"":""Table"",
              ""DefaultSelectableVariableCodes"":null
           },
           ""Version"":""1.1""
        }";

        internal const string V1_1_TEST_SAVEDQUERY4 = @"
        {
            ""Query"":{
              ""TableReference"":{
                 ""Name"":""table.px"",
                 ""Hierarchy"":[
                    ""foo"",
                    ""bar""
                 ]
              },
              ""ChartHeaderEdit"":null,
              ""VariableQueries"":{
                 ""variable-0"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""all""
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""variable-1"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""item"",
                       ""query"":[
                          ""2021"",
                          ""2012"",
                          ""2007""
                       ]
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""variable-2"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""from"",
                       ""query"":""foo""
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 },
                 ""variable-3"":{
                    ""NameEdit"":null,
                    ""ValueEdits"":{
               
                    },
                    ""ValueFilter"":{
                       ""type"":""top"",
                       ""query"":3
                    },
                    ""VirtualValueDefinitions"":null,
                    ""Selectable"":false
                 }
              }
           },
           ""CreationTime"":""2023-05-16T15:31:13.0853655+03:00"",
           ""Archived"":false,
           ""Settings"":{
              ""CutYAxis"":true,
              ""MultiselectableVariableCode"":null,
              ""Layout"":{
                 ""RowVariableCodes"":[
                    ""variable-0""
                 ],
                 ""ColumnVariableCodes"":[
                    ""variable-1""
                 ]
              },
              ""VisualizationType"":""LineChart"",
              ""DefaultSelectableVariableCodes"":null
           },
           ""Version"":""1.1""
        }";

        internal const string V10_TEST_TABLE_SAVEDQUERY = @"
        {
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
                        ""NameEdit"": { },
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"": ""all""
                        },
                        ""VirtualValueDefinitions"": [],
                        ""Selectable"": true
                    },
                    ""Ilmoittava lentoasema"": {
                        ""NameEdit"": { },
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                              ""SSS"",
                              ""EFHK"",
                              ""EFET"",
                              ""EFIV"",
                              ""EFJO"",
                              ]
                        },
                        ""VirtualValueDefinitions"": [],
                        ""Selectable"": false
                    },
                    ""Lennon tyyppi"": {
                        ""NameEdit"": { },
                        ""ValueEdits"": { },
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
                        ""NameEdit"": { },
                        ""ValueEdits"": { },
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
                        ""NameEdit"": { },
                        ""ValueEdits"": { },
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
                        ""NameEdit"": { },
                        ""ValueEdits"": {
                            ""matkustajat"": {
                                ""NameEdit"": null,
                                ""ContentComponent"": {
                                    ""UnitEdit"": { },
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
                                    ""UnitEdit"": { },
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
            ""Archived"": false,
            ""Settings"": {
                ""RowVariableCodes"": [
                    ""Kuukausi"",
                    ""Ilmoittava lentoasema"",
                    ""Lennon tyyppi"",
                    ""Saapuneet/lähteneet""
                ],
                ""ColumnVariableCodes"": [
                    ""Toinen lentoasema"",
                    ""Tiedot""
                ],
                ""SelectedVisualization"": ""table"",
                ""DefaultSelectableVariableCodes"": { }
            }
        }";

        internal const string HORIZONTALBAR_SORTED_ASCENDING = @"
        {
            ""Query"": {
                ""TableReference"": {
                    ""Name"": ""table.px"",
                    ""Hierarchy"": [
                        ""foo"",
                        ""bar""
                    ]
                },
                ""ChartHeaderEdit"": null,
                ""VariableQueries"": {
                    ""Vuosineljännes"": {
                        ""NameEdit"": null,
                        ""ValueEdits"": {},
                        ""ValueFilter"": { 
                            ""type"": ""item"",
                            ""query"": [
                                ""2022Q4""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    },
                    ""Alue"": { 
                        ""NameEdit"": null,
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""091"",
                                ""853"",
                                ""837""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    },
                    ""Huoneluku"": {
                        ""NameEdit"": null,
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""01""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    },
                    ""Rahoitusmuoto"": { 
                        ""NameEdit"": null,
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"":""item"",
                            ""query"": [
                                ""1""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    },
                    ""Tiedot"": {
                        ""NameEdit"": null,
                        ""ValueEdits"": { },
                        ""ValueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""keskivuokra""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    }
                }
            },
            ""CreationTime"": ""2023-08-03T12:35:24.9552447+03:00"",
            ""Archived"": false,
            ""Settings"": {
                ""VisualizationType"": ""HorizontalBarChart"",
                ""Layout"": {
                    ""RowVariableCodes"": [],
                    ""ColumnVariableCodes"": [
                        ""Alue""
                    ]
                },
                ""CutYAxis"": false,
                ""Sorting"": ""ascending""
            },
            ""Version"": ""1.1""
        }";
    }
}

using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using System;
using System.Collections.Generic;

namespace UnitTests.Fixtures.ResponseFixtures
{
    internal static class VisualizationResponseFixtures
    {
        public const string ASCENDING_HORIZONTAL_BARCHART_RESPONSE_FIXTURE = /*lang=json, strict*/ @"
        {
            ""TableReference"": {
            ""Name"": ""table.px"",
            ""Hierarchy"": [
                ""foo"",
                ""bar""
            ]
            },
            ""Data"": [
            0.123,
            1.123,
            2.123
            ],
            ""DataNotes"": {},
            ""MissingDataInfo"": {},
            ""MetaData"": [
            {
                ""code"": ""Vuosineljännes"",
                ""name"": {
                ""fi"": ""Vuosineljännes"",
                ""en"": ""Vuosineljännes.en""
                },
                ""note"": null,
                ""type"": ""T"",
                ""values"": [
                {
                    ""code"": ""2000"",
                    ""name"": {
                    ""fi"": ""2000"",
                    ""en"": ""2000.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                }
                ]
            },
            {
                ""code"": ""Huoneluku"",
                ""name"": {
                ""fi"": ""Huoneluku"",
                ""en"": ""Huoneluku.en""
                },
                ""note"": null,
                ""type"": ""P"",
                ""values"": [
                {
                    ""code"": ""value-0"",
                    ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                }
                ]
            },
            {
                ""code"": ""Rahoitusmuoto"",
                ""name"": {
                ""fi"": ""Rahoitusmuoto"",
                ""en"": ""Rahoitusmuoto.en""
                },
                ""note"": null,
                ""type"": ""F"",
                ""values"": [
                {
                    ""code"": ""value-0"",
                    ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                }
                ]
            },
            {
                ""code"": ""Tiedot"",
                ""name"": {
                ""fi"": ""Tiedot"",
                ""en"": ""Tiedot.en""
                },
                ""note"": null,
                ""type"": ""C"",
                ""values"": [
                {
                    ""code"": ""value-0"",
                    ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": {
                    ""Unit"": {
                        ""fi"": ""value-0-unit"",
                        ""en"": ""value-0-unit.en""
                    },
                    ""Source"": {
                        ""fi"": ""value-0-source"",
                        ""en"": ""value-0-source.en""
                    },
                    ""NumberOfDecimals"": 0,
                    ""LastUpdated"": ""2009-09-01T00:00:00.000Z""
                    }
                }
                ]
            },
            {
                ""code"": ""Alue"",
                ""name"": {
                ""fi"": ""Alue"",
                ""en"": ""Alue.en""
                },
                ""note"": null,
                ""type"": ""G"",
                ""values"": [
                {
                    ""code"": ""value-0"",
                    ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                },
                {
                    ""code"": ""value-1"",
                    ""name"": {
                    ""fi"": ""value-1"",
                    ""en"": ""value-1.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                },
                {
                    ""code"": ""value-2"",
                    ""name"": {
                    ""fi"": ""value-2"",
                    ""en"": ""value-2.en""
                    },
                    ""note"": null,
                    ""isSum"": false,
                    ""contentComponent"": null
                }
                ]
            }
            ],
            ""SelectableVariableCodes"": [],
            ""RowVariableCodes"": [],
            ""ColumnVariableCodes"": [
            ""Alue""
            ],
            ""Header"": {
            ""fi"": ""value-0, value-0, value-0 2000 muuttujana Alue"",
            ""en"": ""value-0.en, value-0.en, value-0.en 2000.en by Alue.en""
            },
            ""VisualizationSettings"": {
            ""VisualizationType"": ""HorizontalBarChart"",
            ""TimeVariableIntervals"": ""Year"",
            ""TimeSeriesStartingPoint"": ""2000-01-01T00:00:00Z"",
            ""CutValueAxis"": false,
            ""Sorting"": ""ascending"",
            ""ShowDataPoints"": false
            }
        }";

        public const string LINE_CHART_RESPONSE_FIXTURE = /*lang=json,strict*/ @"
        {
          ""TableReference"": {
            ""Name"": ""TestPxFile.px"",
            ""Hierarchy"": [
              ""testpath"",
              ""to"",
              ""test"",
              ""file""
            ]
          },
          ""Data"": [
            0.123,
            1.123,
            2.123,
            3.123,
            4.123,
            5.123,
            6.123,
            7.123,
            8.123,
            9.123,
            10.123,
            11.123,
            12.123,
            13.123,
            14.123,
            15.123,
            16.123,
            17.123,
            18.123,
            19.123,
            20.123,
            21.123,
            22.123,
            23.123,
            24.123,
            25.123,
            26.123,
            27.123,
            28.123,
            29.123,
            30.123,
            31.123,
            32.123,
            33.123,
            34.123,
            35.123,
            36.123,
            37.123,
            38.123,
            39.123,
            40.123,
            41.123,
            42.123,
            43.123,
            44.123
          ],
          ""DataNotes"": {},
          ""MissingDataInfo"": {},
          ""MetaData"": [
            {
              ""code"": ""Huoneluku"",
              ""name"": {
                ""fi"": ""Huoneluku"",
                ""en"": ""Huoneluku.en""
              },
              ""note"": null,
              ""type"": ""P"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Rahoitusmuoto"",
              ""name"": {
                ""fi"": ""Rahoitusmuoto"",
                ""en"": ""Rahoitusmuoto.en""
              },
              ""note"": null,
              ""type"": ""F"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Tiedot"",
              ""name"": {
                ""fi"": ""Tiedot"",
                ""en"": ""Tiedot.en""
              },
              ""note"": null,
              ""type"": ""C"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": {
                    ""Unit"": {
                      ""fi"": ""value-0-unit"",
                      ""en"": ""value-0-unit.en""
                    },
                    ""Source"": {
                      ""fi"": ""value-0-source"",
                      ""en"": ""value-0-source.en""
                    },
                    ""NumberOfDecimals"": 0,
                    ""LastUpdated"": ""2009-09-01T00:00:00.000Z""
                  }
                }
              ]
            },
            {
              ""code"": ""Alue"",
              ""name"": {
                ""fi"": ""Alue"",
                ""en"": ""Alue.en""
              },
              ""note"": null,
              ""type"": ""G"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""value-1"",
                  ""name"": {
                    ""fi"": ""value-1"",
                    ""en"": ""value-1.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""value-2"",
                  ""name"": {
                    ""fi"": ""value-2"",
                    ""en"": ""value-2.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Vuosineljännes"",
              ""name"": {
                ""fi"": ""Vuosineljännes"",
                ""en"": ""Vuosineljännes.en""
              },
              ""note"": null,
              ""type"": ""T"",
              ""values"": [
                {
                  ""code"": ""2000"",
                  ""name"": {
                    ""fi"": ""2000"",
                    ""en"": ""2000.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2001"",
                  ""name"": {
                    ""fi"": ""2001"",
                    ""en"": ""2001.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2002"",
                  ""name"": {
                    ""fi"": ""2002"",
                    ""en"": ""2002.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2003"",
                  ""name"": {
                    ""fi"": ""2003"",
                    ""en"": ""2003.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2004"",
                  ""name"": {
                    ""fi"": ""2004"",
                    ""en"": ""2004.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2005"",
                  ""name"": {
                    ""fi"": ""2005"",
                    ""en"": ""2005.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2006"",
                  ""name"": {
                    ""fi"": ""2006"",
                    ""en"": ""2006.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2007"",
                  ""name"": {
                    ""fi"": ""2007"",
                    ""en"": ""2007.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2008"",
                  ""name"": {
                    ""fi"": ""2008"",
                    ""en"": ""2008.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2009"",
                  ""name"": {
                    ""fi"": ""2009"",
                    ""en"": ""2009.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2010"",
                  ""name"": {
                    ""fi"": ""2010"",
                    ""en"": ""2010.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2011"",
                  ""name"": {
                    ""fi"": ""2011"",
                    ""en"": ""2011.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2012"",
                  ""name"": {
                    ""fi"": ""2012"",
                    ""en"": ""2012.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2013"",
                  ""name"": {
                    ""fi"": ""2013"",
                    ""en"": ""2013.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2014"",
                  ""name"": {
                    ""fi"": ""2014"",
                    ""en"": ""2014.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            }
          ],
          ""SelectableVariableCodes"": [],
          ""RowVariableCodes"": [
            ""Alue""
          ],
          ""ColumnVariableCodes"": [
            ""Vuosineljännes""
          ],
          ""Header"": {
            ""fi"": ""value-0, value-0, value-0 2000-2014 muuttujana Alue"",
            ""en"": ""value-0.en, value-0.en, value-0.en in 2000.en to 2014.en by Alue.en""
          },
          ""VisualizationSettings"": {
            ""VisualizationType"": ""LineChart"",
            ""TimeVariableIntervals"": ""Year"",
            ""TimeSeriesStartingPoint"": ""2000-01-01T00:00:00Z"",
            ""CutValueAxis"": false,
            ""ShowDataPoints"": false
          }
        }";
        public const string LINE_CHART_RESPONSE_FIXTURE_WITH_MISSING_VALUES = /*lang=json,strict*/ @"
        {
          ""TableReference"": {
            ""Name"": ""TestPxFile.px"",
            ""Hierarchy"": [
              ""testpath"",
              ""to"",
              ""test"",
              ""file""
            ]
          },
          ""Data"": [
            null,
            1.123,
            2.123,
            null,
            4.123,
            5.123,
            null,
            7.123,
            8.123,
            null,
            10.123,
            11.123,
            null,
            13.123,
            14.123,
            null,
            16.123,
            17.123,
            null,
            19.123,
            20.123,
            null,
            22.123,
            23.123,
            null,
            25.123,
            26.123,
            null,
            28.123,
            29.123,
            null,
            31.123,
            32.123,
            null,
            34.123,
            35.123,
            null,
            37.123,
            38.123,
            null,
            40.123,
            41.123,
            null,
            43.123,
            44.123
          ],
          ""DataNotes"": {},
          ""MissingDataInfo"": {
            ""0"": 1,
            ""3"": 2,
            ""6"": 3,
            ""9"": 4,
            ""12"": 5,
            ""15"": 6,
            ""18"": 7,
            ""21"": 1,
            ""24"": 2,
            ""27"": 3,
            ""30"": 4,
            ""33"": 5,
            ""36"": 6,
            ""39"": 7,
            ""42"": 1
          },
          ""MetaData"": [
            {
              ""code"": ""Huoneluku"",
              ""name"": {
                ""fi"": ""Huoneluku"",
                ""en"": ""Huoneluku.en""
              },
              ""note"": null,
              ""type"": ""P"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Rahoitusmuoto"",
              ""name"": {
                ""fi"": ""Rahoitusmuoto"",
                ""en"": ""Rahoitusmuoto.en""
              },
              ""note"": null,
              ""type"": ""F"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Tiedot"",
              ""name"": {
                ""fi"": ""Tiedot"",
                ""en"": ""Tiedot.en""
              },
              ""note"": null,
              ""type"": ""C"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": {
                    ""Unit"": {
                      ""fi"": ""value-0-unit"",
                      ""en"": ""value-0-unit.en""
                    },
                    ""Source"": {
                      ""fi"": ""value-0-source"",
                      ""en"": ""value-0-source.en""
                    },
                    ""NumberOfDecimals"": 0,
                    ""LastUpdated"": ""2009-09-01T00:00:00.000Z""
                  }
                }
              ]
            },
            {
              ""code"": ""Alue"",
              ""name"": {
                ""fi"": ""Alue"",
                ""en"": ""Alue.en""
              },
              ""note"": null,
              ""type"": ""G"",
              ""values"": [
                {
                  ""code"": ""value-0"",
                  ""name"": {
                    ""fi"": ""value-0"",
                    ""en"": ""value-0.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""value-1"",
                  ""name"": {
                    ""fi"": ""value-1"",
                    ""en"": ""value-1.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""value-2"",
                  ""name"": {
                    ""fi"": ""value-2"",
                    ""en"": ""value-2.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            },
            {
              ""code"": ""Vuosineljännes"",
              ""name"": {
                ""fi"": ""Vuosineljännes"",
                ""en"": ""Vuosineljännes.en""
              },
              ""note"": null,
              ""type"": ""T"",
              ""values"": [
                {
                  ""code"": ""2000"",
                  ""name"": {
                    ""fi"": ""2000"",
                    ""en"": ""2000.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2001"",
                  ""name"": {
                    ""fi"": ""2001"",
                    ""en"": ""2001.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2002"",
                  ""name"": {
                    ""fi"": ""2002"",
                    ""en"": ""2002.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2003"",
                  ""name"": {
                    ""fi"": ""2003"",
                    ""en"": ""2003.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2004"",
                  ""name"": {
                    ""fi"": ""2004"",
                    ""en"": ""2004.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2005"",
                  ""name"": {
                    ""fi"": ""2005"",
                    ""en"": ""2005.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2006"",
                  ""name"": {
                    ""fi"": ""2006"",
                    ""en"": ""2006.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2007"",
                  ""name"": {
                    ""fi"": ""2007"",
                    ""en"": ""2007.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2008"",
                  ""name"": {
                    ""fi"": ""2008"",
                    ""en"": ""2008.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2009"",
                  ""name"": {
                    ""fi"": ""2009"",
                    ""en"": ""2009.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2010"",
                  ""name"": {
                    ""fi"": ""2010"",
                    ""en"": ""2010.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2011"",
                  ""name"": {
                    ""fi"": ""2011"",
                    ""en"": ""2011.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2012"",
                  ""name"": {
                    ""fi"": ""2012"",
                    ""en"": ""2012.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2013"",
                  ""name"": {
                    ""fi"": ""2013"",
                    ""en"": ""2013.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                },
                {
                  ""code"": ""2014"",
                  ""name"": {
                    ""fi"": ""2014"",
                    ""en"": ""2014.en""
                  },
                  ""note"": null,
                  ""isSum"": false,
                  ""contentComponent"": null
                }
              ]
            }
          ],
          ""SelectableVariableCodes"": [],
          ""RowVariableCodes"": [
            ""Alue""
          ],
          ""ColumnVariableCodes"": [
            ""Vuosineljännes""
          ],
          ""Header"": {
            ""fi"": ""value-0, value-0, value-0 2000-2014 muuttujana Alue"",
            ""en"": ""value-0.en, value-0.en, value-0.en in 2000.en to 2014.en by Alue.en""
          },
          ""VisualizationSettings"": {
            ""VisualizationType"": ""LineChart"",
            ""TimeVariableIntervals"": ""Year"",
            ""TimeSeriesStartingPoint"": ""2000-01-01T00:00:00Z"",
            ""CutValueAxis"": false,
            ""ShowDataPoints"": false
          }
        }";
    }
}

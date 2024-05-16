using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.PxWebInterfaceTests.Fixtures
{
    internal static class JsonStat2Fixtures
    {
        internal const string TEST_JSONSTAT2_FI = @"
        {
            ""class"": ""dataset"",
            ""label"": ""Asuntokuntien velat, velkaantumisaste ja tulot muuttujina Vuosi, Tiedot, Maakunta, Viitehenkilön ikä ja Asuntokunnat"",
            ""source"": ""Tilastokeskus, velkaantumistilasto"",
            ""updated"": ""2022-12-21T06.00.00Z"",
            ""id"": [
                ""Vuosi"",
                ""Tiedot"",
                ""Maakunta"",
                ""Viitehenkilön ikä"",
                ""Asuntokunnat""
            ],
            ""size"": [
                1,
                1,
                1,
                1,
                1
            ],
            ""dimension"": {
                ""Vuosi"": {
                    ""label"": ""Vuosi"",
                    ""category"": {
                        ""index"": {
                            ""2002"": 0
                        },
                        ""label"": {
                            ""2002"": ""2002""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Vuosi"": ""SCALE-TYPE=None""
                                }
                            }
                        ]
                    }
                },
                ""Tiedot"": {
                    ""label"": ""Tiedot"",
                    ""category"": {
                        ""index"": {
                            ""velat_yht"": 0
                        },
                        ""label"": {
                            ""velat_yht"": ""Velat yhteensä (euroa)""
                        },
                        ""unit"": {
                            ""velat_yht"": {
                                ""base"": ""euroa"",
                                ""decimals"": 0
                            }
                        }
                    }
                },
                ""Maakunta"": {
                    ""label"": ""Maakunta"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""KOKO MAA""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Maakunta"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                },
                ""Viitehenkilön ikä"": {
                    ""label"": ""Viitehenkilön ikä"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""Yhteensä""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Viitehenkilön ikä"": ""SCALE-TYPE=ordinal""
                                }
                            }
                        ]
                    }
                },
                ""Asuntokunnat"": {
                    ""label"": ""Asuntokunnat"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""Kaikki asuntokunnat""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Asuntokunnat"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                }
            },
            ""value"": [
                58443598209
            ],
            ""role"": {
                ""time"": [
                    ""Vuosi""
                ],
                ""metric"": [
                    ""Tiedot""
                ]
            },
            ""version"": ""2.0"",
            ""extension"": {
                ""px"": {
                    ""tableid"": ""statfin_velk_pxt_13kt"",
                    ""decimals"": 0
                }
            }
        }";

        internal const string TEST_JSONSTAT2_SV = @"
        {
            ""class"": ""dataset"",
            ""label"": ""Bostadshushållernas skulder, skuldsättningsgrad och inkomst efter År, Uppgifter, Landskap, Referenspersons ålder och Bostadsushåll"",
            ""source"": ""Statistikcentralen, statistik över skuldsättning"",
            ""updated"": ""2022-12-21T06.00.00Z"",
            ""id"": [
                ""Vuosi"",
                ""Tiedot"",
                ""Maakunta"",
                ""Viitehenkilön ikä"",
                ""Asuntokunnat""
            ],
            ""size"": [
                1,
                6,
                1,
                1,
                1
            ],
            ""dimension"": {
                ""Vuosi"": {
                    ""label"": ""År"",
                    ""category"": {
                        ""index"": {
                            ""2002"": 0
                        },
                        ""label"": {
                            ""2002"": ""2002""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Vuosi"": ""SCALE-TYPE=None""
                                }
                            }
                        ]
                    }
                },
                ""Tiedot"": {
                    ""label"": ""Uppgifter"",
                    ""category"": {
                        ""index"": {
                            ""velat_yht"": 0,
                            ""vaste"": 1,
                            ""kturaha_summa"": 2,
                            ""kturaha_ka"": 3,
                            ""kturaha_med"": 4,
                            ""asuntokunta_lkm"": 5
                        },
                        ""label"": {
                            ""velat_yht"": ""Skulderna sammanlagt (euro)"",
                            ""vaste"": ""Skuldsättningsgrad (procent)"",
                            ""kturaha_summa"": ""Disponibla penninginkomster (euro)"",
                            ""kturaha_ka"": ""Disponibla penninginkomster, medeltal (euro)"",
                            ""kturaha_med"": ""Disponibla penninginkomster, median (euro)"",
                            ""asuntokunta_lkm"": ""Antal bostadshushåll""
                        },
                        ""unit"": {
                            ""velat_yht"": {
                                ""base"": ""euro"",
                                ""decimals"": 0
                            },
                            ""vaste"": {
                                ""base"": ""procent"",
                                ""decimals"": 1
                            },
                            ""kturaha_summa"": {
                                ""base"": ""euro"",
                                ""decimals"": 0
                            },
                            ""kturaha_ka"": {
                                ""base"": ""euro"",
                                ""decimals"": 0
                            },
                            ""kturaha_med"": {
                                ""base"": ""euro"",
                                ""decimals"": 0
                            },
                            ""asuntokunta_lkm"": {
                                ""base"": ""antal"",
                                ""decimals"": 0
                            }
                        }
                    }
                },
                ""Maakunta"": {
                    ""label"": ""Landskap"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""HELA LANDET""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Maakunta"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                },
                ""Viitehenkilön ikä"": {
                    ""label"": ""Referenspersons ålder"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""Sammanlagt""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Viitehenkilön ikä"": ""SCALE-TYPE=ordinal""
                                }
                            }
                        ]
                    }
                },
                ""Asuntokunnat"": {
                    ""label"": ""Bostadsushåll"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""Alla bostadshushåll""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Asuntokunnat"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                }
            },
            ""value"": [
                58443598209,
                70.3,
                83088568016,
                35296,
                28897,
                2354082
            ],
            ""role"": {
                ""time"": [
                    ""Vuosi""
                ],
                ""metric"": [
                    ""Tiedot""
                ]
            },
            ""version"": ""2.0"",
            ""extension"": {
                ""px"": {
                    ""tableid"": ""statfin_velk_pxt_13kt"",
                    ""decimals"": 0
                }
            }
        }";

        internal const string TEST_JSONSTAT2_EN = @"
        {
            ""class"": ""dataset"",
            ""label"": ""Debts, rate of indebtedness and income of household-dwelling units by Year, Information, Region, Age of reference person and Household-dwelling units"",
            ""source"": ""Statistics Finland, indebtedness"",
            ""updated"": ""2022-12-21T06.00.00Z"",
            ""id"": [
                ""Vuosi"",
                ""Tiedot"",
                ""Maakunta"",
                ""Viitehenkilön ikä"",
                ""Asuntokunnat""
            ],
            ""size"": [
                1,
                6,
                1,
                1,
                1
            ],
            ""dimension"": {
                ""Vuosi"": {
                    ""label"": ""Year"",
                    ""category"": {
                        ""index"": {
                            ""2002"": 0
                        },
                        ""label"": {
                            ""2002"": ""2002""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Vuosi"": ""SCALE-TYPE=None""
                                }
                            }
                        ]
                    }
                },
                ""Tiedot"": {
                    ""label"": ""Information"",
                    ""category"": {
                        ""index"": {
                            ""velat_yht"": 0,
                            ""vaste"": 1,
                            ""kturaha_summa"": 2,
                            ""kturaha_ka"": 3,
                            ""kturaha_med"": 4,
                            ""asuntokunta_lkm"": 5
                        },
                        ""label"": {
                            ""velat_yht"": ""Debts total (euros)"",
                            ""vaste"": ""Rate of indebtedness (per cent)"",
                            ""kturaha_summa"": ""Disposable monetary income (euros)"",
                            ""kturaha_ka"": ""Disposable monetary income, mean (euros)"",
                            ""kturaha_med"": ""Disposable monetary income, median (euros)"",
                            ""asuntokunta_lkm"": ""Number of household-dwelling units""
                        },
                        ""unit"": {
                            ""velat_yht"": {
                                ""base"": ""euros"",
                                ""decimals"": 0
                            },
                            ""vaste"": {
                                ""base"": ""per cent"",
                                ""decimals"": 1
                            },
                            ""kturaha_summa"": {
                                ""base"": ""euros"",
                                ""decimals"": 0
                            },
                            ""kturaha_ka"": {
                                ""base"": ""euros"",
                                ""decimals"": 0
                            },
                            ""kturaha_med"": {
                                ""base"": ""euros"",
                                ""decimals"": 0
                            },
                            ""asuntokunta_lkm"": {
                                ""base"": ""number"",
                                ""decimals"": 0
                            }
                        }
                    }
                },
                ""Maakunta"": {
                    ""label"": ""Region"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""WHOLE COUNTRY""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Maakunta"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                },
                ""Viitehenkilön ikä"": {
                    ""label"": ""Age of reference person"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""Total""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Viitehenkilön ikä"": ""SCALE-TYPE=ordinal""
                                }
                            }
                        ]
                    }
                },
                ""Asuntokunnat"": {
                    ""label"": ""Household-dwelling units"",
                    ""category"": {
                        ""index"": {
                            ""SSS"": 0
                        },
                        ""label"": {
                            ""SSS"": ""All household-dwelling units""
                        }
                    },
                    ""link"": {
                        ""describedby"": [
                            {
                                ""extension"": {
                                    ""Asuntokunnat"": ""SCALE-TYPE=nominal""
                                }
                            }
                        ]
                    }
                }
            },
            ""value"": [
                58443598209,
                70.3,
                83088568016,
                35296,
                28897,
                2354082
            ],
            ""role"": {
                ""time"": [
                    ""Vuosi""
                ],
                ""metric"": [
                    ""Tiedot""
                ]
            },
            ""version"": ""2.0"",
            ""extension"": {
                ""px"": {
                    ""tableid"": ""statfin_velk_pxt_13kt"",
                    ""decimals"": 0
                }
            }
        }";
    }
}

namespace UnitTests.Fixtures
{
    internal static class ArchiveCubeFixtures
    {
        internal const string EXPECTED_ARCHIVE_CUBE_V11 = @"
        {
            ""meta"": {
                ""defaultLanguage"": ""fi"",
                ""availableLanguages"": [""fi"", ""en""],
                ""dimensions"": [
                    {
                        ""type"": ""Content"",
                        ""code"": ""variable-0"",
                        ""name"": {
                            ""fi"": ""variable-0"",
                            ""en"": ""variable-0.en""
                        },
                        ""values"": [
                            {
                                ""unit"": {
                                    ""fi"": ""value-0-unit"",
                                    ""en"": ""value-0-unit.en""
                                },
                                ""lastUpdated"": ""2009-09-01T00:00:00Z"",
                                ""precision"": 0,
                                ""code"": ""value-0"",
                                ""name"": {
                                    ""fi"": ""value-0"",
                                    ""en"": ""value-0.en""
                                },
                                ""additionalProperties"": {
                                    ""SOURCE"": {
                                        ""type"": ""MultilanguageText"",
                                        ""value"": {
                                            ""fi"": ""value-0-source"",
                                            ""en"": ""value-0-source.en""
                                        }
                                    }
                                },
                                ""isVirtual"": false
                            }
                        ],
                        ""additionalProperties"": {}
                    },
                    {
                        ""type"": ""Time"",
                        ""code"": ""variable-1"",
                        ""name"": {
                            ""fi"": ""variable-1"",
                            ""en"": ""variable-1.en""
                        },
                        ""interval"": ""Year"",
                        ""values"": [
                            {
                                ""code"": ""2000"",
                                ""name"": {
                                    ""fi"": ""2000"",
                                    ""en"": ""2000.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            },
                            {
                                ""code"": ""2001"",
                                ""name"": {
                                    ""fi"": ""2001"",
                                    ""en"": ""2001.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            },
                            {
                                ""code"": ""2002"",
                                ""name"": {
                                    ""fi"": ""2002"",
                                    ""en"": ""2002.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            },
                            {
                                ""code"": ""2003"",
                                ""name"": {
                                    ""fi"": ""2003"",
                                    ""en"": ""2003.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            }
                        ],
                        ""additionalProperties"": {}
                    },
                    {
                        ""type"": ""Other"",
                        ""code"": ""variable-2"",
                        ""name"": {
                            ""fi"": ""variable-2"",
                            ""en"": ""variable-2.en""
                        },
                        ""values"": [
                            {
                                ""code"": ""value-0"",
                                ""name"": {
                                    ""fi"": ""value-0"",
                                    ""en"": ""value-0.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            },
                            {
                                ""code"": ""value-1"",
                                ""name"": {
                                    ""fi"": ""value-1"",
                                    ""en"": ""value-1.en""
                                },
                                ""additionalProperties"": {},
                                ""isVirtual"": false
                            }
                        ],
                        ""additionalProperties"": {}
                    }
                ],
                ""additionalProperties"": {
                    ""NOTE"": {
                        ""type"": ""MultilanguageText"",
                        ""value"": {
                            ""fi"": ""Test note"",
                            ""en"": ""Test note.en""
                        }
                    }
                }
            },
            ""version"": ""1.1"",
            ""creationTime"": ""2024-08-19T14:00:00Z"",
            ""data"":[ 0.123, ""."", 2.123, 3.123, 4.123, 5.123, 6.123, 7.123],
            ""dataNotes"": null
        }
        ";

        internal const string ARCHIVE_CUBE_V11 = @"
        {
            ""CreationTime"": ""2024-08-19T14:00:00Z"",
            ""Meta"": {
                ""DefaultLanguage"": ""fi"",
                ""AvailableLanguages"": [""fi"", ""en""],
                ""Dimensions"": [
                    {
                        ""Type"": ""Content"",
                        ""Code"": ""variable-0"",
                        ""Name"": {
                            ""fi"": ""variable-0"",
                            ""en"": ""variable-0.en""
                        },
                        ""Values"": [
                            {
                                ""Unit"": {
                                    ""fi"": ""value-0-unit"",
                                    ""en"": ""value-0-unit.en""
                                },
                                ""LastUpdated"": ""2009-09-01T00:00:00Z"",
                                ""Precision"": 0,
                                ""Code"": ""value-0"",
                                ""Name"": {
                                    ""fi"": ""value-0"",
                                    ""en"": ""value-0.en""
                                },
                                ""AdditionalProperties"": {
                                    ""SOURCE"": {
                                        ""Type"": ""MultilanguageText"",
                                        ""Value"": {
                                            ""fi"": ""value-0-source"",
                                            ""en"": ""value-0-source.en""
                                        }
                                    }
                                },
                                ""IsVirtual"": false
                            }
                        ],
                        ""AdditionalProperties"": {}
                    },
                    {
                        ""Type"": ""Time"",
                        ""Code"": ""variable-1"",
                        ""Name"": {
                            ""fi"": ""variable-1"",
                            ""en"": ""variable-1.en""
                        },
                        ""Interval"": ""Year"",
                        ""Values"": [
                            {
                                ""Code"": ""2000"",
                                ""Name"": {
                                    ""fi"": ""2000"",
                                    ""en"": ""2000.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            },
                            {
                                ""Code"": ""2001"",
                                ""Name"": {
                                    ""fi"": ""2001"",
                                    ""en"": ""2001.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            },
                            {
                                ""Code"": ""2002"",
                                ""Name"": {
                                    ""fi"": ""2002"",
                                    ""en"": ""2002.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            },
                            {
                                ""Code"": ""2003"",
                                ""Name"": {
                                    ""fi"": ""2003"",
                                    ""en"": ""2003.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            }
                        ],
                        ""AdditionalProperties"": {}
                    },
                    {
                        ""Type"": ""Other"",
                        ""Code"": ""variable-2"",
                        ""Name"": {
                            ""fi"": ""variable-2"",
                            ""en"": ""variable-2.en""
                        },
                        ""Values"": [
                            {
                                ""Code"": ""value-0"",
                                ""Name"": {
                                    ""fi"": ""value-0"",
                                    ""en"": ""value-0.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            },
                            {
                                ""Code"": ""value-1"",
                                ""Name"": {
                                    ""fi"": ""value-1"",
                                    ""en"": ""value-1.en""
                                },
                                ""AdditionalProperties"": {},
                                ""IsVirtual"": false
                            }
                        ],
                        ""AdditionalProperties"": {}
                    }
                ],
                ""AdditionalProperties"": {
                    ""NOTE"": {
                        ""Type"": ""MultilanguageText"",
                        ""Value"": {
                            ""fi"": ""Test note"",
                            ""en"": ""Test note.en""
                        }
                    }
                }
            },
            ""Data"":[ 0.123, ""."", 2.123, 3.123, 4.123, 5.123, 6.123, 7.123],
            ""DataNotes"": null,
            ""Version"": ""1.1""
        }
        ";


        public const string ARCHIVE_CUBE_V10 = @"
        {
            ""creationTime"": ""2024-06-06T15:38:51.5768284+03:00"",
            ""meta"": {
                ""Languages"": [""fi"", ""sv"", ""en""],
                ""Header"": {
                    ""fi"": ""Header"",
                    ""sv"": ""Header.sv"",
                    ""en"": ""Header.en""
                },
                ""Note"": {
                    ""fi"": ""Note"",
                    ""sv"": ""Note.sv"",
                    ""en"": ""Note.en""
                },
                ""Variables"": [
                    {
                        ""code"": ""variable-0"",
                        ""name"": {
                            ""fi"": ""variable-0.fi"",
                            ""sv"": ""variable-0.sv"",
                            ""en"": ""variable-0.en""
                        },
                        ""note"": null,
                        ""type"": ""T"",
                        ""values"": [
                            {
                                ""code"": ""variable-0-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-0-value-0.fi"",
                                    ""sv"": ""variable-0-value-0.sv"",
                                    ""en"": ""variable-0-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            }
                        ]
                    },
                    {
                        ""code"": ""variable-1"",
                        ""name"": {
                            ""fi"": ""variable-1.fi"",
                            ""sv"": ""variable-1.sv"",
                            ""en"": ""variable-1.en""
                        },
                        ""note"": null,
                        ""type"": ""G"",
                        ""values"": [
                            {
                                ""code"": ""variable-1-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-0.fi"",
                                    ""sv"": ""variable-1-value-0.sv"",
                                    ""en"": ""variable-1-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-1"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-1.fi"",
                                    ""sv"": ""variable-1-value-1.sv"",
                                    ""en"": ""variable-1-value-1.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-2"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-2.fi"",
                                    ""sv"": ""variable-1-value-2.sv"",
                                    ""en"": ""variable-1-value-2.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-3"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-3.fi"",
                                    ""sv"": ""variable-1-value-3.sv"",
                                    ""en"": ""variable-1-value-3.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-4"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-4.fi"",
                                    ""sv"": ""variable-1-value-4.sv"",
                                    ""en"": ""variable-1-value-4.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-5"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-5.fi"",
                                    ""sv"": ""variable-1-value-5.sv"",
                                    ""en"": ""variable-1-value-5.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            },
                            {
                                ""code"": ""variable-1-value-6"",
                                ""name"": {
                                    ""fi"": ""variable-1-value-6.fi"",
                                    ""sv"": ""variable-1-value-6.sv"",
                                    ""en"": ""variable-1-value-6.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": null
                            }
                        ]
                    },
                    {
                        ""code"": ""variable-2"",
                        ""name"": {
                            ""fi"": ""variable-2.fi"",
                            ""sv"": ""variable-2.sv"",
                            ""en"": ""variable-2.en""
                        },
                        ""note"": null,
                        ""type"": ""F"",
                        ""values"": [
                            {
                                ""code"": ""variable-2-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-2-value-0.fi"",
                                    ""sv"": ""variable-2-value-0.sv"",
                                    ""en"": ""variable-2-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": true,
                                ""contentComponent"": null
                            }
                        ]
                    },
                    {
                        ""code"": ""variable-3"",
                        ""name"": {
                            ""fi"": ""variable-3.fi"",
                            ""sv"": ""variable-3.sv"",
                            ""en"": ""variable-3.en""
                        },
                        ""note"": null,
                        ""type"": ""P"",
                        ""values"": [
                            {
                                ""code"": ""variable-3-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-3-value-0.fi"",
                                    ""sv"": ""variable-3-value-0.sv"",
                                    ""en"": ""variable-3-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": true,
                                ""contentComponent"": null
                            }
                        ]
                    },
                    {
                        ""code"": ""variable-4"",
                        ""name"": {
                            ""fi"": ""variable-4.fi"",
                            ""sv"": ""variable-4.sv"",
                            ""en"": ""variable-4.en""
                        },
                        ""note"": null,
                        ""type"": ""P"",
                        ""values"": [
                            {
                                ""code"": ""variable-4-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-4-value-0.fi"",
                                    ""sv"": ""variable-4-value-0.sv"",
                                    ""en"": ""variable-4-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": true,
                                ""contentComponent"": null
                            }
                        ]
                    },
                    {
                        ""code"": ""variable-5"",
                        ""name"": {
                            ""fi"": ""variable-5.fi"",
                            ""sv"": ""variable-5.sv"",
                            ""en"": ""variable-5.en""
                        },
                        ""note"": null,
                        ""type"": ""C"",
                        ""values"": [
                            {
                                ""code"": ""variable-5-value-0"",
                                ""name"": {
                                    ""fi"": ""variable-5-value-0.fi"",
                                    ""sv"": ""variable-5-value-0.sv"",
                                    ""en"": ""variable-5-value-0.en""
                                },
                                ""note"": null,
                                ""isSum"": false,
                                ""contentComponent"": {
                                    ""Unit"": {
                                        ""fi"": ""variable-5-value-0-unit.fi"",
                                        ""sv"": ""variable-5-value-0-unit.sv"",
                                        ""en"": ""variable-5-value-0-unit.en""
                                    },
                                    ""Source"": {
                                        ""fi"": ""variable-5-value-0-source.fi"",
                                        ""sv"": ""variable-5-value-0-source.sv"",
                                        ""en"": ""variable-5-value-0-source.en""
                                    },
                                    ""NumberOfDecimals"": 0,
                                    ""LastUpdated"": ""2023-08-31T05:00:00Z""
                                }
                            }
                        ]
                    }
                ]
            },
            ""data"": [173.0, null, 0.0, 0.0, 0.0, 0.0, 1.0],
            ""dataNotes"": {
                ""1"": ""...""
            }
        }
        ";
    }
}

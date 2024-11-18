using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using UnitTests.Fixtures;
using UnitTests.Utilities;

namespace UnitTests.SerializerTests
{
    internal class SavedQuerySerializerTests
    {
        private readonly static string testSavedQuery = @"{
            ""Query"": {
                ""TableReference"": {
                    ""Name"": ""table.px"",
                    ""Hierarchy"": [
                        ""StatFin"",
                        ""kivih""
                    ]
                },
                ""ChartHeaderEdit"": null,
                ""VariableQueries"": {
                    ""Vuosi"": {
                        ""NameEdit"": null,
                        ""ValueEdits"": {},
                        ""ValueFilter"": {
                            ""type"": ""all""
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    },
                    ""Tiedot"": {
                        ""NameEdit"": null,
                        ""ValueEdits"": {},
                        ""ValueFilter"": {
                            ""type"": ""item"",
                            ""query"": [
                                ""kulutus_t""
                            ]
                        },
                        ""VirtualValueDefinitions"": null,
                        ""Selectable"": false
                    }
                }
            },
            ""CreationTime"": ""2022-12-16T14:19:11.4380637+02:00"",
            ""Archived"": false,
            ""Settings"": {
                ""CutYAxis"": false,
                ""MultiselectableVariableCode"": null,
                ""SelectedVisualization"": ""LineChart"",
                ""DefaultSelectableVariableCodes"": null
            }
        }";

        private readonly static JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        [Test]
        public void SavedQueryDeserializationTest_Success()
        {
            SavedQuery query = JsonSerializer.Deserialize<SavedQuery>(testSavedQuery, options);
            Assert.That(query.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
        }

        [Test]
        public void SavedQuerySerializationTest_Success()
        {
            SavedQuery query = JsonSerializer.Deserialize<SavedQuery>(testSavedQuery, options);
            string serializedString = JsonSerializer.Serialize(query, options);
            Assert.That(serializedString.Contains("1.1"));
            Assert.That(serializedString.Contains(ChartTypeEnumConverter.ToJsonString(query.Settings.VisualizationType)));
        }

        [Test]
        public void DeserializeSavedQuery__V1_0__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.0"",
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":true,
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ],
                        ""DefaultSelectableVariableCodes"": null
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(3));
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_0DefaultSelectableVariableCodes__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.0"",
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":true,
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ],
                        ""DefaultSelectableVariableCodes"":{""foo"":[""bar""]}
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(3));
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Not.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_1NoPivot__ReturnsV1_1DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.1"",
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
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
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes[0], Is.EqualTo("Kuukausi"));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_1WithUncommonDateTimeFormat__ReturnsV1_1DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.1"",
		            ""Query"":{},
		            ""CreationTime"": ""2024-09-13T16:13:42.074783+03:00"",
		            ""Archived"":true,
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
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2024-09-13T16:13:42.074783+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes[0], Is.EqualTo("Kuukausi"));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_1DefaultSelectableVariableCodes__ReturnsV1_1DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.1"",
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
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
                      ""DefaultSelectableVariableCodes"":{""foo"":[""bar""]}
                   },
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes[0], Is.EqualTo("Kuukausi"));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Not.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_1WithCamelCasePropertyNames__ReturnsV1_1DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""version"":""1.1"",
		            ""query"":{},
		            ""creationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""archived"":true,
                    ""settings"":{
                      ""matchXLabelsToEnd"":false,
                      ""xLabelInterval"":1,
                      ""layout"":{
                         ""rowVariableCodes"":[
                            ""Kuukausi""
                         ],
                         ""columnVariableCodes"":[
                            ""Ilmoittava lentoasema""
                         ]
                      },
                      ""visualizationType"":""GroupVerticalBarChart"",
                      ""defaultSelectableVariableCodes"":{""foo"":[""bar""]}
                   },
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes[0], Is.EqualTo("Kuukausi"));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Settings.DefaultSelectableVariableCodes, Is.Not.Null);
        }

        [Test]
        public void DeserializeSavedQuery__V1_1Pivot__ReturnsV1_1DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.1"",
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                      ""MatchXLabelsToEnd"":false,
                      ""XLabelInterval"":1,
                      ""Layout"":{
                         ""RowVariableCodes"":[
                            ""Ilmoittava lentoasema""                            
                         ],
                         ""ColumnVariableCodes"":[
                            ""Kuukausi""                            
                         ]
                      },
                      ""VisualizationType"":""GroupVerticalBarChart"",
                      ""DefaultSelectableVariableCodes"":null
                   },
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes[0], Is.EqualTo("Kuukausi"));
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
        }

        [Test]
        public void DeserializeSavedQuery__NoVersionDefined__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":true,
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ]
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(3));
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
        }

        [Test]
        public void DeserializeSavedQuery__UnknownVersion__ThrowsNotSupportedException()
        {
            string testJson = @"{
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Version"":""123.123"",
                    ""Settings"":{""SelectedVisualization"":""Table"" }
                }";

            Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<SavedQuery>(testJson, options));
        }

        [Test]
        public void DeserializeSavedQuery__WithLegacyProperties__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
		            ""Query"":{},
		            ""CreationTime"": ""2023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":true,
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ]
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson, options);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(PxSyntaxConstants.ParseDateTime("2023-04-24T14:36:18.7550813+03:00")));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That((bool)savedQuery.LegacyProperties["PivotRequested"], Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(0));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(3));
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
        }

        #region Fixture tests

        [Test]
        public void DeserializeAndSerializeV10SQTest()
        {
            // Serialize
            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(SavedQueryFixtures.V1_0_TEST_SAVEDQUERY1, options);
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
            Assert.That(savedQuery.Query.DimensionQueries.Count, Is.EqualTo(6));
            string pivotKey = "PivotRequested";
            Assert.That(savedQuery.LegacyProperties.ContainsKey(pivotKey) && (bool)(savedQuery.LegacyProperties[pivotKey]));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));

            // Deserialize
            string serializedString = JsonSerializer.Serialize(savedQuery, options);
            JsonUtils.JsonStringsAreEqual(
                SavedQuerySerializerExpectedOutputFixtures.V1_0_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT,
                serializedString);
        }

        [Test]
        public void DeserializeAndSerializeV11SQTest()
        {
            // Serialize
            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY1, options);
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Query.DimensionQueries.Count, Is.EqualTo(6));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));

            // Deserialize
            string serializedString = JsonSerializer.Serialize(savedQuery, options);
            JsonUtils.JsonStringsAreEqual(
                SavedQuerySerializerExpectedOutputFixtures.V1_1_TEST_SAVEDQUERY1_SER_DESER_EXPECTED_OUTPUT,
                serializedString);
        }

        [Test]
        public void DesieralizeAndSerializeV11SQWithVaryingValueFilters()
        {
            //Serialize
            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY4, options);
            Assert.That(savedQuery.Version, Is.EqualTo("1.1"));
            Assert.That(savedQuery.Query.DimensionQueries.Count, Is.EqualTo(4));
            Assert.That(savedQuery.Query.DimensionQueries["variable-0"].ValueFilter.GetType(), Is.EqualTo(typeof(AllFilter)));
            Assert.That(savedQuery.Query.DimensionQueries["variable-1"].ValueFilter.GetType(), Is.EqualTo(typeof(ItemFilter)));
            Assert.That(savedQuery.Query.DimensionQueries["variable-2"].ValueFilter.GetType(), Is.EqualTo(typeof(FromFilter)));
            Assert.That(savedQuery.Query.DimensionQueries["variable-3"].ValueFilter.GetType(), Is.EqualTo(typeof(TopFilter)));

            // Deserialize
            string serializedString = JsonSerializer.Serialize(savedQuery, options);
            JsonUtils.JsonStringsAreEqual(
                SavedQuerySerializerExpectedOutputFixtures.V1_1_TEST_SAVEDQUERY4_SER_DESER_EXPECTED_OUTPUT,
                serializedString);
        }

        [Test]
        public void DeserializeAndSerializeV10TableSQTest()
        {
            // Serialize
            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(SavedQueryFixtures.V10_TEST_TABLE_SAVEDQUERY, options);
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
            Assert.That(savedQuery.Query.DimensionQueries.Count, Is.EqualTo(6));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(4));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(2));

            // Deserialize
            string serializedString = JsonSerializer.Serialize(savedQuery, options);
            JsonUtils.JsonStringsAreEqual(
                SavedQuerySerializerExpectedOutputFixtures.V1_0_TABLETEST1_SER_DESER_EXPECTED_OUTPUT,
                serializedString);
        }

        #endregion
    }
}

using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializerTests
{
    internal class SavedQuerySerializerTests
    {
        private readonly static string testSavedQuery = "{\"Query\":{\"TableReference\":{\"Name\":\"table.px\",\"Hierarchy\":[\"StatFin\",\"kivih\"]},\"ChartHeaderEdit\":null,\"VariableQueries\":{\"Vuosi\":{\"NameEdit\":null,\"ValueEdits\":{},\"ValueFilter\":{\"type\":\"all\"},\"VirtualValueDefinitions\":null,\"Selectable\":false},\"Tiedot\":{\"NameEdit\":null,\"ValueEdits\":{},\"ValueFilter\":{\"type\":\"item\",\"query\":[\"kulutus_t\"]},\"VirtualValueDefinitions\":null,\"Selectable\":false}}},\"CreationTime\":\"2022-12-16T14:19:11.4380637+02:00\",\"Archived\":false,\"Settings\":{\"CutYAxis\":false,\"MultiselectableVariableCode\":null,\"SelectedVisualization\":\"LineChart\",\"DefaultSelectableVariableCodes\":null}}";
        private readonly JsonSerializerOptions options = new()
        {
            Converters = { new SavedQuerySerializer() }
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
            Assert.That(serializedString.Contains(query.Version));
            Assert.That(serializedString.Contains(ChartTypeEnumConverter.ToJsonString(query.Settings.VisualizationType)));
        }

        [Test]
        public void DeserializeSavedQuery__V1_0__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
                    ""Version"":""1.0"",
		            ""Query"":{},
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":""True"",
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ],
                        ""DefaultSelectableVariableCodes"": null
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":""True"",
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ],
                        ""DefaultSelectableVariableCodes"":{""foo"":[""bar""]}
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
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

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
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

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
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

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":""True"",
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ]
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
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
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Version"":""123.123"",
                    ""Settings"":{""SelectedVisualization"":""Table"" }
                }";

            Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<SavedQuery>(testJson));
        }

        [Test]
        public void DeserializeSavedQuery__WithLegacyProperties__ReturnsV1_0DeserializedSavedQuery()
        {
            string testJson = @"{
		            ""Query"":{},
		            ""CreationTime"": ""023-04-24T14:36:18.7550813+03:00"",
		            ""Archived"":true,
                    ""Settings"":{
                        ""SelectedVisualization"":""Table"", 
                        ""PivotRequested"":""True"",
                        ""RowVariableCodes"":[],
                        ""ColumnVariableCodes"":[
                            ""Vuosi"",
                            ""Ikä"",
                            ""Sukupuoli""
                        ]
                    }
                }";

            SavedQuery savedQuery = JsonSerializer.Deserialize<SavedQuery>(testJson);

            Assert.That(savedQuery.CreationTime, Is.EqualTo(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture)));
            Assert.That(savedQuery.Settings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(savedQuery.Archived, Is.True);
            Assert.That((bool)savedQuery.LegacyProperties["PivotRequested"], Is.True);
            Assert.That(savedQuery.Settings.Layout.RowVariableCodes.Count, Is.EqualTo(0));
            Assert.That(savedQuery.Settings.Layout.ColumnVariableCodes.Count, Is.EqualTo(3));
            Assert.That(savedQuery.Version, Is.EqualTo("1.0"));
        }
    }
}

using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System;
using System.Globalization;

namespace SerializerTests
{
    internal class SavedQuerySerializerTests
    {
        private readonly static string testSavedQuery = "{\"Query\":{\"TableReference\":{\"Name\":\"table.px\",\"Hierarchy\":[\"StatFin\",\"kivih\"]},\"ChartHeaderEdit\":null,\"VariableQueries\":{\"Vuosi\":{\"NameEdit\":null,\"ValueEdits\":{},\"ValueFilter\":{\"type\":\"all\"},\"VirtualValueDefinitions\":null,\"Selectable\":false},\"Tiedot\":{\"NameEdit\":null,\"ValueEdits\":{},\"ValueFilter\":{\"type\":\"item\",\"query\":[\"kulutus_t\"]},\"VirtualValueDefinitions\":null,\"Selectable\":false}}},\"CreationTime\":\"2022-12-16T14:19:11.4380637+02:00\",\"Archived\":false,\"Settings\":{\"CutYAxis\":false,\"MultiselectableVariableCode\":null,\"SelectedVisualization\":\"LineChart\",\"DefaultSelectableVariableCodes\":null}}";

        [Test]
        public void SavedQueryDeserializationTest_Success()
        {
            SavedQuery query = JsonConvert.DeserializeObject<SavedQuery>(testSavedQuery);
            Assert.AreEqual(VisualizationType.LineChart, query.Settings.VisualizationType);
        }

        [Test]
        public void SavedQuerySerializationTest_Success()
        {
            SavedQuery query = JsonConvert.DeserializeObject<SavedQuery>(testSavedQuery);
            var serializedString = JsonConvert.SerializeObject(query);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.Table, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(3, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("1.0", savedQuery.Version);
            Assert.IsNull(savedQuery.Settings.DefaultSelectableVariableCodes);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.Table, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(3, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("1.0", savedQuery.Version);
            Assert.IsNotNull(savedQuery.Settings.DefaultSelectableVariableCodes);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.GroupVerticalBarChart, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(1, savedQuery.Settings.Layout.RowVariableCodes.Count);
            Assert.AreEqual("Kuukausi", savedQuery.Settings.Layout.RowVariableCodes[0]);
            Assert.AreEqual(1, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("Ilmoittava lentoasema", savedQuery.Settings.Layout.ColumnVariableCodes[0]);
            Assert.AreEqual("1.1", savedQuery.Version);
            Assert.IsNull(savedQuery.Settings.DefaultSelectableVariableCodes);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.GroupVerticalBarChart, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(1, savedQuery.Settings.Layout.RowVariableCodes.Count);
            Assert.AreEqual("Kuukausi", savedQuery.Settings.Layout.RowVariableCodes[0]);
            Assert.AreEqual(1, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("Ilmoittava lentoasema", savedQuery.Settings.Layout.ColumnVariableCodes[0]);
            Assert.AreEqual("1.1", savedQuery.Version);
            Assert.IsNotNull(savedQuery.Settings.DefaultSelectableVariableCodes);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.GroupVerticalBarChart, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(1, savedQuery.Settings.Layout.RowVariableCodes.Count);
            Assert.AreEqual("Ilmoittava lentoasema", savedQuery.Settings.Layout.RowVariableCodes[0]);
            Assert.AreEqual(1, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("Kuukausi", savedQuery.Settings.Layout.ColumnVariableCodes[0]);
            Assert.AreEqual("1.1", savedQuery.Version);
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.Table, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.AreEqual(3, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("1.0", savedQuery.Version);
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

            Assert.Throws<NotSupportedException>(() => JsonConvert.DeserializeObject<SavedQuery>(testJson));
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

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(testJson);

            Assert.AreEqual(DateTime.Parse("023-04-24T14:36:18.7550813+03:00", CultureInfo.InvariantCulture), savedQuery.CreationTime);
            Assert.AreEqual(VisualizationType.Table, savedQuery.Settings.VisualizationType);
            Assert.IsTrue(savedQuery.Archived);
            Assert.IsTrue((bool)savedQuery.LegacyProperties["PivotRequested"]);
            Assert.AreEqual(0, savedQuery.Settings.Layout.RowVariableCodes.Count);
            Assert.AreEqual(3, savedQuery.Settings.Layout.ColumnVariableCodes.Count);
            Assert.AreEqual("1.0", savedQuery.Version);
        }
    }
}

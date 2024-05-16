using Newtonsoft.Json;
using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Visualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitTests.Fixtures;
using UnitTests.Fixtures.ResponseFixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;
using UnitTests.Utilities;

namespace Visualization
{
    internal class PxVisualizerCubeAdapterTests
    {
        public PxVisualizerCubeAdapterTests()
        {
            Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));
        }

        [Test]
        public static void BuildVisualizationResponseTest_VariableOrderChangedMetadataChanged()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            Assert.AreEqual(new[] { "variable-0", "variable-2", "variable-1", "variable-3" }, result.MetaData.Select(v => v.Code));
        }

        [Test]
        public static void BuildVisualizationResponseTest_OneSelecableCausesVarOrderChange()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                0.123, 1.123, 2.123, 9.123, 10.123, 11.123, 18.123, 19.123, 20.123,
                3.123, 4.123, 5.123, 12.123, 13.123, 14.123, 21.123, 22.123, 23.123,
                6.123, 7.123, 8.123, 15.123, 16.123, 17.123, 24.123, 25.123, 26.123
            };

            Assert.AreEqual(expectedData, result.Data);
        }

        [Test]
        public static void BuildVisualizationResponseTest_PercentHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3)
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123,
                9.123, 10.123, 11.123, 12.123, 13.123, 14.123, 15.123, 16.123, 17.123,
                18.123, 19.123, 20.123, 21.123, 22.123, 23.123, 24.123, 25.123, 26.123
            };

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_PercentVerticalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 3)  { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 3)
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentVerticalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                0.123, 9.123, 18.123, 27.123, 36.123,
                1.123, 10.123, 19.123, 28.123, 37.123,
                2.123, 11.123, 20.123, 29.123, 38.123,
                3.123, 12.123, 21.123, 30.123, 39.123,
                4.123, 13.123, 22.123, 31.123, 40.123,
                5.123, 14.123, 23.123, 32.123, 41.123,
                6.123, 15.123, 24.123, 33.123, 42.123,
                7.123, 16.123, 25.123, 34.123, 43.123,
                8.123, 17.123, 26.123, 35.123, 44.123
            };

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_MultipleSelectableVarsAndPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Time, 4) { Selectable = true },
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                0.123, 32.123, 4.123, 36.123, 8.123, 40.123, 12.123, 44.123,
                1.123, 33.123, 5.123, 37.123, 9.123, 41.123, 13.123, 45.123,
                2.123, 34.123, 6.123, 38.123, 10.123, 42.123, 14.123, 46.123,
                3.123, 35.123, 7.123, 39.123, 11.123, 43.123, 15.123, 47.123,
                16.123, 48.123, 20.123, 52.123, 24.123, 56.123, 28.123, 60.123,
                17.123, 49.123, 21.123, 53.123, 25.123, 57.123, 29.123, 61.123,
                18.123, 50.123, 22.123, 54.123, 26.123, 58.123, 30.123, 62.123,
                19.123, 51.123, 23.123, 55.123, 27.123, 59.123, 31.123, 63.123
            };

            Assert.AreEqual(expectedData, result.Data);
        }

        [Test]
        public static void BuildVisualizationResponseTest_ChangeVarOrder()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 1)
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                0.123, 10.123, 2.123, 12.123, 4.123,
                14.123, 6.123, 16.123, 8.123, 18.123,
                1.123, 11.123, 3.123, 13.123, 5.123,
                15.123, 7.123 , 17.123, 9.123, 19.123
            };

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery1__V1_1()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Time, 3, name: "Kuukausi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Ilmoittava lentoasema"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Lennon tyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Saapuneet/lähteneet"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Toinen lentoasema"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY1);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.GroupVerticalBarChart, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(1, result.ColumnVariableCodes.Count);
            Assert.AreEqual("Ilmoittava lentoasema", result.ColumnVariableCodes[0]);
            Assert.AreEqual(1, result.RowVariableCodes.Count);
            Assert.AreEqual("Kuukausi", result.RowVariableCodes[0]);
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery2__V1_1()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Time, 2, name: "Vuosi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Syntymävaltio"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Adoptiotyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Ikä"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY2);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.GroupHorizontalBarChart, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(1, result.ColumnVariableCodes.Count);
            Assert.AreEqual("Syntymävaltio", result.ColumnVariableCodes[0]);
            Assert.AreEqual(1, result.RowVariableCodes.Count);
            Assert.AreEqual("Vuosi", result.RowVariableCodes[0]);
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery3__V1_1()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1, name: "Joukkoviestimet"),
                new VariableParameters(VariableType.Time, 10, name: "Vuosi")
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY3);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.LineChart, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(1, result.ColumnVariableCodes.Count);
            Assert.AreEqual("Vuosi", result.ColumnVariableCodes[0]);
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedTableSavedQuery__V11()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 5, name: "Joukkoviestimet"),
                new VariableParameters(VariableType.Time, 5, name: "Vuosi")
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V11_TEST_TABLE_SAVEDQUERY);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.Table, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(1, result.RowVariableCodes.Count);
            Assert.AreEqual("Joukkoviestimet", result.RowVariableCodes[0]);
            Assert.AreEqual(1, result.ColumnVariableCodes.Count);
            Assert.AreEqual("Vuosi", result.ColumnVariableCodes[0]);
        }


        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedTableSavedQuery__V10()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Time, 2, name: "Kuukausi"),
                new VariableParameters(VariableType.OtherClassificatory, 5, name: "Ilmoittava lentoasema"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Lennon tyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 5, name: "Saapuneet/lähteneet"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Toinen lentoasema"),
                new VariableParameters(VariableType.Content, 3, name: "Tiedot")
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V10_TEST_TABLE_SAVEDQUERY);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.Table, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(new string[] { "Kuukausi" }, result.SelectableVariableCodes);
            Assert.AreEqual(new string[] { "Ilmoittava lentoasema", "Lennon tyyppi", "Saapuneet/lähteneet" }, result.RowVariableCodes);
            Assert.AreEqual(new string[] { "Toinen lentoasema", "Tiedot" }, result.ColumnVariableCodes);
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery1__V1_0()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Time, 3, name: "Vuosi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Syntymävaltio"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Adoptiotyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Ikä"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Sukupuoli"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot")
            };

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_0_TEST_SAVEDQUERY1);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.AreEqual(VisualizationType.GroupVerticalBarChart, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(1, result.ColumnVariableCodes.Count);
            Assert.AreEqual("Syntymävaltio", result.ColumnVariableCodes[0]);
            Assert.AreEqual(1, result.RowVariableCodes.Count);
            Assert.AreEqual("Vuosi", result.RowVariableCodes[0]);
            Assert.IsTrue((bool)savedQuery.LegacyProperties["PivotRequested"]);
        }

        [Test]
        public void ResponseTestFromSavedQuery_HORIZONTALBAR_SORTED_ASCENDING()
        {
            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.HORIZONTALBAR_SORTED_ASCENDING);
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Time, 1, name: "Vuosineljännes"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            };

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));
            string expexted = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.ASCENDING_HORIZONTAL_BARCHART_RESPONSE_FIXTURE);

            JsonUtils.AreEqual(expexted, normalizedResponse);
        }

        [Test]
        public static void BuildVisualizationResponseTest_DataNotes_isEmpty()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 9),
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyDictionary<int, IReadOnlyMultiLanguageString> dataNotes = result.DataNotes;

            Assert.IsEmpty(dataNotes);
        }

        [Test]
        public void BuildVisualizationResponseTest_MissingDataInfo__Table()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 3),
                new VariableParameters(VariableType.Time, 5)
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.Table,
                RowVariableCodes = new List<string>() { "variable-0" },
                ColumnVariableCodes = new List<string>() { "variable-1" }
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, creationSettings, inputCube);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery);

            IReadOnlyList<double?> expectedData = new List<double?> {
                null, 1.123, 2.123, null, 4.123, 5.123, null, 7.123, 8.123,
                null, 10.123, 11.123, null, 13.123, 14.123
            };

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;

            Assert.AreEqual(VisualizationType.Table, result.VisualizationSettings.VisualizationType);
            Assert.AreEqual(expectedData, result.Data);

            Assert.AreEqual(1, MissingDataInfoFromResult[0]);
            Assert.AreEqual(2, MissingDataInfoFromResult[3]);
            Assert.AreEqual(3, MissingDataInfoFromResult[6]);
            Assert.AreEqual(4, MissingDataInfoFromResult[9]);
            Assert.AreEqual(5, MissingDataInfoFromResult[12]);
        }

        [Test]
        public static void BuildVisualizationResponseTest_MissingDataInfo__BarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 9),
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                null, 1.123, 2.123, null, 4.123, 5.123, null, 7.123, 8.123,
                null, 10.123, 11.123, null, 13.123, 14.123, null, 16.123, 17.123,
                null, 19.123, 20.123, null, 22.123, 23.123, null, 25.123, 26.123
            };


            Assert.AreEqual(expectedData, result.Data);

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;
            
            Assert.AreEqual(9, MissingDataInfoFromResult.Count);

            Assert.AreEqual(1, MissingDataInfoFromResult[0]);
            Assert.AreEqual(2, MissingDataInfoFromResult[3]);
            Assert.AreEqual(3, MissingDataInfoFromResult[6]);
            Assert.AreEqual(4, MissingDataInfoFromResult[9]);
            Assert.AreEqual(5, MissingDataInfoFromResult[12]);
            Assert.AreEqual(6, MissingDataInfoFromResult[15]);
            Assert.AreEqual(7, MissingDataInfoFromResult[18]);
            Assert.AreEqual(1, MissingDataInfoFromResult[21]);
            Assert.AreEqual(2, MissingDataInfoFromResult[24]);
        }

        [Test]
        public static void BuildVisualizationResponseTest_ChangeVarOrder_withMissingData()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 1)
            };

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = new List<double?> {
                null, 10.123, 2.123, null, 4.123,
                14.123, null, 16.123, 8.123, null,
                1.123, 11.123, null, 13.123, 5.123,
                null, 7.123 , 17.123, null, 19.123
            };

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;
            Assert.AreEqual(1, MissingDataInfoFromResult[0]);
            Assert.AreEqual(2, MissingDataInfoFromResult[12]);
            Assert.AreEqual(3, MissingDataInfoFromResult[6]);
            Assert.AreEqual(4, MissingDataInfoFromResult[18]);
            Assert.AreEqual(5, MissingDataInfoFromResult[3]);
            Assert.AreEqual(6, MissingDataInfoFromResult[15]);
            Assert.AreEqual(7, MissingDataInfoFromResult[9]);

        }

        [Test]
        public void ResponseSerializationTest_WithMissing_Returned()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.Time, 15, name: "Vuosineljännes"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            };

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams, missingData: true);
            LineChartVisualizationSettings settings = new(new Layout(new List<string>() { "Alue" }, new List<string>() { "Vuosineljännes" }), false, null);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, settings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));
            string normalizedExpected = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.LINE_CHART_RESPONSE_FIXTURE_WITH_MISSING_VALUES);

            Assert.AreEqual(normalizedExpected, normalizedResponse);
        }

        [Test]
        public void ResponseSerializationTest_Returnd()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.Time, 15, name: "Vuosineljännes"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            };

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            LineChartVisualizationSettings settings = new(new Layout(new List<string>() { "Alue" }, new List<string>() { "Vuosineljännes" }), false, null);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, settings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));

            string normalizedExpected = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.LINE_CHART_RESPONSE_FIXTURE);

            Assert.AreEqual(normalizedExpected, normalizedResponse);
        }
    }
}

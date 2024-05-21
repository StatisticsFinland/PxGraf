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
using System.Collections.Generic;
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
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public static void BuildVisualizationResponseTest_VariableOrderChangedMetadataChanged()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);
            
            List<string> expected = ["variable-0", "variable-2", "variable-1", "variable-3"];
            Assert.That(result.MetaData.Select(v => v.Code), Is.EqualTo(expected));
        }

        [Test]
        public static void BuildVisualizationResponseTest_OneSelecableCausesVarOrderChange()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.LineChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                0.123, 1.123, 2.123, 9.123, 10.123, 11.123, 18.123, 19.123, 20.123,
                3.123, 4.123, 5.123, 12.123, 13.123, 14.123, 21.123, 22.123, 23.123,
                6.123, 7.123, 8.123, 15.123, 16.123, 17.123, 24.123, 25.123, 26.123
            ];
            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_PercentHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3)
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123,
                9.123, 10.123, 11.123, 12.123, 13.123, 14.123, 15.123, 16.123, 17.123,
                18.123, 19.123, 20.123, 21.123, 22.123, 23.123, 24.123, 25.123, 26.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_PercentVerticalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 3)  { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 3)
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentVerticalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                0.123, 9.123, 18.123, 27.123, 36.123,
                1.123, 10.123, 19.123, 28.123, 37.123,
                2.123, 11.123, 20.123, 29.123, 38.123,
                3.123, 12.123, 21.123, 30.123, 39.123,
                4.123, 13.123, 22.123, 31.123, 40.123,
                5.123, 14.123, 23.123, 32.123, 41.123,
                6.123, 15.123, 24.123, 33.123, 42.123,
                7.123, 16.123, 25.123, 34.123, 43.123,
                8.123, 17.123, 26.123, 35.123, 44.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_MultipleSelectableVarsAndPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Time, 4) { Selectable = true },
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                0.123, 32.123, 4.123, 36.123, 8.123, 40.123, 12.123, 44.123,
                1.123, 33.123, 5.123, 37.123, 9.123, 41.123, 13.123, 45.123,
                2.123, 34.123, 6.123, 38.123, 10.123, 42.123, 14.123, 46.123,
                3.123, 35.123, 7.123, 39.123, 11.123, 43.123, 15.123, 47.123,
                16.123, 48.123, 20.123, 52.123, 24.123, 56.123, 28.123, 60.123,
                17.123, 49.123, 21.123, 53.123, 25.123, 57.123, 29.123, 61.123,
                18.123, 50.123, 22.123, 54.123, 26.123, 58.123, 30.123, 62.123,
                19.123, 51.123, 23.123, 55.123, 27.123, 59.123, 31.123, 63.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public static void BuildVisualizationResponseTest_ChangeVarOrder()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 1)
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                0.123, 10.123, 2.123, 12.123, 4.123,
                14.123, 6.123, 16.123, 8.123, 18.123,
                1.123, 11.123, 3.123, 13.123, 5.123,
                15.123, 7.123 , 17.123, 9.123, 19.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery1__V1_1()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Time, 3, name: "Kuukausi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Ilmoittava lentoasema"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Lennon tyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Saapuneet/lähteneet"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Toinen lentoasema"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY1);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.GroupVerticalBarChart));
            Assert.That(result.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.ColumnVariableCodes[0], Is.EqualTo("Ilmoittava lentoasema"));
            Assert.That(result.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.RowVariableCodes[0], Is.EqualTo("Kuukausi"));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery2__V1_1()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Time, 2, name: "Vuosi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Syntymävaltio"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Adoptiotyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Ikä"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY2);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.GroupHorizontalBarChart));
            Assert.That(result.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.ColumnVariableCodes[0], Is.EqualTo("Syntymävaltio"));
            Assert.That(result.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.RowVariableCodes[0], Is.EqualTo("Vuosi"));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery3__V1_1()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1, name: "Joukkoviestimet"),
                new VariableParameters(VariableType.Time, 10, name: "Vuosi")
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_1_TEST_SAVEDQUERY3);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.ColumnVariableCodes[0], Is.EqualTo("Vuosi"));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedTableSavedQuery__V11()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 5, name: "Joukkoviestimet"),
                new VariableParameters(VariableType.Time, 5, name: "Vuosi")
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V11_TEST_TABLE_SAVEDQUERY);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(result.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.RowVariableCodes[0], Is.EqualTo("Joukkoviestimet"));
            Assert.That(result.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.ColumnVariableCodes[0], Is.EqualTo("Vuosi"));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedTableSavedQuery__V10()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Time, 2, name: "Kuukausi"),
                new VariableParameters(VariableType.OtherClassificatory, 5, name: "Ilmoittava lentoasema"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Lennon tyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 5, name: "Saapuneet/lähteneet"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Toinen lentoasema"),
                new VariableParameters(VariableType.Content, 3, name: "Tiedot")
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V10_TEST_TABLE_SAVEDQUERY);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            string[] expectedSelVarCodes = ["Kuukausi"];
            string[] expectedRowVarCodes = ["Ilmoittava lentoasema", "Lennon tyyppi", "Saapuneet/lähteneet"];
            string[] expectedColVarCodes = ["Toinen lentoasema", "Tiedot"];

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(result.SelectableVariableCodes, Is.EqualTo(expectedSelVarCodes));
            Assert.That(result.RowVariableCodes, Is.EqualTo(expectedRowVarCodes));
            Assert.That(result.ColumnVariableCodes, Is.EqualTo(expectedColVarCodes));
        }

        [Test]
        public void BuildVisualizationResponseTest_FromDeserializedSavedQuery1__V1_0()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Time, 3, name: "Vuosi"),
                new VariableParameters(VariableType.OtherClassificatory, 2, name: "Syntymävaltio"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Adoptiotyyppi"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Ikä"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Sukupuoli"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot")
            ];

            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.V1_0_TEST_SAVEDQUERY1);
            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, savedQuery);

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.GroupHorizontalBarChart));
            Assert.That(result.ColumnVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.ColumnVariableCodes[0], Is.EqualTo("Syntymävaltio"));
            Assert.That(result.RowVariableCodes.Count, Is.EqualTo(1));
            Assert.That(result.RowVariableCodes[0], Is.EqualTo("Vuosi"));
            Assert.That((bool)savedQuery.LegacyProperties["PivotRequested"], Is.True);
        }

        [Test]
        public void ResponseTestFromSavedQuery_HORIZONTALBAR_SORTED_ASCENDING()
        {
            SavedQuery savedQuery = JsonConvert.DeserializeObject<SavedQuery>(SavedQueryFixtures.HORIZONTALBAR_SORTED_ASCENDING);
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Time, 1, name: "Vuosineljännes"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));
            string expexted = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.ASCENDING_HORIZONTAL_BARCHART_RESPONSE_FIXTURE);

            JsonUtils.AreEqual(expexted, normalizedResponse);
        }

        [Test]
        public static void BuildVisualizationResponseTest_DataNotes_isEmpty()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 9),
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyDictionary<int, IReadOnlyMultiLanguageString> dataNotes = result.DataNotes;

            Assert.That(dataNotes, Is.Empty);
        }

        [Test]
        public void BuildVisualizationResponseTest_MissingDataInfo__Table()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 3),
                new VariableParameters(VariableType.Time, 5)
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.Table,
                RowVariableCodes = ["variable-0"],
                ColumnVariableCodes = ["variable-1"]
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(cubeParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, creationSettings, inputCube);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery);

            IReadOnlyList<double?> expectedData = [
                null, 1.123, 2.123, null, 4.123, 5.123, null, 7.123, 8.123,
                null, 10.123, 11.123, null, 13.123, 14.123
            ];

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;

            Assert.That(result.VisualizationSettings.VisualizationType, Is.EqualTo(VisualizationType.Table));
            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));

            Assert.That(MissingDataInfoFromResult[0], Is.EqualTo(1));
            Assert.That(MissingDataInfoFromResult[3], Is.EqualTo(2));
            Assert.That(MissingDataInfoFromResult[6], Is.EqualTo(3));
            Assert.That(MissingDataInfoFromResult[9], Is.EqualTo(4));
            Assert.That(MissingDataInfoFromResult[12], Is.EqualTo(5));
        }

        [Test]
        public static void BuildVisualizationResponseTest_MissingDataInfo__BarChart()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 9),
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                null, 1.123, 2.123, null, 4.123, 5.123, null, 7.123, 8.123,
                null, 10.123, 11.123, null, 13.123, 14.123, null, 16.123, 17.123,
                null, 19.123, 20.123, null, 22.123, 23.123, null, 25.123, 26.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;

            Assert.That(MissingDataInfoFromResult.Count, Is.EqualTo(9));

            Assert.That(MissingDataInfoFromResult[0], Is.EqualTo(1));
            Assert.That(MissingDataInfoFromResult[3], Is.EqualTo(2));
            Assert.That(MissingDataInfoFromResult[6], Is.EqualTo(3));
            Assert.That(MissingDataInfoFromResult[9], Is.EqualTo(4));
            Assert.That(MissingDataInfoFromResult[12], Is.EqualTo(5));
            Assert.That(MissingDataInfoFromResult[15], Is.EqualTo(6));
            Assert.That(MissingDataInfoFromResult[18], Is.EqualTo(7));
            Assert.That(MissingDataInfoFromResult[21], Is.EqualTo(8));
            Assert.That(MissingDataInfoFromResult[24], Is.EqualTo(9));
        }

        [Test]
        public static void BuildVisualizationResponseTest_ChangeVarOrder_withMissingData()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true },
                new VariableParameters(VariableType.Time, 1)
            ];

            VisualizationCreationSettings creationSettings = new()
            {
                SelectedVisualization = VisualizationType.PercentHorizontalBarChart,
                PivotRequested = true
            };

            DataCube inputCube = TestDataCubeBuilder.BuildTestDataCube(varParams, missingData: true);
            SavedQuery inputQuery = TestDataCubeBuilder.BuildTestSavedQuery(varParams, false, creationSettings, inputCube);
            VisualizationSettings settings = creationSettings.ToVisualizationSettings(inputCube.Meta, inputQuery.Query);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(inputCube, inputQuery.Query, settings);

            IReadOnlyList<double?> expectedData = [
                null, 10.123, 2.123, null, 4.123,
                14.123, null, 16.123, 8.123, null,
                1.123, 11.123, null, 13.123, 5.123,
                null, 7.123 , 17.123, null, 19.123
            ];

            Assert.That(result.Data, Is.EqualTo(expectedData).Within(0.001));

            IReadOnlyDictionary<int, int> MissingDataInfoFromResult = result.MissingDataInfo;

            Assert.That(MissingDataInfoFromResult[0], Is.EqualTo(1));
            Assert.That(MissingDataInfoFromResult[12], Is.EqualTo(2));
            Assert.That(MissingDataInfoFromResult[6], Is.EqualTo(3));
            Assert.That(MissingDataInfoFromResult[18], Is.EqualTo(4));
            Assert.That(MissingDataInfoFromResult[3], Is.EqualTo(5));
            Assert.That(MissingDataInfoFromResult[15], Is.EqualTo(6));
            Assert.That(MissingDataInfoFromResult[9], Is.EqualTo(7));
        }

        [Test]
        public void ResponseSerializationTest_WithMissing_Returned()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.Time, 15, name: "Vuosineljännes"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams, missingData: true);
            LineChartVisualizationSettings settings = new(new Layout(["Alue"], ["Vuosineljännes"]), false, null);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, settings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));
            string normalizedExpected = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.LINE_CHART_RESPONSE_FIXTURE_WITH_MISSING_VALUES);

            Assert.That(normalizedResponse, Is.EqualTo(normalizedExpected));
        }

        [Test]
        public void ResponseSerializationTest_Returnd()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Geological, 3, name: "Alue"),
                new VariableParameters(VariableType.Ordinal, 1, name: "Huoneluku"),
                new VariableParameters(VariableType.Time, 15, name: "Vuosineljännes"),
                new VariableParameters(VariableType.OtherClassificatory, 1, name: "Rahoitusmuoto"),
                new VariableParameters(VariableType.Content, 1, name: "Tiedot"),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(cubeParams);
            LineChartVisualizationSettings settings = new(new Layout(["Alue"], ["Vuosineljännes"]), false, null);
            SavedQuery savedQuery = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, settings);
            VisualizationResponse result = PxVisualizerCubeAdapter.BuildVisualizationResponse(cube, savedQuery);

            string normalizedResponse = JsonUtils.NormalizeJsonString(JsonConvert.SerializeObject(result));
            string normalizedExpected = JsonUtils.NormalizeJsonString(VisualizationResponseFixtures.LINE_CHART_RESPONSE_FIXTURE);

            Assert.That(normalizedResponse, Is.EqualTo(normalizedExpected));
        }
    }
}

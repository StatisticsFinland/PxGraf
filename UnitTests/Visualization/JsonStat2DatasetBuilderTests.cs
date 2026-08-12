using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Language;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using PxGraf.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnitTests.Fixtures;
using Px.Utils.Models.Metadata.Dimensions;

namespace UnitTests.Visualization
{
    internal class JsonStat2DatasetBuilderTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public void Build_MapsMissingValuesStatusRolesAndUnits()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 3),
                new DimensionParameters(DimensionType.Geographical, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: true);
            VisualizationSettings visualizationSettings = new LineChartVisualizationSettings(
                new Layout([], ["variable-0"]), false, null);

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi", visualizationSettings);

            Assert.That(result.Version, Is.EqualTo("2.0"));
            Assert.That(result.Class, Is.EqualTo("dataset"));
            Assert.That(result.Id, Is.EqualTo(new[] { "variable-0", "variable-1", "variable-2" }));
            Assert.That(result.Size, Is.EqualTo(new[] { 3, 2, 1 }));
            Assert.That(result.Value.Count, Is.EqualTo(6));
            Assert.That(result.Value[0], Is.Null);
            Assert.That(result.Status["0"], Is.EqualTo("1"));
            Assert.That(result.Status["3"], Is.EqualTo("2"));
            Assert.That(result.Role.Time, Is.EqualTo(new[] { "variable-0" }));
            Assert.That(result.Role.Geo, Is.EqualTo(new[] { "variable-1" }));
            Assert.That(result.Role.Metric, Is.EqualTo(new[] { "variable-2" }));
            Assert.That(result.Dimensions["variable-2"].Category.Unit["value-0"].Decimals, Is.EqualTo(0));
            Assert.That(result.Dimensions["variable-0"].Category.Index["2000"], Is.EqualTo(0));
            Assert.That(result.Dimensions["variable-0"].Category.Index["2001"], Is.EqualTo(1));
            Assert.That(result.Dimensions["variable-0"].Category.Index["2002"], Is.EqualTo(2));
            Assert.That(result.Extension.MissingValueDescriptions["3"], Is.Not.Empty);
            Assert.That(result.Extension.VisualizationConfig.ChartType, Is.EqualTo("line"));
        }

        [Test]
        public void Build_OmitsStatus_WhenNoMissingValues()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false);

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi");

            Assert.That(result.Status, Is.Null);
            Assert.That(result.Value, Is.EqualTo(new decimal?[] { 0.123m, 1.123m }));
        }

        [Test]
        public void Build_MapsEverySupportedMissingValueType()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 21),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: true);

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi");

            Assert.That(result.Status.Values.Distinct(), Is.EquivalentTo(["1", "2", "3", "4", "5", "6", "7"]));
        }

        [Test]
        public void Build_ThrowsWhenMatrixHasNoDimensions()
        {
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix([], missingData: false);

            Assert.That(() => JsonStat2DatasetBuilder.Build(matrix, "fi"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Build_ThrowsWhenMatrixDoesNotContainAMetricDimension()
        {
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(
                [new DimensionParameters(DimensionType.Time, 2)],
                missingData: false);

            Assert.That(() => JsonStat2DatasetBuilder.Build(matrix, "fi"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Build_ThrowsForExplicitLanguageUnavailableFromTable()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false, languages: ["en", "fi"]);

            Assert.That(() => JsonStat2DatasetBuilder.Build(matrix, "de"), Throws.ArgumentException);
        }

        [Test]
        public void Build_UsesTableDefaultLanguage_WhenLanguageIsNotProvided()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false, languages: ["en", "fi"]);

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, null);

            Assert.That(result.Label, Is.EqualTo("Test dataset description"));
        }

        [Test]
        public void Build_UsesDefaultLocalizationForMissingDescriptions_WhenTableLanguageIsNotLocalized()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: true, languages: ["de"]);

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "de");

            Assert.That(result.Extension.MissingValueDescriptions["1"], Is.EqualTo(TranslationFixture.Translations["fi"].MissingData.Missing));
        }

        [Test]
        public void Build_SerializesTypedVisualizationSettings()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false);
            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi", new LineChartVisualizationSettings(
                new Layout([], ["variable-0"]), false, null));

            using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(result, GlobalJsonConverterOptions.Default));
            JsonElement settings = document.RootElement.GetProperty("extension").GetProperty("visualizationConfig");
            Assert.That(settings.GetProperty("chartType").GetString(), Is.EqualTo("line"));
        }

        [Test]
        public void Build_AppliesDimensionValueUnitAndSourceEdits()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions).ApplyNameEdits(
                (MatrixMetadata)matrix.Metadata,
                dimensionNameEdits: new Dictionary<string, MultilanguageString>
                {
                    ["variable-0"] = new MultilanguageString("fi", "Muokattu aika")
                },
                valueNameEdits: new Dictionary<string, Dictionary<string, MultilanguageString>>
                {
                    ["variable-1"] = new Dictionary<string, MultilanguageString>
                    {
                        ["value-0"] = new MultilanguageString("fi", "Muokattu tieto")
                    }
                });
            query.DimensionQueries["variable-1"].ValueEdits["value-0"].ContentComponent = new ContentComponentEdition
            {
                UnitEdit = new MultilanguageString("fi", "muokattu yksikkö"),
                SourceEdit = new MultilanguageString("fi", "muokattu lähde")
            };

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi", null, query);

            Assert.That(result.Dimensions["variable-0"].Label, Is.EqualTo("Muokattu aika"));
            Assert.That(result.Dimensions["variable-1"].Category.Label["value-0"], Is.EqualTo("Muokattu tieto"));
            Assert.That(result.Dimensions["variable-1"].Category.Unit["value-0"].Label, Is.EqualTo("muokattu yksikkö"));
            Assert.That(result.Extension.JsonStatChart.Sources.Category["variable-1"]["value-0"], Is.EqualTo("muokattu lähde"));
        }

        [Test]
        public void Build_EmitsDimensionSourceWhenAllContentValuesShareIt()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 2)
            ];
            Matrix<DecimalDataValue> matrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            foreach (string valueCode in matrix.Metadata.Dimensions.Single(dimension => dimension.Type == DimensionType.Content).ValueCodes)
            {
                query.DimensionQueries["variable-1"].ValueEdits[valueCode] = new DimensionQuery.DimensionValueEdition
                {
                    ContentComponent = new ContentComponentEdition
                    {
                        SourceEdit = new MultilanguageString("fi", "yhteinen lähde")
                    }
                };
            }

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi", null, query);

            Assert.That(result.Extension.JsonStatChart.Sources.Dimension["variable-1"], Is.EqualTo("yhteinen lähde"));
            Assert.That(result.Extension.JsonStatChart.Sources.Category["variable-1"].Count, Is.EqualTo(2));
        }
    }
}

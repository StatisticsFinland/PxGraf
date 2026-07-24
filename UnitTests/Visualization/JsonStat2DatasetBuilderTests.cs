using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Language;
using PxGraf.Enums;
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
            VisualizationResponse.PxVisualizerSettings visualizationSettings = new()
            {
                VisualizationType = VisualizationType.LineChart
            };

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
            Assert.That(result.Extension.VisualizationSettings, Is.Not.Null);
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
        public void Build_UsesEmptyOptionalMetadata_WhenNotesAndSourcesAreMissing()
        {
            List<DimensionParameters> dimensions =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];
            Matrix<DecimalDataValue> sourceMatrix = TestDataCubeBuilder.BuildTestMatrix(dimensions, missingData: false);
            Matrix<DecimalDataValue> matrix = new(
                new MatrixMetadata("fi", ["fi", "en"], [.. sourceMatrix.Metadata.Dimensions.Cast<Dimension>()], []),
                sourceMatrix.Data); // Create a new matrix with the same data but without notes and sources

            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi");

            Assert.That(result.Note, Is.Null);
            Assert.That(result.Dimensions.Values.All(dimension => dimension.Note is null && dimension.Category.Note is null), Is.True);
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
        public void Build_ThrowsWhenMatrixValueCountDoesNotMatchDimensionSizes()
        {
            Matrix<DecimalDataValue> validMatrix = TestDataCubeBuilder.BuildTestMatrix(
                [
                    new DimensionParameters(DimensionType.Time, 2),
                    new DimensionParameters(DimensionType.Content, 1)
                ],
                missingData: false);
            Matrix<DecimalDataValue> matrix = new(validMatrix.Metadata, []);

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
            JsonStat2 result = JsonStat2DatasetBuilder.Build(matrix, "fi", new VisualizationResponse.PxVisualizerSettings
            {
                VisualizationType = VisualizationType.LineChart
            });

            using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(result, GlobalJsonConverterOptions.Default));
            JsonElement settings = document.RootElement.GetProperty("extension").GetProperty("visualizationSettings");
            Assert.That(settings.TryGetProperty("visualizationType", out _), Is.True);
            Assert.That(settings.TryGetProperty("selectedVisualization", out _), Is.False);
        }
    }
}

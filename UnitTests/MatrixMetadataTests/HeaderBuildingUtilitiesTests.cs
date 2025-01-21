using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using UnitTests.Fixtures;

namespace UnitTests.MatrixMetadataTests
{
    public class HeaderBuildingUtilitiesTests
    {
        public HeaderBuildingUtilitiesTests()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void GetHeader_WithEditedChartHeaderEdit_ReturnsEditedHeader()
        {
            // Arrange
            List<DimensionParameters> dimensions =
            [
                new (DimensionType.Content, 1, name: "ContentDim"),
                new (DimensionType.Time, 2, name: "TimeDim")
            ];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            query.ChartHeaderEdit = new (new Dictionary<string, string>
            {
                { "en", "Edited Header" },
                { "fi", "Muokattu Otsikko" }
            });
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimensions);

            // Act
            MultilanguageString header = HeaderBuildingUtilities.GetHeader(meta, query);

            // Assert
            Assert.That(header["en"], Is.EqualTo("Edited Header"));
            Assert.That(header["fi"], Is.EqualTo("Muokattu Otsikko"));
        }

        [Test]
        public void GetHeader_WithPartiallyEditedChartHeaderEdit_ReturnsPartiallyEditedHeader()
        {
            // Arrange
            List<DimensionParameters> dimensions =
            [
                new(DimensionType.Content, 1, name: "ContentDim"),
                new(DimensionType.Time, 2, name: "TimeDim")
            ];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            query.ChartHeaderEdit = new(new Dictionary<string, string>
            {
                { "en", "Edited Header" },
            });
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimensions);

            // Act
            MultilanguageString header = HeaderBuildingUtilities.GetHeader(meta, query);

            // Assert
            Assert.That(header["en"], Is.EqualTo("Edited Header"));
            Assert.That(header["fi"], Is.EqualTo("value-0 2000-2001"));
        }

        [Test]
        public void GetHeader_WithDefaultHeader_ReturnsDefaultHeader()
        {
            // Arrange
            List<DimensionParameters> dimensions =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 10),
                new (DimensionType.Nominal, 2),
                new (DimensionType.Ordinal, 1)
            ];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimensions);

            // Act
            MultilanguageString header = HeaderBuildingUtilities.GetHeader(meta, query);

            // Assert
            Assert.That(header["fi"].Equals("value-0, value-0 2000-2009 muuttujana variable-2"));
            Assert.That(header["en"].Equals("value-0.en, value-0.en in 2000.en to 2009.en by variable-2.en"));
        }

        [Test]
        public void GetHeader_WithEditedDimensionAndValueNames_ReturnsEditedNames()
        {
            // Arrange
            List<DimensionParameters> dimensions =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 10),
                new(DimensionType.Nominal, 2),
                new(DimensionType.Ordinal, 1)
            ];
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(dimensions);
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimensions);

            Dictionary<string, MultilanguageString> dimensionNameEdits = new ()
            {
                { "variable-2", new MultilanguageString(new Dictionary<string, string> { { "en", "variable-2-name-edited.en" }, { "fi", "variable-2-name-edited.fi" } }) }
            };
            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                { "variable-0", new Dictionary<string, MultilanguageString> { { "value-0", new MultilanguageString(new Dictionary<string, string> { { "en", "variable-0-value-0-name-edited.en" }, { "fi", "variable-1-value-0-name-edited.fi" } }) } } }
            };

            query.ApplyNameEdits(meta, dimensionNameEdits, valueNameEdits);

            // Act
            MultilanguageString header = HeaderBuildingUtilities.GetHeader(meta, query);

            // Assert
            Assert.That(header["fi"].Equals("variable-1-value-0-name-edited.fi, value-0 2000-2009 muuttujana variable-2-name-edited.fi"));
            Assert.That(header["en"].Equals("variable-0-value-0-name-edited.en, value-0.en in 2000.en to 2009.en by variable-2-name-edited.en"));
        }
    }
}

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Language;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.Text.Json;
using UnitTests.Fixtures;
using UnitTests.Utilities;
using PxGraf.Settings;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Language;
using System.Linq;

namespace UnitTests.SerializerTests
{
    public class ArchiveMatrixSerializerTests
    {
        private JsonSerializerOptions _jsonOptions;

        [SetUp]
        public void Setup()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> inMemorySettings = new()
            {
                {"pxwebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public void ArchiveMatrixSerializer_SerializeArchiveCubeV11_ReturnsCorrectJson()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 4),
                new DimensionParameters(DimensionType.Other, 2)
            ];
            
            ArchiveCube archiveCube = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            
            // Act
            string actual = JsonSerializer.Serialize(archiveCube, _jsonOptions);

            // Assert
            JsonUtils.JsonStringsAreEqual(ArchiveCubeFixtures.ARCHIVE_CUBE_V11, actual);
        }

        [Test]
        public void ArchiveMatrixSerializer_DeserializeV11_ReturnsCorrectObject()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 4),
                new DimensionParameters(DimensionType.Other, 2)
            ];
            ArchiveCube expected = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            expected.Version = "1.1";

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V11, _jsonOptions);

            // Assert
            JsonUtils.AreEqual(expected, actual);
        }

        [Test]
        public void ArchiveMatrixSerializer_DeserializeV10_ReturnsCorrectObject()
        {
            List<DecimalDataValue> expectedData =
            [
                new (173.0m, Px.Utils.Models.Data.DataValueType.Exists),
                new (0, Px.Utils.Models.Data.DataValueType.Missing),
                new (0.0m, Px.Utils.Models.Data.DataValueType.Exists),
                new (0.0m, Px.Utils.Models.Data.DataValueType.Exists),
                new (0.0m, Px.Utils.Models.Data.DataValueType.Exists),
                new (0.0m, Px.Utils.Models.Data.DataValueType.Exists),
                new (1.0m, Px.Utils.Models.Data.DataValueType.Exists)
            ];
            Dictionary<int, MultilanguageString> expectedDataNotes = new Dictionary<int, MultilanguageString>
            {
                { 1, new MultilanguageString(new Dictionary<string, string> { { "fi", "..." }, { "en", "..." }, { "sv", "..." } })}
            };
            string[] expectedLanguages = ["fi", "en", "sv"];

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V10, _jsonOptions);

            // Assert
            Assert.That((actual.Version).Equals("1.0"));
            Assert.That((actual.Data.Count).Equals(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.That((actual.Data[i]).Equals(expectedData[i]));
            }
            Assert.That(actual.DataNotes.Count.Equals(expectedDataNotes.Count));
            Assert.That(actual.DataNotes[1].Equals(expectedDataNotes[1]));
            Assert.That(actual.Meta.AdditionalProperties.Count.Equals(1));
            Assert.That(actual.Meta.AdditionalProperties.ContainsKey("SOURCE"));
            Assert.That(actual.Meta.DefaultLanguage.Equals("fi"));
            foreach (string language in expectedLanguages)
            {
                Assert.That(actual.Meta.AvailableLanguages.Contains(language));
            }
            Assert.That(actual.Meta.Dimensions.Count.Equals(6));
            Assert.That(actual.Meta.Dimensions[0].Values.Count.Equals(1));
            Assert.That(actual.Meta.Dimensions[1].Values.Count.Equals(7));
            Assert.That(actual.Meta.Dimensions[2].Values.Count.Equals(1));
            Assert.That(actual.Meta.Dimensions[3].Values.Count.Equals(1));
            Assert.That(actual.Meta.Dimensions[4].Values.Count.Equals(1));
            Assert.That(actual.Meta.Dimensions[5].Values.Count.Equals(1));
            Assert.That(actual.Meta.Dimensions[0].Type.Equals(DimensionType.Time));
            Assert.That(actual.Meta.Dimensions[5].Type.Equals(DimensionType.Content));
        }
    }
}

﻿using Microsoft.Extensions.Configuration;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Data;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Language;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System;
using UnitTests.Fixtures;
using UnitTests.Utilities;

namespace UnitTests.SerializerTests
{
    public class ArchiveMatrixSerializerTests
    {

        [SetUp]
        public void Setup()
        {
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
            DecimalDataValue missingValue = new(0, DataValueType.Missing);
            List<DecimalDataValue> expectedData = new(archiveCube.Data)
            {
                [1] = missingValue
            };
            archiveCube = new(
                archiveCube.CreationTime,
                archiveCube.Meta,
                expectedData,
                archiveCube.DataNotes,
                archiveCube.Version
                );

            // Act
            string actual = JsonSerializer.Serialize(archiveCube, GlobalJsonConverterOptions.Default);

            // Assert
            JsonUtils.JsonStringsAreEqual(ArchiveCubeFixtures.EXPECTED_ARCHIVE_CUBE_V11, actual);
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
            ArchiveCube expected = TestDataCubeBuilder.BuildTestArchiveCube(metaParams, null, "1.1");
            DecimalDataValue missingValue = new(0, DataValueType.Missing);
            List<DecimalDataValue> expectedData = new(expected.Data)
            {
                [1] = missingValue
            };
            expected = new(
                expected.CreationTime,
                expected.Meta,
                expectedData,
                expected.DataNotes,
                expected.Version
            );

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V11, GlobalJsonConverterOptions.Default);

            // Assert
            JsonUtils.AreEqual(expected, actual);
        }

        [Test]
        public void ArchiveMatrixSerializer_DeserializeV10_ReturnsCorrectObject()
        {
            List<DecimalDataValue> expectedData =
            [
                new (173.0m, DataValueType.Exists),
                new (0, DataValueType.Confidential),
                new (0.0m, DataValueType.Exists),
                new (0.0m, DataValueType.Exists),
                new (0.0m, DataValueType.Exists),
                new (0.0m, DataValueType.Exists),
                new (1.0m, DataValueType.Exists)
            ];
            Dictionary<int, MultilanguageString> expectedDataNotes = new()
            {
                { 1, new MultilanguageString(new Dictionary<string, string> { { "fi", "..." }, { "en", "..." }, { "sv", "..." } })}
            };
            string[] expectedLanguages = ["fi", "en", "sv"];

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V10, GlobalJsonConverterOptions.Default);

            // Assert
            Assert.That((actual.Version).Equals("1.0"));
            Assert.That((actual.Data.Count).Equals(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.That((actual.Data[i]).Equals(expectedData[i]));
            }
            Assert.That(actual.DataNotes.Count, Is.EqualTo(expectedDataNotes.Count));
            Assert.That(actual.DataNotes[1].Equals(expectedDataNotes[1]));
            Assert.That(actual.Meta.AdditionalProperties.Count, Is.EqualTo(1));
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
            Assert.That(actual.Meta.Dimensions[5].Values[0].AdditionalProperties.ContainsKey("SOURCE"));
            Assert.That(actual.Meta.AdditionalProperties.ContainsKey("NOTE"));
            Assert.That(actual.Meta.AdditionalProperties["NOTE"].Type.Equals(MetaPropertyType.MultilanguageText));
        }

        [Test]
        public void ArhiveMatrixSerializer_DeserializeV10_WithPascalCasePropertyNames_ReturnsCorrectObject()
        {
            List<DecimalDataValue> expectedData =
            [
                new(173.0m, DataValueType.Exists),
                new(0, DataValueType.Confidential),
                new(0.0m, DataValueType.Exists),
                new(0.0m, DataValueType.Exists),
                new(0.0m, DataValueType.Exists),
                new(0.0m, DataValueType.Exists),
                new(1.0m, DataValueType.Exists)
            ];
            Dictionary<int, MultilanguageString> expectedDataNotes = new()
            {
                { 1, new MultilanguageString(new Dictionary<string, string> { { "fi", "..." }, { "en", "..." }, { "sv", "..." } })}
            };
            string[] expectedLanguages = ["fi", "en", "sv"];

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V10_WITH_PASCAL_CASE_PROPERTY_NAMES, GlobalJsonConverterOptions.Default);

            // Assert
            Assert.That((actual.Version).Equals("1.0"));
            Assert.That((actual.Data.Count).Equals(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.That((actual.Data[i]).Equals(expectedData[i]));
            }
            Assert.That(actual.DataNotes.Count, Is.EqualTo(expectedDataNotes.Count));
            Assert.That(actual.DataNotes[1].Equals(expectedDataNotes[1]));
            Assert.That(actual.Meta.AdditionalProperties.Count, Is.EqualTo(1));
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
            Assert.That(actual.Meta.Dimensions[5].Values[0].AdditionalProperties.ContainsKey("SOURCE"));
            Assert.That(actual.Meta.AdditionalProperties.ContainsKey("NOTE"));
            Assert.That(actual.Meta.AdditionalProperties["NOTE"].Type.Equals(MetaPropertyType.MultilanguageText));
        }
    }
}

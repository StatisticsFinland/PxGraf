using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.Text.Json;
using UnitTests.Fixtures;
using UnitTests.Utilities;

namespace UnitTests.SerializerTests
{
    public class ArchiveMatrixSerializerTests
    {
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
            string actual = JsonSerializer.Serialize(archiveCube);

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
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V11);

            // Assert
            JsonUtils.AreEqual(expected, actual);
        }

        [Test]
        public void ArchiveMatrixSerializer_DeserializeV10_ReturnsCorrectObject()
        {
            ArchiveCube archiveCube = JsonSerializer.Deserialize<ArchiveCube>(ArchiveCubeFixtures.ARCHIVE_CUBE_V10);
            string actual = JsonSerializer.Serialize(archiveCube);

            //TODO: Finish this test
        }
    }
}

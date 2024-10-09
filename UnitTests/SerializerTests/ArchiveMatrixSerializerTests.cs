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
using UnitTests.Utilities;

namespace UnitTests.SerializerTests
{
    public class ArchiveMatrixSerializerTests
    {
        [Test]
        public void ArchiveMatrixSerializer_SerializeArchiveCube_ReturnsCorrectJson()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 4),
                new DimensionParameters(DimensionType.Other, 2)
            ];
            string expected = "{\"CreationTime\":\"2024-08-19T14:00:00Z\",\"Meta\":{\"DefaultLanguage\":\"fi\",\"AvailableLanguages\":[\"fi\",\"en\"],\"Dimensions\":[{\"Type\":\"Content\",\"Code\":\"variable-0\",\"Name\":{\"fi\":\"variable-0\",\"en\":\"variable-0.en\"},\"Values\":[{\"Unit\":{\"fi\":\"value-0-unit\",\"en\":\"value-0-unit.en\"},\"LastUpdated\":\"2009-09-01T00:00:00Z\",\"Precision\":0,\"Code\":\"value-0\",\"Name\":{\"fi\":\"value-0\",\"en\":\"value-0.en\"},\"AdditionalProperties\":{\"SOURCE\":{\"Type\":\"MultilanguageText\",\"Value\":{\"fi\":\"value-0-source\",\"en\":\"value-0-source.en\"}}},\"IsVirtual\":false}],\"AdditionalProperties\":{}},{\"Type\":\"Time\",\"Code\":\"variable-1\",\"Name\":{\"fi\":\"variable-1\",\"en\":\"variable-1.en\"},\"Interval\":\"Year\",\"Values\":[{\"Code\":\"2000\",\"Name\":{\"fi\":\"2000\",\"en\":\"2000.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2001\",\"Name\":{\"fi\":\"2001\",\"en\":\"2001.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2002\",\"Name\":{\"fi\":\"2002\",\"en\":\"2002.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2003\",\"Name\":{\"fi\":\"2003\",\"en\":\"2003.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false}],\"AdditionalProperties\":{}},{\"Type\":\"Other\",\"Code\":\"variable-2\",\"Name\":{\"fi\":\"variable-2\",\"en\":\"variable-2.en\"},\"Values\":[{\"Code\":\"value-0\",\"Name\":{\"fi\":\"value-0\",\"en\":\"value-0.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"value-1\",\"Name\":{\"fi\":\"value-1\",\"en\":\"value-1.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false}],\"AdditionalProperties\":{}}],\"AdditionalProperties\":{\"NOTE\":{\"Type\":\"MultilanguageText\",\"Value\":{\"fi\":\"Test note\",\"en\":\"Test note.en\"}}}},\"Data\":null,\"DataNotes\":null,\"Version\":\"1.1\"}";
            ArchiveCube archiveCube = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            
            // Act
            string actual = JsonSerializer.Serialize(archiveCube);

            // Assert
            JsonUtils.JsonStringsAreEqual(expected, actual);
        }

        [Test]
        public void ArchiveMatrixSerializer_Deserialize_ReturnsCorrectObject()
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
            string json = "{\"CreationTime\":\"2024-08-19T14:00:00Z\",\"Meta\":{\"DefaultLanguage\":\"fi\",\"AvailableLanguages\":[\"fi\",\"en\"],\"Dimensions\":[{\"Type\":\"Content\",\"Code\":\"variable-0\",\"Name\":{\"fi\":\"variable-0\",\"en\":\"variable-0.en\"},\"Values\":[{\"Unit\":{\"fi\":\"value-0-unit\",\"en\":\"value-0-unit.en\"},\"LastUpdated\":\"2009-09-01T00:00:00Z\",\"Precision\":0,\"Code\":\"value-0\",\"Name\":{\"fi\":\"value-0\",\"en\":\"value-0.en\"},\"AdditionalProperties\":{\"SOURCE\":{\"Type\":\"MultilanguageText\",\"Value\":{\"fi\":\"value-0-source\",\"en\":\"value-0-source.en\"}}},\"IsVirtual\":false}],\"AdditionalProperties\":{}},{\"Type\":\"Time\",\"Code\":\"variable-1\",\"Name\":{\"fi\":\"variable-1\",\"en\":\"variable-1.en\"},\"Interval\":\"Year\",\"Values\":[{\"Code\":\"2000\",\"Name\":{\"fi\":\"2000\",\"en\":\"2000.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2001\",\"Name\":{\"fi\":\"2001\",\"en\":\"2001.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2002\",\"Name\":{\"fi\":\"2002\",\"en\":\"2002.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"2003\",\"Name\":{\"fi\":\"2003\",\"en\":\"2003.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false}],\"AdditionalProperties\":{}},{\"Type\":\"Other\",\"Code\":\"variable-2\",\"Name\":{\"fi\":\"variable-2\",\"en\":\"variable-2.en\"},\"Values\":[{\"Code\":\"value-0\",\"Name\":{\"fi\":\"value-0\",\"en\":\"value-0.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false},{\"Code\":\"value-1\",\"Name\":{\"fi\":\"value-1\",\"en\":\"value-1.en\"},\"AdditionalProperties\":{},\"IsVirtual\":false}],\"AdditionalProperties\":{}}],\"AdditionalProperties\":{\"NOTE\":{\"Type\":\"MultilanguageText\",\"Value\":{\"fi\":\"Test note\",\"en\":\"Test note.en\"}}}},\"Data\":null,\"DataNotes\":null,\"Version\":\"1.1\"}";

            // Act
            ArchiveCube actual = JsonSerializer.Deserialize<ArchiveCube>(json);

            // Assert
            JsonUtils.AreEqual(expected, actual);
        }

        // TODO: Tests for reading ArchiveCubeV10 json to ArchiveCube
    }
}

#nullable enable
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;

namespace UnitTests.MatrixMetadataTests
{
    public class MatrixMetadataExtensionsTests
    {
        private readonly List<DimensionParameters> _metaParams =
        [
            new DimensionParameters(DimensionType.Content, 1),
            new DimensionParameters(DimensionType.Time, 10),
            new DimensionParameters(DimensionType.Other, 2),
            new DimensionParameters(DimensionType.Other, 2),
        ];
        private readonly List<DimensionParameters> _queryParams =
        [
            new DimensionParameters(DimensionType.Content, 1),
            new DimensionParameters(DimensionType.Time, 4),
            new DimensionParameters(DimensionType.Other, 1),
            new DimensionParameters(DimensionType.Other, 1),
        ];

        private readonly MatrixMetadata _input;
        private readonly MatrixQuery _query;

        public MatrixMetadataExtensionsTests()
        {
            _input = TestDataCubeBuilder.BuildTestMeta(_metaParams);
            _query = TestDataCubeBuilder.BuildTestCubeQuery(_queryParams);
        }

        [Test]
        public void FilterDimensionValuesReturnsFilteredMatrixMetadata()
        {
            // Act
            IReadOnlyMatrixMetadata result = _input.FilterDimensionValues(_query);

            // Assert
            Assert.That(result.Dimensions.Count.Equals(4));
            Assert.That(result.Dimensions[0].Values.Count.Equals(1));
            Assert.That(result.Dimensions[1].Values.Count.Equals(4));
            Assert.That(result.Dimensions[2].Values.Count.Equals(1));
            Assert.That(result.Dimensions[3].Values.Count.Equals(1));
        }

        [Test]
        public void GetNumberOfMultivalueDimensionsReturnsCorrectNumberOfMultivalueDimensions()
        {
            // Act
            int result = _input.GetNumberOfMultivalueDimensions();

            // Assert
            Assert.That(result.Equals(3));
        }

        [Test]
        public void GetMultivalueDimensionsReturnsCorrectMultivalueDimensions()
        {
            // Act
            IReadOnlyList<IReadOnlyDimension> result = _input.GetMultivalueDimensions();

            // Assert
            Assert.That(result.Count.Equals(3));
            Assert.That(result[0].Values.Count.Equals(10));
            Assert.That(result[1].Values.Count.Equals(2));
            Assert.That(result[2].Values.Count.Equals(2));
        }

        [Test]
        public void GetSinglevalueDimensionsReturnsCorrectSinglevalueDimensions()
        {
            // Act
            IReadOnlyList<IReadOnlyDimension> result = _input.GetSinglevalueDimensions();

            // Assert
            Assert.That(result.Count.Equals(1));
            Assert.That(result[0].Values.Count.Equals(1));
        }

        [Test]
        public void GetSortedMultivalueDimensionsReturnsCorrectSortedMultivalueDimensions()
        {
            // Act
            IReadOnlyList<IReadOnlyDimension> result = _input.GetSortedMultivalueDimensions();

            // Assert
            Assert.That(result.Count.Equals(3));
            Assert.That(result[0].Values.Count.Equals(10));
            Assert.That(result[1].Values.Count.Equals(2));
            Assert.That(result[2].Values.Count.Equals(2));
        }

        [Test]
        public void GetLargestMultivalueDimensionReturnsCorrectLargestMultivalueDimension()
        {
            // Act
            IReadOnlyDimension? result = _input.GetLargestMultivalueDimension();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Values.Count.Equals(10));
        }

        [Test]
        public void GetSmallerMultivalueDimensionReturnsCorrectSecondLargestMultivalueDimension()
        {
            // Act
            IReadOnlyDimension? result = _input.GetSmallerMultivalueDimension();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Values.Count.Equals(2));
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalCalledWithTimeDimensionReturnsCorrectTimeDimension()
        {
            // Act
            IReadOnlyDimension? result = _input.GetMultivalueTimeOrLargestOrdinal();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Values.Count.Equals(10));
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalCalledWithSinglevalueTimeDimensionReturnsCorrectOrdinalDimension()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 4),
                new DimensionParameters(DimensionType.Other, 2)
            ];
            MatrixMetadata input = TestDataCubeBuilder.BuildTestMeta(metaParams);

            // Act
            IReadOnlyDimension? result = input.GetMultivalueTimeOrLargestOrdinal();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Values.Count.Equals(4));
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalCalledWithoutTimeOrOrdinalDimensionsReturnsNull()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Other, 2)
            ];
            MatrixMetadata input = TestDataCubeBuilder.BuildTestMeta(metaParams);

            // Act
            IReadOnlyDimension? result = input.GetMultivalueTimeOrLargestOrdinal();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetLastUpdatedReturnsCorrectLastUpdated()
        {
            // Act
            DateTime? result = _input.GetLastUpdated();

            // Assert
            Assert.That(result, Is.EqualTo(new DateTime(2009, 9, 1, 0, 0, 0, DateTimeKind.Utc)));
        }

        [Test]
        public void GetMatrixMultilanguagePropertyReturnsCorrectMultilanguageString()
        {
            // Arrange
            Dictionary<string, string> translations = new()
            {
                { "fi", "Test.fi" },
                { "en", "Test.en" },
                { "sv", "Test.sv" }
            };
            MultilanguageStringProperty prop = new(new MultilanguageString(translations));
            _input.AdditionalProperties.Add("TEST", prop);

            // Act
            MultilanguageString? result = _input.GetMatrixMultilanguageProperty("TEST");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!["fi"].Equals("Test.fi"));
            Assert.That(result!["en"].Equals("Test.en"));
            Assert.That(result!["sv"].Equals("Test.sv"));
        }

        [Test]
        public void GetMatrixMultilanguagePropertyCalledWithNonExistingPropertyReturnsNull()
        {
            // Act
            MultilanguageString? result = _input.GetMatrixMultilanguageProperty("FOO");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
#nullable disable

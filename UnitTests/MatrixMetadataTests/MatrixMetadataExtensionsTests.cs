using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
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

        private MatrixMetadata _input;
        private MatrixQuery _query;

        [SetUp]
        public void Setup()
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
            IReadOnlyDimension result = _input.GetLargestMultivalueDimension();

            // Assert
            Assert.That(result.Values.Count.Equals(10));
        }

        [Test]
        public void GetSmallerMultivalueDimensionReturnsCorrectSecondLargestMultivalueDimension()
        {
            // Act
            IReadOnlyDimension result = _input.GetSmallerMultivalueDimension();

            // Assert
            Assert.That(result.Values.Count.Equals(2));
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalCalledWithTimeDimensionReturnsCorrectTimeDimension()
        {
            // Act
            IReadOnlyDimension result = _input.GetMultivalueTimeOrLargestOrdinal();

            // Assert
            Assert.That(result.Values.Count.Equals(10));
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
            IReadOnlyDimension result = input.GetMultivalueTimeOrLargestOrdinal();

            // Assert
            Assert.That(result.Values.Count.Equals(4));
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
            MultilanguageString result = _input.GetMatrixMultilanguageProperty("TEST");

            // Assert
            Assert.That(result["fi"].Equals("Test.fi"));
            Assert.That(result["en"].Equals("Test.en"));
            Assert.That(result["sv"].Equals("Test.sv"));
        }

        [Test]
        public void AssignSourceToContentDimensionValuesReturnsCorrectSourceForContentDimensionValuesWithDimensionValueLevelSource()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];
            MatrixMetadata originalMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            MultilanguageStringProperty dimensionLevelSource = new(new MultilanguageString(new Dictionary<string, string> { { "fi", "Dimension level source.fi" }, { "en", "Dimension level source.en" } }));
            Dictionary<string, MetaProperty> dimensionAdditionalProperties = new()
            {
                { PxSyntaxConstants.SOURCE_KEY, dimensionLevelSource }
            };
            ContentValueList originalCdValues = (ContentValueList)originalMeta.Dimensions[0].Values;
            ContentDimension contentDimensionWithSource = new(
                originalMeta.Dimensions[0].Code,
                originalMeta.Dimensions[0].Name,
                dimensionAdditionalProperties,
                originalCdValues);
            MultilanguageStringProperty tableLevelSource = new(new MultilanguageString(new Dictionary<string, string> { { "fi", "Table level source.fi" }, { "en", "Table level source.en" } }));
            Dictionary<string, MetaProperty> tableAdditionalProperties = new()
            {
                { PxSyntaxConstants.SOURCE_KEY, tableLevelSource }
            };
            MatrixMetadata input = new(
                "fi",
                ["fi", "en"],
                [contentDimensionWithSource],
                tableAdditionalProperties
            );

            // Act
            input.AssignSourceToContentDimensionValues();
            MetaProperty source = input.Dimensions[0].Values[0].AdditionalProperties["SOURCE"];
            MultilanguageStringProperty sourceMls = (MultilanguageStringProperty)source;

            // Assert
            Assert.That(sourceMls.Value["fi"].Equals("value-0-source"));
        }

        [Test]
        public void AssignSourceToContentDimensionValuesReturnsCorrectSourceForContentDimensionValuesWithDimensionLevelSource()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];
            MatrixMetadata originalMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            MultilanguageStringProperty dimensionLevelSource = new(new MultilanguageString(new Dictionary<string, string> { { "fi", "Dimension level source.fi" }, { "en", "Dimension level source.en" } }));
            Dictionary<string, MetaProperty> dimensionAdditionalProperties = new()
            {
                { PxSyntaxConstants.SOURCE_KEY, dimensionLevelSource }
            };
            MultilanguageStringProperty tableLevelSource = new(new MultilanguageString(new Dictionary<string, string> { { "fi", "Table level source.fi" }, { "en", "Table level source.en" } }));
            Dictionary<string, MetaProperty> tableAdditionalProperties = new()
            {
                { PxSyntaxConstants.SOURCE_KEY, tableLevelSource }
            };
            ContentDimensionValue originalCdv = (ContentDimensionValue)originalMeta.Dimensions[0].Values[0];
            ContentDimensionValue contentDimensionValueWithoutSource = new(
                originalCdv.Code,
                originalCdv.Name,
                originalCdv.Unit,
                originalCdv.LastUpdated,
                originalCdv.Precision,
                false,
                []
                );
            ContentValueList contentDimensionValues = new(new[] { contentDimensionValueWithoutSource });
            ContentDimension contentDimensionWithSource = new(
                originalMeta.Dimensions[0].Code,
                originalMeta.Dimensions[0].Name,
                dimensionAdditionalProperties,
                contentDimensionValues);
            MatrixMetadata input = new(
                "fi",
                ["fi", "en"],
                [contentDimensionWithSource],
                tableAdditionalProperties
                );

            // Act 
            input.AssignSourceToContentDimensionValues();
            MetaProperty source = input.Dimensions[0].Values[0].AdditionalProperties["SOURCE"];
            MultilanguageStringProperty sourceMls = (MultilanguageStringProperty)source;

            // Assert
            Assert.That(sourceMls.Value["fi"].Equals("Dimension level source.fi"));
        }

        [Test]
        public void AssignSourceToContentDimensionValuesReturnsCorrectSourceForContentDimensionValuesWithTableLevelSource()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];
            MatrixMetadata originalMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            MultilanguageStringProperty tableLevelSource = new(new MultilanguageString(new Dictionary<string, string> { { "fi", "Table level source.fi" }, { "en", "Table level source.en" } }));
            Dictionary<string, MetaProperty> tableAdditionalProperties = new()
            {
                { PxSyntaxConstants.SOURCE_KEY, tableLevelSource }
            };
            ContentDimensionValue originalCdv = (ContentDimensionValue)originalMeta.Dimensions[0].Values[0];
            ContentDimensionValue contentDimensionValueWithoutSource = new(
                originalCdv.Code,
                originalCdv.Name,
                originalCdv.Unit,
                originalCdv.LastUpdated,
                originalCdv.Precision,
                false,
                []
                );
            ContentValueList contentDimensionValues = new(new[] { contentDimensionValueWithoutSource });
            ContentDimension contentDimensionWithoutSource = new(
                originalMeta.Dimensions[0].Code,
                originalMeta.Dimensions[0].Name,
                [],
                contentDimensionValues);
            MatrixMetadata input = new(
                "fi",
                ["fi", "en"],
                [contentDimensionWithoutSource],
                tableAdditionalProperties
                );

            // Act 
            input.AssignSourceToContentDimensionValues();
            MetaProperty source = input.Dimensions[0].Values[0].AdditionalProperties["SOURCE"];
            MultilanguageStringProperty sourceMls = (MultilanguageStringProperty)source;

            // Assert
            Assert.That(sourceMls.Value["fi"].Equals("Table level source.fi"));
        }

        [Test]
        public void AssignSourceToContentDimensionValuesThrowsWhenNoSourceIsProvided()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];
            MatrixMetadata originalMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            ContentDimensionValue originalCdv = (ContentDimensionValue)originalMeta.Dimensions[0].Values[0];
            ContentDimensionValue contentDimensionValueWithoutSource = new(
                originalCdv.Code,
                originalCdv.Name,
                originalCdv.Unit,
                originalCdv.LastUpdated,
                originalCdv.Precision,
                false,
                []
                );
            ContentValueList contentDimensionValues = new(new[] { contentDimensionValueWithoutSource });
            ContentDimension contentDimensionWithoutSource = new(
                originalMeta.Dimensions[0].Code,
                originalMeta.Dimensions[0].Name,
                [],
                contentDimensionValues);
            MatrixMetadata input = new(
                "fi",
                ["fi", "en"],
                [contentDimensionWithoutSource],
                []
                );

            // Act && Assert
            Assert.Throws<InvalidOperationException>(() => input.AssignSourceToContentDimensionValues());
        }

        [Test]
        public void AssignOrdinalDimensionTypesReturnsCorrectOrdinalDimensionTypes()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Geographical, 2),
                new DimensionParameters(DimensionType.Nominal, 2),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Unknown, 2),
            ];
            MultilanguageStringProperty metaId = new(new MultilanguageString(new Dictionary<string, string> { { "fi", PxSyntaxConstants.ORDINAL_VALUE }, { "en", PxSyntaxConstants.ORDINAL_VALUE } }));
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            meta.Dimensions[0].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);
            meta.Dimensions[1].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);
            meta.Dimensions[2].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);
            meta.Dimensions[3].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);
            meta.Dimensions[4].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);
            meta.Dimensions[5].AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);

            // Act
            MatrixMetadata result = meta.AssignOrdinalDimensionTypes();

            // Assert
            // Only other and unknown dimension types should be affected
            Assert.That(result.Dimensions[0].Type.Equals(DimensionType.Content));
            Assert.That(result.Dimensions[1].Type.Equals(DimensionType.Time));
            Assert.That(result.Dimensions[2].Type.Equals(DimensionType.Geographical));
            Assert.That(result.Dimensions[3].Type.Equals(DimensionType.Nominal));
            Assert.That(result.Dimensions[4].Type.Equals(DimensionType.Ordinal));
            Assert.That(result.Dimensions[5].Type.Equals(DimensionType.Ordinal));
        }
    }
}

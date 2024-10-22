#nullable enable
using NUnit.Framework;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Utility;
using System;

namespace UnitTests.MatrixMetadataTests
{
    public class DimensionExtensionsTests
    {
        [Test]
        public void ConvertToVariableWithoutDimensionQueryReturnsCorrectVariable()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 2)
            ];
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);

            // Act
            Variable variable = meta.Dimensions[0].ConvertToVariable([], meta);

            // Assert
            Assert.That(variable, Is.Not.Null);
            Assert.That(variable.Name.Equals(meta.Dimensions[0].Name));
            Assert.That(variable.Code.Equals(meta.Dimensions[0].Code));
            Assert.That(variable.Type.Equals("C"));
            Assert.That(variable.Values.Count.Equals(meta.Dimensions[0].Values.Count));
        }

        [Test]
        public void ConvertToVariableWithDimensionQueryReturnsCorrectVariableWithNameEdit()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Time, 2)
            ];
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);
            Dictionary<string, string> nameEdits = new()
            {
                { "fi", "NameEdit.fi" },
                { "en", "NameEdit.en" }
            };

            Dictionary<string, DimensionQuery> dimensionQueries = new()
            {
                { meta.Dimensions[0].Code, new DimensionQuery { NameEdit = new MultilanguageString(nameEdits) } }
            };

            // Act
            Variable variable = meta.Dimensions[0].ConvertToVariable(dimensionQueries, meta);

            // Assert
            Assert.That(variable, Is.Not.Null);
            Assert.That(variable.Name.Equals(new MultilanguageString(nameEdits)));
            Assert.That(variable.Code.Equals(meta.Dimensions[0].Code));
            Assert.That(variable.Type.Equals("T"));
            Assert.That(variable.Values.Count.Equals(meta.Dimensions[0].Values.Count));
        }

        [Test]
        public void GetEliminationValueCodeReturnsCorrectEliminationValueCodeWithStringProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Other, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            dimension.AdditionalProperties.Add(PxSyntaxConstants.ELIMINATION_KEY, new StringProperty("eliminationValueCode"));

            // Act
            string? eliminationValueCode = dimension.GetEliminationValueCode();

            // Assert
            Assert.That(eliminationValueCode, Is.Not.Null);
            Assert.That(eliminationValueCode!.Equals("eliminationValueCode"));
        }

        [Test]
        public void GetEliminationValueCodeReturnsCorrectEliminationValueCodeWithMultilanguageStringProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Other, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            Dictionary<string, string> eliminationValueName = new()
            {
                { "fi", "value0.fi" },
                { "en", "value0.en" }
            };
            dimension.AdditionalProperties.Add(PxSyntaxConstants.ELIMINATION_KEY, new MultilanguageStringProperty(new(eliminationValueName)));
            dimension.Values.Add(new ContentDimensionValue("value0-code", new MultilanguageString(eliminationValueName), new([]), DateTime.MinValue, 0));
            // Act
            string? eliminationValueCode = dimension.GetEliminationValueCode();

            // Assert
            Assert.That(eliminationValueCode, Is.Not.Null);
            Assert.That(eliminationValueCode!.Equals("value0-code"));
        }

        [Test]
        public void GetEliminationValueCodeReturnsNullWithoutEliminationValueCode()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Other, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);

            // Act
            string? eliminationValueCode = dimension.GetEliminationValueCode();

            // Assert
            Assert.That(eliminationValueCode, Is.Null);
        }

        [Test]
        public void GetMultilanguageDimensionPropertyReturnsCorrectMultilanguageStringProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Other, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            Dictionary<string, string> note = new()
            {
                { "fi", "note.fi" },
                { "en", "note.en" }
            };
            dimension.AdditionalProperties.Add(PxSyntaxConstants.NOTE_KEY, new MultilanguageStringProperty(new(note)));

            // Act
            MultilanguageString? noteProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY);

            // Assert
            Assert.That(noteProperty, Is.Not.Null);
            Assert.That(noteProperty!.Equals(new MultilanguageString(note)));
            Assert.That(dimension.AdditionalProperties.ContainsKey(PxSyntaxConstants.NOTE_KEY), Is.True);
        }

        [Test]
        public void GetMultilanguageDimensionPropertyReturnsNullWithoutProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Other, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);

            // Act
            MultilanguageString? noteProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY);

            // Assert
            Assert.That(noteProperty, Is.Null);
        }

        [Test]
        public void GetMultilanguageDimensionPropertyWithRemoveFlagRemovesProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Unknown, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            MultilanguageStringProperty metaString = new(new MultilanguageString(new Dictionary<string, string> { { "fi", PxSyntaxConstants.ORDINAL_VALUE }, { "en", PxSyntaxConstants.ORDINAL_VALUE } }));
            dimension.AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaString);

            // Act
            MultilanguageString? metaProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.META_ID_KEY, true);

            // Assert
            Assert.That(metaProperty, Is.Not.Null);
            Assert.That(dimension.AdditionalProperties.ContainsKey(PxSyntaxConstants.NOTE_KEY), Is.False);
        }

        [Test]
        public void GetDimensionTypeReturnsOrdinalDimensionType()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Unknown, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            MultilanguageStringProperty metaId = new(new MultilanguageString(new Dictionary<string, string> { { "fi", PxSyntaxConstants.ORDINAL_VALUE }, { "en", PxSyntaxConstants.ORDINAL_VALUE } }));
            dimension.AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);

            // Act
            DimensionType dimensionType = dimension.GetDimensionType();

            // Assert
            Assert.That(dimensionType, Is.EqualTo(DimensionType.Ordinal));
        }

        [Test]
        public void GetDimensionTypeReturnsNominalDimensionType()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Unknown, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            MultilanguageStringProperty metaId = new(new MultilanguageString(new Dictionary<string, string> { { "fi", PxSyntaxConstants.NOMINAL_VALUE }, { "en", PxSyntaxConstants.NOMINAL_VALUE } }));
            dimension.AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaId);

            // Act
            DimensionType dimensionType = dimension.GetDimensionType();

            // Assert
            Assert.That(dimensionType, Is.EqualTo(DimensionType.Nominal));
        }

        [Test]
        public void GetDimensionTypeReturnsUnknownDimensionType()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Unknown, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);

            // Act
            DimensionType dimensionType = dimension.GetDimensionType();

            // Assert
            Assert.That(dimensionType, Is.EqualTo(DimensionType.Unknown));
        }
    }
}
#nullable disable

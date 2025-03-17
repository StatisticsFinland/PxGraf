#nullable enable
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Models.Metadata;
using PxGraf.Utility;
using System.Collections.Generic;
using System;

namespace UnitTests.MatrixMetadataTests
{
    public class DimensionExtensionsTests
    {

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
            MultilanguageString? noteProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY, "fi");

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
            MultilanguageString? noteProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.NOTE_KEY, "fi");

            // Assert
            Assert.That(noteProperty, Is.Null);
        }

        [Test]
        public void GetMultilanguageDimensionPropertyReturnProperty()
        {
            // Arrange
            DimensionParameters dimParams = new(DimensionType.Unknown, 1);
            Dimension dimension = TestDataCubeBuilder.BuildTestDimension("foo", dimParams, ["fi", "en"]);
            MultilanguageStringProperty metaString = new(new MultilanguageString(new Dictionary<string, string> { { "fi", PxSyntaxConstants.ORDINAL_VALUE }, { "en", PxSyntaxConstants.ORDINAL_VALUE } }));
            dimension.AdditionalProperties.Add(PxSyntaxConstants.META_ID_KEY, metaString);

            // Act
            MultilanguageString? metaProperty = dimension.GetMultilanguageDimensionProperty(PxSyntaxConstants.META_ID_KEY, "fi");

            // Assert
            Assert.That(metaProperty, Is.Not.Null);
        }
    }
}
#nullable disable

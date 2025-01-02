using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System.Collections.Generic;

namespace UnitTests.MatrixMetadataTests
{
    public class DimensionValueExtensionsTests
    {
        [Test]
        public void ConvertToVariableValueReturnsVariableWithoutQuery()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1, true)
            ];
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);
            Dimension dimension = meta.Dimensions[0];
            DimensionValue value = dimension.Values[0];

            // Act
            VariableValue variableValue = value.ConvertToVariableValue(value.Code, null);

            // Assert
            Assert.That(variableValue, Is.Not.Null);
            Assert.That(variableValue.Code.Equals(value.Code));
            Assert.That(variableValue.Name.Equals(value.Name));
            Assert.That(variableValue.IsSumValue.Equals(true));
        }

        [Test]
        public void ConvertToVariableValueReturnsVariableWithQuery()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Time, 1, true)
            ];
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);
            Dimension dimension = meta.Dimensions[0];
            DimensionValue value = dimension.Values[0];
            MultilanguageString nameEditMls = new(new Dictionary<string, string> { { "fi", "NameEdit.fi" }, { "en", "NameEdit.en" } });
            DimensionQuery dimensionQuery = new()
            {
                ValueEdits = new Dictionary<string, DimensionQuery.DimensionValueEdition>
                {
                    { value.Code, new DimensionQuery.DimensionValueEdition { NameEdit = nameEditMls } }
                }
            };

            // Act
            VariableValue variableValue = value.ConvertToVariableValue(value.Code, dimensionQuery);

            // Assert
            Assert.That(variableValue, Is.Not.Null);
            Assert.That(variableValue.Code.Equals(value.Code));
            Assert.That(variableValue.Name.Equals(nameEditMls));
            Assert.That(variableValue.IsSumValue.Equals(true));
        }

        [Test]
        public void ConvertToVariableValueWithContentComponentEdit()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);
            Dimension dimension = meta.Dimensions[0];
            ContentDimensionValue value = (ContentDimensionValue)dimension.Values[0];
            ContentComponentEdition contentComponentEdit = new()
            {
                SourceEdit = new MultilanguageString(new Dictionary<string, string> { { "fi", "SourceEdit.fi" }, { "en", "SourceEdit.en" } }),
                UnitEdit = new MultilanguageString(new Dictionary<string, string> { { "fi", "UnitEdit.fi" }, { "en", "UnitEdit.en" } })
            };
            DimensionQuery dimensionQuery = new()
            {
                ValueEdits = new Dictionary<string, DimensionQuery.DimensionValueEdition>
                {
                    { value.Code, new DimensionQuery.DimensionValueEdition { NameEdit = null, ContentComponent = contentComponentEdit } }
                }
            };

            // Act
            VariableValue variableValue = value.ConvertToVariableValue(null, dimensionQuery);

            // Assert
            Assert.That(variableValue, Is.Not.Null);
            Assert.That(variableValue.Code.Equals(value.Code));
            Assert.That(variableValue.Name.Equals(value.Name));
            Assert.That(variableValue.IsSumValue.Equals(false));
            Assert.That(variableValue.ContentComponent, Is.Not.Null);
            Assert.That(variableValue.ContentComponent.NumberOfDecimals.Equals(value.Precision));
            Assert.That(variableValue.ContentComponent.Source.Equals(contentComponentEdit.SourceEdit));
            Assert.That(variableValue.ContentComponent.Unit.Equals(contentComponentEdit.UnitEdit));
        }

        [Test]
        public void ConvertToVariableValueWithNullContentComponentEdit()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1)
            ];

            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(dimParams);
            Dimension dimension = meta.Dimensions[0];
            ContentDimensionValue value = (ContentDimensionValue)dimension.Values[0];
            MultilanguageString nameEditMls = new(new Dictionary<string, string> { { "fi", "NameEdit.fi" }, { "en", "NameEdit.en" } });
            DimensionQuery dimensionQuery = new()
            {
                ValueEdits = new Dictionary<string, DimensionQuery.DimensionValueEdition>
                {
                    { value.Code, new DimensionQuery.DimensionValueEdition { NameEdit = nameEditMls, ContentComponent = null } }
                }
            };

            // Act
            VariableValue variableValue = value.ConvertToVariableValue(null, dimensionQuery);

            // Assert
            Assert.That(variableValue, Is.Not.Null);
            Assert.That(variableValue.Code.Equals(value.Code));
            Assert.That(variableValue.Name.Equals(nameEditMls));
            Assert.That(variableValue.IsSumValue.Equals(false));
            Assert.That(variableValue.ContentComponent, Is.Not.Null);
            Assert.That(variableValue.ContentComponent.NumberOfDecimals.Equals(value.Precision));
            if(value.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty metaProperty) &&
                metaProperty is MultilanguageStringProperty mlsProperty)
            {
                Assert.That(variableValue.ContentComponent.Source.Equals(mlsProperty.Value));
            }
            else Assert.Fail();
            Assert.That(variableValue.ContentComponent.Unit.Equals(value.Unit));
        }
    }
}

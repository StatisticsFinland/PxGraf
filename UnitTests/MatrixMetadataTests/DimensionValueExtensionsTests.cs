using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using System.Collections.Generic;
using Px.Utils.Models.Metadata.Dimensions;
using PxGraf.Utility;
using PxGraf.Data.MetaData;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using Px.Utils.Language;

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
            VariableValue variableValue = value.ConvertToVariableValue(value.Code, null, meta);

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
            VariableValue variableValue = value.ConvertToVariableValue(value.Code, dimensionQuery, meta);

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
            VariableValue variableValue = value.ConvertToVariableValue(null, dimensionQuery, meta);

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
            VariableValue variableValue = value.ConvertToVariableValue(null, dimensionQuery, meta);

            // Assert
            Assert.That(variableValue, Is.Not.Null);
            Assert.That(variableValue.Code.Equals(value.Code));
            Assert.That(variableValue.Name.Equals(nameEditMls));
            Assert.That(variableValue.IsSumValue.Equals(false));
            Assert.That(variableValue.ContentComponent, Is.Not.Null);
            Assert.That(variableValue.ContentComponent.NumberOfDecimals.Equals(value.Precision));
            Assert.That(variableValue.ContentComponent.Source.Equals(value.GetSource(meta)));
            Assert.That(variableValue.ContentComponent.Unit.Equals(value.Unit));
        }
    }
}

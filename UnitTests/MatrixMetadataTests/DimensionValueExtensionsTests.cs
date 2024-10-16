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
                ValueEdits = new Dictionary<string, DimensionQuery.VariableValueEdition>
                {
                    { value.Code, new DimensionQuery.VariableValueEdition { NameEdit = nameEditMls } }
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
    }
}

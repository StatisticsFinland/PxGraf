using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class ValidateTableMetaDataTests
    {
        [TestCase(DimensionType.Content, DimensionType.Time, true, true, true)]
        [TestCase(DimensionType.Time, DimensionType.Unknown, false, true, true)]
        [TestCase(DimensionType.Content, DimensionType.Unknown, true, false, true)]
        [TestCase(DimensionType.Unknown, DimensionType.Unknown, false, false, true)]
        [TestCase(DimensionType.Content, DimensionType.Time, false, true, false, 0)]
        public async Task ValidateTableMetaData_ReturnsExpectedResult(
            DimensionType firstDimensionType,
            DimensionType secondDimensionType,
            bool hasContentVariable,
            bool hasTimeVariable,
            bool noZeroSizedVariables,
            int firstVariableSize = 1)
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(firstDimensionType, firstVariableSize),
                new DimensionParameters(secondDimensionType, 1)
            ];

            CreationController controller = TestCreationControllerBuilder.BuildController([], dimParams);

            // Act
            TableMetaValidationResult result = await controller.ValidateTableMetaData("path/table.px");

            // Assert
            Assert.That(result.TableHasContentVariable, Is.EqualTo(hasContentVariable));
            Assert.That(result.TableHasTimeVariable, Is.EqualTo(hasTimeVariable));
            Assert.That(result.AllVariablesContainValues, Is.EqualTo(noZeroSizedVariables));
        }

        [Test]
        public async Task ValidateTableMetadata_CalledForNullTable_ReturnsExpectedResult()
        {
            // Arrange
            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) =>
                {
                    return null;
                });
            CreationController controller = new (dataSource.Object, logger.Object);

            // Act
            TableMetaValidationResult result = await controller.ValidateTableMetaData("foo");

            // Assert
            Assert.That(result.TableHasContentVariable, Is.False);
            Assert.That(result.TableHasTimeVariable, Is.False);
            Assert.That(result.AllVariablesContainValues, Is.False);
        }
    }
}
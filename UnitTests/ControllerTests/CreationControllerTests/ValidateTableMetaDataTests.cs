using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
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
    }
}
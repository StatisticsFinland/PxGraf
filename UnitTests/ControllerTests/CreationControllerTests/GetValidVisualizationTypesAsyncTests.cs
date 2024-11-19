
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetValidVisualizationTypesAsyncTests
    {
        [Test]
        public async Task GetValidVisualizationTypesAsyncTest()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Nominal, 3),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            CreationController controller = TestCreationControllerBuilder.BuildController(metaParams, metaParams, null);
            MatrixQuery matrixQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            List<string> expected = ["LineChart", "GroupVerticalBarChart", "StackedVerticalBarChart", "PercentVerticalBarChart", "Table"];

            // Act
            ActionResult<List<string>> result = await controller.GetValidVisualizationTypesAsync(matrixQuery);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expected));
        }
    }
}

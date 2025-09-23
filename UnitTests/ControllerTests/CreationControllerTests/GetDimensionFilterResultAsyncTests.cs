using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetVariableFilterResultAsyncTests
    {
        [Test]
        public async Task GetVariableFilterResultAsyncTest()
        {
            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new (DimensionType.Content, 3),
                new (DimensionType.Time, 12),
                new (DimensionType.Nominal, 5),
                new (DimensionType.Other, 3)
            ];
            string path = Path.Combine("foo", "bar", "baz");
            FilterRequest filter = new()
            {
                TableReference = new(path),
                Filters = new Dictionary<string, IValueFilter>
                {
                    { "variable-0", new ItemFilter(["value-0"]) },
                    { "variable-1", new AllFilter() },
                    { "variable-2", new FromFilter("value-3") },
                    { "variable-3", new TopFilter(2) },
                    { "foo", new AllFilter() }
                }
            };
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams, null);

            // Act
            ActionResult<Dictionary<string, List<string>>> result = await controller.GetDimensionFilterResultAsync(filter);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Count, Is.EqualTo(5));
            Assert.That(result.Value["variable-0"].Count, Is.EqualTo(1));
            Assert.That(result.Value["variable-1"].Count, Is.EqualTo(12));
            Assert.That(result.Value["variable-2"].Count, Is.EqualTo(2));
            Assert.That(result.Value["variable-3"].Count, Is.EqualTo(2));
            Assert.That(result.Value["foo"], Is.Not.Null);
            Assert.That(result.Value["foo"].Count, Is.EqualTo(0));
        }
    }
}

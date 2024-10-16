using NUnit.Framework;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using Px.Utils.Language;
using PxGraf.Controllers;
using System;
using PxGraf.Utility;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetDataBaseListingTests
    {
        [Test]
        public async Task GetDatabaseListingAsync_EmptyPath_ReturnsHeaderGroups()
        {
            // Arrange
            Dictionary<string, string> header1Name = new()
            {
                ["fi"] = "header1",
                ["en"] = "header1.en"
            };

            Dictionary<string, string> header2Name = new()
            {
                ["fi"] = "header2",
                ["en"] = "header2.en"
            };

            List<DatabaseGroupHeader> expectedHeaders =
            [
                new DatabaseGroupHeader("header1", ["fi", "en"], new MultilanguageString(header1Name)),
                new DatabaseGroupHeader("header2", ["fi", "en"], new MultilanguageString(header2Name))
            ];
            DatabaseGroupContents expectedContents = new(expectedHeaders, []);
            CreationController controller = TestCreationControllerBuilder.BuildController([], [], expectedContents);

            // Act
            DatabaseGroupContents result = await controller.GetDataBaseListingAsync(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Headers, Is.EqualTo(expectedHeaders));
            Assert.That(expectedHeaders[0].Code, Is.EqualTo(result.Headers[0].Code));
        }

        [Test]
        public async Task GetDatabaseListingAsync_WithPath_ReturnsFiles()
        {
            // Arrange
            Dictionary<string, string> table1Name = new()
            {
                ["fi"] = "table1",
                ["en"] = "table1.en"
            };

            Dictionary<string, string> table2Name = new()
            {
                ["fi"] = "table2",
                ["en"] = "table2.en"
            };

            DateTime lastUpdated = PxSyntaxConstants.ParseDateTime("2021-01-01T00:00:00.000Z");

            List<DatabaseTable> expectedTables =
            [
                new DatabaseTable("table1", new(table1Name), lastUpdated, ["fi, en"]),
                new DatabaseTable("table2", new(table2Name), lastUpdated, ["fi, en"]),
            ];
            DatabaseGroupContents expectedContents = new([], expectedTables);
            CreationController controller = TestCreationControllerBuilder.BuildController([], [], expectedContents);

            // Act
            DatabaseGroupContents result = await controller.GetDataBaseListingAsync("database/subgroup/folder");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Files, Is.EqualTo(expectedTables));
            Assert.That(expectedTables[0].Code, Is.EqualTo(result.Files[0].Code));
        }
    }
}

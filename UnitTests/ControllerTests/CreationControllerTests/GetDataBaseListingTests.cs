using NUnit.Framework;
using PxGraf.Models.Responses.DatabaseItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using Px.Utils.Language;
using PxGraf.Controllers;
using System;
using PxGraf.Utility;
using Microsoft.AspNetCore.Mvc;
using PxGraf.Models.Responses;
using Microsoft.Extensions.Configuration;
using PxGraf.Language;
using UnitTests.Fixtures;
using PxGraf.Settings;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetDataBaseListingTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> settings = new()
            {
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"},
                {"LocalFilesystemDatabaseConfig:DatabaseWhitelist:0", "StatFin" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetDatabaseListingAsync_EmptyPath_ReturnsWhitelistedHeaderGroups()
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
                new DatabaseGroupHeader("StatFin", ["fi", "en"], new MultilanguageString(header1Name)),
            ];

            List<DatabaseGroupHeader> allHeaders =
            [
                new DatabaseGroupHeader("StatFin", ["fi", "en"], new MultilanguageString(header1Name)),
                new DatabaseGroupHeader("NotStatFin", ["fi", "en"], new MultilanguageString(header2Name))
            ];

            DatabaseGroupContents allDatabases = new(allHeaders, []);
            CreationController controller = TestCreationControllerBuilder.BuildController([], [], allDatabases);

            // Act
            ActionResult<DatabaseGroupContents> actionResult = await controller.GetDataBaseListingAsync(null);

            // Assert
            Assert.That(actionResult.Value, Is.Not.Null);
            Assert.That(actionResult.Value.Headers.Count, Is.EqualTo(expectedHeaders.Count));
            Assert.That(expectedHeaders[0].Code, Is.EqualTo(actionResult.Value.Headers[0].Code));
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
            ActionResult<DatabaseGroupContents> actionResult = await controller.GetDataBaseListingAsync("StatFin/subgroup/folder");

            // Assert
            Assert.That(actionResult.Value, Is.Not.Null);
            Assert.That(actionResult.Value.Files, Is.EqualTo(expectedTables));
            Assert.That(expectedTables[0].FileName, Is.EqualTo(actionResult.Value.Files[0].FileName));
        }

        [Test]
        public async Task GetDatabaseListingAsync_WithPathWithUnallowedDatabase_ReturnsNotFound()
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
            ActionResult<DatabaseGroupContents> actionResult = await controller.GetDataBaseListingAsync("database/subgroup/folder");

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

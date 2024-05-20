using NUnit.Framework;
using PxGraf.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using PxGraf.PxWebInterface;
using Microsoft.Extensions.Configuration;
using UnitTests.Fixtures;
using PxGraf.Settings;
using PxGraf.PxWebInterface.SerializationModels;
using System.Linq;

namespace CreationControllerTests
{
    internal class GetDataBaseListingTests
    {
        private Mock<ICachedPxWebConnection> _mockCachedPxWebConnection;
        private Mock<ILogger<CreationController>> _mockLogger;
        private CreationController _controller;
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void DoSetup()
        {
            _mockCachedPxWebConnection = new Mock<ICachedPxWebConnection>();
            _mockLogger = new Mock<ILogger<CreationController>>();
            _controller = new CreationController(_mockCachedPxWebConnection.Object, _mockLogger.Object);

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(_configuration);

            Configuration.Current.LanguageOptions.Available = ["foo", "bar"];
            Configuration.Current.LanguageOptions.Default = "foo";
        }

        [Test]
        public async Task GetDataBaseListingAsync_NoDbPath_ReturnsDatabasesForAllLanguages()
        {
            // Arrange
            List<DataBaseListResponseItem> primaryLanguageDatabases = [new() { Dbid = "dbid1", Text = "foobar1" }];
            List<DataBaseListResponseItem> otherDatabases = [
                new DataBaseListResponseItem { Dbid = "dbid1", Text = "foobar1" },
                new DataBaseListResponseItem { Dbid = "dbid2", Text = "foobar2" }
            ];

            _mockCachedPxWebConnection.Setup(x => x.GetDataBaseListingAsync("foo")).ReturnsAsync(primaryLanguageDatabases);
            _mockCachedPxWebConnection.Setup(x => x.GetDataBaseListingAsync("bar")).ReturnsAsync(otherDatabases);

            // Act
            var result = await _controller.GetDataBaseListingAsync(null, new Dictionary<string, string> { { "lang", "foo" } });

            // Assert
            Assert.IsTrue(result.Exists(r => r.Id == "dbid1" && r.Languages.Count == 2));
            Assert.IsTrue(result.Exists(r => r.Id == "dbid2" && r.Languages.Count == 1));
        }

        [Test]
        public async Task GetDatabaseListingAsync_WithPath_ReturnsTablesForAllLanguages()
        {
            // Arrange
            string mockPath = "path/db";
            List<string> mockPathList = [.. mockPath.Split("/")];
            List<TableListResponseItem> primaryLanguageTables = [
                new() { Id = "id1", Text = "foobar1" },
                new() { Id = "id2", Text = "foobar2" }
            ];

            List<TableListResponseItem> otherTables = [
                new TableListResponseItem { Id = "id1", Text = "foobar1" },
                new TableListResponseItem { Id = "id2", Text = "foobar2" },
                new TableListResponseItem { Id = "id3", Text = "foobar3" }
            ];

            _mockCachedPxWebConnection.Setup(x => x.GetDataTableItemListingAsync("foo", mockPathList)).ReturnsAsync(primaryLanguageTables);
            _mockCachedPxWebConnection.Setup(x => x.GetDataTableItemListingAsync("bar", mockPathList)).ReturnsAsync(otherTables);

            // Act
            var result = await _controller.GetDataBaseListingAsync(mockPath, new Dictionary<string, string> { { "lang", "foo" } });

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Exists(r => r.Id == "id1" && r.Languages.Count == 2));
            Assert.IsTrue(result.Exists(r => r.Id == "id2" && r.Languages.Count == 2));
            Assert.IsTrue(result.Exists(r => r.Id == "id3" && r.Languages.Count == 1));
        }

        [Test]
        public async Task GetDataBaseListingAsync_WithoutPreferredLanguage_ReturnsDataBasesPreferringDefaultLanguage()
        {
            // Arrange
            List<DataBaseListResponseItem> primaryLanguageDatabases = [new() { Dbid = "dbid1", Text = "foobar1" }];
            List<DataBaseListResponseItem> otherDatabases = [
                new() { Dbid = "dbid1", Text = "foobar1" },
                new() { Dbid = "dbid2", Text = "foobar2"}
            ];

            _mockCachedPxWebConnection.Setup(x => x.GetDataBaseListingAsync("foo")).ReturnsAsync(primaryLanguageDatabases);
            _mockCachedPxWebConnection.Setup(x => x.GetDataBaseListingAsync("bar")).ReturnsAsync(otherDatabases);

            // Act
            var result = await _controller.GetDataBaseListingAsync(null, new Dictionary<string, string> { { "param", "baz" } });

            // Assert
            Assert.IsTrue(result.Exists(r => r.Id == "dbid1" && r.Languages.Count == 2));
            Assert.IsTrue(result.Exists(r => r.Id == "dbid2" && r.Languages.Count == 1));
        }
    }
}

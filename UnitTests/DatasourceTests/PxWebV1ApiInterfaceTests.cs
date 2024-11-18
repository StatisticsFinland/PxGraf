#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using UnitTests.Fixtures;
using PxGraf.Datasource.ApiDatasource;

namespace UnitTests.DatasourceTests
{
    internal class PxWebV1ApiInterfaceTests
    {
        private IConfiguration? _configuration;

        [OneTimeSetUp]
        public void DoSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(_configuration);

            Configuration.Current.LanguageOptions.Available = ["en", "fi", "sv"];
            Configuration.Current.LanguageOptions.Default = "fi";
        }

        [Test]
        public async Task GetDatabaseItemGroup_WithEmptyHierarchy()
        {
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);

            HttpResponseMessage mockEnResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarEn\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/en/", "")).ReturnsAsync(mockEnResponse);

            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarFi\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/", "")).ReturnsAsync(mockFiResponse);

            HttpResponseMessage mockSvResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarSv\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/sv/", "")).ReturnsAsync(mockSvResponse);

            DatabaseGroupContents dbContent = await objectUnderTest.GetDatabaseItemGroup([]);

            Assert.That(dbContent.Headers.Count, Is.EqualTo(1));
            Assert.That(dbContent.Files.Count, Is.EqualTo(0));
            Assert.That(dbContent.Headers[0].Name["en"], Is.EqualTo("FooBarEn"));
            Assert.That(dbContent.Headers[0].Name["fi"], Is.EqualTo("FooBarFi"));
            Assert.That(dbContent.Headers[0].Name["sv"], Is.EqualTo("FooBarSv"));
        }

        [Test]
        public async Task GetDatabaseItemGroup_WithHierarchy()
        {
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);

            HttpResponseMessage mockEnResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"id\":\"FooBar\", \"text\":\"FooBarEn\", \"type\":\"t\", \"updated\":\"2024-08-24T13:15:00.000\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/en/database/subgroup/folder/", "")).ReturnsAsync(mockEnResponse);

            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"id\":\"FooBar\", \"text\":\"FooBarFi\", \"type\":\"t\", \"updated\":\"2024-08-24T13:15:00.000\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/database/subgroup/folder/", "")).ReturnsAsync(mockFiResponse);

            HttpResponseMessage mockSvResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"id\":\"FooBar\", \"text\":\"FooBarSv\", \"type\":\"t\", \"updated\":\"2024-08-24T13:15:00.000\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/sv/database/subgroup/folder/", "")).ReturnsAsync(mockSvResponse);

            DatabaseGroupContents dbContent = await objectUnderTest.GetDatabaseItemGroup(["database", "subgroup", "folder"]);

            Assert.That(dbContent.Headers.Count, Is.EqualTo(0));
            Assert.That(dbContent.Files.Count, Is.EqualTo(1));
            Assert.That(dbContent.Files[0].Name, Is.Not.Null);
            Assert.That(dbContent.Files[0].Name!["en"], Is.EqualTo("FooBarEn"));
            Assert.That(dbContent.Files[0].Name!["fi"], Is.EqualTo("FooBarFi"));
            Assert.That(dbContent.Files[0].Name!["sv"], Is.EqualTo("FooBarSv"));
        }

        [Test]
        public async Task GetDatabaseItemGroup_WithHierarchy_ReturnsHeaders()
        {
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);

            HttpResponseMessage mockEnResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarEn\", \"type\":\"l\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/en/database/", "")).ReturnsAsync(mockEnResponse);

            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarFi\", \"type\":\"l\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/database/", "")).ReturnsAsync(mockFiResponse);

            HttpResponseMessage mockSvResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"dbid\":\"FooBar\", \"text\":\"FooBarSv\", \"type\":\"l\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/sv/database/", "")).ReturnsAsync(mockSvResponse);

            DatabaseGroupContents dbContent = await objectUnderTest.GetDatabaseItemGroup(["database"]);

            Assert.That(dbContent.Headers.Count, Is.EqualTo(1));
            Assert.That(dbContent.Files.Count, Is.EqualTo(0));
            Assert.That(dbContent.Headers[0].Name["en"], Is.EqualTo("FooBarEn"));
            Assert.That(dbContent.Headers[0].Name["fi"], Is.EqualTo("FooBarFi"));
            Assert.That(dbContent.Headers[0].Name["sv"], Is.EqualTo("FooBarSv"));
        }

        [Test]
        public void GetLastWriteTime_ReturnsNotSupported()
        {
            // Arrange
            PxTableReference tableReference = new("mock/table/reference/", '/');

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => PxWebV1ApiInterface.GetLastWriteTime(tableReference));
        }

        [Test]
        public async Task GetLastWriteTimeAsyncTest()
        {
            // Arrange
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);
            PxTableReference tableReference = new("mock/table/reference/FooBar", '/');
            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent("[{ \"id\":\"FooBar\", \"text\":\"FooBarFi\", \"type\":\"t\", \"updated\":\"2024-08-24T13:15:00.000\" }]")
            };
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/mock/table/reference/", "")).ReturnsAsync(mockFiResponse);
            DateTime expectedLastWriteTime = new(2024, 8, 24, 13, 15, 0, DateTimeKind.Local);

            // Act
            DateTime lastWriteTime = await objectUnderTest.GetLastWriteTimeAsync(tableReference);

            // Assert
            Assert.That(lastWriteTime, Is.EqualTo(expectedLastWriteTime));
        }

        [Test]
        public void GetMatrixMetadata_ReturnsNotSupported()
        {
            // Arrange
            PxTableReference tableReference = new("mock/table/reference");

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => PxWebV1ApiInterface.GetMatrixMetadata(tableReference));
        }

        [Test]
        public async Task GetMatrixMetadataAsyncTest()
        {
            // Arrange
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);
            PxTableReference tableReference = new("mock/table/reference/FooBar", '/');

            HttpResponseMessage mockEnResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockEnContent)
            };
            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockFiContent)
            };
            HttpResponseMessage mockSvResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockSvContent)
            };

            mockConnection.Setup(mc => mc.GetAsync("api/v1/en/mock/table/reference/FooBar", "")).ReturnsAsync(mockEnResponse);
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/mock/table/reference/FooBar", "")).ReturnsAsync(mockFiResponse);
            mockConnection.Setup(mc => mc.GetAsync("api/v1/sv/mock/table/reference/FooBar", "")).ReturnsAsync(mockSvResponse);

            HttpResponseMessage mockJsonStatResponseEn = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockEnJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/en/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseEn);

            HttpResponseMessage mockJsonStatResponseFi = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockFiJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/fi/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseFi);

            HttpResponseMessage mockJsonStatResponseSv = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockSvJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/sv/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseSv);

            IEnumerable<string> expectedLangs = new[] { "en", "fi", "sv" };
            Dictionary<string, string> expectedUnitValues = new()
            {
                ["en"] = "unit.en",
                ["fi"] = "unit",
                ["sv"] = "unit.sv"
            };
            MultilanguageString expectedUnitName = new(expectedUnitValues);

            // Act
            IReadOnlyMatrixMetadata matrixMetadata = await objectUnderTest.GetMatrixMetadataAsync(tableReference);
            ContentDimensionValue cdv1 = (ContentDimensionValue)matrixMetadata.Dimensions[0].Values[0];

            // Assert
            Assert.That(matrixMetadata.AvailableLanguages, Is.EquivalentTo(expectedLangs));
            Assert.That(matrixMetadata.DefaultLanguage, Is.EqualTo("fi"));
            Assert.That(matrixMetadata.Dimensions.Count, Is.EqualTo(2));
            Assert.That(matrixMetadata.Dimensions[0].Code, Is.EqualTo("variable-0"));
            Assert.That(matrixMetadata.Dimensions[1].Code, Is.EqualTo("variable-1"));
            Assert.That(matrixMetadata.Dimensions[0].Name["en"], Is.EqualTo("variable-0-text.en"));
            Assert.That(matrixMetadata.Dimensions[0].Name["fi"], Is.EqualTo("variable-0-text"));
            Assert.That(matrixMetadata.Dimensions[0].Name["sv"], Is.EqualTo("variable-0-text.sv"));
            Assert.That(matrixMetadata.Dimensions[0].Values.Count, Is.EqualTo(2));
            Assert.That(matrixMetadata.Dimensions[0].Type, Is.EqualTo(DimensionType.Content));
            Assert.That(matrixMetadata.Dimensions[0].Values[0].Code, Is.EqualTo("value-0"));
            Assert.That(matrixMetadata.Dimensions[0].Values[1].Code, Is.EqualTo("value-1"));
            Assert.That(matrixMetadata.Dimensions[0].Values[0].Name["en"], Is.EqualTo("value-0-text.en"));
            Assert.That(matrixMetadata.Dimensions[0].Values[1].Name["en"], Is.EqualTo("value-1-text.en"));
            Assert.That(matrixMetadata.Dimensions[0].Values[0].Name["fi"], Is.EqualTo("value-0-text"));
            Assert.That(matrixMetadata.Dimensions[0].Values[1].Name["fi"], Is.EqualTo("value-1-text"));
            Assert.That(matrixMetadata.Dimensions[0].Values[0].Name["sv"], Is.EqualTo("value-0-text.sv"));
            Assert.That(matrixMetadata.Dimensions[0].Values[1].Name["sv"], Is.EqualTo("value-1-text.sv"));
            Assert.That(cdv1.Unit.Equals(expectedUnitName));
            Assert.That(matrixMetadata.Dimensions[1].Name["en"], Is.EqualTo("variable-1-text.en"));
            Assert.That(matrixMetadata.Dimensions[1].Name["fi"], Is.EqualTo("variable-1-text"));
            Assert.That(matrixMetadata.Dimensions[1].Name["sv"], Is.EqualTo("variable-1-text.sv"));
            Assert.That(matrixMetadata.Dimensions[1].Values.Count, Is.EqualTo(2));
            Assert.That(matrixMetadata.Dimensions[1].Type, Is.EqualTo(DimensionType.Time));
            Assert.That(matrixMetadata.Dimensions[1].Values[0].Code, Is.EqualTo("2000"));
            Assert.That(matrixMetadata.Dimensions[1].Values[1].Code, Is.EqualTo("2001"));
            Assert.That(matrixMetadata.Dimensions[1].Values[0].Name["en"], Is.EqualTo("2000"));
            Assert.That(matrixMetadata.Dimensions[1].Values[1].Name["en"], Is.EqualTo("2001"));
            Assert.That(matrixMetadata.Dimensions[1].Values[0].Name["fi"], Is.EqualTo("2000"));
            Assert.That(matrixMetadata.Dimensions[1].Values[1].Name["fi"], Is.EqualTo("2001"));
            Assert.That(matrixMetadata.Dimensions[1].Values[0].Name["sv"], Is.EqualTo("2000"));
            Assert.That(matrixMetadata.Dimensions[1].Values[1].Name["sv"], Is.EqualTo("2001"));
        }

        [Test]
        public void GetMatrix_ReturnsNotSupported()
        {
            // Arrange
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 2),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 1),
                new(DimensionType.Other, 1),
            ];
            PxTableReference tableReference = new("mock/table/reference");
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => PxWebV1ApiInterface.GetMatrix(tableReference, meta));
        }

        [Test]
        public async Task GetMatrixAsyncTest()
        {
            // Arrange
            List<DimensionParameters> varParams =
            [
               new(DimensionType.Content, 2),
               new(DimensionType.Time, 2)
            ];
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            Mock<IPxWebConnection> mockConnection = new();
            PxWebV1ApiInterface objectUnderTest = CreateInterface(mockConnection);
            PxTableReference tableReference = new("mock/table/reference/FooBar", '/');

            HttpResponseMessage mockEnResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockEnContent)
            };
            HttpResponseMessage mockFiResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockFiContent)
            };
            HttpResponseMessage mockSvResponse = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockSvContent)
            };

            mockConnection.Setup(mc => mc.GetAsync("api/v1/en/mock/table/reference/FooBar", "")).ReturnsAsync(mockEnResponse);
            mockConnection.Setup(mc => mc.GetAsync("api/v1/fi/mock/table/reference/FooBar", "")).ReturnsAsync(mockFiResponse);
            mockConnection.Setup(mc => mc.GetAsync("api/v1/sv/mock/table/reference/FooBar", "")).ReturnsAsync(mockSvResponse);

            HttpResponseMessage mockJsonStatResponseEn = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockEnJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/en/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseEn);

            HttpResponseMessage mockJsonStatResponseFi = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockFiJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/fi/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseFi);

            HttpResponseMessage mockJsonStatResponseSv = new(HttpStatusCode.OK)
            {
                Content = new StringContent(PxWebV1ApiInterfaceFixtures.MockSvJsonStat2)
            };
            mockConnection.Setup(mc => mc.PostAsync("api/v1/sv/mock/table/reference/FooBar", It.IsAny<string>())).ReturnsAsync(mockJsonStatResponseSv);
            decimal[] expectedData = [1.0m, 2.0m, 3.0m, 4.0m];

            // Act
            Matrix<DecimalDataValue> matrix = await objectUnderTest.GetMatrixAsync(tableReference, meta);

            // Assert
            Assert.That(matrix, Is.InstanceOf<Matrix<DecimalDataValue>>());
            decimal[] decimals = matrix.Data.Select(dv => dv.UnsafeValue).ToArray();
            Assert.That(decimals, Is.EqualTo(expectedData));
        }

        private static PxWebV1ApiInterface CreateInterface(Mock<IPxWebConnection>? mockConnection = null)
        {
            mockConnection ??= new();
            Mock<ILogger<PxWebV1ApiInterface>> mockLogger = new();
            return new PxWebV1ApiInterface(mockConnection.Object, mockLogger.Object);
        }
    }
}

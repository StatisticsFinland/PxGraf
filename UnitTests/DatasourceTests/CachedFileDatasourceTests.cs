#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnitTests.Fixtures;

namespace UnitTests.DatasourceTests
{
    public class CachedFileDatasourceTests
    {
        private readonly List<string> _languages = ["fi", "en", "sv"];
        private IConfiguration? _configuration;

        private readonly List<PxTableReference> _fileReferences =
        [
            new("database\\subgroup\\folder\\table_1.px"),
            new("database\\subgroup\\folder\\table_2.px"),
            new("database\\subgroup\\folder\\table_3.px")
        ];

        private readonly Dictionary<string, string> _table1Name = new()
        {
            ["fi"] = "Table1",
            ["en"] = "Table1.en",
            ["sv"] = "Table1.sv"
        };

        private readonly Dictionary<string, string> _table2Name = new()
        {
            ["fi"] = "Table2",
            ["en"] = "Table2.en",
            ["sv"] = "Table2.sv"
        };

        private readonly DateTime _lastUpdated = new(2009, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly List<DimensionParameters> _tableParams =
        [
            new(DimensionType.Content, 1),
            new(DimensionType.Time, 10),
            new(DimensionType.Other, 5),
            new(DimensionType.Other, 6),
        ];

        [OneTimeSetUp]
        public void DoSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(_configuration);

            Configuration.Current.LanguageOptions.Available = _languages;
            Configuration.Current.LanguageOptions.Default = "fi";
        }

        [Test]
        public async Task GetGroupContentsAsyncEmptyHierarchyReturnsGroupHeaders()
        {
            // Arrange
            Dictionary<string, string> header1Names = new()
            {
                ["fi"] = "Header",
                ["en"] = "Header.en",
                ["sv"] = "Header.sv"
            };
            Dictionary<string, string> header2Names = new()
            {
                ["fi"] = "Header2",
                ["en"] = "Header2.en",
                ["sv"] = "Header2.sv"
            };
            List<DatabaseGroupHeader> expectedHeaders =
            [
                new("header1", _languages, new(header1Names)),
                new("header2", _languages, new(header2Names))
            ];
            CachedFileDatasource datasource = BuildDatasource(expectedHeaders, [], []);

            // Act
            DatabaseGroupContents contents = await datasource.GetGroupContentsCachedAsync([]);

            // Assert
            Assert.That(contents.Headers, Is.EqualTo(expectedHeaders));
        }

        [Test]
        public async Task GetGroupContentAsyncWithHierarchyReturnsFiles()
        {
            // Arrange
            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            List<DimensionParameters> noContent =
            [
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 5),
            ];

            MatrixMetadata brokenMeta = TestDataCubeBuilder.BuildTestMeta(noContent, [.. _languages]);

            StringProperty table1IDProperty = new("table1-id");
            table1Meta.AdditionalProperties[PxSyntaxConstants.TABLEID_KEY] = table1IDProperty; // Adds TABLE_ID property for table1
            table1Meta.AdditionalProperties[PxSyntaxConstants.DESCRIPTION_KEY] = new MultilanguageStringProperty(new MultilanguageString(_table1Name));
            table2Meta.AdditionalProperties[PxSyntaxConstants.DESCRIPTION_KEY] = new MultilanguageStringProperty(new MultilanguageString(_table2Name));
            Dictionary<PxTableReference, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0]] = table1Meta,
                [_fileReferences[1]] = table2Meta,
                [_fileReferences[2]] = brokenMeta
            };

            CachedFileDatasource datasource = BuildDatasource([], _fileReferences, metadataResponses);

            // Act
            DatabaseGroupContents contents = await datasource.GetGroupContentsCachedAsync(["database", "subgroup", "folder"]);

            // Assert
            Assert.That(contents.Files.Count, Is.EqualTo(3));

            DatabaseTable file0 = contents.Files[0];
            Assert.That(file0.Code, Is.EqualTo("table1-id"));
            Assert.That(file0.Name["fi"], Is.EqualTo("Table1"));
            Assert.That(file0.Name["en"], Is.EqualTo("Table1.en"));
            Assert.That(file0.Name["sv"], Is.EqualTo("Table1.sv"));
            Assert.That(file0.LastUpdated, Is.EqualTo(_lastUpdated));

            DatabaseTable file1 = contents.Files[1];
            Assert.That(file1.Code, Is.EqualTo("table_2"));
            Assert.That(file1.Name["fi"], Is.EqualTo("Table2"));
            Assert.That(file1.Name["en"], Is.EqualTo("Table2.en"));
            Assert.That(file1.Name["sv"], Is.EqualTo("Table2.sv"));
            Assert.That(file1.LastUpdated, Is.EqualTo(_lastUpdated));

            DatabaseTable file2 = contents.Files[2];
            Assert.That(file2.Code, Is.EqualTo("table_3"));
            Assert.That(file2.LastUpdated, Is.Null);
            Assert.That(file2.Error, Is.True);
        }

        [Test]
        public async Task GetGroupContentAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            List<DatabaseGroupHeader> headers =
            [
                new("header1", _languages, new(new Dictionary<string, string> { ["fi"] = "Header1.fi", ["sv"] = "Header1.sv", ["en"] = "Header1.en"})),
                new("header2", _languages, new(new Dictionary<string, string> { ["fi"] = "Header2.fi", ["sv"] = "Header2.sv", ["en"] = "Header2.en"}))
            ];


            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            List<DimensionParameters> noContent =
            [
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 5),
            ];

            MatrixMetadata brokenMeta = TestDataCubeBuilder.BuildTestMeta(noContent, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = table1Meta,
                [_fileReferences[1].Name] = table2Meta,
                [_fileReferences[2].Name] = brokenMeta
            };

            mockSource.Setup(d => d.GetGroupHeadersAsync(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(headers);
            mockSource.Setup(d => d.GetTablesAsync(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(_fileReferences);
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            DatabaseGroupContents call1 = await datasource.GetGroupContentsCachedAsync([]);
            DatabaseGroupContents call2 = await datasource.GetGroupContentsCachedAsync([]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            mockSource.Verify(d => d.GetGroupHeadersAsync(It.IsAny<IReadOnlyList<string>>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixMetadataAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            List<DimensionParameters> noContent =
            [
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 5),
            ];

            MatrixMetadata brokenMeta = TestDataCubeBuilder.BuildTestMeta(noContent, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = table1Meta,
                [_fileReferences[1].Name] = table2Meta,
                [_fileReferences[2].Name] = brokenMeta
            };

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            IReadOnlyMatrixMetadata call1 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);
            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            Assert.That(call1, Is.EqualTo(table1Meta));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixMetadataAsyncStaleMetaShouldCheckForUpdates()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = table1Meta,
                [_fileReferences[1].Name] = table2Meta,
            };

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));
            DateTime lastUpdate = new(2020, 10, 10, 8, 00, 00, DateTimeKind.Utc);
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(lastUpdate);

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            IReadOnlyMatrixMetadata call1 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            await Task.Delay(2);

            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            Assert.That(call1, Is.EqualTo(table1Meta));
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
        }

        [Test]
        public void GetMatrixMetadataMetadataFetchTaskThatCausesErrorShouldBeRefetched()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .ThrowsAsync(new Exception("test exception"));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async() => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[2]));
            Assert.ThrowsAsync<Exception>(async() => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[2]));

            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetMatrixMetadataShouldRecoverFromStaleStateIfGetLastWriteTimeThrows()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = table1Meta,
                [_fileReferences[1].Name] = table2Meta,
            };

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ThrowsAsync(new Exception("test exception"));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            await Task.Delay(2);

            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]));

            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(new DateTime(2020, 10, 10, 8, 00, 00, DateTimeKind.Utc));
            await Task.Delay(2);

            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call2, Is.EqualTo(table1Meta));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Exactly(1));
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetMatrixCachedAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()))
                .ReturnsAsync((PxTableReference _, IReadOnlyMatrixMetadata metadata, IMatrixMap _, CancellationToken? _) => new Matrix<DecimalDataValue>(metadata, []));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            Matrix<DecimalDataValue> call1 = await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);
            Matrix<DecimalDataValue> call2 = await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);

            // Assert
            Assert.That(call1.Metadata, Is.EqualTo(call2.Metadata));
            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixCachedAsyncStaleDataShouldTriggerRevalidation()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(new DateTime(2020, 10, 10, 8, 00, 00, DateTimeKind.Utc));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()))
                .ReturnsAsync((PxTableReference _, IReadOnlyMatrixMetadata metadata, IMatrixMap _, CancellationToken? _) => new Matrix<DecimalDataValue>(metadata, []));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act
            await Task.Delay(2);
            await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);

            // Assert
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()), Times.Once);
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(2)); // Once for meta, once for data
        }

        [Test]
        public void GetMatrixCachedAsyncExceptionShouldTriggerRefetchForNextCall()
        {
            Mock<IFileDatasource> mockSource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()))
                .ThrowsAsync(new Exception("test exception"));

            using MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedFileDatasource datasource = new(mockSource.Object, taskCache, logger.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta));
            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta));

            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()), Times.Exactly(2));
        }

        private static CachedFileDatasource BuildDatasource(
            List<DatabaseGroupHeader> headers,
            List<PxTableReference> fileReferences,
            Dictionary<PxTableReference, IReadOnlyMatrixMetadata> metadataResponses,
            Matrix<DecimalDataValue>? expectedData = null)
        {
            Mock<IFileDatasource> datasource = new();
            Mock<ILogger<CachedFileDatasource>> logger = new();

            datasource.Setup(d => d.GetGroupHeadersAsync(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(headers);
            datasource.Setup(d => d.GetTablesAsync(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(fileReferences);
            datasource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference]));
            datasource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<IMatrixMap>(), It.IsAny<CancellationToken?>()))
                .Returns((PxTableReference reference, IReadOnlyMatrixMetadata meta, IMatrixMap map, CancellationToken? cancellationToken) =>
                {
                    return Task.FromResult(expectedData);
                });

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));

            return new CachedFileDatasource(datasource.Object, taskCache, logger.Object);
        }
    }
}
#nullable enable
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Datasource.ApiDatasource;
using PxGraf.Datasource.Cache;
using PxGraf.Datasource.PxWebInterface;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnitTests.Fixtures;

namespace UnitTests.DatasourceTests
{
    public class CachedApiDatasourceTests
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
            List<DatabaseGroupHeader> headers =
            [
                new("header1", _languages, new(new Dictionary<string, string> { ["fi"] = "Header1.fi", ["sv"] = "Header1.sv", ["en"] = "Header1.en"})),
                new("header2", _languages, new(new Dictionary<string, string> { ["fi"] = "Header2.fi", ["sv"] = "Header2.sv", ["en"] = "Header2.en"}))
            ];

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetDatabaseItemGroup(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(new DatabaseGroupContents(headers, []));
            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            DatabaseGroupContents contents = await datasource.GetGroupContentsCachedAsync([]);

            // Assert
            Assert.That(contents.Files.Count, Is.EqualTo(0));
            Assert.That(contents.Headers, Is.EqualTo(headers));
        }

        [Test]
        public async Task GetGroupContentAsyncWithHierarchyReturnsFiles()
        {
            // Arrange
            List<DatabaseTable> mockTables =
                [
                    new("table1-id", new MultilanguageString(_table1Name), _lastUpdated, _languages),
                    new("table2-id", new MultilanguageString(_table2Name), _lastUpdated, _languages),
                ];

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetDatabaseItemGroup(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(new DatabaseGroupContents([], mockTables));
            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            DatabaseGroupContents contents = await datasource.GetGroupContentsCachedAsync(["database", "subgroup", "folder"]);

            // Assert
            Assert.That(contents.Files.Count, Is.EqualTo(2));
            Assert.That(contents.Headers.Count, Is.EqualTo(0));

            DatabaseTable file0 = contents.Files[0];
            Assert.That(file0.Code, Is.EqualTo("table1-id"));
            Assert.That(file0.Name["fi"], Is.EqualTo("Table1"));
            Assert.That(file0.Name["en"], Is.EqualTo("Table1.en"));
            Assert.That(file0.Name["sv"], Is.EqualTo("Table1.sv"));
            Assert.That(file0.LastUpdated, Is.EqualTo(_lastUpdated));

            DatabaseTable file1 = contents.Files[1];
            Assert.That(file1.Code, Is.EqualTo("table2-id"));
            Assert.That(file1.Name["fi"], Is.EqualTo("Table2"));
            Assert.That(file1.Name["en"], Is.EqualTo("Table2.en"));
            Assert.That(file1.Name["sv"], Is.EqualTo("Table2.sv"));
            Assert.That(file1.LastUpdated, Is.EqualTo(_lastUpdated));
        }

        [Test]
        public async Task GetGroupContentAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            // Arrange
            List<DatabaseGroupHeader> headers =
            [
                new("header1", _languages, new(new Dictionary<string, string> { ["fi"] = "Header1.fi", ["sv"] = "Header1.sv", ["en"] = "Header1.en"})),
                new("header2", _languages, new(new Dictionary<string, string> { ["fi"] = "Header2.fi", ["sv"] = "Header2.sv", ["en"] = "Header2.en"}))
            ];

            List<DatabaseTable> mockTables =
                [
                    new("table1-id", new MultilanguageString(_table1Name), _lastUpdated, _languages),
                    new("table2-id", new MultilanguageString(_table2Name), _lastUpdated, _languages),
                ];

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetDatabaseItemGroup(It.IsAny<IReadOnlyList<string>>())).ReturnsAsync(new DatabaseGroupContents(headers, mockTables));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            DatabaseGroupContents call1 = await datasource.GetGroupContentsCachedAsync([]);
            DatabaseGroupContents call2 = await datasource.GetGroupContentsCachedAsync([]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            mockSource.Verify(d => d.GetDatabaseItemGroup(It.IsAny<IReadOnlyList<string>>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixMetadataAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            Mock<IApiDatasource> mockSource = new();

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
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            IReadOnlyMatrixMetadata call1 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);
            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            Assert.That(call1, Is.EqualTo(table1Meta));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixMetadataAsyncShouldHandleMetaWithMissingContentVariable()
        {
            // Arrange
            List<DimensionParameters> noContent =
            [
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 5),
            ];

            MatrixMetadata brokenMeta = TestDataCubeBuilder.BuildTestMeta(noContent, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = brokenMeta
            };

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(_lastUpdated);

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            IReadOnlyMatrixMetadata call1 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            await Task.Delay(2);

            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            Assert.That(call1, Is.EqualTo(brokenMeta));
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixMetadataAsyncStaleMetaShouldCheckForUpdates()
        {
            // Arrange
            MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
            MatrixMetadata table2Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = table1Meta,
                [_fileReferences[1].Name] = table2Meta,
            };

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(_lastUpdated);

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            IReadOnlyMatrixMetadata call1 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]); // Null state

            await Task.Delay(2);

            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]); // Stale state

            // Assert
            Assert.That(call1, Is.EqualTo(call2));
            Assert.That(call1, Is.EqualTo(table1Meta));
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(1));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Once);
        }

        [Test]
        public void GetMatrixMetadataMetadataFetchTaskThatCausesErrorShouldBeRefetched()
        {
            Mock<IApiDatasource> mockSource = new();

            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .ThrowsAsync(new Exception("test exception"));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async() => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[2]));
            Assert.ThrowsAsync<Exception>(async() => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[2]));

            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetMatrixMetadataShouldRecoverFromStaleStateIfGetLastWriteTimeThrows()
        {
            // Arrange
            List<DimensionParameters> noContent =
            [
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 5),
            ];

            MatrixMetadata brokenMeta = TestDataCubeBuilder.BuildTestMeta(noContent, [.. _languages]);

            Dictionary<string, IReadOnlyMatrixMetadata> metadataResponses = new()
            {
                [_fileReferences[0].Name] = brokenMeta
            };

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(metadataResponses[reference.Name]));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ThrowsAsync(new Exception("test exception"));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            await Task.Delay(2);
            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]));

            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(new DateTime(2020, 10, 10, 8, 00, 00, DateTimeKind.Utc));
            await Task.Delay(2);

            IReadOnlyMatrixMetadata call2 = await datasource.GetMatrixMetadataCachedAsync(_fileReferences[0]);

            // Assert
            Assert.That(call2, Is.EqualTo(brokenMeta));
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetMatrixCachedAsyncShouldUseCachingAndOnlyDoOneCallToSource()
        {
            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()))
                .ReturnsAsync((PxTableReference _, IReadOnlyMatrixMetadata metadata, CancellationToken? _) => new Matrix<DecimalDataValue>(metadata, []));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            Matrix<DecimalDataValue> call1 = await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);
            Matrix<DecimalDataValue> call2 = await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);

            // Assert
            Assert.That(call1.Metadata, Is.EqualTo(call2.Metadata));
            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()), Times.Once);
        }

        [Test]
        public async Task GetMatrixCachedAsyncStaleDataShouldTriggerRevalidation()
        {
            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>())).ReturnsAsync(new DateTime(2020, 10, 10, 8, 00, 00, DateTimeKind.Utc));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()))
                .ReturnsAsync((PxTableReference _, IReadOnlyMatrixMetadata metadata,CancellationToken? _) => new Matrix<DecimalDataValue>(metadata, []));

            MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMilliseconds(1));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act
            await Task.Delay(2);
            await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta);

            // Assert
            mockSource.Verify(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()), Times.Never);
            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()), Times.Once);
            mockSource.Verify(d => d.GetLastWriteTimeAsync(It.IsAny<PxTableReference>()), Times.Never);
        }

        [Test]
        public void GetMatrixCachedAsyncExceptionShouldTriggerRefetchForNextCall()
        {
            // Arrange
            IReadOnlyMatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);

            Mock<IApiDatasource> mockSource = new();
            mockSource.Setup(d => d.GetMatrixMetadataAsync(It.IsAny<PxTableReference>()))
                .Returns((PxTableReference reference) => Task.FromResult(table1Meta));
            mockSource.Setup(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()))
                .ThrowsAsync(new Exception("test exception"));

            using MultiStateMemoryTaskCache taskCache = new(10, TimeSpan.FromMinutes(10));
            CachedApiDatasource datasource = new(mockSource.Object, taskCache);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta));
            Assert.ThrowsAsync<Exception>(async () => await datasource.GetMatrixCachedAsync(_fileReferences[0], table1Meta));

            mockSource.Verify(d => d.GetMatrixAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>(), It.IsAny<CancellationToken?>()), Times.Exactly(2));
        }
    }
}


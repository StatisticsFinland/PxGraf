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
using PxGraf.Datasource;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;
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

        private readonly DateTime _lastUpdated = new(2009, 9, 1, 0, 0, 0, DateTimeKind.Local);
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
        public async Task GetGroupContentsAsync_EmptyHierarchy_ReturnsGroupHeaders()
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
        public async Task GetGroupContentAsync_WithHierarchy_ReturnsFiles()
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
            Assert.That(file2.Code, Is.EqualTo("table_3.px"));
            Assert.That(file2.LastUpdated, Is.Null);
            Assert.That(file2.Error, Is.True);
        }

        [Test]
    public async Task GetMatrixAsync_ReturnsMatrix()
    {
        // Arrange
        MatrixMetadata table1Meta = TestDataCubeBuilder.BuildTestMeta(_tableParams, [.. _languages]);
        table1Meta.AdditionalProperties[PxSyntaxConstants.DESCRIPTION_KEY] = new MultilanguageStringProperty(new MultilanguageString(_table1Name));
        Dictionary<PxTableReference, IReadOnlyMatrixMetadata> metadataResponses = new()
        {
            [_fileReferences[0]] = table1Meta,
        };
        Matrix<DecimalDataValue> expectedData = TestDataCubeBuilder.BuildTestMatrix(_tableParams);
        CachedFileDatasource datasource = BuildDatasource([], _fileReferences, metadataResponses, expectedData);

        // Act
        Matrix<DecimalDataValue> matrix = await datasource.GetMatrixAsync(_fileReferences[0], table1Meta);

        // Assert
        Assert.That(matrix.Data, Is.EqualTo(expectedData.Data));
    }

    private static CachedFileDatasource BuildDatasource(
        List<DatabaseGroupHeader> headers,
        List<PxTableReference> fileReferences,
        Dictionary<PxTableReference, IReadOnlyMatrixMetadata> metadataResponses,
        Matrix<DecimalDataValue>? expectedData = null)
    {
        Mock<IFileDatasource> datasource = new();
        Mock<IMultiStateMemoryTaskCache> taskCache = new();
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

        taskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<DatabaseGroupContents>>.IsAny))
            .Returns((string key, out Task<DatabaseGroupContents> value) =>
            {
                value = Task.FromResult(new DatabaseGroupContents(headers, []));
                return MultiStateMemoryTaskCache.CacheEntryState.Null;
            });

        taskCache.Setup(x => x.TryGet(It.IsAny<string>(), out It.Ref<Task<CachedDatasource.MetaCacheHousing>>.IsAny))
            .Returns((string key, out Task<CachedDatasource.MetaCacheHousing> value) =>
            {
                PxTableReference reference = new(key, '-');
                IReadOnlyMatrixMetadata meta = metadataResponses[metadataResponses.Keys.First(k => k.Name == reference.Name)];
                DateTime lastUpdated = meta.GetLastUpdated() ?? throw new ArgumentException("Failed to get lastWriteTime, the test should fail.");
                value = Task.FromResult(new CachedDatasource.MetaCacheHousing(lastUpdated, meta));
                return MultiStateMemoryTaskCache.CacheEntryState.Null;
            });

        return new CachedFileDatasource(datasource.Object, taskCache.Object, logger.Object);
    }
}
}

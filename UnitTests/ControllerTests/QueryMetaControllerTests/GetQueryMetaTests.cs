using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.QueryMetaControllerTests
{
    internal class GetQueryMetaTests
    {
        private Mock<ISqFileInterface> _sqFileInterface;
        private Mock<ICachedDatasource> _cachedDatasource;
        private Mock<ILogger<QueryMetaController>> _logger;
        private Mock<IAuditLogService> _auditLogService;
        
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> inMemorySettings = new()
            {
                {"pxwebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [SetUp]
        public void Setup()
        {
            _sqFileInterface = new Mock<ISqFileInterface>();
            _cachedDatasource = new Mock<ICachedDatasource>();
            _logger = new Mock<ILogger<QueryMetaController>>();
            _auditLogService = new Mock<IAuditLogService>();
        }

        private QueryMetaController CreateController()
        {
            return new QueryMetaController(_sqFileInterface.Object, _cachedDatasource.Object, _logger.Object, _auditLogService.Object);
        }

        private void SetupMocksForSuccessfulQuery(SavedQuery savedQuery, List<DimensionParameters> dimParams, bool isArchived = false, ArchiveCube archiveCube = null)
        {
            _sqFileInterface.Setup(fi => fi.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _sqFileInterface.Setup(fi => fi.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(savedQuery);

            if (!isArchived)
            {
                _cachedDatasource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                    .ReturnsAsync(() => TestDataCubeBuilder.BuildTestMeta(dimParams));
            }
            else
            {
                _sqFileInterface.Setup(fi => fi.ArchiveCubeExists(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(archiveCube != null);

                if (archiveCube != null)
                {
                    _sqFileInterface.Setup(fi => fi.ReadArchiveCubeFromFile(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(archiveCube);
                }
            }
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnValidMeta()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            
            SetupMocksForSuccessfulQuery(sq, dimParams);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            // Assert
            Assert.That(result.Value.Header["fi"], Is.EqualTo("value-0, value-0 2000-2009 muuttujana variable-2"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2"));
            Assert.That(result.Value.Archived, Is.False);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description, Is.Null);
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));

            // Verify audit log was called with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnSelectableTrue()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = [],
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            
            SetupMocksForSuccessfulQuery(sq, dimParams);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            // Assert
            Assert.That(result.Value.Selectable, Is.True);
            
            // Verify audit log was called with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_NotFound()
        {
            // Arrange
            _sqFileInterface.Setup(fi => fi.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            
            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            
            // Verify audit log was called with INVALID_OR_MISSING_SQID
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == LoggerConstants.INVALID_OR_MISSING_SQID),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_ArchivedQuery()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 2),
                new(DimensionType.Other, 1)
            ];
            List<DimensionParameters> metaParams =
            [
                new(DimensionType.Content, 4),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 3),
                new(DimensionType.Other, 2)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, true, settings);
            ArchiveCube archiveCube = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            
            SetupMocksForSuccessfulQuery(sq, dimParams, true, archiveCube);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            // Assert
            Assert.That(result.Value.Header["fi"], Is.EqualTo("variable-0 2000-2009 muuttujina variable-0, variable-2, variable-3"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("variable-0 [FIRST]-[LAST] muuttujina variable-0, variable-2, variable-3"));
            Assert.That(result.Value.Archived, Is.True);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description, Is.Null);
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));
            
            // Verify audit log was called with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_Table_Not_Found()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            
            _sqFileInterface.Setup(fi => fi.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            
            _sqFileInterface.Setup(fi => fi.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(sq);
            
            _cachedDatasource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((IReadOnlyMatrixMetadata)null);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            
            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
            
            // Verify audit log was called once with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_ArchiveFileNotFound()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 2),
                new(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, true, settings);
            
            SetupMocksForSuccessfulQuery(sq, dimParams, true, null);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            
            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
            
            // Verify audit log was called once with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetQueryMetaTest_WithEditedHeaderAndNames_ReturnsCorrectResult()
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 2),
                new(DimensionType.Other, 1)
            ];
            List<DimensionParameters> metaParams =
            [
                new(DimensionType.Content, 4),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 3),
                new(DimensionType.Other, 2)
            ];
            Layout layout = new()
            {
                RowDimensionCodes = [],
                ColumnDimensionCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, true, settings);
            Dictionary<string, string> headerEditTranslations = new()
            {
                ["fi"] = "editedHeader.fi",
                ["en"] = "editedHeader.en"
            };
            MultilanguageString editedHeader = new(headerEditTranslations);
            sq.Query.ChartHeaderEdit = editedHeader;
            ArchiveCube archiveCube = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            
            SetupMocksForSuccessfulQuery(sq, dimParams, true, archiveCube);
            
            QueryMetaController controller = CreateController();

            // Act
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            
            // Assert
            Assert.That(result.Value.Header["fi"].Equals("editedHeader.fi"));
            Assert.That(result.Value.Header["en"].Equals("editedHeader.en"));
            
            // Verify audit log was called with the correct parameters
            _auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(action => action == "api/sq/meta"),
                It.Is<string>(resource => resource == "test"),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }
    }
}

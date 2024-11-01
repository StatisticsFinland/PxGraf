using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.QueryMetaControllerTests
{
    internal class GetQueryMetaTests
    {
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

        [Test]
        public async Task GetQueryMetaTest_ReturnValidMeta()
        {
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowVariableCodes = [],
                ColumnVariableCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, dimParams);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            Assert.That(result.Value.Header["fi"], Is.EqualTo("value-0, value-0 2000-2009 muuttujana variable-2"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2"));
            Assert.That(result.Value.Archived, Is.False);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description["fi"], Is.EqualTo("Test note"));
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));
        }

        [Test]
        public async Task GetQueryMetaTest_ReturnSelectableTrue()
        {
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowVariableCodes = [],
                ColumnVariableCodes = [],
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, dimParams);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            Assert.That(result.Value.Selectable, Is.True);
        }

        [Test]
        public async Task GetQueryMetaTest_NotFound()
        {
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController([], Configuration.Current.SavedQueryDirectory, []);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetQueryMetaTest_ArchivedQuery()
        {
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
                RowVariableCodes = [],
                ColumnVariableCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, true, settings);
            ArchiveCube archiveCube = TestDataCubeBuilder.BuildTestArchiveCube(metaParams);
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            Dictionary<string, ArchiveCube> archiveCubes = new()
            {
                {"goesNowhere/test", archiveCube}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, dimParams, archiveCubes: archiveCubes);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");

            Assert.That(result.Value.Header["fi"], Is.EqualTo("variable-0 2000-2009 muuttujina variable-0, variable-2, variable-3"));
            Assert.That(result.Value.HeaderWithPlaceholders["fi"], Is.EqualTo("variable-0 [FIRST]-[LAST] muuttujina variable-0, variable-2, variable-3"));
            Assert.That(result.Value.Archived, Is.True);
            Assert.That(result.Value.Selectable, Is.False);
            Assert.That(result.Value.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
            Assert.That(result.Value.TableId, Is.EqualTo("TestPxFile.px"));
            Assert.That(result.Value.Description["fi"], Is.EqualTo("Test note"));
            Assert.That(result.Value.LastUpdated, Is.EqualTo("2009-09-01T00:00:00Z"));
            Assert.That(result.Value.TableReference.Name, Is.EqualTo("TestPxFile.px"));

            List<string> expectedHierarchy = ["testpath", "to", "test", "file"];
            Assert.That(result.Value.TableReference.Hierarchy, Is.EqualTo(expectedHierarchy));
        }

        [Test]
        public void GetQueryMetaTest_Table_Not_Found()
        {
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowVariableCodes = [],
                ColumnVariableCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, false, settings);
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, []);
            ActionResult<QueryMetaResponse> result = controller.GetQueryMeta("test").Result;
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void GetQueryMetaTest_ArchiveFileNotFound()
        {

            List<DimensionParameters> dimParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 10),
                new(DimensionType.Other, 2),
                new(DimensionType.Other, 1)
            ];
            Layout layout = new()
            {
                RowVariableCodes = [],
                ColumnVariableCodes = ["variable-1"]
            };
            LineChartVisualizationSettings settings = new(layout, false, null);
            SavedQuery sq = TestDataCubeBuilder.BuildTestSavedQuery(dimParams, true, settings);
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, []);
            ActionResult<QueryMetaResponse> result = controller.GetQueryMeta("test").Result;
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetQueryMetaTest_WithEditedHeaderAndNames_ReturnsCorrectResult()
        {
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
                RowVariableCodes = [],
                ColumnVariableCodes = ["variable-1"]
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
            Dictionary<string, SavedQuery> savedQueries = new()
            {
                {"goesNowhere/test", sq}
            };
            Dictionary<string, ArchiveCube> archiveCubes = new()
            {
                {"goesNowhere/test", archiveCube}
            };
            QueryMetaController controller = TestQueryMetaControllerBuilder.BuildController(savedQueries, Configuration.Current.SavedQueryDirectory, dimParams, archiveCubes: archiveCubes);
            ActionResult<QueryMetaResponse> result = await controller.GetQueryMeta("test");
            Assert.That(result.Value.Header["fi"].Equals("editedHeader.fi"));
            Assert.That(result.Value.Header["en"].Equals("editedHeader.en"));
        }
    }
}

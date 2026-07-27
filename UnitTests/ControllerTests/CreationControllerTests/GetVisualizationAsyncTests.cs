using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Datasource.ApiDatasource.SerializationModels;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    internal class GetVisualizationAsyncTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> inMemorySettings = new()
            {
                {"DatabaseConfig:Type", "PxWeb"},
                {"DatabaseConfig:PxWebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"QueryOptions:MaxHeaderLength", "120"},
                {"QueryOptions:MaxQuerySize", "100000"},
                {"CacheOptions:Visualization:SlidingExpirationMinutes", "15" },
                {"CacheOptions:Visualization:AbsoluteExpirationMinutes", "720" },
                {"CacheOptions:Visualization:ItemAmountLimit", "1000" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetVisualizationTest_Fresh_Data_Is_Returned()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = ["variable-1"],
                ColumnDimensionCodes = [],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> initialResult = await testController.GetVisualizationAsync(chartRequest);
            ContentComponent contentClone = initialResult.Value.MetaData.First(d => d.DimensionType == DimensionType.Content).Values[0].ContentComponent.Clone();
            contentClone.LastUpdated = "2008-09-01T00:00:00.000Z";
            initialResult.Value.MetaData.First(d => d.DimensionType == DimensionType.Content).Values[0].ContentComponent = contentClone;
            ActionResult<VisualizationResponse> freshResult = await testController.GetVisualizationAsync(chartRequest);

            // Assert
            Assert.That(initialResult.Value, Is.InstanceOf<VisualizationResponse>());
            Assert.That(freshResult.Value, Is.InstanceOf<VisualizationResponse>());
            Assert.That(initialResult.Value, Is.Not.EqualTo(freshResult.Value));
        }

        [Test]
        public async Task GetVisualizationTest_Volume_0_Cube_Returns_BadRequest()
        {
            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 0) // Note 0 values
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = ["variable-1"],
                ColumnDimensionCodes = [],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task GetVisualizationTest_Valid_VisualizationType_DoesNotReturn_BadRequest()
        {
            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = ["variable-1"],
                ColumnDimensionCodes = [],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.TypeOf<VisualizationResponse>());
        }

        [Test]
        public async Task GetVisualizationTest_Invalid_VisualizationType_Returns_BadRequest()
        {
            // Arrange
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 10),
                new DimensionParameters(DimensionType.Other, 5)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = ["variable-1"],
                ColumnDimensionCodes = [],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public void GetJsonStat2VisualizationAsync_UsesJsonStatVisualizationRoute()
        {
            HttpPostAttribute route = typeof(CreationController)
                .GetMethod(nameof(CreationController.GetJsonStat2VisualizationAsync))!
                .GetCustomAttribute<HttpPostAttribute>()!;

            Assert.That(route.Template, Is.EqualTo("jsonstat/visualization"));
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_ReturnsJsonStatDataset()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams);
            testController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationCreationSettings settings = new()
            {
                SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                RowDimensionCodes = ["variable-1"],
                ColumnDimensionCodes = [],
                MultiselectableDimensionCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableDimensionCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableDimensionValues = [],
                Language = "fi",
            };

            ActionResult<JsonStat2> result = await testController.GetJsonStat2VisualizationAsync(chartRequest, null);

            Assert.That(result.Result, Is.InstanceOf<JsonResult>());
            JsonResult jsonResult = (JsonResult)result.Result!; 
            Assert.That(jsonResult.ContentType, Is.EqualTo("application/vnd.jsonstat2+json"));
            Assert.That(jsonResult.Value, Is.InstanceOf<JsonStat2>());
            JsonStat2 dataset = (JsonStat2)jsonResult.Value;
            Assert.That(dataset.Extension.VisualizationSettings, Is.InstanceOf<VisualizationResponse.PxVisualizerSettings>());
            Assert.That(dataset.Extension.VisualizationSettings.VisualizationType, Is.EqualTo(PxGraf.Enums.VisualizationType.LineChart));
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithUnsupportedLanguage_ReturnsBadRequest()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Geographical, 2)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams);
            testController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            ChartRequest chartRequest = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                VisualizationSettings = new VisualizationCreationSettings
                {
                    SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-1"],
                    ColumnDimensionCodes = [],
                    MultiselectableDimensionCode = string.Empty,
                    Sorting = "NO_SORTING",
                    DefaultSelectableDimensionCodes = []
                },
                ActiveSelectableDimensionValues = [],
                Language = "fi"
            };

            ActionResult<JsonStat2> result = await testController.GetJsonStat2VisualizationAsync(chartRequest, "de");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithInvalidVisualizationType_ReturnsBadRequest()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 10),
                new DimensionParameters(DimensionType.Other, 5)
            ];
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams);
            ChartRequest request = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                VisualizationSettings = new VisualizationCreationSettings
                {
                    SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-1"],
                    ColumnDimensionCodes = [],
                    MultiselectableDimensionCode = string.Empty,
                    Sorting = "NO_SORTING",
                    DefaultSelectableDimensionCodes = []
                },
                ActiveSelectableDimensionValues = [],
                Language = "fi"
            };

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(request, "fi");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetJsonStat2VisualizationTest_WithEmptyDimension_ReturnsBadRequest()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 0)
            ];
            CreationController controller = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams);
            ChartRequest request = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                VisualizationSettings = new VisualizationCreationSettings
                {
                    SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-1"],
                    ColumnDimensionCodes = [],
                    MultiselectableDimensionCode = string.Empty,
                    Sorting = "NO_SORTING",
                    DefaultSelectableDimensionCodes = []
                },
                ActiveSelectableDimensionValues = [],
                Language = "fi"
            };

            ActionResult<JsonStat2> result = await controller.GetJsonStat2VisualizationAsync(request, "fi");

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetVisualizationTest_WithAcceptHeader_ReturnsVisualizationResponse()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Other, 2)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, cubeParams);
            testController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            testController.ControllerContext.HttpContext.Request.Headers.Accept = "application/xml";

            ChartRequest chartRequest = new()
            {
                Query = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams),
                VisualizationSettings = new VisualizationCreationSettings
                {
                    SelectedVisualization = PxGraf.Enums.VisualizationType.LineChart,
                    RowDimensionCodes = ["variable-1"],
                    ColumnDimensionCodes = [],
                    MultiselectableDimensionCode = string.Empty,
                    Sorting = "NO_SORTING",
                    DefaultSelectableDimensionCodes = []
                },
                ActiveSelectableDimensionValues = [],
                Language = "fi"
            };

            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            Assert.That(result.Value, Is.InstanceOf<VisualizationResponse>());
        }
    }
}

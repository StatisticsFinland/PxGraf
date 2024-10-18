using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Data.MetaData;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
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
                {"pxwebUrl", "http://pxwebtesturl:12345/"},
                {"pxgrafUrl", "http://pxgraftesturl:8443/PxGraf"},
                {"savedQueryDirectory", "goesNowhere"},
                {"archiveFileDirectory", "goesNowhere"},
                {"LocalFileSystemDatabaseConfig:Encoding", "latin1"},
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
                RowVariableCodes = ["variable-1"],
                ColumnVariableCodes = [],
                MultiselectableVariableCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableVariableCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableVariableValues = [],
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
                RowVariableCodes = ["variable-1"],
                ColumnVariableCodes = [],
                MultiselectableVariableCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableVariableCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableVariableValues = [],
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
                RowVariableCodes = ["variable-1"],
                ColumnVariableCodes = [],
                MultiselectableVariableCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableVariableCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableVariableValues = [],
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
                RowVariableCodes = ["variable-1"],
                ColumnVariableCodes = [],
                MultiselectableVariableCode = string.Empty,
                Sorting = "NO_SORTING",
                DefaultSelectableVariableCodes = []
            };

            ChartRequest chartRequest = new()
            {
                Query = cubeQuery,
                VisualizationSettings = settings,
                ActiveSelectableVariableValues = [],
                Language = "fi",
            };

            // Act
            ActionResult<VisualizationResponse> result = await testController.GetVisualizationAsync(chartRequest);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestResult>());
        }
    }
}

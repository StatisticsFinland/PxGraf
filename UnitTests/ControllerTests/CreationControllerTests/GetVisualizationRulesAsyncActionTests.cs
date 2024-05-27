using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.Caching;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;
using Microsoft.Extensions.Logging;
using PxGraf.Models.Queries;

namespace CreationControllerTests
{
    internal class GetVisualizationRulesAsyncActionTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        private static CreationController BuildController(List<VariableParameters> cubeParams, List<VariableParameters> metaParams)
        {
            PxWebApiDummy pxWebApiDummy = new(cubeParams, metaParams);

            ServiceCollection services = new();
            services.AddMemoryCache();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IMemoryCache memoryCache = serviceProvider.GetService<IMemoryCache>();
            IPxWebApiResponseCache apiCache = new PxWebApiResponseCache(memoryCache);

            return new CreationController(new CachedPxWebConnection(pxWebApiDummy, apiCache), new Mock<ILogger<CreationController>>().Object);
        }

        [Test]
        public async Task SimpleSuccessTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult, Is.InstanceOf<ActionResult<VisualizationRules>>());
        }

        [Test]
        public async Task SortingNoManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["descending", "ascending", "no_sorting"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimWithManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimNoManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task TimeSeriesTest_DoNotAllowManualPivot()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupVerticalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.AllowManualPivot, Is.False);
        }

        [Test]
        public async Task TimeSeriesLineTest_MultiselectAllowed()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.MultiselectVariableAllowed, Is.True);
        }

        [Test]
        public async Task GetVisualizationRulesAsync_Creates_TypeSpecificVisualizationRules()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.OtherClassificatory, 4)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> result = await testController.GetVisualizationRulesAsync(rulesRequest);

            Assert.That(result.Value.VisualizationTypeSpecificRules, Is.Not.Null);
            Assert.That(result.Value.VisualizationTypeSpecificRules.AllowShowingDataPoints, Is.False);
            Assert.That(result.Value.VisualizationTypeSpecificRules.AllowMatchXLabelsToEnd, Is.False);
            Assert.That(result.Value.VisualizationTypeSpecificRules.AllowSetMarkerScale, Is.False);
        }

        [Test]
        public async Task GetVisualizationRulesAsync_VerticalBarChart_AllowsShowingDataPoints()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.VerticalBarChart,
                PivotRequested = false,
                Query = cubeQuery,
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.VisualizationTypeSpecificRules.AllowShowingDataPoints, Is.True);
        }

        [Test]
        [TestCase(VisualizationType.GroupVerticalBarChart, 1, 3, 3, 1, 1)]
        [TestCase(VisualizationType.StackedVerticalBarChart, 1, 2, 5, 1, 1)]
        [TestCase(VisualizationType.PercentVerticalBarChart, 1, 2, 5, 1, 1)]
        [TestCase(VisualizationType.HorizontalBarChart, 1, 1, 5, 1, 1)]
        [TestCase(VisualizationType.GroupHorizontalBarChart, 1, 2, 2, 1, 1)]
        [TestCase(VisualizationType.StackedHorizontalBarChart, 1, 1, 3, 3, 1)]
        [TestCase(VisualizationType.PercentHorizontalBarChart, 1, 1, 3, 3, 1)]
        [TestCase(VisualizationType.LineChart, 1, 10, 1, 1, 1)]
        [TestCase(VisualizationType.ScatterPlot, 2, 10, 1, 1, 1)]
        [TestCase(VisualizationType.PieChart, 1, 1, 5, 1, 1)]
        public async Task GetVisualizationRulesAsync_OtherVisualizationTypes_DontAllowShowingDataPoints(
            VisualizationType visualizationType,
            int cVarAmount,
            int timeVarAmount,
            int otherVarAAmount,
            int otherVarBAmount,
            int otherVarCAmount)
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, cVarAmount),
                new VariableParameters(VariableType.Time, timeVarAmount),
                new VariableParameters(VariableType.OtherClassificatory, otherVarAAmount),
                new VariableParameters(VariableType.OtherClassificatory, otherVarBAmount),
                new VariableParameters(VariableType.OtherClassificatory, otherVarCAmount)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = visualizationType,
                PivotRequested = false,
                Query = cubeQuery,
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.VisualizationTypeSpecificRules.AllowShowingDataPoints, Is.False);
        }
    }
}


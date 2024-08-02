using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Language;
using PxGraf.Settings;
using UnitTests.Fixtures;

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

        // TODO: Fix tests

        /*
        private static CreationController BuildController(List<VariableParameters> cubeParams, List<VariableParameters> metaParams)
        {
            // TODO: Fix
            throw new NotImplementedException();
        }

        [Test]
        public async Task SimpleSuccessTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 1),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 5),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 1),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 1),
                new VariableParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 5),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimWithManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 1),
                new VariableParameters(DimensionType.Other, 4),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 5),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimNoManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 1),
                new VariableParameters(DimensionType.Other, 4),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 5),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task TimeSeriesTest_DoNotAllowManualPivot()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 4),
                new VariableParameters(DimensionType.Other, 3) { Selectable = true },
                new VariableParameters(DimensionType.Other, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 15),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 12),
                new VariableParameters(DimensionType.Other, 4),
                new VariableParameters(DimensionType.Other, 3) { Selectable = true },
                new VariableParameters(DimensionType.Other, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 15),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 7),
                new VariableParameters(DimensionType.Other, 4),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 12),
                new VariableParameters(DimensionType.Other, 4)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 15),
                new VariableParameters(DimensionType.Other, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 15),
                new VariableParameters(DimensionType.Other, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
        [TestCase(VisualizationType.StackedHorizontalBarChart, 1, 1, 3, 1, 3)]
        [TestCase(VisualizationType.PercentHorizontalBarChart, 1, 1, 3, 1, 3)]
        [TestCase(VisualizationType.LineChart, 1, 10, 1, 1, 1)]
        [TestCase(VisualizationType.ScatterPlot, 2, 10, 1, 1, 1)]
        [TestCase(VisualizationType.PieChart, 1, 1, 5, 1, 1)]
        [TestCase(VisualizationType.PyramidChart, 1, 1, 2, 5, 1)]
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
                new VariableParameters(DimensionType.Content, cVarAmount),
                new VariableParameters(DimensionType.Time, timeVarAmount),
                new VariableParameters(DimensionType.Geographical, otherVarAAmount),
                new VariableParameters(DimensionType.Ordinal, otherVarBAmount),
                new VariableParameters(DimensionType.Other, otherVarCAmount)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 5),
                new VariableParameters(DimensionType.Time, 15),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 6),
                new VariableParameters(DimensionType.Other, 6)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = visualizationType,
                PivotRequested = false,
                Query = cubeQuery,
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.VisualizationTypeSpecificRules.AllowShowingDataPoints, Is.False);
        }
        */
    }
}


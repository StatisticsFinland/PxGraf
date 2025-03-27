using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Enums;
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

        [Test]
        public async Task SimpleSuccessTest_Success()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = [.. actionResult.Value.SortingOptions.Select(so => so.Code)];
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimWithManualPivotTest_Success()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 4),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = [.. actionResult.Value.SortingOptions.Select(so => so.Code)];
            List<string> expected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task SortingMultidimNoManualPivotTest_Success()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 4),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            List<string> sortingOptionCodes = [.. actionResult.Value.SortingOptions.Select(so => so.Code)];
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptionCodes, Is.EqualTo(expected));
        }

        [Test]
        public async Task TimeSeriesTest_DoNotAllowManualPivot()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 4),
                new DimensionParameters(DimensionType.Other, 3) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Other, 4),
                new DimensionParameters(DimensionType.Other, 3) { Selectable = true },
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.That(actionResult.Value.MultiselectDimensionAllowed, Is.True);
        }

        [Test]
        public async Task GetVisualizationRulesAsync_Creates_TypeSpecificVisualizationRules()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Other, 4)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, cVarAmount),
                new DimensionParameters(DimensionType.Time, timeVarAmount),
                new DimensionParameters(DimensionType.Geographical, otherVarAAmount),
                new DimensionParameters(DimensionType.Ordinal, otherVarBAmount),
                new DimensionParameters(DimensionType.Other, otherVarCAmount)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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

        [Test]
        public async Task GetVisualizationRulesAsync_CalledWithZeroSizedQuery_ReturnsExpectedResult()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Geographical, 1),
                new DimensionParameters(DimensionType.Ordinal, 1),
                new DimensionParameters(DimensionType.Other, 0)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 4),
                new DimensionParameters(DimensionType.Time, 24),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);

            Assert.That(actionResult.Value.AllowManualPivot, Is.False);
            Assert.That(actionResult.Value.MultiselectDimensionAllowed, Is.False);
            Assert.That(actionResult.Value.VisualizationTypeSpecificRules, Is.Null);
        }

        [Test]
        public async Task GetVisualizationRulesAync_CalledWithInvalidVisualizationType_ReturnsExpectedResults()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Geographical, 1),
                new DimensionParameters(DimensionType.Ordinal, 1),
                new DimensionParameters(DimensionType.Other, 3)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 4),
                new DimensionParameters(DimensionType.Time, 24),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 6)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            VisualizationSettingsRequest rulesRequest = new()
            {
                SelectedVisualization = VisualizationType.ScatterPlot,
                PivotRequested = false,
                Query = cubeQuery
            };

            ActionResult<VisualizationRules> actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);

            Assert.That(actionResult.Value.AllowManualPivot, Is.False);
            Assert.That(actionResult.Value.MultiselectDimensionAllowed, Is.False);
            Assert.That(actionResult.Value.VisualizationTypeSpecificRules, Is.Null);
        }
    }
}


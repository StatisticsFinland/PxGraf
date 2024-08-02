using Microsoft.Extensions.Configuration;
using PxGraf.Language;
using PxGraf.Settings;
using UnitTests.Fixtures;
using NUnit.Framework;

namespace CreationControllerTests
{
    internal class GetQueryInfoAsyncActionTests
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
            // TODO Fix
            throw new NotImplementedException();
        }

        [Test]
        public async Task SimpleSuccessTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 1),
                new VariableParameters(DimensionType.Other, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 7),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult, Is.InstanceOf<ActionResult<QueryInfoResponse>>());
        }

        [Test]
        public async Task ValidChartTypesTest_LineChart_HorizontalBarChart_Table()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 1),
                new VariableParameters(DimensionType.Other, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 7),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            List<VisualizationType> expected = [VisualizationType.LineChart, VisualizationType.VerticalBarChart, VisualizationType.Table];
            Assert.That(actionResult.Value.ValidVisualizations, Is.EqualTo(expected));
        }

        [Test]
        public async Task SizeTest_450()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 15)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            // 10 * 3 * 15
            Assert.That(actionResult.Value.Size, Is.EqualTo(450));
        }

        [Test]
        public async Task RejectionReasonsTest_8Rejected()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            List<VisualizationType> expectedTypes =
            [
                VisualizationType.HorizontalBarChart,
                VisualizationType.GroupHorizontalBarChart,
                VisualizationType.StackedHorizontalBarChart,
                VisualizationType.PercentHorizontalBarChart,
                VisualizationType.VerticalBarChart,
                VisualizationType.PieChart,
                VisualizationType.PyramidChart,
                VisualizationType.ScatterPlot
            ];

            Assert.That(actionResult.Value.VisualizationRejectionReasons.Keys, Is.EqualTo(expectedTypes));
        }

        [Test]
        public async Task RejectionReasonsTextTest_MultiLanguageString()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            KeyValuePair<VisualizationType, MultilanguageString> firstReason = actionResult.Value.VisualizationRejectionReasons.First();
            Assert.That(firstReason.Key, Is.EqualTo(VisualizationType.HorizontalBarChart));
            Assert.That(firstReason.Value.Languages.Count(), Is.EqualTo(3));
            Assert.That(firstReason.Value["fi"], Is.EqualTo("Poiminnassa on liikaa monivalintaulottuvuuksia (2 / 1)"));
        }

        [Test]
        public async Task MaxAndWarningLimits_WarningSmallerThanMax()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 3),
                new VariableParameters(DimensionType.Other, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.SizeWarningLimit, Is.LessThan(actionResult.Value.MaximumSupportedSize));
        }

        [Test]
        public async Task ActualSizeAndMaxLimits_SizeBiggerThanMax()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(DimensionType.Content, 1),
                new VariableParameters(DimensionType.Time, 12),
                new VariableParameters(DimensionType.Other, 10),
                new VariableParameters(DimensionType.Other, 10),
                new VariableParameters(DimensionType.Other, 100)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(DimensionType.Content, 10),
                new VariableParameters(DimensionType.Time, 10),
                new VariableParameters(DimensionType.Other, 15),
                new VariableParameters(DimensionType.Other, 70),
                new VariableParameters(DimensionType.Other, 70)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.Size, Is.GreaterThan(actionResult.Value.MaximumSupportedSize));
        }
        */
    }
}

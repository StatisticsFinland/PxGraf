using Microsoft.Extensions.Configuration;
using PxGraf.Language;
using PxGraf.Settings;
using UnitTests.Fixtures;
using NUnit.Framework;
using PxGraf.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using PxGraf.Models.Responses;
using PxGraf.Enums;
using Px.Utils.Language;
using System.Linq;
using System;

namespace UnitTests.ControllerTests.CreationControllerTests
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

        [Test]
        public async Task SimpleSuccessTest_Success()
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
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult, Is.InstanceOf<ActionResult<QueryInfoResponse>>());
        }

        [Test]
        public async Task ValidChartTypesTest_LineChart_HorizontalBarChart_Table()
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
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            List<VisualizationType> expected = [VisualizationType.LineChart, VisualizationType.VerticalBarChart, VisualizationType.Table];
            Assert.That(actionResult.Value.ValidVisualizations, Is.EqualTo(expected));
        }

        [Test]
        public async Task SizeTest_450()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 15)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 70),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            // 10 * 3 * 15
            Assert.That(actionResult.Value.Size, Is.EqualTo(450));
        }

        [Test]
        public async Task RejectionReasonsTest_8Rejected()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 70),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 70),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
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
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 1),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 70),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.SizeWarningLimit, Is.LessThan(actionResult.Value.MaximumSupportedSize));
        }

        [Test]
        public async Task ActualSizeAndMaxLimits_SizeBiggerThanMax()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Other, 10),
                new DimensionParameters(DimensionType.Other, 10),
                new DimensionParameters(DimensionType.Other, 100)
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 70),
                new DimensionParameters(DimensionType.Other, 70)
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.Size, Is.GreaterThan(actionResult.Value.MaximumSupportedSize));
        }
    }
}

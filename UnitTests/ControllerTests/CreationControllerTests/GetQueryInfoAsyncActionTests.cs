﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.Caching;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

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
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult, Is.InstanceOf<ActionResult<QueryInfoResponse>>());
        }

        [Test]
        public async Task ValidChartTypesTest_LineChart_HorizontalBarChart_Table()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            List<VisualizationType> expected = [VisualizationType.LineChart, VisualizationType.VerticalBarChart, VisualizationType.Table];
            Assert.That(actionResult.Value.ValidVisualizations, Is.EqualTo(expected));
        }

        [Test]
        public async Task SizeTest_450()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 15)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            // 10 * 3 * 15
            Assert.That(actionResult.Value.Size, Is.EqualTo(450));
        }

        [Test]
        public async Task RejectionReasonsTest_8Rejected()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
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
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            KeyValuePair<VisualizationType, MultiLanguageString> firstReason = actionResult.Value.VisualizationRejectionReasons.First();
            Assert.That(firstReason.Key, Is.EqualTo(VisualizationType.HorizontalBarChart));
            Assert.That(firstReason.Value.Languages.Count(), Is.EqualTo(3));
            Assert.That(firstReason.Value["fi"], Is.EqualTo("Poiminnassa on liikaa monivalintaulottuvuuksia (2 / 1)"));
        }

        [Test]
        public async Task MaxAndWarningLimits_WarningSmallerThanMax()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 70),
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.SizeWarningLimit, Is.LessThan(actionResult.Value.MaximumSupportedSize));
        }

        [Test]
        public async Task ActualSizeAndMaxLimits_SizeBiggerThanMax()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.OtherClassificatory, 10),
                new VariableParameters(VariableType.OtherClassificatory, 10),
                new VariableParameters(VariableType.OtherClassificatory, 100)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 70),
                new VariableParameters(VariableType.OtherClassificatory, 70)
            ];

            CreationController testController = BuildController(cubeParams, metaParams);
            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            ActionResult<QueryInfoResponse> actionResult = await testController.GetQueryInfoAsync(cubeQuery);

            Assert.That(actionResult.Value.Size, Is.GreaterThan(actionResult.Value.MaximumSupportedSize));
        }
    }
}

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;
using Microsoft.Extensions.Logging; 

namespace CreationControllerTests
{
    internal class GetVisualizationRulesAsyncActionTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));

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
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.IsInstanceOf<ActionResult<VisualizationRules>>(actionResult);
        }

        [Test]
        public async Task SortingNoManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            var sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            var expected = new List<string>() { "descending", "ascending", "no_sorting" };
            Assert.AreEqual(expected, sortingOptionCodes);
        }

        [Test]
        public async Task SortingMultidimWithManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 2) { Selectable = true},
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            var sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            var expected = new List<string>() { "value-0", "value-1", "value-2", "value-3", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptionCodes);
        }

        [Test]
        public async Task SortingMultidimNoManualPivotTest_Success()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            var sortingOptionCodes = actionResult.Value.SortingOptions.Select(so => so.Code).ToList();
            var expected = new List<string>() { "value-0", "value-1", "value-2", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptionCodes);
        }

        [Test]
        public async Task TimeSeriesTest_DoNotAllowManualPivot()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupVerticalBarChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.IsFalse(actionResult.Value.AllowManualPivot);
        }

        [Test]
        public async Task TimeSeriesLineTest_MultiselectAllowed()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.OtherClassificatory, 1),
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6),
                new VariableParameters(VariableType.OtherClassificatory, 7),
                new VariableParameters(VariableType.OtherClassificatory, 4),
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var actionResult = await testController.GetVisualizationRulesAsync(rulesRequest);
            Assert.IsTrue(actionResult.Value.MultiselectVariableAllowed);
        }

        [Test]
        public async Task GetVisualizationRulesAsync_Creates_TypeSpecificVisualizationRules()
        {
            List<VariableParameters> cubeParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.OtherClassificatory, 4)
            };

            List<VariableParameters> metaParams = new()
            {
                new VariableParameters(VariableType.Content, 5),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.OtherClassificatory, 6)
            };

            var testController = BuildController(cubeParams, metaParams);
            var cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            var rulesRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.LineChart,
                PivotRequested = false,
                Query = cubeQuery
            };

            var result = await testController.GetVisualizationRulesAsync(rulesRequest);

            Assert.IsNotNull(result.Value.VisualizationTypeSpecificRules);
            Assert.IsTrue(result.Value.VisualizationTypeSpecificRules.AllowShowingDataPoints);
            Assert.IsFalse(result.Value.VisualizationTypeSpecificRules.AllowMatchXLabelsToEnd);
            Assert.IsFalse(result.Value.VisualizationTypeSpecificRules.AllowSetMarkerScale);
        }
    }
}


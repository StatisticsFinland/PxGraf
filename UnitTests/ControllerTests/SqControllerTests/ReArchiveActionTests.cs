using Microsoft.AspNetCore.Mvc;
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
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.Caching;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace ControllerTests
{
    internal class ReArchiveActionTests
    {
        private const string TEST_SQ_ID = "abc-123";

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        private static SqController BuildController(List<VariableParameters> cubeParams, List<VariableParameters> metaParams, VisualizationSettings vSettings = null)
        {
            PxWebApiDummy pxWebApiDummy = new(cubeParams, metaParams);
            Dictionary<string, SavedQuery> testQueries = new()
            {
                [TEST_SQ_ID] = TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, vSettings ?? new LineChartVisualizationSettings(null, false, null))
            };
            SqFileInterfaceDummy sqFileDummy = new(testQueries);

            ServiceCollection services = new();
            services.AddMemoryCache();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IMemoryCache memoryCache = serviceProvider.GetService<IMemoryCache>();
            IPxWebApiResponseCache apiCache = new PxWebApiResponseCache(memoryCache);

            return new SqController(new CachedPxWebConnection(pxWebApiDummy, apiCache), sqFileDummy, new Mock<ILogger<SqController>>().Object);
        }

        [Test]
        public async Task SqNotFoundTest_NotFoundResult()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            SqController testController = BuildController(cubeParams, metaParams);
            ActionResult<ReArchiveResponse> actionResult = await testController.ReArchiveExistingQueryAsync(new ReArchiveRequest() { SqId = "not_found_asdf" });
            Assert.IsInstanceOf<NotFoundResult>(actionResult.Result);
        }

        [Test]
        public async Task SqNotFoundTest_WrongChartType()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2), // horizontal bar chart is not possible with multiple time values
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            SqController testController = BuildController(cubeParams, metaParams, new HorizontalBarChartVisualizationSettings(null));
            ActionResult<ReArchiveResponse> actionResult = await testController.ReArchiveExistingQueryAsync(new ReArchiveRequest() { SqId = TEST_SQ_ID });
            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);
        }

        [Test]
        public async Task SqNotFoundTest_Success()
        {
            List<VariableParameters> cubeParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.OtherClassificatory, 1)
            ];

            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 10),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 15),
                new VariableParameters(VariableType.OtherClassificatory, 7),
            ];

            SqController testController = BuildController(cubeParams, metaParams, new HorizontalBarChartVisualizationSettings(null));
            ActionResult<ReArchiveResponse> actionResult = await testController.ReArchiveExistingQueryAsync(new ReArchiveRequest() { SqId = TEST_SQ_ID });
            Assert.IsInstanceOf<ReArchiveResponse>(actionResult.Value);
        }
    }
}

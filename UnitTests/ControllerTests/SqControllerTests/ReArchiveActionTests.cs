using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.SqControllerTests
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

        private static SqController BuildController(List<DimensionParameters> cubeParams, List<DimensionParameters> metaParams)
        {
            Mock<ICachedDatasource> mockCachedDatasource = new();
            Mock<ISqFileInterface> mockSqFileInterface = new();
            Mock<ILogger<SqController>> mockLogger = new();

            mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            mockSqFileInterface.Setup(s => s.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(cubeParams.Count > 0);

            mockSqFileInterface.Setup(s => s.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null))));

            return new SqController(mockCachedDatasource.Object, mockSqFileInterface.Object, mockLogger.Object);
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_NotFoundResult()
        {
            SqController controller = BuildController([], []);
            ReArchiveRequest request = new ()
            {
                SqId = TEST_SQ_ID
            };
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_WrongChartType()
        {
            List<DimensionParameters> cubeParameters =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 5),
            ];
            List<DimensionParameters> metaParameters =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7)
            ];
            SqController controller = BuildController(cubeParameters, metaParameters);
            ReArchiveRequest request = new()
            {
                SqId = TEST_SQ_ID
            };
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_Success()
        {
            List<DimensionParameters> cubeParameters =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 1),
                new DimensionParameters(DimensionType.Other, 1),
            ];
            List<DimensionParameters> metaParameters =
            [
                new DimensionParameters(DimensionType.Content, 10),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 15),
                new DimensionParameters(DimensionType.Other, 7)
            ];
            SqController controller = BuildController(cubeParameters, metaParameters);
            ReArchiveRequest request = new()
            {
                SqId = TEST_SQ_ID
            };
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<ReArchiveResponse>());
        }
    }
}

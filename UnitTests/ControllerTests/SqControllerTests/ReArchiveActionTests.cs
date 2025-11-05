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
using PxGraf.Services;
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
        private Mock<ICachedDatasource> _mockCachedDatasource;
        private Mock<ISqFileInterface> _mockSqFileInterface;
        private Mock<ILogger<SqController>> _mockLogger;
        private Mock<IAuditLogService> _mockAuditLogService;
        private Mock<IPublicationWebhookService> _mockWebhookService;

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        [SetUp]
        public void Setup()
        {
            _mockCachedDatasource = new Mock<ICachedDatasource>();
            _mockSqFileInterface = new Mock<ISqFileInterface>();
            _mockLogger = new Mock<ILogger<SqController>>();
            _mockAuditLogService = new Mock<IAuditLogService>();
            _mockWebhookService = new Mock<IPublicationWebhookService>();
        }

        private SqController BuildController(List<DimensionParameters> cubeParams, List<DimensionParameters> metaParams)
        {
            _mockCachedDatasource.Setup(c => c.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .Returns(Task.Run(() => (IReadOnlyMatrixMetadata)TestDataCubeBuilder.BuildTestMeta(metaParams)));

            _mockCachedDatasource.Setup(c => c.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<IReadOnlyMatrixMetadata>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestMatrix(cubeParams)));

            _mockSqFileInterface.Setup(s => s.SerializeToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SavedQuery>()))
                .Returns(Task.CompletedTask);

            _mockSqFileInterface.Setup(s => s.SavedQueryExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(cubeParams.Count > 0);

            _mockSqFileInterface.Setup(s => s.ReadSavedQueryFromFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.Run(() => TestDataCubeBuilder.BuildTestSavedQuery(cubeParams, false, new LineChartVisualizationSettings(null, false, null))));

            return new SqController(_mockCachedDatasource.Object, _mockSqFileInterface.Object, _mockLogger.Object, _mockAuditLogService.Object, _mockWebhookService.Object);
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_NotFoundResult()
        {
            // Arrange
            SqController controller = BuildController([], []);
            ReArchiveRequest request = new()
            {
                SqId = TEST_SQ_ID
            };
            
            // Act
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            
            // Verify audit log was called with INVALID_OR_MISSING_SQID for not found queries
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/re-archive"),
                    It.Is<string>(resource => resource == LoggerConstants.INVALID_OR_MISSING_SQID)),
                Times.Once);
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_WrongChartType()
        {
            // Arrange
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
            
            // Act
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/re-archive"),
                    It.Is<string>(resource => resource == TEST_SQ_ID)),
                Times.Once);
        }

        [Test]
        public async Task ReArchiveExistingQueryAsync_Success()
        {
            // Arrange
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
            
            // Act
            ActionResult<ReArchiveResponse> result = await controller.ReArchiveExistingQueryAsync(request);

            // Assert
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.InstanceOf<ReArchiveResponse>());
            
            // Verify audit log was called with the correct parameters
            _mockAuditLogService.Verify(
                a => a.LogAuditEvent(
                    It.Is<string>(action => action == "api/sq/re-archive"),
                    It.Is<string>(resource => resource == TEST_SQ_ID)),
                Times.Once);
        }
    }
}

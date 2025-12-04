using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
using PxGraf.Services;
using PxGraf.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class ValidateTableMetaDataTests
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

        [TestCase(DimensionType.Content, DimensionType.Time, true, true, true)]
        [TestCase(DimensionType.Time, DimensionType.Unknown, false, true, true)]
        [TestCase(DimensionType.Content, DimensionType.Unknown, true, false, true)]
        [TestCase(DimensionType.Unknown, DimensionType.Unknown, false, false, true)]
        [TestCase(DimensionType.Content, DimensionType.Time, false, true, false, 0)]
        public async Task ValidateTableMetaData_ReturnsExpectedResult(
            DimensionType firstDimensionType,
            DimensionType secondDimensionType,
            bool hasContentVariable,
            bool hasTimeVariable,
            bool noZeroSizedVariables,
            int firstVariableSize = 1)
        {
            // Arrange
            List<DimensionParameters> dimParams =
            [
                new DimensionParameters(firstDimensionType, firstVariableSize),
                new DimensionParameters(secondDimensionType, 1)
            ];

            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();


            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) =>
                {
                    return TestDataCubeBuilder.BuildTestMeta(dimParams);
                });

            dataSource.Setup(ds => ds.GetMatrixCachedAsync(It.IsAny<PxTableReference>(), It.IsAny<MatrixMetadata>()))
                .ReturnsAsync((PxTableReference tableReference, MatrixMetadata metadata) =>
                {
                    return TestDataCubeBuilder.BuildTestMatrix([]);
                });

            CreationController controller = new(dataSource.Object, logger.Object, auditLogService.Object);

            // Act
            ActionResult<TableMetaValidationResult> actionResult = await controller.ValidateTableMetaData("StatFin/path/table.px");

            // Assert
            Assert.That(actionResult.Value.TableHasContentDimension, Is.EqualTo(hasContentVariable));
            Assert.That(actionResult.Value.TableHasTimeDimension, Is.EqualTo(hasTimeVariable));
            Assert.That(actionResult.Value.AllDimensionsContainValues, Is.EqualTo(noZeroSizedVariables));
        }

        [Test]
        public async Task ValidateTableMetadata_CalledForTableInUnlistedDatabase_ReturnsNotFoundResult()
        {
            // Arrange
            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            Mock<IAuditLogService> auditLogService = new();
            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) => null);
            CreationController controller = new(dataSource.Object, logger.Object, auditLogService.Object);

            // Act
            ActionResult<TableMetaValidationResult> actionResult = await controller.ValidateTableMetaData("foo/bar/baz.px");

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundResult>());
            auditLogService.Verify(a => a.LogAuditEvent(
                It.Is<string>(s => s == "api/creation/validate-table-metadata"),
                It.Is<string>(s => s == "foo/bar/baz.px")),
                Times.Once);
        }
    }
}
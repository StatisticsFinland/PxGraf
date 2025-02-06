using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Datasource;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses;
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

            CreationController controller = TestCreationControllerBuilder.BuildController([], dimParams);

            // Act
            ActionResult<TableMetaValidationResult> actionResult = await controller.ValidateTableMetaData("StatFin/path/table.px");

            // Assert
            Assert.That(actionResult.Value.TableHasContentDimension, Is.EqualTo(hasContentVariable));
            Assert.That(actionResult.Value.TableHasTimeDimension, Is.EqualTo(hasTimeVariable));
            Assert.That(actionResult.Value.AllDimensionsContainValues, Is.EqualTo(noZeroSizedVariables));
        }

        [Test]
        public async Task ValidateTableMetadata_CalledForNullTable_ReturnsExpectedResult()
        {
            // Arrange
            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) => null);
            CreationController controller = new(dataSource.Object, logger.Object);

            // Act
            ActionResult<TableMetaValidationResult> actionResult = await controller.ValidateTableMetaData("StatFin/bar/baz.px");

            // Assert
            Assert.That(actionResult.Value.TableHasContentDimension, Is.False);
            Assert.That(actionResult.Value.TableHasTimeDimension, Is.False);
            Assert.That(actionResult.Value.AllDimensionsContainValues, Is.False);
        }

        [Test]
        public async Task ValidateTableMetadata_CalledForTableInUnlistedDatabase_ReturnsNotFoundResult()
        {
            // Arrange
            Mock<ICachedDatasource> dataSource = new();
            Mock<ILogger<CreationController>> logger = new();
            dataSource.Setup(ds => ds.GetMatrixMetadataCachedAsync(It.IsAny<PxTableReference>()))
                .ReturnsAsync((PxTableReference tableReference) => null);
            CreationController controller = new(dataSource.Object, logger.Object);

            // Act
            ActionResult<TableMetaValidationResult> actionResult = await controller.ValidateTableMetaData("foo/bar/baz.px");

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
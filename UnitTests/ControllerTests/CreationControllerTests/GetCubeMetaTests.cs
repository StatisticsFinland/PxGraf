using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using Px.Utils.Models.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PxGraf.Language;
using UnitTests.Fixtures;
using PxGraf.Settings;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetCubeMetaTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            Dictionary<string, string> settings = new()
            {
                {"LocalFilesystemDatabaseConfig:Encoding", "latin1"},
                {"LocalFilesystemDatabaseConfig:DatabaseWhitelist:0", "StatFin" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
            Configuration.Load(configuration);
        }

        [Test]
        public async Task GetCubeMetaAsyncTest()
        {
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 3),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Nominal, 5),
                new DimensionParameters(DimensionType.Other, 3)
            ];

            CreationController controller = TestCreationControllerBuilder.BuildController(metaParams, metaParams, null);

            ActionResult<IReadOnlyMatrixMetadata> result = await controller.GetCubeMetaAsync("Statfin/foo");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Dimensions.Count, Is.EqualTo(4));
            Assert.That(result.Value.Dimensions[0].Type, Is.EqualTo(DimensionType.Content));
            Assert.That(result.Value.Dimensions[1].Type, Is.EqualTo(DimensionType.Time));
            Assert.That(result.Value.Dimensions[2].Type, Is.EqualTo(DimensionType.Nominal));
            Assert.That(result.Value.Dimensions[3].Type, Is.EqualTo(DimensionType.Other));
            Assert.That(result.Value.Dimensions[0].Values.Count, Is.EqualTo(3));
            Assert.That(result.Value.Dimensions[1].Values.Count, Is.EqualTo(12));
            Assert.That(result.Value.Dimensions[2].Values.Count, Is.EqualTo(5));
            Assert.That(result.Value.Dimensions[3].Values.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetCubeMetaAsync_WithUnallowedDatabase_ReturnsNotFoundResult()
        {
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 3),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Nominal, 5),
                new DimensionParameters(DimensionType.Other, 3)
            ];

            CreationController controller = TestCreationControllerBuilder.BuildController(metaParams, metaParams, null);

            ActionResult<IReadOnlyMatrixMetadata> result = await controller.GetCubeMetaAsync("foo/bar");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }
    }
}

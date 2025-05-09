using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using PxGraf.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PxGraf.Language;
using UnitTests.Fixtures;
using PxGraf.Settings;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetEditorContentsTests
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
        public async Task GetEditorContents_SimpleSuccessTest_LineChart()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);
            Assert.That(editorContent, Is.Not.Null);
        }

        [Test]
        public async Task GetEditorContents_QueryWithoutDimensions_ReturnsEmpty()
        {
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController([], metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery([]);

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            Assert.That(editorContent, Is.Not.Null);
            Assert.That(editorContent.Value, Is.Not.Null);
            Assert.That(editorContent.Value.Size.Equals(0), Is.True);
        }

        [Test]
        public async Task GetEditorContents_QueryWithZeroSize_ReturnsEmpty()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Other, 0), // Zero size dimension query
                new DimensionParameters(DimensionType.Other, 2) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Other, 6),
                new DimensionParameters(DimensionType.Other, 7),
                new DimensionParameters(DimensionType.Other, 4),
            ];


            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            Assert.That(editorContent, Is.Not.Null);
            Assert.That(editorContent.Value, Is.Not.Null);
            Assert.That(editorContent.Value.Size.Equals(0), Is.True);
        }
    }
}

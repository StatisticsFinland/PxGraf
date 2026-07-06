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
using Moq;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using PxGraf.Services;

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
            Assert.That(editorContent.Value, Is.Not.Null);
            Assert.That(editorContent.Value.PublicationWebhookEnabled, Is.Not.Null);
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
            Assert.That(editorContent.Value.PublicationWebhookEnabled, Is.Not.Null);
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
            Assert.That(editorContent.Value.PublicationWebhookEnabled, Is.Not.Null);
        }

        [Test]
        public async Task GetEditorContents_ExceedinglyLargeQuery_ReturnsWithNoValidVisualizations()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 96),
                new DimensionParameters(DimensionType.Other, 100),
                new DimensionParameters(DimensionType.Other, 100),
                new DimensionParameters(DimensionType.Other, 10) { Selectable = true},
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 5),
                new DimensionParameters(DimensionType.Time, 192),
                new DimensionParameters(DimensionType.Other, 128),
                new DimensionParameters(DimensionType.Other, 128),
                new DimensionParameters(DimensionType.Other, 16),
            ];


            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);
            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            Assert.That(editorContent, Is.Not.Null);
            Assert.That(editorContent.Value, Is.Not.Null);
            Assert.That(editorContent.Value.Size.Equals(9600000), Is.True);
            Assert.That(editorContent.Value.VisualizationOptions.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEditorContents_WithOnlyVirtualValuesSelected_ReturnsVisualizationOptions()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            // The post-virtual matrix has three values in variable-2: value-0, value-1 (real operands) and value-2 (virtual).
            // value-2 is used as the virtual value code because it is absent from metaParams other(2).
            List<DimensionParameters> postVirtualParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 3),
            ];

            Matrix<DecimalDataValue> matrixWithVirtual = TestDataCubeBuilder.BuildTestMatrix(postVirtualParams);

            Mock<IVirtualValueComputationService> virtualValueMock = new();
            virtualValueMock
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns(matrixWithVirtual);

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams, virtualValueComputationService: virtualValueMock);

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            cubeQuery.DimensionQueries["variable-2"] = new DimensionQuery
            {
                ValueFilter = new ItemFilter(["value-2"]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "value-2", OperandCodes = ["value-0", "value-1"] }
                ]
            };

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(editorContent.Value, Is.Not.Null);
                Assert.That(editorContent.Value.VisualizationOptions, Is.Not.Empty);
                Assert.That(editorContent.Value.Size, Is.GreaterThan(0));
            }
        }

        [Test]
        public async Task GetEditorContents_WithVirtualAndRealValues_SizeReflectsTrimmedMatrix()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            // The post-virtual matrix has value-0 (real, user-selected), value-1 (real, operand-only), value-2 (virtual).
            List<DimensionParameters> postVirtualParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 3),
            ];

            Matrix<DecimalDataValue> matrixWithVirtual = TestDataCubeBuilder.BuildTestMatrix(postVirtualParams);

            Mock<IVirtualValueComputationService> virtualValueMock = new();
            virtualValueMock
                .Setup(s => s.ApplyVirtualValues(
                    It.IsAny<Matrix<DecimalDataValue>>(),
                    It.IsAny<MatrixQuery>()))
                .Returns(matrixWithVirtual);

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams, virtualValueComputationService: virtualValueMock);

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            cubeQuery.DimensionQueries["variable-2"] = new DimensionQuery
            {
                ValueFilter = new ItemFilter(["value-0", "value-2"]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "value-2", OperandCodes = ["value-0", "value-1"] }
                ]
            };

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(editorContent.Value, Is.Not.Null);
                // After trimming: variable-2 retains value-0 (real, selected) + value-2 (virtual).
                // value-1 is operand-only and must be removed.
                // Total: content(1) × time(8) × other(2) = 16
                Assert.That(editorContent.Value.Size, Is.EqualTo(16));
            }
        }

        [Test]
        public async Task GetEditorContents_VirtualValue_ItemFilter_EmptySelection_ReturnsSizeZero()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 8),
                new DimensionParameters(DimensionType.Other, 2),
            ];

            CreationController testController = TestCreationControllerBuilder.BuildController(cubeParams, metaParams);

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(cubeParams);
            // Key scenario: ItemFilter is empty (no real values selected)
            // VirtualValueDefinitions has one entry with code "virtual_1"
            // But ItemFilter.Codes does NOT include "virtual_1"
            // Result: effective output count = 0, should return Size = 0 (not crash, not auto-select)
            cubeQuery.DimensionQueries["variable-2"] = new DimensionQuery
            {
                ValueFilter = new ItemFilter([]),
                VirtualValueDefinitions =
                [
                    new SumDefinition { Code = "virtual_1", OperandCodes = ["value-0", "value-1"] }
                ]
            };

            ActionResult<EditorContentsResponse> editorContent = await testController.GetEditorContents(cubeQuery);

            Assert.That(editorContent, Is.Not.Null);
            Assert.That(editorContent.Value, Is.Not.Null);
            Assert.That(editorContent.Value.Size, Is.EqualTo(0));
        }
    }
}

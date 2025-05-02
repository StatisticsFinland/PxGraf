using NUnit.Framework;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using PxGraf.Controllers;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PxGraf.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetEditorContentsTests
    {

        [Test]
        public async Task GetEditorContents_SimpleSuccessTest_LineChart()
        {
            List<DimensionParameters> cubeParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
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
    }
}

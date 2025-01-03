using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Controllers;
using PxGraf.Language;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace UnitTests.ControllerTests.CreationControllerTests
{
    public class GetDefaultHeaderTests
    {
        public GetDefaultHeaderTests()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public async Task GetDefaultHeaderTest()
        {
            // Arrange
            List<DimensionParameters> metaParams =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 12),
                new DimensionParameters(DimensionType.Nominal, 3),
                new DimensionParameters(DimensionType.Other, 1)
            ];

            CreationController controller = TestCreationControllerBuilder.BuildController(metaParams, metaParams, null);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);

            // Act
            ActionResult<MultilanguageString> result = await controller.GetDefaultHeaderAsync(query);
        
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value["fi"], Is.EqualTo("value-0, value-0 [FIRST]-[LAST] muuttujana variable-2"));
            Assert.That(result.Value["en"], Is.EqualTo("value-0.en, value-0.en in [FIRST] to [LAST] by variable-2.en"));
        }
    }
}

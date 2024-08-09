using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System.Collections.Generic;
using UnitTests;
using UnitTests.Fixtures;

namespace DataCubeTests
{
    internal class MatrixMetadataHeaderGenerationTests
    {
        public MatrixMetadataHeaderGenerationTests()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void CreateDefaultChartHeader_1TimeVar_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 1),
                new (DimensionType.Other, 4),
                new (DimensionType.Other, 2)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[2].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));
            cubeMeta.Dimensions[3].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 [FIRST] muuttujina foobar 2, foobar 3";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_MultipleTimeVars_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 10),
                new (DimensionType.Other, 4),
                new (DimensionType.Other, 2)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[2].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));
            cubeMeta.Dimensions[3].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujina foobar 2, foobar 3";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_1ContentVar1Value_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 5),
                new (DimensionType.Other, 1)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[^1].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));

            string expectedResult = "foobar 1, foobar 2 [FIRST]-[LAST]";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_1ContentVarMultipleValues_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 5),
                new (DimensionType.Other, 4)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[^1].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujana variable-2";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_1TimeVarMultipleValues_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 1),
                new (DimensionType.Other, 1),
                new (DimensionType.Other, 4),
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[2].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));
            cubeMeta.Dimensions[3].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1, foobar 2 [FIRST] muuttujana variable-3";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_SumType_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 5),
                new (DimensionType.Other, 1) { HasCombinationValue = true }
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));

            string expectedResult = "foobar 1 [FIRST]-[LAST]";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_SelectableTimeValue_ReturnsCorrectHeader()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 10) { Selectable = true },
                new (DimensionType.Other, 4),
                new (DimensionType.Other, 2)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 1"));
            cubeMeta.Dimensions[2].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 2"));
            cubeMeta.Dimensions[3].Name.CopyAndEdit(new MultilanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 muuttujina foobar 2, foobar 3";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateDefaultChartHeader_MultipleTimeVars_ReturnsCorrectHeaderForEn()
        {
            List<DimensionParameters> metaParams =
            [
                new (DimensionType.Content, 1),
                new (DimensionType.Time, 10),
                new (DimensionType.Other, 4),
                new (DimensionType.Other, 2)
            ];

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Dimensions[0].Values[0].Name.CopyAndEdit(new MultilanguageString("en", "foobar 1.en"));
            cubeMeta.Dimensions[2].Name.CopyAndEdit(new MultilanguageString("en", "foobar 2.en"));
            cubeMeta.Dimensions[3].Name.CopyAndEdit(new MultilanguageString("en", "foobar 3.en"));

            string expectedResult = "foobar 1.en in [FIRST] to [LAST] by foobar 2.en, foobar 3.en";
            MultilanguageString header = cubeMeta.ToQueriedCubeMeta(cubeQuery).Header;
            string result = header["fi"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}

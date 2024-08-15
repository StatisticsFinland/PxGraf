using NUnit.Framework;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Language;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using UnitTests;
using UnitTests.Fixtures;

namespace DataCubeTests
{
    internal class MatrixMetadataHeaderGenerationTests
    {
        private readonly string[] _languages = [ "fi", "en" ];

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

            Dictionary<string, MultilanguageString> dimensionNameEdits = new()
            {
                ["variable-2"] = new("fi", "foobar 2"),
                ["variable-3"] = new("fi", "foobar 3")
            };

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") }
            };

            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            query.ApplyNameEdits(meta, dimensionNameEdits, valueNameEdits);

            string expectedResult = "foobar 1 [FIRST] muuttujina foobar 2, foobar 3";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(meta.Dimensions, query, _languages);
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

            Dictionary<string, MultilanguageString> dimensionNameEdits = new()
            {
                ["variable-2"] = new("fi", "foobar 2"),
                ["variable-3"] = new("fi", "foobar 3")
            };

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, dimensionNameEdits, valueNameEdits);

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujina foobar 2, foobar 3";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") },
                ["variable-2"] = new() { ["value-0"] = new("fi", "foobar 2") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, valueNameEdits: valueNameEdits);

            string expectedResult = "foobar 1, foobar 2 [FIRST]-[LAST]";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") },
                ["variable-2"] = new() { ["value-0"] = new("fi", "foobar 2") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, valueNameEdits: valueNameEdits);

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujana variable-2";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") },
                ["variable-2"] = new() { ["value-0"] = new("fi", "foobar 2") },
                ["variable-3"] = new() { ["value-0"] = new("fi", "foobar 3") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, valueNameEdits: valueNameEdits);

            string expectedResult = "foobar 1, foobar 2 [FIRST] muuttujana variable-3";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, valueNameEdits: valueNameEdits);

            string expectedResult = "foobar 1 [FIRST]-[LAST]";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, MultilanguageString> dimensionNameEdits = new()
            {
                ["variable-2"] = new("fi", "foobar 2"),
                ["variable-3"] = new("fi", "foobar 3")
            };

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("fi", "foobar 1") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, dimensionNameEdits, valueNameEdits);

            string expectedResult = "foobar 1 muuttujina foobar 2, foobar 3";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
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

            Dictionary<string, MultilanguageString> dimensionNameEdits = new()
            {
                ["variable-2"] = new("en", "foobar 2.en"),
                ["variable-3"] = new("en", "foobar 3.en")
            };

            Dictionary<string, Dictionary<string, MultilanguageString>> valueNameEdits = new()
            {
                ["variable-0"] = new() { ["value-0"] = new("en", "foobar 1.en") }
            };

            MatrixQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            MatrixMetadata cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeQuery.ApplyNameEdits(cubeMeta, dimensionNameEdits, valueNameEdits);

            string expectedResult = "foobar 1.en in [FIRST] to [LAST] by foobar 2.en, foobar 3.en";
            MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(cubeMeta.Dimensions, cubeQuery, _languages);
            string result = header["en"];

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}

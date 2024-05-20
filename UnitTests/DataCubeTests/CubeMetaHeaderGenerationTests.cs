using NUnit.Framework;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace DataCubeTests
{
    internal class CubeMetaHeaderGenerationTests
    {
        public CubeMetaHeaderGenerationTests()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void CreateDefaultChartHeader_1TimeVar_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 1),
                new (VariableType.OtherClassificatory, 4),
                new (VariableType.OtherClassificatory, 2)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[2].Name.Edit(new MultiLanguageString("fi", "foobar 2"));
            cubeMeta.Variables[3].Name.Edit(new MultiLanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 [FIRST] muuttujina foobar 2, foobar 3";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_MultipleTimeVars_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 10),
                new (VariableType.OtherClassificatory, 4),
                new (VariableType.OtherClassificatory, 2)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[2].Name.Edit(new MultiLanguageString("fi", "foobar 2"));
            cubeMeta.Variables[3].Name.Edit(new MultiLanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujina foobar 2, foobar 3";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_1ContentVar1Value_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 5),
                new (VariableType.OtherClassificatory, 1)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[^1].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 2"));

            string expectedResult = "foobar 1, foobar 2 [FIRST]-[LAST]";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_1ContentVarMultipleValues_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 5),
                new (VariableType.OtherClassificatory, 4)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[^1].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 2"));

            string expectedResult = "foobar 1 [FIRST]-[LAST] muuttujana variable-2";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_1TimeVarMultipleValues_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 1),
                new (VariableType.OtherClassificatory, 1),
                new (VariableType.OtherClassificatory, 4),
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[2].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 2"));
            cubeMeta.Variables[3].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1, foobar 2 [FIRST] muuttujana variable-3";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_SumType_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 5),
                new (VariableType.OtherClassificatory, 1) { HasCombinationValue = true }
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));

            string expectedResult = "foobar 1 [FIRST]-[LAST]";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_SelectableTimeValue_ReturnsCorrectHeader()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 10) { Selectable = true },
                new (VariableType.OtherClassificatory, 4),
                new (VariableType.OtherClassificatory, 2)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("fi", "foobar 1"));
            cubeMeta.Variables[2].Name.Edit(new MultiLanguageString("fi", "foobar 2"));
            cubeMeta.Variables[3].Name.Edit(new MultiLanguageString("fi", "foobar 3"));

            string expectedResult = "foobar 1 muuttujina foobar 2, foobar 3";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("fi", out string result);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDefaultChartHeader_MultipleTimeVars_ReturnsCorrectHeaderForEn()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 10),
                new (VariableType.OtherClassificatory, 4),
                new (VariableType.OtherClassificatory, 2)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.Variables[0].IncludedValues[0].Name.Edit(new MultiLanguageString("en", "foobar 1.en"));
            cubeMeta.Variables[2].Name.Edit(new MultiLanguageString("en", "foobar 2.en"));
            cubeMeta.Variables[3].Name.Edit(new MultiLanguageString("en", "foobar 3.en"));

            string expectedResult = "foobar 1.en in [FIRST] to [LAST] by foobar 2.en, foobar 3.en";
            cubeMeta.CreateDefaultChartHeader(cubeQuery).TryGetLanguage("en", out string result);

            Assert.AreEqual(expectedResult, result);
        }
    }
}

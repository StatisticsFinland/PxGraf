using NUnit.Framework;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace DataCubeTests
{
    [TestFixture]
    internal class CubeMetaUtilitiesTests
    {
        [Test]
        public void GetNumberOfMultivalueVariablesTest_3()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            int numOfMultivalue = meta.GetNumberOfMultivalueVariables();
            Assert.AreEqual(3, numOfMultivalue);
        }

        [Test]
        public void GetNumberOfMultivalueVariablesTest_0()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            int numOfMultivalue = meta.GetNumberOfMultivalueVariables();
            Assert.AreEqual(0, numOfMultivalue);
        }

        [Test]
        public void GetMultivalueVariablesTest_3()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyList<IReadOnlyVariable> multivalueVars = meta.GetMultivalueVariables();
            Assert.AreEqual(3, multivalueVars.Count);

            List<IReadOnlyVariable> expected = [meta.Variables[1], meta.Variables[2], meta.Variables[3]];
            Assert.AreEqual(expected, multivalueVars);

            //Time, 2
            Assert.AreEqual(VariableType.Time, multivalueVars[0].Type);
            Assert.AreEqual(2, multivalueVars[0].IncludedValues.Count);

            //OtherClassificatory, 5
            Assert.AreEqual(VariableType.OtherClassificatory, multivalueVars[1].Type);
            Assert.AreEqual(5, multivalueVars[1].IncludedValues.Count);

            //Unknown, 3
            Assert.AreEqual(VariableType.Unknown, multivalueVars[2].Type);
            Assert.AreEqual(3, multivalueVars[2].IncludedValues.Count);

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2", "value-3", "value-4"];
            Assert.AreEqual(expectedValues, multivalueVars[1].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void GetMultivalueVariablesTest_0()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyList<IReadOnlyVariable> multivalueVars = meta.GetMultivalueVariables();
            Assert.IsEmpty(multivalueVars);
        }

        [Test]
        public void GetSortedMultivalueVariablesTest_3()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyList<IReadOnlyVariable> multivalueVars = meta.GetSortedMultivalueVariables();
            Assert.AreEqual(3, multivalueVars.Count);

            List<IReadOnlyVariable> expected = [meta.Variables[2], meta.Variables[3], meta.Variables[1]];
            Assert.AreEqual(expected, multivalueVars);

            //OtherClassificatory, 5
            Assert.AreEqual(VariableType.OtherClassificatory, multivalueVars[0].Type);
            Assert.AreEqual(5, multivalueVars[0].IncludedValues.Count);

            //Unknown, 3
            Assert.AreEqual(VariableType.Unknown, multivalueVars[1].Type);
            Assert.AreEqual(3, multivalueVars[1].IncludedValues.Count);

            //Time, 2
            Assert.AreEqual(VariableType.Time, multivalueVars[2].Type);
            Assert.AreEqual(2, multivalueVars[2].IncludedValues.Count);

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2"];
            Assert.AreEqual(expectedValues, multivalueVars[1].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void GetSortedMultivalueVariablesTest_0()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyList<IReadOnlyVariable> multivalueVars = meta.GetSortedMultivalueVariables();
            Assert.IsEmpty(multivalueVars);
        }

        [Test]
        public void GetLargestMultivalueVariableTest_3()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable largest = meta.GetLargestMultivalueVariable();
            Assert.AreSame(meta.Variables[2], largest);
        }

        [Test]
        public void GetLargestMultivalueVariableTest_3_values()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable largest = meta.GetLargestMultivalueVariable();
            Assert.NotNull(largest);

            Assert.AreEqual(VariableType.OtherClassificatory, largest.Type);
            Assert.AreEqual(5, largest.IncludedValues.Count);

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2", "value-3", "value-4"];
            Assert.AreEqual(expectedValues, largest.IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void GetLargestMultivalueVariableTest_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable largest = meta.GetLargestMultivalueVariable();
            Assert.IsNull(largest);
        }

        [Test]
        public void GetSmallerMultivalueVariableTest_0_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable smaller = meta.GetSmallerMultivalueVariable();
            Assert.IsNull(smaller);
        }

        [Test]
        public void GetSmallerMultivalueVariableTest_1_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 1),
                new VariableParameters(VariableType.Time, 5)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable smaller = meta.GetSmallerMultivalueVariable();
            Assert.IsNull(smaller);
        }

        [Test]
        public void GetSmallerMultivalueVariableTest_1_3()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Geological, 3),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable smaller = meta.GetSmallerMultivalueVariable();
            Assert.AreSame(meta.Variables[2], smaller);
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalTest_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable largest = meta.GetMultivalueTimeOrLargestOrdinal();
            Assert.IsNull(largest);
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalTest_1t_1()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable mvtolo = meta.GetMultivalueTimeOrLargestOrdinal();
            Assert.AreSame(meta.Variables[1], mvtolo);
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalTest_2_t()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 3),
                new VariableParameters(VariableType.Ordinal, 4),
            ];

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var mvtolo = meta.GetMultivalueTimeOrLargestOrdinal();
            Assert.AreSame(meta.Variables[1], mvtolo);
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalTest_2_1()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Ordinal, 4),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable mvtolo = meta.GetMultivalueTimeOrLargestOrdinal();
            Assert.AreSame(meta.Variables[1], mvtolo);
        }

        [Test]
        public void GetMultivalueTimeOrLargestOrdinalTest_2_2()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Ordinal, 5),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable mvtolo = meta.GetMultivalueTimeOrLargestOrdinal();
            Assert.AreSame(meta.Variables[2], mvtolo);
        }

        [Test]
        public void GetContentVariableTest_2_throw()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Content, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            Assert.Throws<ArgumentException>(() => meta.GetContentVariable());
        }

        [Test]
        public void GetContentVariableTest_0_throw()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Ordinal, 5)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            Assert.Throws<ArgumentException>(() => meta.GetContentVariable());
        }

        [Test]
        public void GetContentVariableTest_1_2()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Content, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Time, 2)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable contentVar = meta.GetContentVariable();
            Assert.AreSame(meta.Variables[1], contentVar);
        }


        [Test]
        public void GetTimeVariableTest_2_throw()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            Assert.Throws<ArgumentException>(() => meta.GetTimeVariable());
        }

        [Test]
        public void GetTimeVariableTest_0_throw()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Content, 5)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            Assert.Throws<ArgumentException>(() => meta.GetTimeVariable());
        }

        [Test]
        public void GetTimeVariableTest_1_2()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Content, 2)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            IReadOnlyVariable timeVar = meta.GetTimeVariable();
            Assert.AreSame(meta.Variables[1], timeVar);
        }

        [Test]
        public void GetUnambiguousUnitsInLangTest_TestUnit()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Content, 3) { SameUnit = true }
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            string unit = meta.GetUnambiguousUnitsInLang("fi");
            Assert.AreEqual("testUnit", unit);
        }

        [Test]
        public void GetUnambiguousUnitsInLangTest_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Content, 3) { SameUnit = false }
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            string unit = meta.GetUnambiguousUnitsInLang("fi");
            Assert.IsNull(unit);
        }

        [Test]
        public void GetUnambiguoussourceInLangTest_TestSource()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Content, 3) { SameSource = true }
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            string unit = meta.GetUnambiguousSourceInLang("fi");
            Assert.AreEqual("testSource", unit);
        }

        [Test]
        public void GetUnambiguousSourceInLangTest_Null()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Ordinal, 4),
                new VariableParameters(VariableType.Time, 4),
                new VariableParameters(VariableType.OtherClassificatory, 5),
                new VariableParameters(VariableType.Content, 3) { SameSource = false }
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            string unit = meta.GetUnambiguousSourceInLang("fi");
            Assert.IsNull(unit);
        }

        [Test]
        public void GetHeaderWithoutTimePlaceholdersTest_FirstAndLast()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 5),
                new (VariableType.OtherClassificatory, 4)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.ApplyEditionFromQuery(cubeQuery);

            Assert.AreEqual("value-0 2000-2004 muuttujana variable-2", cubeMeta.GetHeaderWithoutTimePlaceholders()["fi"]);
        }

        [Test]
        public void GetHeaderWithoutTimePlaceholdersTest_FirstOnly()
        {
            List<VariableParameters> metaParams =
            [
                new (VariableType.Content, 1),
                new (VariableType.Time, 1),
                new (VariableType.OtherClassificatory, 5)
            ];

            CubeQuery cubeQuery = TestDataCubeBuilder.BuildTestCubeQuery(metaParams);
            CubeMeta cubeMeta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            cubeMeta.ApplyEditionFromQuery(cubeQuery);

            Assert.AreEqual("value-0 2000 muuttujana variable-2", cubeMeta.GetHeaderWithoutTimePlaceholders()["fi"]);
        }
    }
}

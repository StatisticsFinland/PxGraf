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
            Assert.That(numOfMultivalue, Is.EqualTo(3));
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
            Assert.That(numOfMultivalue, Is.EqualTo(0));
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
            Assert.That(multivalueVars.Count, Is.EqualTo(3));

            List<IReadOnlyVariable> expected = [meta.Variables[1], meta.Variables[2], meta.Variables[3]];
            Assert.That(multivalueVars, Is.EquivalentTo(expected));

            //Time, 2
            Assert.That(multivalueVars[0].Type, Is.EqualTo(VariableType.Time));
            Assert.That(multivalueVars[0].IncludedValues.Count, Is.EqualTo(2));

            //OtherClassificatory, 5
            Assert.That(multivalueVars[1].Type, Is.EqualTo(VariableType.OtherClassificatory));
            Assert.That(multivalueVars[1].IncludedValues.Count, Is.EqualTo(5));

            //Unknown, 3
            Assert.That(multivalueVars[2].Type, Is.EqualTo(VariableType.Unknown));
            Assert.That(multivalueVars[2].IncludedValues.Count, Is.EqualTo(3));

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2", "value-3", "value-4"];
            Assert.That(multivalueVars[1].IncludedValues.Select(vv => vv.Code), Is.EquivalentTo(expectedValues));
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
            Assert.That(multivalueVars, Is.Empty);
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
            Assert.That(multivalueVars.Count, Is.EqualTo(3));

            List<IReadOnlyVariable> expected = [meta.Variables[2], meta.Variables[3], meta.Variables[1]];
            Assert.That(multivalueVars, Is.EquivalentTo(expected));

            //OtherClassificatory, 5
            Assert.That(multivalueVars[0].Type, Is.EqualTo(VariableType.OtherClassificatory));
            Assert.That(multivalueVars[0].IncludedValues.Count, Is.EqualTo(5));

            //Unknown, 3
            Assert.That(multivalueVars[1].Type, Is.EqualTo(VariableType.Unknown));
            Assert.That(multivalueVars[1].IncludedValues.Count, Is.EqualTo(3));

            //Time, 2
            Assert.That(multivalueVars[2].Type, Is.EqualTo(VariableType.Time));
            Assert.That(multivalueVars[2].IncludedValues.Count, Is.EqualTo(2));

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2"];
            Assert.That(multivalueVars[1].IncludedValues.Select(vv => vv.Code), Is.EquivalentTo(expectedValues));
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
            Assert.That(multivalueVars, Is.Empty);
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
            Assert.That(meta.Variables[2], Is.SameAs(meta.GetLargestMultivalueVariable()));
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
            Assert.That(largest, Is.Not.Null);

            Assert.That(largest.Type, Is.EqualTo(VariableType.OtherClassificatory));
            Assert.That(largest.IncludedValues.Count, Is.EqualTo(5));

            //Variable values in order
            IEnumerable<string> expectedValues = ["value-0", "value-1", "value-2", "value-3", "value-4"];
            Assert.That(largest.IncludedValues.Select(vv => vv.Code), Is.EquivalentTo(expectedValues));
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
            Assert.That(largest, Is.Null);
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
            Assert.That(smaller, Is.Null);
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
            Assert.That(smaller, Is.Null);
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
            Assert.That(smaller, Is.SameAs(meta.Variables[2]));
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
            Assert.That(largest, Is.Null);
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
            Assert.That(mvtolo, Is.SameAs(meta.Variables[1]));
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
            Assert.That(mvtolo, Is.SameAs(meta.Variables[1]));
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
            Assert.That(mvtolo, Is.SameAs(meta.Variables[1]));
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
            Assert.That(mvtolo, Is.SameAs(meta.Variables[2]));
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
            Assert.That(contentVar, Is.SameAs(meta.Variables[1]));
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
            Assert.That(timeVar, Is.SameAs(meta.Variables[1]));
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
            Assert.That(unit, Is.EqualTo("testUnit"));
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
            Assert.That(unit, Is.Null);
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
            string source = meta.GetUnambiguousSourceInLang("fi");
            Assert.That(source, Is.EqualTo("testSource"));
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
            Assert.That(unit, Is.Null);
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

            Assert.That(cubeMeta.GetHeaderWithoutTimePlaceholders()["fi"], Is.EqualTo("value-0 2000-2004 muuttujana variable-2"));
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

            Assert.That(cubeMeta.GetHeaderWithoutTimePlaceholders()["fi"], Is.EqualTo("value-0 2000 muuttujana variable-2"));
        }
    }
}

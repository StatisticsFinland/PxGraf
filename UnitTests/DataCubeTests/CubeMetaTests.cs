using NUnit.Framework;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace DataCubeTests
{
    internal class CubeMetaTests
    {
        [Test]
        public void ApplyEditionFromQueryTest_NoExceptions_With_Valid_Query()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            Assert.DoesNotThrow(() => meta.ApplyEditionFromQuery(query));
        }

        [Test]
        public void ApplyEditionsFromQueryTest_Meta_Has_More_Variables_Than_Query_Throw_ArgumentException()
        {
            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            List<VariableParameters> queryParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(queryParams);

            Assert.Throws<ArgumentException>(() => meta.ApplyEditionFromQuery(query));
        }

        [Test]
        public void ApplyEditionsFromQueryTest_Meta_Has_Different_Variables_Than_Query_Throw_ArgumentException()
        {
            List<VariableParameters> metaParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3)
            ];

            List<VariableParameters> queryParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3)
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(metaParams);
            meta.Variables[0] = TestDataCubeBuilder.BuildTestVariable("Foobar", new VariableParameters(VariableType.Unknown, 3));

            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(queryParams);

            Assert.Throws<ArgumentException>(() => meta.ApplyEditionFromQuery(query));
        }
    }
}

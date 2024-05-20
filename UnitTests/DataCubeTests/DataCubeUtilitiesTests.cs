using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Utility;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;
using UnitTests.Utilities;

namespace DataCubeTests
{
    [TestFixture]
    internal class DataCubeUtilitiesTests
    {
        [Test]
        public void ApplySelectableVariableSelectionsDataTest1()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            Dictionary<string, IReadOnlyList<string>> selections = new()
            {
                ["variable-3"] = ["value-0"]
            };
            cube = DataCubeUtilities.ApplySelectableVariableSelections(cube, selections);
            IReadOnlyList<DataValue> expected = DataValueUtilities.List(0.123, 3.123, 6.123, 9.123, 12.123, 15.123);

            Assert.IsTrue(DataValueUtilities.Compare(expected, cube.Data, out string message), message);
        }

        [Test]
        public void ApplySelectableVariableSelectionsDataTest2()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            Dictionary<string, IReadOnlyList<string>> selections = new()
            {
                ["variable-2"] = ["value-0", "value-2"]
            };
            cube = DataCubeUtilities.ApplySelectableVariableSelections(cube, selections);
            IReadOnlyList<DataValue> expected = DataValueUtilities.List(0.123, 1.123, 2.123, 6.123, 7.123, 8.123, 12.123, 13.123, 14.123, 18.123, 19.123, 20.123);

            Assert.IsTrue(DataValueUtilities.Compare(expected, cube.Data, out string message), message);
        }
    }
}

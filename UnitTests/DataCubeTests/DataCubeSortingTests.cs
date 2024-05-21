using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;
using UnitTests.Utilities;

namespace DataCubeTests
{
    internal class DataCubeSortingTests
    {
        [Test]
        public void DataCubeSortingTest_GHBC_value1_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            GroupHorizontalBarChartVisualizationSettings ghbcs = new(null, "value-1");
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(10.123, 9.123, 11.123, 7.123, 6.123, 8.123, 4.123, 3.123, 5.123, 1.123, 0.123, 2.123);
            List<string> expectedVarValueOrder1 = ["value-3", "value-2", "value-1", "value-0"];
            List<string> expectedVarValueOrder2 = ["value-1", "value-0", "value-2"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_GHBC_value1_Pivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            GroupHorizontalBarChartVisualizationSettings ghbcs = new(null, "value-1");
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, true);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(5.123, 4.123, 3.123, 2.123, 1.123, 0.123, 8.123, 7.123, 6.123, 11.123, 10.123, 9.123);
            List<string> expectedVarValueOrder1 = ["value-1", "value-0", "value-2", "value-3"];
            List<string> expectedVarValueOrder2 = ["value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_GHBC_sum_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            GroupHorizontalBarChartVisualizationSettings ghbcs = new(null, CubeSorting.SUM);
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(9.123, 10.123, 11.123, 6.123, 7.123, 8.123, 3.123, 4.123, 5.123, 0.123, 1.123, 2.123);
            List<string> expectedVarValueOrder1 = ["value-3", "value-2", "value-1", "value-0"];
            List<string> expectedVarValueOrder2 = ["value-0", "value-1", "value-2"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value0_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, "value-0");
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(2.123, 1.123, 0.123, 5.123, 4.123, 3.123, 8.123, 7.123, 6.123);
            List<string> expectedVarValueOrder1 = ["value-0", "value-1", "value-2"];
            List<string> expectedVarValueOrder2 = ["value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value1_Pivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, "value-1");
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, true);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(7.123, 6.123, 8.123, 4.123, 3.123, 5.123, 1.123, 0.123, 2.123);
            List<string> expectedVarValueOrder1 = ["value-2", "value-1", "value-0"];
            List<string> expectedVarValueOrder2 = ["value-1", "value-0", "value-2"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value2_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, "value-2");
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(8.123, 7.123, 6.123, 2.123, 1.123, 0.123, 5.123, 4.123, 3.123);
            List<string> expectedVarValueOrder1 = ["value-2", "value-0", "value-1"];
            List<string> expectedVarValueOrder2 = ["value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_sum_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, CubeSorting.SUM);
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(2.123, 1.123, 0.123, 5.123, 4.123, 3.123, 8.123, 7.123, 6.123);
            List<string> expectedVarValueOrder1 = ["value-0", "value-1", "value-2"];
            List<string> expectedVarValueOrder2 = ["value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), Is.True, message);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_no_sorting_NoPivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, CubeSorting.NO_SORTING);
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123);
            List<string> expectedVarValueOrder1 = ["value-0", "value-1", "value-2"];
            List<string> expectedVarValueOrder2 = ["value-0", "value-1", "value-2"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), Is.True, message2);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_no_sorting_Pivot()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            StackedHorizontalBarChartVisualizationSettings shbcs = new(null, CubeSorting.NO_SORTING);
            DataCube result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, true);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123);
            List<string> expectedVarValueOrder1 = ["value-0", "value-1", "value-2"];
            List<string> expectedVarValueOrder2 = ["value-0", "value-1", "value-2"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), Is.True, message2);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder1));
            Assert.That(result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder2));
        }

        [Test]
        public void DataCubeSortingTest_HBC_no_sorting()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            HorizontalBarChartVisualizationSettings hbcs = new(null, CubeSorting.NO_SORTING);
            DataCube result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123);
            List<string> expectedVarValueOrder = ["value-0", "value-1", "value-2", "value-3", "value-4"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), Is.True, message2);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder));
        }

        [Test]
        public void DataCubeSortingTest_HBC_ascending()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            HorizontalBarChartVisualizationSettings hbcs = new(null, CubeSorting.ASCENDING);
            DataCube result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123);
            List<string> expectedVarValueOrder = ["value-0", "value-1", "value-2", "value-3", "value-4"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder));
        }

        [Test]
        public void DataCubeSortingTest_HBC_descending()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            HorizontalBarChartVisualizationSettings hbcs = new(null, CubeSorting.DESCENDING);
            DataCube result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(4.123, 3.123, 2.123, 1.123, 0.123);
            List<string> expectedVarValueOrder = ["value-4", "value-3", "value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder));
        }

        [Test]
        public void DataCubeSortingTest_PC_descending()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            ];

            DataCube cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            PieChartVisualizationSettings pcs = new(null, CubeSorting.DESCENDING);
            DataCube result = CubeSorting.SortOneDimensionalCharts(cube, pcs.Sorting);
            IReadOnlyList<DataValue> expectedDataOrder = DataValueUtilities.List(4.123, 3.123, 2.123, 1.123, 0.123);
            List<string> expectedVarValueOrder = ["value-4", "value-3", "value-2", "value-1", "value-0"];

            Assert.That(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), Is.True, message1);
            Assert.That(result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code), Is.EqualTo(expectedVarValueOrder));
        }
    }
}

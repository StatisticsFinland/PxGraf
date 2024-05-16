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
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var ghbcs = new GroupHorizontalBarChartVisualizationSettings(null, "value-1");
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(10.123, 9.123, 11.123, 7.123, 6.123, 8.123, 4.123, 3.123, 5.123, 1.123, 0.123, 2.123);
            var expectedVarValueOrder1 = new List<string>() { "value-3", "value-2", "value-1", "value-0" };
            var expectedVarValueOrder2 = new List<string>() { "value-1", "value-0", "value-2" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_GHBC_value1_Pivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var ghbcs = new GroupHorizontalBarChartVisualizationSettings(null, "value-1");
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, true);
            var expectedDataOrder = DataValueUtilities.List(5.123, 4.123, 3.123, 2.123, 1.123, 0.123, 8.123, 7.123, 6.123, 11.123, 10.123, 9.123);
            var expectedVarValueOrder1 = new List<string>() { "value-1", "value-0", "value-2", "value-3" };
            var expectedVarValueOrder2 = new List<string>() { "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_GHBC_sum_NoPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var ghbcs = new GroupHorizontalBarChartVisualizationSettings(null, CubeSorting.SUM);
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.GroupHorizontalBarChart, cube, ghbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(9.123, 10.123, 11.123, 6.123, 7.123, 8.123, 3.123, 4.123, 5.123, 0.123, 1.123, 2.123);
            var expectedVarValueOrder1 = new List<string>() { "value-3", "value-2", "value-1", "value-0" };
            var expectedVarValueOrder2 = new List<string>() { "value-0", "value-1", "value-2" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value0_NoPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, "value-0");
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(2.123, 1.123, 0.123, 5.123, 4.123, 3.123, 8.123, 7.123, 6.123);
            var expectedVarValueOrder1 = new List<string>() { "value-0", "value-1", "value-2" };
            var expectedVarValueOrder2 = new List<string>() { "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value1_Pivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, "value-1");
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, true);
            var expectedDataOrder = DataValueUtilities.List(7.123, 6.123, 8.123, 4.123, 3.123, 5.123, 1.123, 0.123, 2.123);
            var expectedVarValueOrder1 = new List<string>() { "value-2", "value-1", "value-0" };
            var expectedVarValueOrder2 = new List<string>() { "value-1", "value-0", "value-2" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_value2_NoPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, "value-2");
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(8.123, 7.123, 6.123, 2.123, 1.123, 0.123, 5.123, 4.123, 3.123);
            var expectedVarValueOrder1 = new List<string>() { "value-2", "value-0", "value-1" };
            var expectedVarValueOrder2 = new List<string>() { "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_sum_NoPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, CubeSorting.SUM);
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(2.123, 1.123, 0.123, 5.123, 4.123, 3.123, 8.123, 7.123, 6.123);
            var expectedVarValueOrder1 = new List<string>() { "value-0", "value-1", "value-2" };
            var expectedVarValueOrder2 = new List<string>() { "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message), message);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_no_sorting_NoPivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, CubeSorting.NO_SORTING);
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, false);
            var expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123);
            var expectedVarValueOrder1 = new List<string>() { "value-0", "value-1", "value-2" };
            var expectedVarValueOrder2 = new List<string>() { "value-0", "value-1", "value-2" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.IsTrue(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), message2);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_SHBC_no_sorting_Pivot()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var shbcs = new StackedHorizontalBarChartVisualizationSettings(null, CubeSorting.NO_SORTING);
            var result = CubeSorting.SortMultidimHorizontalBarChart(VisualizationType.StackedHorizontalBarChart, cube, shbcs.Sorting, true);
            var expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123, 5.123, 6.123, 7.123, 8.123);
            var expectedVarValueOrder1 = new List<string>() { "value-0", "value-1", "value-2" };
            var expectedVarValueOrder2 = new List<string>() { "value-0", "value-1", "value-2" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.IsTrue(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), message2);
            Assert.AreEqual(expectedVarValueOrder1, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
            Assert.AreEqual(expectedVarValueOrder2, result.Meta.Variables[2].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_HBC_no_sorting()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var hbcs = new HorizontalBarChartVisualizationSettings(null, CubeSorting.NO_SORTING);
            var result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            var expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123);
            var expectedVarValueOrder = new List<string>() { "value-0", "value-1", "value-2", "value-3", "value-4" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.IsTrue(DataValueUtilities.Compare(cube.Data, result.Data, out string message2), message2);
            Assert.AreEqual(expectedVarValueOrder, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_HBC_ascending()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var hbcs = new HorizontalBarChartVisualizationSettings(null, CubeSorting.ASCENDING);
            var result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            var expectedDataOrder = DataValueUtilities.List(0.123, 1.123, 2.123, 3.123, 4.123);
            var expectedVarValueOrder = new List<string>() { "value-0", "value-1", "value-2", "value-3", "value-4" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.AreEqual(expectedVarValueOrder, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_HBC_descending()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var hbcs = new HorizontalBarChartVisualizationSettings(null, CubeSorting.DESCENDING);
            var result = CubeSorting.SortOneDimensionalCharts(cube, hbcs.Sorting);
            var expectedDataOrder = DataValueUtilities.List(4.123, 3.123, 2.123, 1.123, 0.123);
            var expectedVarValueOrder = new List<string>() { "value-4", "value-3", "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.AreEqual(expectedVarValueOrder, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
        }

        [Test]
        public void DataCubeSortingTest_PC_descending()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 5)
            };

            var cube = TestDataCubeBuilder.BuildTestDataCube(varParams);
            var pcs = new PieChartVisualizationSettings(null, CubeSorting.DESCENDING);
            var result = CubeSorting.SortOneDimensionalCharts(cube, pcs.Sorting);
            var expectedDataOrder = DataValueUtilities.List(4.123, 3.123, 2.123, 1.123, 0.123);
            var expectedVarValueOrder = new List<string>() { "value-4", "value-3", "value-2", "value-1", "value-0" };

            Assert.IsTrue(DataValueUtilities.Compare(expectedDataOrder, result.Data, out string message1), message1);
            Assert.AreEqual(expectedVarValueOrder, result.Meta.Variables[1].IncludedValues.Select(vv => vv.Code));
        }
    }
}

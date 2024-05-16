using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace SortingTests
{
    internal class VariableSortingOptionsTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));
        }

        [Test]
        public void SortingOptionsTest_HorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 1),
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "descending", "ascending", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 3),
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "value-0", "value-1", "value-2", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsSelectedVarTest_HorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 5) { Selectable = true },
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "descending", "ascending", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsSelectedVarTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.OtherClassificatory, 4) { Selectable = true},
                new(VariableType.Unknown, 3),
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "value-0", "value-1", "value-2", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsTwoSelectedVarsTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.OtherClassificatory, 4) { Selectable = true},
                new(VariableType.Unknown, 3),
                new(VariableType.Time, 4) { Selectable = true},
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "value-0", "value-1", "value-2", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsPivotTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.Time, 2),
                new(VariableType.Unknown, 2),
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "2001", "2000", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }

        [Test]
        public void SortingOptionsTest_StackedHorizontalBarChart()
        {
            List<VariableParameters> varParams = new()
            {
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 3),
            };

            var meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            var query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            var settingsRequest = new VisualizationSettingsRequest()
            {
                SelectedVisualization = VisualizationType.StackedHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            var sortingOptions = CubeSorting.Get(meta, settingsRequest);
            var expected = new List<string>() { "value-0", "value-1", "value-2", "sum", "no_sorting" };
            Assert.AreEqual(expected, sortingOptions.Select(so => so.Code).ToList());
        }
    }
}

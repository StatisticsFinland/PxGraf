using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace SortingTests
{
    internal class VariableSortingOptionsTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void SortingOptionsTest_HorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 1),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsSelectedVarTest_HorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 5) { Selectable = true },
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.HorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsSelectedVarTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.OtherClassificatory, 4) { Selectable = true},
                new(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsTwoSelectedVarsTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.OtherClassificatory, 4) { Selectable = true},
                new(VariableType.Unknown, 3),
                new(VariableType.Time, 4) { Selectable = true},
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsPivotTest_GroupHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 2),
                new(VariableType.Unknown, 2),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = true,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["2001", "2000", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsTest_StackedHorizontalBarChart()
        {
            List<VariableParameters> varParams =
            [
                new(VariableType.Content, 1),
                new(VariableType.OtherClassificatory, 4),
                new(VariableType.Unknown, 3),
            ];

            CubeMeta meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            CubeQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.StackedHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            IReadOnlyList<SortingOption> sortingOptions = CubeSorting.Get(meta, settingsRequest);
            List<string> expected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }
    }
}

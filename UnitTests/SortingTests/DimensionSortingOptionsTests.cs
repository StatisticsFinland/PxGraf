using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata;
using PxGraf.Data;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Models.Requests;
using PxGraf.Models.Responses;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Fixtures;

namespace UnitTests.SortingTests
{
    internal class DimensionSortingOptionsTests
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);
        }

        [Test]
        public void SortingOptionsTest_HorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Unknown, 1),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.HorizontalBarChart, meta, true, query);
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsTest_GroupHorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Unknown, 3),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.GroupHorizontalBarChart, meta, true, query);
            List<string> defaultExpected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(defaultExpected));
            List<string> pivotedExpected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(pivotedExpected));
        }

        [Test]
        public void SortingOptionsSelectedVarTest_HorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Unknown, 5) { Selectable = true },
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.HorizontalBarChart, meta, true, query);
            List<string> expected = ["descending", "ascending", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(expected));
            Assert.That(sortingOptions.Pivoted.Select(so => so.Code).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void SortingOptionsSelectedVarTest_GroupHorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Other, 4) { Selectable = true},
                new(DimensionType.Unknown, 3),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.GroupHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.GroupHorizontalBarChart, meta, true, query);
            List<string> defaultExpected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(defaultExpected));
            List<string> pivotedExpected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Pivoted.Select(so => so.Code).ToList(), Is.EqualTo(pivotedExpected));
        }

        [Test]
        public void SortingOptionsTwoSelectedVarsTest_GroupHorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Other, 4) { Selectable = true},
                new(DimensionType.Unknown, 3),
                new(DimensionType.Time, 4) { Selectable = true},
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.GroupHorizontalBarChart, meta, true, query);
            List<string> defaultExpected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(defaultExpected));
            List<string> pivotedExpected = ["value-0", "value-1", "value-2", "value-3", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(pivotedExpected));
        }

        [Test]
        public void SortingOptionsPivotTest_GroupHorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 2),
                new(DimensionType.Unknown, 2),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.GroupHorizontalBarChart, meta, true, query);
            List<string> defaultExpected = ["2001", "2000", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(defaultExpected));
            List<string> pivotedExpected = ["2001", "2000", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Pivoted.Select(so => so.Code).ToList(), Is.EqualTo(pivotedExpected));
        }

        [Test]
        public void SortingOptionsTest_StackedHorizontalBarChart()
        {
            List<DimensionParameters> varParams =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Other, 4),
                new(DimensionType.Unknown, 3),
            ];

            IReadOnlyMatrixMetadata meta = TestDataCubeBuilder.BuildTestMeta(varParams);
            MatrixQuery query = TestDataCubeBuilder.BuildTestCubeQuery(varParams);

            VisualizationSettingsRequest settingsRequest = new()
            {
                SelectedVisualization = VisualizationType.StackedHorizontalBarChart,
                PivotRequested = false,
                Query = query
            };
            VisualizationOption.SortingOptionsCollection sortingOptions = CubeSorting.Get(VisualizationType.StackedHorizontalBarChart, meta, true, query);
            List<string> defaultExpected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Default.Select(so => so.Code).ToList(), Is.EqualTo(defaultExpected));
            List<string> pivotedExpected = ["value-0", "value-1", "value-2", "sum", "no_sorting", "reversed"];
            Assert.That(sortingOptions.Pivoted.Select(so => so.Code).ToList(), Is.EqualTo(pivotedExpected));
        }
    }
}

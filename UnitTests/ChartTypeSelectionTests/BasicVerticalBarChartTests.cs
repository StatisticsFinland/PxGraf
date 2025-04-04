﻿using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace UnitTests.ChartTypeSelectionTests
{
    [TestFixture]
    public class BasicVerticalBarChartTests
    {
        private ChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No dimensions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoDataTest()
        {
            List<DimensionParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Content size 1, time size 20
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 20),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Content size 1, time size 1, progressive size 20
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithClassifierTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 20),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Case: Content size 1, time size 1
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselectsTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Content size 1, time size 5, orher size 5
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void TwoMultiselectsTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: Content size 1, time size 5, orher size 5
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void MultipleContentStillNoTimeOrProgressiveTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 3) { SameUnit = true },
                new DimensionParameters(DimensionType.Time, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOrProgressiveRequired));
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: ContentOverMax
        /// </summary>
        [Test]
        public void NoTimeOrProgressiveTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOrProgressiveRequired));
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: ContentOverMax
        /// </summary>
        [Test]
        public void TooManyTimeValuesTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 2000),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Content size 1, time size 1, multiselect size 2000
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyProgressiveValuesTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 2000),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Content size 1, irregular time variable size 11
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyIrregularTimeVariableValuesTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 11) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.IrregularTimeOverMax));
        }

        /// <summary>
        /// Case: Content size 1, irregular time variable size 10
        /// Result: Pass
        /// </summary>
        [Test]
        public void IrregularTimeVariableValuesTest_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Query has "too many multiselects" but it's ok since those are selectable.
        /// </summary>
        [Test]
        public void SelectablePassTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Other, 10),
                new DimensionParameters(DimensionType.Other, 10),
            ];

            SanityCheck();
            ValidWithSelectables();
            WithDynamicQueries();
            FailWithoutSelectables();

            // Sanity check: Too many multiselects by default
            void SanityCheck()
            {
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
            }

            // Selectable should have be considered as single values => check should pass
            void ValidWithSelectables()
            { 
                dimension[2].Selectable = true; // OtherClassificatory
                dimension[3].Selectable = true; // OtherClassificatory
            
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
            }

            // Dynamic queries force multivalue interpretation, UNLESS variable is selectable => check should still pass
            void WithDynamicQueries()
            {
                dimension[2].ValueFilter = new AllFilter(); // OtherClassificatory
                dimension[3].ValueFilter = new FromFilter("abc"); // OtherClassificatory

                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
            }

            // Sanity check: And when variables are not selectable, query should fail again
            void FailWithoutSelectables()
            {
                dimension[2].Selectable = false; // OtherClassificatory
                dimension[3].Selectable = false; // OtherClassificatory,
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
            }
        }
    }
}

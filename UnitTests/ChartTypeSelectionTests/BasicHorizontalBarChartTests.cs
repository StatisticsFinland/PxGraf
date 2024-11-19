using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;

namespace UnitTests.ChartTypeSelectionTests
{
    [TestFixture]
    public class BasicHorizontalBarChartTests
    {
        private ChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No diemsnions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoData_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Valid data
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidData_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 20)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: No multiselect dimensions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselects_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: 2 Multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void TooManyMultiselects_TooManyhMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Unknown, 11)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content, one multiselect dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContent_ContentRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Content has 4
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHasMultipleWithDifferentUnits_UnambiguousContentUnitsRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 4),
                new DimensionParameters(DimensionType.Time, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: No time, one multiselect
        /// Result: TimeRequired
        /// </summary>
        [Test]
        public void NoTime_TimeRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeRequired));
        }

        /// <summary>
        /// Case: Time has 4 selections
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeHas4_TimeOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 4),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOverMax));
        }

        /// <summary>
        /// Case: One progressive multiselect dimensions
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void ProgressiveMultiselect_ProgressiveNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ProgressiveNotAllowed));
        }

        /// <summary>
        /// Case: One multiselect dimension with 50 selections
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectHas50_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 50),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            HorizontalBarChartCheck check = new(Limits.HorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }
    }
}

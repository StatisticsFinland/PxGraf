using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;
using UnitTests;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class GroupHorizontalBarChartTests
    {
        private IChartSelectionLimits Limits { get; set; }

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
        public void NoData_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

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
                new DimensionParameters(DimensionType.Unknown, 10),
                new DimensionParameters(DimensionType.Geographical, 3)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Valid data with a time dimension
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidDataWithTime_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 15),
                new DimensionParameters(DimensionType.Time, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Case: One multiselect dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselectDimensions_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 15),
                new DimensionParameters(DimensionType.Time, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Three multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselectDimensions_TooManyMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 15),
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Geographical, 3)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content dimenion
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Unknown, 15),
                new DimensionParameters(DimensionType.Time, 2),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Three values selected from the content dimensions
        /// Result: Pass
        /// </summary>
        [Test]
        public void ThreeSelectedFromContent_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 3) { SameUnit = true },
                new DimensionParameters(DimensionType.Unknown, 5),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: One progressive multiselect dimension
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void OneProgressiveMultiselectDimension_ProgressiveNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 3),
                new DimensionParameters(DimensionType.Unknown, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ProgressiveNotAllowed));
        }

        /// <summary>
        /// Case: Two progressive multiselect dimensions
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void TwoProgressiveMultiselectDimensions_ProgressiveNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 3),
                new DimensionParameters(DimensionType.Ordinal, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ProgressiveNotAllowed));
        }

        /// <summary>
        /// Case: Five values selected from the time dimensions
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeDimensionsHasFiveSelectons_TimeOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOverMax));
        }

        /// <summary>
        /// Case: Contains a multiselect dimension with a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void OneMultiselectWithCombinationValue_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Geographical, 5),
                new DimensionParameters(DimensionType.Unknown, 3, true), // <- Combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: Contains two multiselect dimensions with a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void TwoMultiselectsWithCombinationValues_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Geographical, 5, true), // <- Combination
                new DimensionParameters(DimensionType.Unknown, 3, true) // <- Combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: One multiselect dimension has 25 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectWith25Values_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Geographical, 25),
                new DimensionParameters(DimensionType.Unknown, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Two multiselect dimensions have over 4 (but under 20) value selected from them.
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void TwoMultiselectsWithOver4Values_SecondMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Geographical, 12),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: First multiselect dimension has 15, and second 4, which gives a product of 60
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void ProductOver40_MultiselectProductOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Geographical, 15),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.MultiselectProductOverMax));
        }
    }
}

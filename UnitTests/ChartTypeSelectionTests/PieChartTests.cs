using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using System.Collections.Generic;
using UnitTests;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class PieChartTests
    {
        private IChartSelectionLimits Limits { get; set; }

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
            PieChartCheck check = new(Limits.PieChartLimits);

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
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: No multiselect diemsnions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselectDimensions_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Two multiselect diemsnions
        /// Result: TooManyhMultiselections
        /// </summary>
        [Test]
        public void TwoMultiselectDimensions_TooManyMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Geographical, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: The content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2_UnambiguousContentUnitRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: The content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas3WithSameUnit_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 3) {SameUnit = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: No time dimensions
        /// Result: TimeRequired
        /// </summary>
        [Test]
        public void NoTimeDimension_TimeRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeRequired));
        }

        /// <summary>
        /// Case: The time dimensions has 2 values selected
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeHas2_TimeOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOverMax));
        }


        /// <summary>
        /// Case: Multiselect has a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void MultiselectHasCombinationValue_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 4, true)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: Multiselect has 11 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectHasOver10_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 11)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }


        /// <summary>
        /// Case: Data contains negative values.
        /// Result: NegativeDataNotAllowed
        /// </summary>
        [Test]
        public void NegativeData_NegativeDataNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension, true); // <- true causes negative data to be built
            PieChartCheck check = new(Limits.PieChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NegativeDataNotAllowed));
        }
    }
}

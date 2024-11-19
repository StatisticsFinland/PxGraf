using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using System.Collections.Generic;

namespace UnitTests.ChartTypeSelectionTests
{
    [TestFixture]
    public class PyramidChartTests
    {
        private ChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No variables
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoData_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

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
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
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
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2_UnambiguousContentUnitRequired()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 2),
                new(DimensionType.Time, 1),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: Content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2WithSameUnit_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 2) { SameUnit = true },
                new(DimensionType.Time, 1),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);
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
                new(DimensionType.Content, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeRequired));
        }

        /// <summary>
        /// Case: Time dimensions has 2 values selected
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeHas2_TimeOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 2),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOverMax));
        }

        /// <summary>
        /// Case: Second multiselect has 3 values selected
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void SecondMultiselectHas3_SecondMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 3),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: Both multiselects have 2 values selected
        /// Result: FirstMultiselectBelowMin
        /// </summary>
        [Test]
        public void BothMultiselectsHave2_FirstMultiselectBelowMin()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectBelowMin));
        }

        /// <summary>
        /// Case: First multiselect is not progressive
        /// Result: ProgressiveRequired
        /// </summary>
        [Test]
        public void FirstMultiselectIsNotProgressive_ProgressiveRequired()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Unknown, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ProgressiveRequired));
        }

        /// <summary>
        /// Case: First multiselect is not progressive, but second is
        /// Result: Pass
        /// </summary>
        [Test]
        public void SecondMultiselectProgressive_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Ordinal, 2),
                new(DimensionType.Unknown, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: First multiselect has 1000 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void FirstMultiselectHas1000_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 1000)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: First multiselect has a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void FirstMultiselectHas1000_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 800, true)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: Second multiselect has a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void SecondMultiselectHas1000_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2, true),
                new(DimensionType.Ordinal, 800)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
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
                new(DimensionType.Content, 1),
                new(DimensionType.Time, 1),
                new(DimensionType.Unknown, 2),
                new(DimensionType.Ordinal, 200)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension, true); // <- true causes negative data to be built
            PyramidChartCheck check = new(Limits.PyramidChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NegativeDataNotAllowed));
        }
    }
}

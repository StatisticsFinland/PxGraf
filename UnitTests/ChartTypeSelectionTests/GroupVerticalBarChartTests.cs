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
    public class GroupVerticalBarChartTests
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

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Valid data
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void ValidData_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Valid data without time dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void ValidWithoutTime_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Unknown, 3),
            ];

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }


        /// <summary>
        /// Case: One multidimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselect_NotEnoughMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Three multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselect_TooManyMultiselections()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Ordinal, 8),

            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContent_ContentRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 5),
                new DimensionParameters(DimensionType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Content dimension has 5 selections, Time dimension has 5 selections
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void ContentHas5_SecondMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 5) { SameUnit = true },
                new DimensionParameters(DimensionType.Time, 5),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: Content dimension has 5 selections with the same unit, Time dimension has 4 selections
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas5_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 5) { SameUnit = true },
                new DimensionParameters(DimensionType.Time, 4),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Content dimension has 3 selections with different units, Time dimension has 4 selections
        /// Result: --
        /// </summary>
        [Test]
        public void ContentHas3_UnambiguousContentUnitRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 3),
                new DimensionParameters(DimensionType.Time, 4),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: No time or progressive multiselect dimensions
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void NoTimeOrProgressive_TimeOrProgressiveRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Unknown, 5),
                new DimensionParameters(DimensionType.Unknown, 10),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOrProgressiveRequired));
        }

        /// <summary>
        /// Case: One of the multiselect dimensions has a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void HasCombinationValue_CombinationValuesNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 5, true), // <- combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: One of the multiselect dimensions has 30 selected values
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void OneMultiselectHas30_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 30),
                new DimensionParameters(DimensionType.Unknown, 3), // <- combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Both multiselect dimenensions have over 4, but under 20 values.
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void BothMultiselectHaveOver4_SecondMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 15), // <- combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: First multiselect dimension has 20, and second 4, which gives a product of 80
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void MultiselecrProduct80_MultiselectProductOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Time, 20),
                new DimensionParameters(DimensionType.Unknown, 4), // <- combination
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.MultiselectProductOverMax));
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 11
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyIrregularTimeVariableValuesTest()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Time, 11) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.IrregularTimeOverMax));
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 10
        /// Result: Pass
        /// </summary>
        [Test]
        public void IrregularTimeVariableValuesTest_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Content, 1),
                new DimensionParameters(DimensionType.Other, 3),
                new DimensionParameters(DimensionType.Time, 10) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }
    }
}

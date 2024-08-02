using NUnit.Framework;
using PxGraf.ChartTypeSelection.JsonObjects;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class GroupVerticalBarChartTests
    {
        private IChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /*
        /// <summary>
        /// Case: No dimensions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoData_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = [];

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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 3),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 3),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Ordinal, 8),

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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 5)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 5) { SameUnit = true },
                new VariableParameters(VariableType.Time, 5),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 5) { SameUnit = true },
                new VariableParameters(VariableType.Time, 4),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 3),
                new VariableParameters(VariableType.Time, 4),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Unknown, 10),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5, true), // <- combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 30),
                new VariableParameters(VariableType.Unknown, 3), // <- combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 15), // <- combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 20),
                new VariableParameters(VariableType.Unknown, 4), // <- combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 11) { Irregular = true}
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 10) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupVerticalBarChartCheck check = new(Limits.GroupVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }
        */
    }
}

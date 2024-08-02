using NUnit.Framework;
using PxGraf.ChartTypeSelection.JsonObjects;

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

        // TODO: Fix tests

        /*
        /// <summary>
        /// Case: No dimensions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoData_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = [];

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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 10),
                new VariableParameters(VariableType.Geological, 3)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 1)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Geological, 3)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 3) { SameUnit = true },
                new VariableParameters(VariableType.Unknown, 5),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Unknown, 15),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Ordinal, 15),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 3),
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 5),
                new VariableParameters(VariableType.Unknown, 3, true), // <- Combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 5, true), // <- Combination
                new VariableParameters(VariableType.Unknown, 3, true) // <- Combination
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 25),
                new VariableParameters(VariableType.Unknown, 2)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 12),
                new VariableParameters(VariableType.Unknown, 5)
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
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 15),
                new VariableParameters(VariableType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            GroupHorizontalBarChartCheck check = new(Limits.GroupHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.MultiselectProductOverMax));
        }
        */
    }
}

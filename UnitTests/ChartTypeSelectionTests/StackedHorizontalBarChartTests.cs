using NUnit.Framework;
using PxGraf.ChartTypeSelection.JsonObjects;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class StackedHorizontalBarChartTests
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
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

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
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 5),
                new(VariableType.Geological, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: One multiselect dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselectDimension_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 15)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: One multiselect dimension
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselectDimensions_TooManyMultiselections()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 15),
                new(VariableType.Geological, 4),
                new(VariableType.Unknown, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 15),
                new(VariableType.Geological, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: 4 values selected from the content dimension
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas4WithSameUnits_Pass()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 4) { SameUnit = true },
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }


        /// <summary>
        /// Case: 4 values selected from the content dimension
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas4_UnambiguousContentUnitRequired()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 4),
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: 2 selected from the time dimension
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TwoFromTime_TimeOverMax()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 2),
                new(VariableType.Unknown, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOverMax));
        }

        /// <summary>
        /// Case: Contains one progressive multiselect dimension
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void ProgressiveMultiselect_ProgressiveNotAllowed()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 4),
                new(VariableType.Ordinal, 15),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ProgressiveNotAllowed));
        }

        /// <summary>
        /// Case: Contains one progressive dimension with one selected value
        /// Result: Pass
        /// </summary>
        [Test]
        public void ProgressiveWithOneValue_Pass()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Time, 1),
                new(VariableType.Unknown, 4),
                new(VariableType.Unknown, 5),
                new(VariableType.Ordinal, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: First multiselect dimension contains a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void FirstMultiselectHasCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 5, true),
                new(VariableType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: Second multiselect dimension contains a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void SecondMultiselectHasCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 5),
                new(VariableType.Unknown, 4, true)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: One multiselect dimension has over 30 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectHAsOver30_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 31),
                new(VariableType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Both multiselect dimensions have over 10 values selected
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void BothMultiselectsHaveOver10_SecondMultiselectOverMax()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 11),
                new(VariableType.Unknown, 12)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: Data contains negative values.
        /// Result: NegativeDataNotAllowed
        /// </summary>
        [Test]
        public void NegativeData_NegativeDataNotAllowed()
        {
            List<VariableParameters> dimension =
            [
                new(VariableType.Content, 1),
                new(VariableType.Unknown, 5),
                new(VariableType.Geological, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension, true); // <- true causes negative data to be built
            StackedHorizontalBarChartCheck check = new(Limits.StackedHorizontalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NegativeDataNotAllowed));
        }
        */
    }
}

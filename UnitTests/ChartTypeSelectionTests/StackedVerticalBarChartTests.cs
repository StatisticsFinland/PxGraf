using NUnit.Framework;
using NUnit.Framework.Constraints;
using PxGraf.ChartTypeSelection;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class StackedVerticalBarChartTests
    {
        private IChartSelectionLimits Limits { get; set; }

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
            List<VariableParameters> variable = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Valid data
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidData_Pass()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(varParams);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Case: One multiselect variable
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselect_NotEnoughMultiselections()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NotEnoughMultiselections));
        }

        /// <summary>
        /// Case: Three multiselect variable
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselect_TooManyMultiselections()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Ordinal, 6),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TooManyMultiselections));
        }

        /// <summary>
        /// Case: No content variables
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Ordinal, 6)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(varParams);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: Content variables has two values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2SelectionsWithSameUnits_Pass()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Content, 2) { SameUnit = true }
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Content variables has two values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2Selections_UnambiguousContentUnitRequired()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Content, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.UnambiguousContentUnitRequired));
        }

        /// <summary>
        /// Case: The multiselect variables do not contain time or progressive variables
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void NoTimeOrProgressive_TimeOrProgressiveRequired()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Geological, 5),
                new VariableParameters(VariableType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOrProgressiveRequired));
        }

        /// <summary>
        /// Case: One multiselect dimenion contains a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void ContainOneCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Geological, 5, true), // <- Combination
                new VariableParameters(VariableType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.CombinationValuesNotAllowed));
        }

        /// <summary>
        /// Case: One multiselect dimenion contains has 45 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void OneMultiselectDimensionHasOver40_FirstMultiselectOverMax()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Geological, 45), // <- Combination
                new VariableParameters(VariableType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Both multiselect dimenion have over 10 (but under 40) values selected
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void BothMultiselectsOver10_SecondMultiselectOverMax()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Time, 12),
                new VariableParameters(VariableType.Geological, 15), // <- Combination
                new VariableParameters(VariableType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.SecondMultiselectOverMax));
        }

        /// <summary>
        /// Case: Data contains negative values.
        /// Result: NegativeDataNotAllowed
        /// </summary>
        [Test]
        public void NegativeData_NegativeDataNotAllowed()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 2)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable, true); // <- true causes negative data to be built
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.NegativeDataNotAllowed));
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 11
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyIrregularTimeVariableValuesTest()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 11) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.IrregularTimeOverMax));
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 10
        /// Result: Pass
        /// </summary>
        [Test]
        public void IrregularTimeVariableValuesTest_Pass()
        {
            List<VariableParameters> variable =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 10) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(variable);
            StackedVerticalBarChartCheck check = new(Limits.StackedVerticalBarChartLimits);

            Assert.That(check.CheckValidity(input).Count, Is.EqualTo(0));
        }
    }
}

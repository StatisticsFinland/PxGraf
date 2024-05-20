using NUnit.Framework;
using PxGraf.ChartTypeSelection;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace ChartTypeSelectionTests
{
    [TestFixture]
    class BasicVerticalBarChartTests
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
        public void NoDataTest()
        {
            List<VariableParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, time size 20
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 20),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(reasons.Count == 0, msg);
        }

        /// <summary>
        /// Case: Content size 1, time size 1, progressive size 20
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithClassifierTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 20),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.True(check.CheckValidity(input).Count == 0);
        }

        /// <summary>
        /// Case: Content size 1, time size 1
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselectsTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, time size 5, orher size 5
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void TwoMultiselectsTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, time size 5, orher size 5
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void MultipleContentStillNoTimeOrProgressiveTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 3) { SameUnit = true },
                new VariableParameters(VariableType.Time, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeOrProgressiveRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: ContentOverMax
        /// </summary>
        [Test]
        public void NoTimeOrProgressiveTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 4)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeOrProgressiveRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 3, time size 1
        /// Result: ContentOverMax
        /// </summary>
        [Test]
        public void TooManyTimeValuesTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 2000),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, time size 1, multiselect size 2000
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyProgressiveValuesTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 2000),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, irregular time variable size 11
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyIrregularTimeVariableValuesTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 11) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.IrregularTimeOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, irregular time variable size 10
        /// Result: Pass
        /// </summary>
        [Test]
        public void IrregularTimeVariableValuesTest_Pass()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10) { Irregular = true}
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);

            Assert.True(check.CheckValidity(input).Count == 0);
        }

        /// <summary>
        /// Query has "too many multiselects" but it's ok since those are selectable.
        /// </summary>
        [Test]
        public void SelectablePassTest()
        {
            List<VariableParameters> dimension =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.OtherClassificatory, 10),
                new VariableParameters(VariableType.OtherClassificatory, 10),
            ];

            // Sanity check: Too many multiselects by default
            {
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
            }

            // Selectable should have be considered as single values => check should pass
            dimension[2].Selectable = true; // OtherClassificatory
            dimension[3].Selectable = true; // OtherClassificatory
            {
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.IsFalse(check.CheckValidity(input).Count != 0);
            }

            // Dynamic queries force multivalue interpretation, UNLESS variable is selectable => check should still pass
            dimension[2].ValueFilter = new AllFilter(); // OtherClassificatory
            dimension[3].ValueFilter = new FromFilter("abc"); // OtherClassificatory
            {
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.IsFalse(check.CheckValidity(input).Count != 0);
            }

            // Sanity check: And when variables are not selectable, query should fail again
            dimension[2].Selectable = false; // OtherClassificatory
            dimension[3].Selectable = false; // OtherClassificatory,
            {
                VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
                VerticalBarChartCheck check = new(Limits.VerticalBarChartLimits);
                Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
            }
        }
    }
}

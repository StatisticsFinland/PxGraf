using NUnit.Framework;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.Enums;
using System.Collections.Generic;
using System.Linq;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

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
            List<VariableParameters> dimension = new();

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Valid data
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidData_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: No multiselect diemsnions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselectDimensions_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Two multiselect diemsnions
        /// Result: TooManyhMultiselections
        /// </summary>
        [Test]
        public void TwoMultiselectDimensions_TooManyMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Geological, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: No content dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 4),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: The content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas2_UnambiguousContentUnitRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 2)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);
            var reasons = check.CheckValidity(input);

            Assert.AreEqual(RejectionReason.UnambiguousContentUnitRequired, reasons[0].Reason);
        }

        /// <summary>
        /// Case: The content dimensions has 2 values selected
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas3WithSameUnit_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 3) {SameUnit = true}
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: No time dimensions
        /// Result: TimeRequired
        /// </summary>
        [Test]
        public void NoTimeDimension_TimeRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.TimeRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: The time dimensions has 2 values selected
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeHas2_TimeOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Content, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.TimeOverMax, check.CheckValidity(input)[0].Reason);
        }


        /// <summary>
        /// Case: Multiselect has a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void MultiselectHasCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4, true)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.CombinationValuesNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Multiselect has 11 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectHasOver10_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 11)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }


        /// <summary>
        /// Case: Data contains negative values.
        /// Result: NegativeDataNotAllowed
        /// </summary>
        [Test]
        public void NegativeData_NegativeDataNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 5)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension, true); // <- true causes negative data to be built
            var check = new PieChartCheck(Limits.PieChartLimits);

            Assert.AreEqual(RejectionReason.NegativeDataNotAllowed, check.CheckValidity(input)[0].Reason);
        }
    }
}

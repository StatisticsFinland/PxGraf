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
    class BasicHorizontalBarChartTests
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
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

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
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 20)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.IsTrue(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: No multiselect dimensions
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void NoMultiselects_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: 2 Multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void TooManyMultiselects_TooManyhMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Unknown, 11)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: No content, one multiselect dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContent_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content has 4
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHasMultipleWithDifferentUnits_UnambiguousContentUnitsRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 4),
                new VariableParameters(VariableType.Time, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.UnambiguousContentUnitRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: No time, one multiselect
        /// Result: TimeRequired
        /// </summary>
        [Test]
        public void NoTime_TimeRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 5),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Time has 4 selections
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeHas4_TimeOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 4),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One progressive multiselect dimensions
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void ProgressiveMultiselect_ProgressiveNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Ordinal, 15),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.ProgressiveNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One multiselect dimension with 50 selections
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectHas50_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 50),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new HorizontalBarChartCheck(Limits.HorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }
    }
}

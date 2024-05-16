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
    class ScatterPlotTests
    {
        private IChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No diemsnions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoData_ContentRequired()
        {
            List<VariableParameters> dimension = new();

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Valid data with only the content dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void ValidDataWithContentOnly_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 2),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Valid data two multiselect dimensions
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidDataWith2Multiselect_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 2),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: No content dimension
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContent_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: The content dimension has only one value selected
        /// Result: ContentBelowMin
        /// </summary>
        [Test]
        public void ContentHasOne_ContentBelowMin()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.ContentBelowMin, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: The content dimension has three values selected
        /// Result: ContentOverMax
        /// </summary>
        [Test]
        public void ContentHasThree_ContentOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 3),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.ContentOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: First multiselect dimension has 1000 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void FirstMultiselectHAs1000_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 2),
                new VariableParameters(VariableType.Unknown, 1000)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new ScatterPlotCheck(Limits.ScatterPlotLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }
    }
}

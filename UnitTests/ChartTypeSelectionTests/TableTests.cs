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
    class TableTests
    {
        private IChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No dimensions
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void NoData_FirstMultiselectBelowMin()
        {
            List<VariableParameters> dimension = new();

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new TableCheck(Limits.TableLimits);

            Assert.AreEqual(RejectionReason.DataRequired, check.CheckValidity(input)[0].Reason);
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
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Ordinal, 6),
                new VariableParameters(VariableType.Unknown, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new TableCheck(Limits.TableLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }
    }
}

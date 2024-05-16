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
    class LineChartTests
    {
        private IChartSelectionLimits Limits { get; set; }

        [OneTimeSetUp]
        public void SetupChartSelectionLimits()
        {
            Limits = new ChartSelectionLimits();
        }

        /// <summary>
        /// Case: No variable
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void NoData_TimeOrProgressiveRequired()
        {
            List<VariableParameters> dimension = new();

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);

            Assert.AreEqual(RejectionReason.TimeOrProgressiveRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTime_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Content, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeAndContent_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Content, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeContentAndOther_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Content, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Valid with a progressive variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithProgressive_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 2),
                new VariableParameters(VariableType.Content, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Multiselect has 1000 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void FirstMultiselectHAs1000_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 1000)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Multiselects have 4 and 5 values selected, which gives a product of 20
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void LargeMultiselectProduct_MultiselectProductOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 6),
                new VariableParameters(VariableType.Unknown, 4),
                new VariableParameters(VariableType.Time, 5)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);

            Assert.AreEqual(RejectionReason.MultiselectProductOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One multiselect variable with valid values but without content
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void ProgressiveAndTime_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 1),
                new VariableParameters(VariableType.Time, 6)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One multiselect variable with valid values
        /// Result: Pass
        /// </summary>
        [Test]
        public void ProgressiveAndTime_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 1),
                new VariableParameters(VariableType.Time, 6),
                new VariableParameters(VariableType.Content, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Time variable with multiple values and selectable content variable with multiple values
        /// Result: Pass
        /// </summary>
        [Test]
        public void TimeAndSelectableContent_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 1),
                new VariableParameters(VariableType.Time, 15),
                new VariableParameters(VariableType.Content, 5) { Selectable = true },
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Ordinal variable with multiple values, nominal variable with 4 values and a content variable with 1 value
        /// Result: Pass
        /// </summary>
        [Test]
        public void LargeOrdinalAndNominal_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Ordinal, 25),
                new VariableParameters(VariableType.OtherClassificatory, 4),
                new VariableParameters(VariableType.Content, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Time variable with multiple irregular values, nominal dimension with 2 values and a content dimension with 1 value
        /// Result: IrregularTimeNotAllowed
        /// </summary>
        [Test]
        public void IrregularTime_IrregularTimeNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 10) { Irregular = true },
                new VariableParameters(VariableType.OtherClassificatory, 2),
                new VariableParameters(VariableType.Content, 1),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new LineChartCheck(Limits.LineChartLimits);

            Assert.AreEqual(RejectionReason.IrregularTimeNotAllowed, check.CheckValidity(input)[0].Reason);
        }
    }
}

﻿using NUnit.Framework;
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
    class GroupVerticalBarChartTests
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
        public void NoData_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new();

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Valid data
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void ValidData_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Valid data without time dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void ValidWithoutTime_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 5),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.True(!check.CheckValidity(input).Any());
        }


        /// <summary>
        /// Case: One multidimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselect_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Three multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselect_TooManyMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Ordinal, 8),

            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: No content dimensions
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContent_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 5)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content dimension has 5 selections, Time dimension has 5 selections
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void ContentHas5_SecondMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 5) { SameUnit = true },
                new VariableParameters(VariableType.Time, 5),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.SecondMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content dimension has 5 selections with the same unit, Time dimension has 4 selections
        /// Result: Pass
        /// </summary>
        [Test]
        public void ContentHas5_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 5) { SameUnit = true },
                new VariableParameters(VariableType.Time, 4),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Content dimension has 3 selections with different units, Time dimension has 4 selections
        /// Result: --
        /// </summary>
        [Test]
        public void ContentHas3_UnambiguousContentUnitRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 3),
                new VariableParameters(VariableType.Time, 4),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);
            var reasons = check.CheckValidity(input);

            Assert.AreEqual(RejectionReason.UnambiguousContentUnitRequired, reasons[0].Reason);
        }

        /// <summary>
        /// Case: No time or progressive multiselect dimensions
        /// Result: TimeOrProgressiveRequired
        /// </summary>
        [Test]
        public void NoTimeOrProgressive_TimeOrProgressiveRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 1),
                new VariableParameters(VariableType.Unknown, 5),
                new VariableParameters(VariableType.Unknown, 10),

            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeOrProgressiveRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One of the multiselect dimensions has a combination value
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void HasCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 5, true), // <- combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.CombinationValuesNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One of the multiselect dimensions has 30 selected values
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void OneMultiselectHas30_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 30),
                new VariableParameters(VariableType.Unknown, 3), // <- combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Both multiselect dimenensions have over 4, but under 20 values.
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void BothMultiselectHaveOver4_SecondMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 10),
                new VariableParameters(VariableType.Unknown, 15), // <- combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.SecondMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: First multiselect dimension has 20, and second 4, which gives a product of 80
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void MultiselecrProduct80_MultiselectProductOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 20),
                new VariableParameters(VariableType.Unknown, 4), // <- combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.MultiselectProductOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 11
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void TooManyIrregularTimeVariableValuesTest()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 11) { Irregular = true}
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.AreEqual(RejectionReason.IrregularTimeOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Content size 1, classifier 3, irregular time variable size 10
        /// Result: Pass
        /// </summary>
        [Test]
        public void IrregularTimeVariableValuesTest_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.Time, 10) { Irregular = true}
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupVerticalBarChartCheck(Limits.GroupVerticalBarChartLimits);

            Assert.True(!check.CheckValidity(input).Any());
        }
    }
}

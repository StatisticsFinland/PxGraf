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
    class GroupHorizontalBarChartTests
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
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

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
                new VariableParameters(VariableType.Unknown, 10),
                new VariableParameters(VariableType.Geological, 3)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: Valid data with a time dimension
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidDataWithTime_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.True(!check.CheckValidity(input).Any());
        }

        /// <summary>
        /// Case: One multiselect dimension
        /// Result: NotEnoughMultiselections
        /// </summary>
        [Test]
        public void OneMultiselectDimensions_NotEnoughMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 1)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.NotEnoughMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Three multiselect dimensions
        /// Result: TooManyMultiselections
        /// </summary>
        [Test]
        public void ThreeMultiselectDimensions_TooManyMultiselections()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2),
                new VariableParameters(VariableType.Geological, 3)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.TooManyMultiselections, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: No content dimenion
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void NoContentDimension_ContentRequired()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Unknown, 15),
                new VariableParameters(VariableType.Time, 2),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.ContentRequired, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Three values selected from the content dimensions
        /// Result: Pass
        /// </summary>
        [Test]
        public void ThreeSelectedFromContent_Pass()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 3) { SameUnit = true },
                new VariableParameters(VariableType.Unknown, 5),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);
            var reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.True(!reasons.Any(), msg);
        }

        /// <summary>
        /// Case: One progressive multiselect dimension
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void OneProgressiveMultiselectDimension_ProgressiveNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Unknown, 15),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.ProgressiveNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Two progressive multiselect dimensions
        /// Result: ProgressiveNotAllowed
        /// </summary>
        [Test]
        public void TwoProgressiveMultiselectDimensions_ProgressiveNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Ordinal, 3),
                new VariableParameters(VariableType.Ordinal, 15),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.ProgressiveNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Five values selected from the time dimensions
        /// Result: TimeOverMax
        /// </summary>
        [Test]
        public void TimeDimensionsHasFiveSelectons_TimeOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Time, 5),
                new VariableParameters(VariableType.Unknown, 3),
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.TimeOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Contains a multiselect dimension with a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void OneMultiselectWithCombinationValue_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 5),
                new VariableParameters(VariableType.Unknown, 3, true), // <- Combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.CombinationValuesNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Contains two multiselect dimensions with a combination value selected
        /// Result: CombinationValuesNotAllowed
        /// </summary>
        [Test]
        public void TwoMultiselectsWithCombinationValues_CombinationValuesNotAllowed()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 5, true), // <- Combination
                new VariableParameters(VariableType.Unknown, 3, true) // <- Combination
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.CombinationValuesNotAllowed, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: One multiselect dimension has 25 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void MultiselectWith25Values_FirstMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 25),
                new VariableParameters(VariableType.Unknown, 2)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.FirstMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: Two multiselect dimensions have over 4 (but under 20) value selected from them.
        /// Result: SecondMultiselectOverMax
        /// </summary>
        [Test]
        public void TwoMultiselectsWithOver4Values_SecondMultiselectOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 12),
                new VariableParameters(VariableType.Unknown, 5)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.SecondMultiselectOverMax, check.CheckValidity(input)[0].Reason);
        }

        /// <summary>
        /// Case: First multiselect dimension has 15, and second 4, which gives a product of 60
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void ProductOver40_MultiselectProductOverMax()
        {
            List<VariableParameters> dimension = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.Geological, 15),
                new VariableParameters(VariableType.Unknown, 4)
            };

            var input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            var check = new GroupHorizontalBarChartCheck(Limits.GroupHorizontalBarChartLimits);

            Assert.AreEqual(RejectionReason.MultiselectProductOverMax, check.CheckValidity(input)[0].Reason);
        }
    }
}

using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.ChartTypeSelection.ChartSpecificLimits;
using PxGraf.ChartTypeSelection.JsonObjects;
using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using System.Collections.Generic;

namespace UnitTests.ChartTypeSelectionTests
{
    [TestFixture]
    public class LineChartTests
    {
        private ChartSelectionLimits Limits { get; set; }

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
            List<DimensionParameters> dimension = [];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.TimeOrProgressiveRequired));
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTime_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 2),
                new DimensionParameters(DimensionType.Content, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeAndContent_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Valid with a time variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithTimeContentAndOther_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 10),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Content, 1)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Valid with a progressive variable
        /// Result: Pass
        /// </summary>
        [Test]
        public void ValidWithProgressive_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 2),
                new DimensionParameters(DimensionType.Content, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Multiselect has 1000 values selected
        /// Result: FirstMultiselectOverMax
        /// </summary>
        [Test]
        public void FirstMultiselectHAs1000_FirstMultiselectOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1000)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.FirstMultiselectOverMax));
        }

        /// <summary>
        /// Case: Multiselects have 4 and 5 values selected, which gives a product of 20
        /// Result: MultiselectProductOverMax
        /// </summary>
        [Test]
        public void LargeMultiselectProduct_MultiselectProductOverMax()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 6),
                new DimensionParameters(DimensionType.Unknown, 4),
                new DimensionParameters(DimensionType.Time, 5)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.MultiselectProductOverMax));
        }

        /// <summary>
        /// Case: One multiselect variable with valid values but without content
        /// Result: ContentRequired
        /// </summary>
        [Test]
        public void ProgressiveAndTime_ContentRequired()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 1),
                new DimensionParameters(DimensionType.Time, 6)
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.ContentRequired));
        }

        /// <summary>
        /// Case: One multiselect variable with valid values
        /// Result: Pass
        /// </summary>
        [Test]
        public void ProgressiveAndTime_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 1),
                new DimensionParameters(DimensionType.Time, 6),
                new DimensionParameters(DimensionType.Content, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Time variable with multiple values and selectable content variable with multiple values
        /// Result: Pass
        /// </summary>
        [Test]
        public void TimeAndSelectableContent_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 1),
                new DimensionParameters(DimensionType.Time, 15),
                new DimensionParameters(DimensionType.Content, 5) { Selectable = true },
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Ordinal variable with multiple values, nominal variable with 4 values and a content variable with 1 value
        /// Result: Pass
        /// </summary>
        [Test]
        public void LargeOrdinalAndNominal_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Ordinal, 25),
                new DimensionParameters(DimensionType.Other, 4),
                new DimensionParameters(DimensionType.Content, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);

            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();

            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Time variable with multiple irregular values, nominal dimension with 2 values and a content dimension with 1 value
        /// Result: IrregularTimeNotAllowed
        /// </summary>
        [Test]
        public void IrregularTime_IrregularTimeNotAllowed()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 10) { Irregular = true },
                new DimensionParameters(DimensionType.Other, 2),
                new DimensionParameters(DimensionType.Content, 1),
            ];

            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);

            Assert.That(check.CheckValidity(input)[0].Reason, Is.EqualTo(RejectionReason.IrregularTimeNotAllowed));
        }

        /// <summary>
        /// Case: Selectable time variable with multiple irregular values, ordinal dimension with multiple values and a content dimension with 1 value
        /// Result: Pass
        /// </summary>
        [Test]
        public void SelectableIrregularTimeAndMultivalueOrdinal_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 10) { Irregular = true, Selectable = true },
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Content, 1),
            ];
            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);
            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();
            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }

        /// <summary>
        /// Case: Time variable with 1 value, ordinal dimension with multiple values and a content dimension with 1 value
        /// Result: Pass
        /// </summary>
        [Test]
        public void SizeOneTimeVariableAndMultivalueOrdinal_Pass()
        {
            List<DimensionParameters> dimension =
            [
                new DimensionParameters(DimensionType.Time, 1),
                new DimensionParameters(DimensionType.Ordinal, 5),
                new DimensionParameters(DimensionType.Content, 1),
            ];
            VisualizationTypeSelectionObject input = TestDataCubeBuilder.BuildTestVisualizationTypeSelectionObject(dimension);
            LineChartCheck check = new(Limits.LineChartLimits);
            List<ChartRejectionInfo> reasons = check.CheckValidity(input);
            string msg = "Ok";
            if (reasons.Count > 0) msg = reasons[0].ToString();
            Assert.That(reasons.Count, Is.EqualTo(0), msg);
        }
    }
}

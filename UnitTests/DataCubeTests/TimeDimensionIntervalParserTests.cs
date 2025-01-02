using NUnit.Framework;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data;
using System;
using System.Collections.Generic;

namespace UnitTests.DataCubeTests
{
    internal class TimeDimensionIntervalParserTests
    {
        [Test]
        public void ValidWeekSeriesTest()
        {
            List<string> testInput = ["2021W49", "2021W50", "2021W51", "2021W52", "2022W01"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Week));
        }

        [Test]
        public void ValidSingleWeekSeriesTest()
        {
            List<string> testInput = ["2021W49"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Week));
        }

        [Test]
        public void IrregularWeekSeriesTest()
        {
            List<string> testInput = ["2021W49", "2021W50", "2021W51", "2022W01"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void ValidMonthSeriesTest()
        {
            List<string> testInput = ["2021M10", "2021M11", "2021M12", "2022M01", "2022M02"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Month));
        }

        [Test]
        public void ValidSingleMonthSeriesTest()
        {
            List<string> testInput = ["2021M11"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Month));
        }

        [Test]
        public void IrregularMonthSeriesTest()
        {
            List<string> testInput = ["2021M10", "2021M12", "2022M01", "2022M02"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void ValidQuarterlySeriesTest()
        {
            List<string> testInput = ["2021Q1", "2021Q2", "2021Q3", "2021Q4", "2022Q1"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Quarter));
        }

        [Test]
        public void ValidSingleQuarterSeriesTest()
        {
            List<string> testInput = ["2021Q1"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Quarter));
        }

        [Test]
        public void ValidBiAnnualSeriesTest()
        {
            List<string> testInput = ["2021H1", "2021H2", "2022H1", "2022H2", "2023H1"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.HalfYear));
        }

        [Test]
        public void ValidSingleBiAnnualSeriesTest()
        {
            List<string> testInput = ["2020H1"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.HalfYear));
        }

        [Test]
        public void IrregularQuarterlySeriesTest()
        {
            List<string> testInput = ["2021Q1", "2021Q2", "2021Q3", "2022Q1"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void ValidYearlySeriesTest()
        {
            List<string> testInput = ["2021", "2022", "2023", "2024", "2025"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Year));
        }

        [Test]
        public void ValidSingleYearSeriesTest()
        {
            List<string> testInput = ["2022"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Year));
        }

        [Test]
        public void IrregularYearlySeriesTest()
        {
            List<string> testInput = ["2021", "2022", "2023", "2025"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void IrregularNonsenseSeriesTest()
        {
            List<string> testInput = ["foo", "bar", "12234", "lol"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void IrregularNonsenseSeriesTest2()
        {
            List<string> testInput = ["1234", "foobar"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void IrregularNumericNonsenseSeriesTest()
        {
            List<string> testInput = ["1234", "4563", "12234", "3"];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void EmptySeriesTest()
        {
            List<string> testInput = [];
            TimeDimensionInterval result = TimeDimensionIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeDimensionInterval.Irregular));
        }

        [Test]
        public void ValidWeekStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("2022W47");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(11));
            Assert.That(result.Value.Day, Is.EqualTo(25));
        }

        [Test]
        public void ValidMonthStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("2022M07");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidQuarterStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("2022Q3");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidHalfYearStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("2022H2");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidYearStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("2022");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(1));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void InvalidStartingPointTest()
        {
            DateTime? result = TimeDimensionIntervalParser.DetermineTimeDimStartingPointFromCode("foobar");
            Assert.That(result, Is.Null);
        }
    }
}

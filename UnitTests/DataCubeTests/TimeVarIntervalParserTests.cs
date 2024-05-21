using NUnit.Framework;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using System;
using System.Collections.Generic;
using UnitTests.TestDummies;
using UnitTests.TestDummies.DummyQueries;

namespace DataCubeTests
{
    internal class TimeVarIntervalParserTests
    {
        [Test]
        public void ValidWeekSeriesTest()
        {
            List<string> testInput = ["2021W49", "2021W50", "2021W51", "2021W52", "2022W01"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Week));
        }

        [Test]
        public void ValidSingleWeekSeriesTest()
        {
            List<string> testInput = ["2021W49"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Week));
        }

        [Test]
        public void IrregularWeekSeriesTest()
        {
            List<string> testInput = ["2021W49", "2021W50", "2021W51", "2022W01"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void ValidMonthSeriesTest()
        {
            List<string> testInput = ["2021M10", "2021M11", "2021M12", "2022M01", "2022M02"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Month));
        }

        [Test]
        public void ValidSingleMonthSeriesTest()
        {
            List<string> testInput = ["2021M11"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Month));
        }

        [Test]
        public void IrregularMonthSeriesTest()
        {
            List<string> testInput = ["2021M10", "2021M12", "2022M01", "2022M02"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void ValidQuarterlySeriesTest()
        {
            List<string> testInput = ["2021Q1", "2021Q2", "2021Q3", "2021Q4", "2022Q1"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Quarter));
        }

        [Test]
        public void ValidSingleQuarterSeriesTest()
        {
            List<string> testInput = ["2021Q1"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Quarter));
        }

        [Test]
        public void ValidBiAnnualSeriesTest()
        {
            List<string> testInput = ["2021H1", "2021H2", "2022H1", "2022H2", "2023H1"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.HalfYear));
        }

        [Test]
        public void ValidSingleBiAnnualSeriesTest()
        {
            List<string> testInput = ["2020H1"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.HalfYear));
        }

        [Test]
        public void IrregularQuarterlySeriesTest()
        {
            List<string> testInput = ["2021Q1", "2021Q2", "2021Q3", "2022Q1"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void ValidYearlySeriesTest()
        {
            List<string> testInput = ["2021", "2022", "2023", "2024", "2025"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Year));
        }

        [Test]
        public void ValidSingleYearSeriesTest()
        {
            List<string> testInput = ["2022"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Year));
        }

        [Test]
        public void IrregularYearlySeriesTest()
        {
            List<string> testInput = ["2021", "2022", "2023", "2025"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void IrregularNonsenseSeriesTest()
        {
            List<string> testInput = ["foo", "bar", "12234", "lol"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void IrregularNonsenseSeriesTest2()
        {
            List<string> testInput = ["1234", "foobar"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void IrregularNumericNonsenseSeriesTest()
        {
            List<string> testInput = ["1234", "4563", "12234", "3"];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void EmptySeriesTest()
        {
            List<string> testInput = [];
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.That(result, Is.EqualTo(TimeVariableInterval.Irregular));
        }

        [Test]
        public void ValidWeekStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022W47");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(11));
            Assert.That(result.Value.Day, Is.EqualTo(25));
        }

        [Test]
        public void ValidMonthStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022M07");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidQuarterStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022Q3");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidHalfYearStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022H2");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(7));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void ValidYearStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022");
            Assert.That(result.Value.Year, Is.EqualTo(2022));
            Assert.That(result.Value.Month, Is.EqualTo(1));
            Assert.That(result.Value.Day, Is.EqualTo(1));
        }

        [Test]
        public void InvalidStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("foobar");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ParserTest()
        {
            List<VariableParameters> varParams =
            [
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            ];

            IReadOnlyCubeMeta inputMeta = TestDataCubeBuilder.BuildTestMeta(varParams);
            TimeVarIntervalParser.TimeVariableInformation result = TimeVarIntervalParser.Parse(inputMeta);
            Assert.That(result.Interval, Is.EqualTo(TimeVariableInterval.Year));
            Assert.That(result.StartingPoint.Value.Year, Is.EqualTo(2000));
            Assert.That(result.StartingPoint.Value.Month, Is.EqualTo(1));
            Assert.That(result.StartingPoint.Value.Day, Is.EqualTo(1));
        }
    }
}

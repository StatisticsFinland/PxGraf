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
            List<string> testInput = new() { "2021W49", "2021W50", "2021W51", "2021W52", "2022W01" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Week, result);
        }

        [Test]
        public void ValidSingleWeekSeriesTest()
        {
            List<string> testInput = new() { "2021W49" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Week, result);
        }

        [Test]
        public void IrregularWeekSeriesTest()
        {
            List<string> testInput = new() { "2021W49", "2021W50", "2021W51", "2022W01" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void ValidMonthSeriesTest()
        {
            List<string> testInput = new() { "2021M10", "2021M11", "2021M12", "2022M01", "2022M02" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Month, result);
        }

        [Test]
        public void ValidSingleMonthSeriesTest()
        {
            List<string> testInput = new() { "2021M11" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Month, result);
        }

        [Test]
        public void IrregularMonthSeriesTest()
        {
            List<string> testInput = new() { "2021M10", "2021M12", "2022M01", "2022M02" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void ValidQuarterlySeriesTest()
        {
            List<string> testInput = new() { "2021Q1", "2021Q2", "2021Q3", "2021Q4", "2022Q1" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Quarter, result);
        }

        [Test]
        public void ValidSingleQuarterSeriesTest()
        {
            List<string> testInput = new() { "2021Q1" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Quarter, result);
        }

        [Test]
        public void ValidBiAnnualSeriesTest()
        {
            List<string> testInput = new() { "2021H1", "2021H2", "2022H1", "2022H2", "2023H1" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.HalfYear, result);
        }

        [Test]
        public void ValidSingleBiAnnualSeriesTest()
        {
            List<string> testInput = new() { "2020H1" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.HalfYear, result);
        }

        [Test]
        public void IrregularQuarterlySeriesTest()
        {
            List<string> testInput = new() { "2021Q1", "2021Q2", "2021Q3", "2022Q1" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void ValidYearlySeriesTest()
        {
            List<string> testInput = new() { "2021", "2022", "2023", "2024", "2025" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Year, result);
        }

        [Test]
        public void ValidSingleYearSeriesTest()
        {
            List<string> testInput = new() { "2022" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Year, result);
        }

        [Test]
        public void IrregularYearlySeriesTest()
        {
            List<string> testInput = new() { "2021", "2022", "2023", "2025" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void IrregularNonsenseSeriesTest()
        {
            List<string> testInput = new() { "foo", "bar", "12234", "lol" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void IrregularNonsenseSeriesTest2()
        {
            List<string> testInput = new() { "1234", "foobar" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void IrregularNumericNonsenseSeriesTest()
        {
            List<string> testInput = new() { "1234", "4563", "12234", "3" };
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void EmptySeriesTest()
        {
            List<string> testInput = new();
            TimeVariableInterval result = TimeVarIntervalParser.DetermineIntervalFromCodes(testInput);
            Assert.AreEqual(TimeVariableInterval.Irregular, result);
        }

        [Test]
        public void ValidWeekStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022W47");
            Assert.AreEqual(2022, result.Value.Year);
            Assert.AreEqual(11, result.Value.Month);
            Assert.AreEqual(25, result.Value.Day);
        }

        [Test]
        public void ValidMonthStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022M07");
            Assert.AreEqual(2022, result.Value.Year);
            Assert.AreEqual(7, result.Value.Month);
            Assert.AreEqual(1, result.Value.Day);
        }

        [Test]
        public void ValidQuarterStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022Q3");
            Assert.AreEqual(2022, result.Value.Year);
            Assert.AreEqual(7, result.Value.Month);
            Assert.AreEqual(1, result.Value.Day);
        }

        [Test]
        public void ValidHalfYearStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022H2");
            Assert.AreEqual(2022, result.Value.Year);
            Assert.AreEqual(7, result.Value.Month);
            Assert.AreEqual(1, result.Value.Day);
        }

        [Test]
        public void ValidYearStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("2022");
            Assert.AreEqual(2022, result.Value.Year);
            Assert.AreEqual(1, result.Value.Month);
            Assert.AreEqual(1, result.Value.Day);
        }

        [Test]
        public void InvalidStartingPointTest()
        {
            DateTime? result = TimeVarIntervalParser.DetermineTimeVarStartingPointFromCode("foobar");
            Assert.IsNull(result);
        }

        [Test]
        public void ParserTest()
        {
            List<VariableParameters> varParams = new()
            {
                new VariableParameters(VariableType.Content, 1),
                new VariableParameters(VariableType.OtherClassificatory, 3),
                new VariableParameters(VariableType.OtherClassificatory, 3) { Selectable = true },
                new VariableParameters(VariableType.Time, 3),
            };

            IReadOnlyCubeMeta inputMeta = TestDataCubeBuilder.BuildTestMeta(varParams);
            TimeVarIntervalParser.TimeVariableInformation result = TimeVarIntervalParser.Parse(inputMeta);
            Assert.AreEqual(TimeVariableInterval.Year, result.Interval);
            Assert.AreEqual(2000, result.StartingPoint.Value.Year);
            Assert.AreEqual(1, result.StartingPoint.Value.Month);
            Assert.AreEqual(1, result.StartingPoint.Value.Day);
        }
    }
}

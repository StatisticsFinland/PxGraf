﻿using PxGraf.Data.MetaData;
using PxGraf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxGraf.Data
{
    public static partial class TimeVarIntervalParser
    {
        [GeneratedRegex("^[12][0-9]{3}W[0-5][0-9]$")]
        private static partial Regex WeeklyValuesRegex();

        [GeneratedRegex("^[12][0-9]{3}M[01][0-9]$")]
        private static partial Regex MonthlyValuesRegex();

        [GeneratedRegex("^[12][0-9]{3}Q[1-4]$")]
        private static partial Regex QuarterlyValuesRegex();

        [GeneratedRegex("^[12][0-9]{3}H[12]$")]
        private static partial Regex BiAnnualValuesRegex();

        [GeneratedRegex("^[12][0-9]{3}$")]
        private static partial Regex AnnualValuesRegex();

        public class TimeVariableInformation(TimeVariableInterval interval, DateTime? startingPoint)
        {
            public TimeVariableInterval Interval { get; } = interval;
            public DateTime? StartingPoint { get; } = startingPoint;
        }

        /// <summary>
        /// Returns a time variable interval based on the cube meta.
        /// </summary>
        public static TimeVariableInformation Parse(IReadOnlyCubeMeta meta)
        {
            IEnumerable<TimeVariableInterval> intervals = meta.Variables
                .Where(v => v.Type == VariableType.Time)
                .Select(v => DetermineIntervalFromCodes(v.IncludedValues.Select(val => val.Code)));

            TimeVariableInterval interval = intervals.Min();
            if (interval == TimeVariableInterval.Irregular)
            {
                return new TimeVariableInformation(interval, null);
            }
            else
            {
                DateTime? startingPoint = meta.Variables
                    .Where(v => v.Type == VariableType.Time)
                    .Select(v => DetermineTimeVarStartingPointFromCode(v.IncludedValues[0].Code))
                    .Min();
                return new TimeVariableInformation(interval, startingPoint);
            }
        }

        /// <summary>
        /// This function determines the time series starting point from the first code in the time variable.
        /// This is a separate function for unit testing purposes.
        /// </summary>
        public static DateTime? DetermineTimeVarStartingPointFromCode(string firstCode)
        {
            if (WeeklyValuesRegex().IsMatch(firstCode))
            {
                WeeklyTimeStamp weekly = new(firstCode);
                return new DateTime(weekly.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(7 * weekly.Week - 1);
            }

            if (MonthlyValuesRegex().IsMatch(firstCode))
            {
                MonthlyTimeStamp monthly = new(firstCode);
                return new DateTime(monthly.Year, monthly.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            if (QuarterlyValuesRegex().IsMatch(firstCode))
            {
                QuarterlyTimeStamp quarterly = new(firstCode);
                return new DateTime(quarterly.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(3 * (quarterly.Quarter - 1));
            }

            if (BiAnnualValuesRegex().IsMatch(firstCode))
            {
                BiAnnualTimeStamp quarterly = new(firstCode);
                return new DateTime(quarterly.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(6 * (quarterly.Half - 1));
            }

            if (AnnualValuesRegex().IsMatch(firstCode))
            {
                AnnualTimeStamp yearly = new(firstCode);
                return new DateTime(yearly.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            return null;
        }

        /// <summary>
        /// This function determines the time series interval from the codes in the time variable.
        /// This is a separate function for unit testing purposes.
        /// </summary>
        public static TimeVariableInterval DetermineIntervalFromCodes(IEnumerable<string> valueCodes)
        {
            if (valueCodes is null || !valueCodes.Any()) return TimeVariableInterval.Irregular;

            if (WeeklyValuesRegex().IsMatch(valueCodes.First()) && Validate(valueCodes, code => new WeeklyTimeStamp(code)))
            {
                return TimeVariableInterval.Week;
            }

            if (MonthlyValuesRegex().IsMatch(valueCodes.First()) && Validate(valueCodes, code => new MonthlyTimeStamp(code)))
            {
                return TimeVariableInterval.Month;
            }

            if (QuarterlyValuesRegex().IsMatch(valueCodes.First()) && Validate(valueCodes, code => new QuarterlyTimeStamp(code)))
            {
                return TimeVariableInterval.Quarter;
            }


            if (BiAnnualValuesRegex().IsMatch(valueCodes.First()) && Validate(valueCodes, code => new BiAnnualTimeStamp(code)))
            {
                return TimeVariableInterval.HalfYear;
            }

            if (AnnualValuesRegex().IsMatch(valueCodes.First()) && Validate(valueCodes, code => new AnnualTimeStamp(code)))
            {
                return TimeVariableInterval.Year;
            }

            return TimeVariableInterval.Irregular;
        }

        private static bool Validate(IEnumerable<string> timeVarValueCodes, Func<string, IterableTimeStamp> iteratorBuilder)
        {
            IterableTimeStamp iterableStamp = iteratorBuilder(timeVarValueCodes.First());
            foreach (string code in timeVarValueCodes)
            {
                if (!iterableStamp.Match(code))
                {
                    return false;
                }
                iterableStamp.Next();
            }
            return true;
        }

        private abstract class IterableTimeStamp
        {
            public int Year { get; protected set; }

            public abstract void Next();

            public abstract bool Match(string timeStamp);
        }

        private sealed class WeeklyTimeStamp : IterableTimeStamp
        {
            public int Week { get; private set; }

            public override void Next()
            {
                if (Week == 52)
                {
                    Year++;
                    Week = 1;
                }
                else
                {
                    Week++;
                }
            }

            public override bool Match(string timeStamp)
            {
                return timeStamp == $"{Year:D4}W{Week:D2}";
            }

            public WeeklyTimeStamp(string timeStamp)
            {
                Year = int.Parse(timeStamp[..4]);
                Week = int.Parse(timeStamp[5..]);
            }
        }

        private sealed class MonthlyTimeStamp : IterableTimeStamp
        {
            public int Month { get; private set; }

            public override void Next()
            {
                if (Month == 12)
                {
                    Year++;
                    Month = 1;
                }
                else
                {
                    Month++;
                }
            }

            public override bool Match(string timeStamp)
            {
                return timeStamp == $"{Year:D4}M{Month:D2}";
            }

            public MonthlyTimeStamp(string timeStamp)
            {
                Year = int.Parse(timeStamp[..4]);
                Month = int.Parse(timeStamp[5..]);
            }
        }

        private sealed class QuarterlyTimeStamp : IterableTimeStamp
        {
            public int Quarter { get; private set; }

            public override void Next()
            {
                if (Quarter == 4)
                {
                    Year++;
                    Quarter = 1;
                }
                else
                {
                    Quarter++;
                }
            }

            public override bool Match(string timeStamp)
            {
                return timeStamp == $"{Year:D4}Q{Quarter}";
            }

            public QuarterlyTimeStamp(string timeStamp)
            {
                Year = int.Parse(timeStamp[..4]);
                Quarter = int.Parse(timeStamp[5..]);
            }
        }

        private sealed class BiAnnualTimeStamp : IterableTimeStamp
        {
            public int Half { get; private set; }

            public override void Next()
            {
                if (Half == 2)
                {
                    Year++;
                    Half = 1;
                }
                else
                {
                    Half = 2;
                }
            }

            public override bool Match(string timeStamp)
            {
                return timeStamp == $"{Year:D4}H{Half}";
            }

            public BiAnnualTimeStamp(string timeStamp)
            {
                Year = int.Parse(timeStamp[..4]);
                Half = int.Parse(timeStamp[5..]);
            }
        }

        private sealed class AnnualTimeStamp : IterableTimeStamp
        {
            public override void Next()
            {
                Year++;
            }

            public override bool Match(string timeStamp)
            {
                return timeStamp == $"{Year}";
            }

            public AnnualTimeStamp(string timeStamp)
            {
                Year = int.Parse(timeStamp);
            }
        }
    }
}

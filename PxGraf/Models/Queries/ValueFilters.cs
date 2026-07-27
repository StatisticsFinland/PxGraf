#nullable enable
using Px.Utils.Models.Metadata.Dimensions;
using PxGraf.Utility.CustomJsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Base class for filtering values of a dimension.
    /// </summary>
    [JsonConverter(typeof(ValueFilterJsonConverter))]
    public interface IValueFilter
    {
        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values);

        /// <summary>
        /// Filters a list of value codes using the same logic as <see cref="Filter(IReadOnlyList{IReadOnlyDimensionValue})"/>,
        /// operating directly on code strings without requiring full dimension value objects.
        /// </summary>
        public IEnumerable<string> Filter(IReadOnlyList<string> codes);
    }

    /// <summary>
    /// Filter that selects the last N values.
    /// </summary>
    public class TopFilter(int count) : IValueFilter
    {
        public int Count { get; set; } = count;

        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            return values.Skip(values.Count - Count);
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            return codes.Skip(codes.Count - Count);
        }
    }

    /// <summary>
    /// Filter that selects values starting from a specific value.
    /// </summary>
    public class FromFilter(string code) : IValueFilter
    {
        public string Code { get; set; } = code;

        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Code == Code)
                {
                    return values.Skip(i);
                }
            }
            return Enumerable.Empty<DimensionValue>();
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i] == Code)
                {
                    return codes.Skip(i);
                }
            }
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Filter that selects all values.
    /// </summary>
    public class AllFilter : IValueFilter
    {
        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            return values;
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            return codes;
        }
    }

    /// <summary>
    /// Filter that allows user to pick specific values manually.
    /// </summary>
    public class ItemFilter(List<string> codes) : IValueFilter
    {
        public List<string> Codes { get; set; } = codes;

        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            return values.Where(value => Codes.Contains(value.Code));
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            return codes.Where(Codes.Contains);
        }
    }

    /// <summary>
    /// Filter that allows user to manually pick values to exclude. All values that are not picked are included.
    /// </summary>
    public class InverseItemFilter(List<string> codes) : IValueFilter
    {
        public List<string> Codes { get; set; } = codes;

        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            return values.Where(value => !Codes.Contains(value.Code));
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            return codes.Where(code => !Codes.Contains(code));
        }
    }

    /// <summary>
    /// Filter that selects values whose code matches a regular expression. Matching is case-sensitive and
    /// looks for the pattern anywhere within the code (use ^ and $ to match the whole code).
    /// Codes are excluded if the pattern is invalid or matching against it times out.
    /// </summary>
    public class RegexFilter(string pattern) : IValueFilter
    {
        // NOTE: this timeout applies per code, not per request - a pathological pattern against a dimension
        // with many values could still take up to (MatchTimeout * value count) in the worst case.
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(500);

        public string Pattern { get; set; } = pattern;

        public IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values)
        {
            Regex? regex = TryCreateRegex();
            if (regex is null) return [];
            return values.Where(value => IsMatch(regex, value.Code));
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            Regex? regex = TryCreateRegex();
            if (regex is null) return [];
            return codes.Where(code => IsMatch(regex, code));
        }

        private Regex? TryCreateRegex()
        {
            try
            {
                return new Regex(Pattern, RegexOptions.None, MatchTimeout);
            }
            catch (ArgumentException)
            {
                // Invalid regular expression pattern; no codes match.
                return null;
            }
        }

        private static bool IsMatch(Regex regex, string code)
        {
            try
            {
                return regex.IsMatch(code);
            }
            catch (RegexMatchTimeoutException)
            {
                // Matching took too long; treat as no match.
                return false;
            }
        }
    }
}

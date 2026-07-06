using Px.Utils.Models.Metadata.Dimensions;
using PxGraf.Utility.CustomJsonConverters;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Base class for filtering values of a dimension.
    /// </summary>
    [JsonConverter(typeof(ValueFilterJsonConverter))]
    public interface IValueFilter
    {
        public abstract IEnumerable<IReadOnlyDimensionValue> Filter(IReadOnlyList<IReadOnlyDimensionValue> values);

        /// <summary>
        /// Filters a list of value codes using the same logic as <see cref="Filter(IReadOnlyList{IReadOnlyDimensionValue})"/>,
        /// operating directly on code strings without requiring full dimension value objects.
        /// </summary>
        public abstract IEnumerable<string> Filter(IReadOnlyList<string> codes);
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
            int index = values.ToList().FindIndex(value => value.Code == Code);
            if (index >= 0)
            {
                return values.Skip(index);
            }
            else
            {
                return Enumerable.Empty<DimensionValue>();
            }
        }

        public IEnumerable<string> Filter(IReadOnlyList<string> codes)
        {
            int index = codes.ToList().IndexOf(Code);
            if (index >= 0)
            {
                return codes.Skip(index);
            }
            else
            {
                return Enumerable.Empty<string>();
            }
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
}

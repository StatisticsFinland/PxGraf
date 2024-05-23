using Newtonsoft.Json;
using PxGraf.Data.MetaData;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Base class for filtering values of a variable.
    /// </summary>
    [JsonConverter(typeof(ValueFilterJsonConverter))]
    public abstract class ValueFilter
    {
        public abstract IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values);
    }

    /// <summary>
    /// Filter that selects the last N values.
    /// </summary>
    public class TopFilter(int count) : ValueFilter
    {
        public int Count { get; set; } = count;

        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            return values.Skip(values.Count - Count);
        }
    }

    /// <summary>
    /// Filter that selects values starting from a specific value.
    /// </summary>
    public class FromFilter(string code) : ValueFilter
    {
        public string Code { get; set; } = code;

        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            var index = values.FindIndex(value => value.Code == Code);
            if (index >= 0)
            {
                return values.Skip(index);
            }
            else
            {
                return Enumerable.Empty<VariableValue>();
            }
        }
    }

    /// <summary>
    /// Filter that selects all values.
    /// </summary>
    public class AllFilter : ValueFilter
    {
        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            return values;
        }
    }

    /// <summary>
    /// Filter that allows user to pick specific values manually.
    /// </summary>
    public class ItemFilter(List<string> codes) : ValueFilter
    {
        public List<string> Codes { get; set; } = codes;

        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            return values.Where(value => Codes.Contains(value.Code));
        }
    }
}

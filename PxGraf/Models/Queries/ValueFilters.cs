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
    public class TopFilter : ValueFilter
    {
        public int Count { get; set; }

        public TopFilter(int count)
        {
            Count = count;
        }

        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            return values.Skip(values.Count - Count);
        }
    }

    /// <summary>
    /// Filter that selects values starting from a specific value.
    /// </summary>
    public class FromFilter : ValueFilter
    {
        public string Code { get; set; }

        public FromFilter(string code)
        {
            Code = code;
        }

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
    public class ItemFilter : ValueFilter
    {
        public List<string> Codes { get; set; }

        public ItemFilter(List<string> codes)
        {
            Codes = codes;
        }

        public override IEnumerable<IReadOnlyVariableValue> Filter(IReadOnlyList<IReadOnlyVariableValue> values)
        {
            return values.Where(value => Codes.Contains(value.Code));
        }
    }
}

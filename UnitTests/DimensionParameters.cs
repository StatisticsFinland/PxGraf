using Px.Utils.Models.Metadata.Enums;
using PxGraf.Models.Queries;

namespace UnitTests
{
    public class DimensionParameters(DimensionType type, int size, bool hasCombiVal = false, string name = null)
    {
        public DimensionType Type { get; } = type;
        public int Size { get; } = size;
        public bool HasCombinationValue { get; set; } = hasCombiVal;
        public bool SameUnit { get; set; } = false;
        public bool SameSource { get; set; } = false;
        public int Decimals { get; set; } = 0;
        public bool Selectable { get; set; } = false;
        public IValueFilter ValueFilter { get; set; }
        public string Name { get; set; } = name;
        public bool Irregular { get; set; } = false;
    }
}

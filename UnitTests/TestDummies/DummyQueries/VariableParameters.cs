using PxGraf.Enums;
using PxGraf.Models.Queries;

namespace UnitTests.TestDummies
{
    public class VariableParameters(VariableType type, int size, bool hasCombiVal = false, string name = null)
    {
        public VariableType Type { get; } = type;
        public int Size { get; } = size;
        public bool HasCombinationValue { get; set; } = hasCombiVal;
        public bool SameUnit { get; set; } = false;
        public bool SameSource { get; set; } = false;
        public int Decimals { get; set; } = 0;
        public bool Selectable { get; set; } = false;
        public ValueFilter ValueFilter { get; set; }
        public string Name { get; set; } = name;
        public bool Irregular { get; set; } = false;
    }
}

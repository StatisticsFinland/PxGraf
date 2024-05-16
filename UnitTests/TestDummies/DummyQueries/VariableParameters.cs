using PxGraf.Enums;
using PxGraf.Models.Queries;

namespace UnitTests.TestDummies
{
    public class VariableParameters
    {
        public VariableType Type { get; }
        public int Size { get; }
        public bool HasCombinationValue { get; set; }
        public bool SameUnit { get; set; } = false;
        public bool SameSource { get; set; } = false;
        public int Decimals { get; set; } = 0;
        public bool Selectable { get; set; } = false;
        public ValueFilter ValueFilter { get; set; }
        public string Name { get; set; }
        public bool Irregular { get; set; } = false;

        public VariableParameters(VariableType type, int size, bool hasCombiVal = false, string name = null)
        {
            Type = type;
            Size = size;
            HasCombinationValue = hasCombiVal;
            Name = name;
        }
    }
}

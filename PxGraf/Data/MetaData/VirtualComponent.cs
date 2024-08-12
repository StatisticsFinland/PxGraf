using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Data.MetaData
{
    [ExcludeFromCodeCoverage] // This is a placeholder class without actual use
    public class VirtualComponent(string operator_, List<string> operandCodes)
    {
        public string Operator { get; } = operator_;

        public IReadOnlyList<string> OperandCodes { get; } = operandCodes;

        public VirtualComponent Clone()
        {
            return new VirtualComponent(Operator, [.. OperandCodes]);
        }
    }
}

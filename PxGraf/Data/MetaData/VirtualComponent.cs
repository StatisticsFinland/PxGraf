using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data.MetaData
{
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

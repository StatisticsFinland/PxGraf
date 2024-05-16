using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data.MetaData
{
    public class VirtualComponent
    {
        public string Operator { get; }

        public IReadOnlyList<string> OperandCodes { get; }

        public VirtualComponent(string operator_, List<string> operandCodes)
        {
            Operator = operator_;
            OperandCodes = operandCodes;
        }

        public VirtualComponent Clone()
        {
            return new VirtualComponent(Operator, OperandCodes.ToList());
        }
    }
}

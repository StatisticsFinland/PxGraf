using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    public class VirtualValueDefinition
    {
        public string Operator { get; set; } // TODO: operator enums

        public List<string> OperandCodes { get; set; }
    }
}

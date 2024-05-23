using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    public class VirtualValueDefinition
    {
        public string Operator { get; set; }

        public List<string> OperandCodes { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Models.Queries
{
    public class Layout
    {
        public IReadOnlyList<string> RowVariableCodes { get; set; }

        public IReadOnlyList<string> ColumnVariableCodes { get; set; }

        public Layout(IReadOnlyList<string> rowVariableCodes, IReadOnlyList<string> columnVariableCodes)
        {
            RowVariableCodes = rowVariableCodes;
            ColumnVariableCodes = columnVariableCodes;
        }

        [JsonConstructor]
        public Layout()
        {

        }

        public override bool Equals(object obj)
        {
            if (obj is not Layout other)
            {
                return false;
            }
                        
            return this.RowVariableCodes.SequenceEqual(other.RowVariableCodes) &&
                   this.ColumnVariableCodes.SequenceEqual(other.ColumnVariableCodes);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
